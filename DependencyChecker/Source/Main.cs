using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;

namespace DependencyChecker {

    /// <summary>
    /// Our Entry Point
    ///
    /// This is where we find all loaded checker assemblies, mod related directories and execute on the latest version.
    /// </summary>
    [StaticConstructorOnStartup]
    public static class Main {

        /// <summary>
        /// Token that identifies our game object
        /// </summary>
        private const string TokenObjectName = "DependencyCheckerToken";

        /// <summary>
        /// Entry Point
        /// </summary>
        static Main() {
            try {
                var latestVersion = LatestCheckerVersion; // Try and get our latest checker version GameObject
                if (latestVersion == null) { // No GameObject was found
                    latestVersion = RunInitChecks(); // This is the first Instance to load
                }
                if (latestVersion != null && CurrentAssemblyVersion >= latestVersion) { // compare version
                    Execute(); // This assembly is the latest version found
                }
            }
            catch (Exception e) { // An exception occurred, output the version to help with debugging
                Log.Error(string.Format("An exception was caused by {0} assembly version {1}. Exception was: {2}", CurrentAssemblyName, CurrentAssemblyVersion.ToString(), e));
            }
        }

        /// <summary>
        /// Get all checker assemblies
        /// </summary>
        private static List<Assembly> AllCheckerAssemblies { get { return AppDomain.CurrentDomain.GetAssemblies().ToList().FindAll((ass => ass.GetName().Name == CurrentAssemblyName)); } }

        /// <summary>
        /// Get all checker assemblies that are not the executing assembly
        /// </summary>
        private static List<Assembly> AllCheckerAssembliesExceptCurrent { get { return AllCheckerAssemblies.FindAll(ass => ass.FullName != Assembly.GetExecutingAssembly().FullName); } }

        /// <summary>
        /// Stores the latest checker version in a GameObject component. Returns null if it doesn't exist
        /// </summary>
        private static List<string> CheckerEnabledMods { // Getter is not version specific
            get { // see LatestCheckerVersion Property for more info
                var tokenObject = GameObject.Find(TokenObjectName);
                if (tokenObject != null) {
                    foreach (Assembly assembly in AllCheckerAssemblies) { // We need to check all assemblies including current
                        var component = tokenObject.GetComponent(assembly.GetTypes().First(type => type.Name == typeof(CheckerEnabledModsComponent).Name));
                        if (component != null) {
                            return (List<string>)component.GetType().GetField("checkerEnabledModsIdentifiers").GetValue(component);
                        }
                    }
                }
                return null;
            }
            set {
                GameObject gameObject = GameObject.Find(TokenObjectName);
                if (gameObject == null)
                    gameObject = new GameObject(TokenObjectName);
                gameObject.SetActive(true);
                gameObject.AddComponent(typeof(CheckerEnabledModsComponent));
                CheckerEnabledModsComponent component = gameObject.GetComponent<CheckerEnabledModsComponent>();
                component.enabled = true;
                component.checkerEnabledModsIdentifiers = value;
            }
        }

        /// <summary>
        /// Gets the executing assemblies name
        /// </summary>
        private static string CurrentAssemblyName { get { return Assembly.GetExecutingAssembly().GetName().Name; } }

        /// <summary>
        /// Gets the executing assemblies version
        /// </summary>
        private static Version CurrentAssemblyVersion { get { return Assembly.GetExecutingAssembly().GetName().Version; } }

