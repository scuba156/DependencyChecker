using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;

namespace DependencyChecker.Utils {

    public static class LanguageUtility {
        public static readonly string LanguageDirectory = Path.Combine(Application.temporaryCachePath, "DC");

        public static void LoadAllLanguages() {
            TryExtractAllLanguages();

            foreach (var languageDir in Directory.GetDirectories(Path.Combine(LanguageDirectory, "Languages"))) {
                LoadLanguage(new DirectoryInfo(languageDir));
            }
        }

        private static void LoadLanguage(DirectoryInfo langDir) {
            LoadedLanguage loadedLanguage = LanguageDatabase.AllLoadedLanguages.FirstOrDefault((LoadedLanguage lib) => lib.folderName == langDir.Name);
            if (loadedLanguage == null) {
                Log.Warning(langDir.Name + " language not found");
            } else {
                DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(langDir.ToString(), "Keyed"));
                if (directoryInfo.Exists) {
                    FileInfo[] files = directoryInfo.GetFiles("*.xml", SearchOption.AllDirectories);
                    for (int i = 0; i < files.Length; i++) {
                        FileInfo file = files[i];
                        loadedLanguage.GetType().GetMethod("LoadFromFile_Keyed", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(loadedLanguage, new object[] { file });
                    }
                }
            }
        }

        private static string ParseLanguagePathFromResourceName(string resourceName) {
            string result = string.Empty;
            var splitName = resourceName.Split('.');
            int startingIndex = splitName.FirstIndexOf(s=>s.ToLower() == "languages");
            if ( startingIndex < 1) {
                return null;
            }

            for (int i = startingIndex; i < splitName.Count() - 1; i++) {
                if (!result.NullOrEmpty()) {
                    result += Path.DirectorySeparatorChar;
                }
                result += splitName[i];
            }
            result += "." + splitName[splitName.Count() - 1];

            return result;
        }

        private static void TryExtractAllLanguages() {
            try {
                foreach (var resourceName in AssemblyUtility.CurrentAssembly.GetManifestResourceNames()) {
                    string outputFilePath = ParseLanguagePathFromResourceName(resourceName);
                    if (outputFilePath != null) {
                        outputFilePath = Path.Combine(LanguageDirectory, outputFilePath);
                        Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath));

                        using (Stream stream = AssemblyUtility.CurrentAssembly.GetManifestResourceStream(resourceName)) {
                            using (FileStream fileStream = new FileStream(outputFilePath, FileMode.Create)) {
                                for (int i = 0; i < stream.Length; i++) {
                                    fileStream.WriteByte((byte)stream.ReadByte());
                                }
                                fileStream.Close();
                            }
                        }
                    }
                }
            } catch (Exception e) {
                throw e;
            }
        }
    }
}