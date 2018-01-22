using DependencyChecker.Dependencies;
using DependencyChecker.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace DependencyChecker {

    /// <summary>
    /// Main Entry Point
    ///
    /// This is where we find all loaded checker assemblies, mod related directories and execute on the latest version.
    /// </summary>
    [StaticConstructorOnStartup]
    public static class Main {
        /// <summary>
        /// Entry Point
        /// </summary>
        static Main() {
            try {
                var latestVersion = GameObjectUtility.StoredVersion; // Try and get our latest checker version GameObject
                if (latestVersion == null) { // No GameObject was found
                    latestVersion = Init(); // This is the first Instance to load
                }
                if (AssemblyUtility.CurrentAssemblyVersion >= latestVersion) { // compare version
                    Execute(); // This assembly is the latest version found
                }
            }
            catch (Exception e) { // An exception occurred, output the version to help with debugging
                Log.Error(string.Format("An exception was caused by {0} assembly version {1}. Exception was: {2}", AssemblyUtility.CurrentAssemblyName, AssemblyUtility.CurrentAssemblyVersion.ToString(), e));
            }
        }

        /// <summary>
        /// This is where the latest code will execute from
        /// </summary>
        private static void Execute() { // This is not version specific
            //TODO: remove this log message
            Log.Message(string.Format("We are executing on version {0}", AssemblyUtility.CurrentAssemblyVersion.ToString()));

            var relatedMods = GameObjectUtility.StoredModIdentifiers;
            if (relatedMods != null) {
                DependencyController.Start(relatedMods);
            }
        }

        /// <summary>
        /// Runs initial checks and creates our GameObjects for later instances
        ///
        /// Should only be invoked on the first instance created
        /// </summary>
        /// <returns>The version that should execute</returns>
        private static Version Init() {
            var checkerAssemblyName = Assembly.GetExecutingAssembly().GetName(); // Hold our AssemblyName
            var relatedModIDs = new List<string>(); // Hold a list of mods that are related to our checker
            Version latestCheckerVersion = checkerAssemblyName.Version; // Hold the version of the latest assembly

            foreach (ModContentPack modContentPack in LoadedModManager.RunningMods) {
                foreach (var assembly in modContentPack.assemblies.loadedAssemblies) {
                    if (assembly.GetName().Name == checkerAssemblyName.Name) {
                        relatedModIDs.Add(modContentPack.Identifier); // Add this mod to our list
                        if (assembly.GetName().Version > latestCheckerVersion)
                            latestCheckerVersion = assembly.GetName().Version; // This assembly version is newer
                        break; // Already found a version of our assembly in this ModPack
                    }
                }
            }

            // Create our GameObjects to be accessed by another instance
            GameObjectUtility.StoredVersion = latestCheckerVersion;
            GameObjectUtility.StoredModIdentifiers = relatedModIDs;

            return latestCheckerVersion; // Return the latest version so we know if we should execute on this instance
        }
    }
}