        /// <summary>
        /// Stores the latest checker version in a GameObject component. Returns null if it doesn't exist
        /// </summary>
        private static Version LatestCheckerVersion { // Getter is not version specific as long as it returns typeof Version
            get {
                var tokenObject = GameObject.Find(TokenObjectName); // Try and find our GameObject
                if (tokenObject != null) { // If found
                    // Here we have to find the type for our class 'CheckerVersionComponent' that matches the
                    // version of the original 'CheckerVersionComponent' instance that was created. The only way to really
                    // do this is to try and get each 'CheckerVersionComponent' as a component from our GameObject. We can only
                    // retrieve a component by type, and using
                    //
                    //       Type.GetType("DependencyChecker.DependencyChecker+CheckerVersionComponent, 0DependencyChecker")
                    //
                    // seems to return the first instance of a 'CheckerVersionComponent' type it finds which is the one in
                    // the current DependencyChecker instance and wont match the stored component types AssemblyQualifiedName.
                    // This causes the stored value to return as null not allowing the latest version to be passed along and
                    // every instance running RunInitChecks() when only the first instance should.

                    // tl;dr: This bit is done this way because I spent too long trying to get a better way to work. It seems to
                    // work so any future changes should not break previous versions and only provide speed enhancements.

                    foreach (Assembly assembly in AllCheckerAssembliesExceptCurrent) { // We need to check all assemblies except current
                        var component = tokenObject.GetComponent(assembly.GetTypes().First(type => type.Name == typeof(CheckerVersionComponent).Name));
                        if (component != null) // If our version component was found, return it, else look in next assembly
                            {
                            return (Version)component.GetType().GetField("executingVersion").GetValue(component);
                        }
                    }
                }
                return null; // Return null if GameObject or component was not found
            }
            set { // Creates a GameObject and stores the latest checker assembly version
                GameObject gameObject = GameObject.Find(TokenObjectName);
                if (gameObject == null)
                    gameObject = new GameObject(TokenObjectName);
                gameObject.SetActive(true);
                gameObject.AddComponent(typeof(CheckerVersionComponent));
                CheckerVersionComponent component = gameObject.GetComponent<CheckerVersionComponent>();
                component.enabled = true;
                component.executingVersion = value;
            }
        }

        /// <summary>
        /// Schedules a dialog to display
        /// </summary>
        internal static void ScheduleDialog(Window dialog, bool tryRemoveOtherDialogs = false) {
            LongEventHandler.QueueLongEvent(() => {
                if (tryRemoveOtherDialogs) {
                    List<Window> toRemove = Find.WindowStack.Windows.ToList().FindAll(win => win.layer != WindowLayer.GameUI).ListFullCopy();
                    foreach (var win in toRemove) {
                        if (!Find.WindowStack.TryRemove(win))
                            Log.Message(CurrentAssemblyName + " failed to remove window of type" + win.GetType().Name);
                    }
                }
                Find.WindowStack.Add(dialog);
            }, null, false, null);
        }

        /// <summary>
        /// This is where the latest code will execute from
        /// </summary>
        private static void Execute() { // This is not version specific
            // TODO: Get our list of mods to work on out of our GameObject and do stuff with them
            var mods = CheckerEnabledMods;
            if (mods != null) {
                // TODO: need to check here if the mod has issues before sending to dialog, the UI should not be doing any of the work

                UI.Dialog_MissingAndOutDatedMods.CreateDialog(mods);
            }
            // We probably should clean up the GameObjects here. They should be destroyed on scene change and when
            // RimWorld closes/restarts so they should not exist past the menus I believe, but better to be safe than
            // sorry I guess.
            Log.Message(string.Format("We are executing on version {0}", CurrentAssemblyVersion.ToString()));
        }

        /// <summary>
        /// Runs initial checks and creates our GameObjects for later instances
        ///
        /// Should only be invoked on the first instance created
        /// </summary>
        /// <returns>The version that should execute</returns>
        private static Version RunInitChecks() {
            var checkerAssemblyName = Assembly.GetExecutingAssembly().GetName(); // Hold our AssemblyName
            var relatedModIDs = new List<string>(); // Hold a list of mods that are related to our checker
            Version latestCheckerVersion = checkerAssemblyName.Version; // Hold the version of the latest assembly

            foreach (ModContentPack modContentPack in LoadedModManager.RunningMods) {
                foreach (var assembly in modContentPack.assemblies.loadedAssemblies) {
                    if (assembly.GetName().Name == checkerAssemblyName.Name) {
                        relatedModIDs.Add(modContentPack.Identifier); // Add this mod to our list
                        if (assembly.GetName().Version > latestCheckerVersion)
                            latestCheckerVersion = assembly.GetName().Version; // This assembly version is newer
                        break; // Already found a version of our assembly in this modpack
                    }
                }
            }

            // Create our GameObjects to be accessed by another instance
            LatestCheckerVersion = latestCheckerVersion;
            CheckerEnabledMods = relatedModIDs;

            return latestCheckerVersion; // Return the latest version so we know if we should execute on this instance
        }
    }

    /// <summary>
    /// This is where we all the identifiers for mods we need to work with is stored within a GameObject Component
    /// </summary>
    public class CheckerEnabledModsComponent : MonoBehaviour {
        public List<string> checkerEnabledModsIdentifiers = new List<string>();
    }

    /// <summary>
    /// This is where the latest version is stored within a GameObject Component
    /// </summary>
    public class CheckerVersionComponent : MonoBehaviour {
        public Version executingVersion = new Version();
    }
}