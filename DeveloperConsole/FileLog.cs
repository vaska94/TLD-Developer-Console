using HarmonyLib;
using Il2Cpp;
using MelonLoader.Utils;

namespace DeveloperConsole {
	internal static class FileLog {
		private static int numNullReference = 0;
		private const string NULL_REFERENCE_TEXT = "NullReferenceException: Object reference not set to an instance of an object.";
		internal const string FILE_NAME = "DeveloperConsole.log";

		private static string GetFilePath() => Path.GetFullPath(Path.Combine(MelonEnvironment.ModsDirectory, FILE_NAME));

		internal static void CreateLogFile() {
			try {
				using (File.Create(GetFilePath())) { }
			} catch {
				// Ignore file creation errors
			}
		}

		private static void Log(string text) {
			if (text is null) return;
			if (text == NULL_REFERENCE_TEXT) {
				numNullReference++;
				return;
			} else {
				MaybeLogNullReference();
				TryWriteToFile(new string[] { text });
			}
		}

		internal static void MaybeLogNullReference() {
			if (numNullReference > 0) {
				TryWriteToFile(new string[] { "(" + numNullReference + ") " + NULL_REFERENCE_TEXT });
				numNullReference = 0;
			}
		}

		private static void TryWriteToFile(string[] lines) {
			try {
				File.AppendAllLines(GetFilePath(), lines);
			} catch {
				// Silently ignore file access errors to prevent spam
			}
		}

		[HarmonyPatch(typeof(uConsoleLog), "Add")]
		internal class UConsoleLog_Add {
			private static void Postfix(string text) {
				Log(text);
			}
		}
	}
}
