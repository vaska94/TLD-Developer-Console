using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace DeveloperConsole {

    [HarmonyPatch(typeof(BootUpdate), "Start")]
    internal static class InitializePatch {

        private static void Prefix() {
            try {
                GameObject prefab = null;
                string[] assetPaths = {
                    "uConsole",
                    "uConsoleGUI", 
                    "uConsole/Prefabs/uConsole.prefab",
                    "Assets/uConsole",
                    "Assets/uConsoleGUI",
                    "Console/uConsole",
                    "Console/uConsoleGUI"
                };
                
                foreach (string assetPath in assetPaths) {
                    try {
                        var handle = Addressables.LoadAssetAsync<GameObject>(assetPath);
                        handle.WaitForCompletion();
                        
                        if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result != null) {
                            prefab = handle.Result;
                            break;
                        }
                    } catch (System.Exception) {
                        // Continue to next asset path
                    }
                }
                
                if (prefab != null) {
                    UnityEngine.Object.Instantiate(prefab);
                    uConsole.m_Instance.m_Activate = KeyCode.F1;
                }
            } catch (System.Exception ex) {
                MelonLogger.Error($"Error initializing Developer Console: {ex.Message}");
            }
        }
    }
}
