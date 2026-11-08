using Il2Cpp;
using Il2CppTLD.AddressableAssets;
using Il2CppTLD.Scenes;
using MelonLoader;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using Il2CppCollection = Il2CppSystem.Collections.Generic;

namespace DeveloperConsole {

    /// <summary>
    /// Creates the uConsole instance and keeps the "scene" autocomplete list current.
    /// Driven from the MelonMod scene callbacks rather than a Harmony patch on BootUpdate.Start,
    /// because the BootUpdate class no longer exists since The Long Dark 2.60.
    /// </summary>
    internal static class ConsoleBootstrap {

        // Addressable keys to try for the console prefab, most likely first.
        private static readonly string[] PrefabKeys = {
            "uConsole",
            "Assets/uConsole/Prefabs/uConsole.prefab",
        };

        private const string SceneCommand = "scene";

        private static bool consoleCreated;
        private static bool prefabMissingLogged;
        private static int registeredSceneCount = -1;

        internal static void OnSceneInitialized() {
            EnsureConsole();
            RefreshSceneAutocomplete();
        }

        private static void EnsureConsole() {
            if (consoleCreated) return;

            if (uConsole.m_Instance != null) {
                consoleCreated = true;
                return;
            }

            try {
                GameObject prefab = LoadPrefab();
                if (prefab == null) {
                    if (!prefabMissingLogged) {
                        MelonLogger.Warning("uConsole prefab not available from Addressables yet; retrying on the next scene load.");
                        prefabMissingLogged = true;
                    }
                    return;
                }

                GameObject instance = UnityEngine.Object.Instantiate(prefab);
                UnityEngine.Object.DontDestroyOnLoad(instance);

                uConsole console = uConsole.m_Instance ?? instance.GetComponent<uConsole>();
                if (console != null) {
                    console.m_Activate = KeyCode.F1;
                }

                consoleCreated = true;
                MelonLogger.Msg("Developer console ready, press F1 to toggle it.");
            } catch (Exception ex) {
                MelonLogger.Error($"Failed to create the developer console: {ex}");
            }
        }

        private static GameObject LoadPrefab() {
            foreach (string key in PrefabKeys) {
                try {
                    AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(key);
                    handle.WaitForCompletion();
                    if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result != null) {
                        return handle.Result;
                    }
                } catch (Exception) {
                    // Key not in the catalog (or Addressables not ready); try the next one.
                }
            }
            return null;
        }

        /// <summary>
        /// Registers (or updates) the parameter set that lets TAB complete scene names for the
        /// "scene" command. Re-run on every scene load because content catalogs (base game and
        /// DLC) finish loading at different times during boot.
        /// </summary>
        private static void RefreshSceneAutocomplete() {
            try {
                Il2CppCollection.List<IResourceLocation> locations =
                    AssetHelper.FindAllAssetsLocations<SceneSet>()?.TryCast<Il2CppCollection.List<IResourceLocation>>();
                if (locations == null) return;

                int count = locations.Count;
                if (count == 0 || count == registeredSceneCount) return;

                Il2CppCollection.List<string> parameters = new Il2CppCollection.List<string>(count * 2);
                foreach (IResourceLocation location in locations) {
                    parameters.Add(location.PrimaryKey);
                    parameters.Add(location.PrimaryKey.ToLowerInvariant());
                }
                parameters.Sort();

                uConsoleCommandParameterSet existing = FindParameterSet(SceneCommand);
                if (existing != null) {
                    existing.m_AllowedParameters = parameters;
                } else {
                    uConsoleAutoComplete.CreateCommandParameterSet(SceneCommand, parameters);
                }
                registeredSceneCount = count;
            } catch (Exception ex) {
                MelonLogger.Warning($"Could not refresh scene name autocompletion: {ex.Message}");
            }
        }

        private static uConsoleCommandParameterSet FindParameterSet(string command) {
            Il2CppCollection.List<uConsoleCommandParameterSet> sets = uConsoleAutoComplete.m_CommandParameterSets;
            if (sets == null) return null;

            foreach (uConsoleCommandParameterSet set in sets) {
                if (set?.m_Commands != null && set.m_Commands.Contains(command)) {
                    return set;
                }
            }
            return null;
        }
    }
}
