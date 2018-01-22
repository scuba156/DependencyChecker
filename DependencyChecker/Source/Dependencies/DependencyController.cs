using DependencyChecker.Dependencies.SupportedFiles;
using DependencyChecker.UI;
using DependencyChecker.Utils;
using System.Collections.Generic;
using Verse;

namespace DependencyChecker.Dependencies {

    internal static class DependencyController {
        internal static List<DependencyContainer> Dependencies { get; set; }

        /// <summary>
        /// Currently always shows the dialog for debug purposes
        /// </summary>
        /// <param name="mods"></param>
        internal static void Start(List<string> mods) {
            ParseRelevantModFiles(mods);

            if (Dependencies.Count > 0) {
                LanguageUtility.LoadAllLanguages();
                RimWorldUtility.ScheduleDialog(new Dialog_DependencyChecker(Dependencies.FindAll(d => d.CurrentStatus != StatusType.Enabled)), true);
            }
        }

        private static void AddDependency(DependencyMetaData dependency, ModMetaData dependent) {
            DependencyContainer existing = Dependencies.Find(d => d.RequiredMod.Name == dependency.Name);
            if (existing == null) {
                DependencyContainer newDependency = new DependencyContainer(dependency);
                newDependency.DependentMods.Add(dependent);
                Dependencies.Add(newDependency);
            } else {
                if (!existing.DependentMods.Contains(dependent)) {
                    if (existing.RequiredMod.FriendlyName.NullOrEmpty()) {
                        existing.RequiredMod.FriendlyName = dependency.FriendlyName;
                    }
                    if (existing.RequiredMod.SteamID > 0) {
                        existing.RequiredMod.SteamID = dependency.SteamID;
                    }
                    existing.DependentMods.Add(dependent);
                }
            }
        }

        private static void ParseRelevantModFiles(List<string> relevantMods) {
            Dependencies = new List<DependencyContainer>();
            foreach (var identifier in relevantMods) {
                var mod = ModUtility.GetModByIdentifier(identifier);
                if (mod != null) {
                    ReadHugsLibVersionFile(mod);
                    ReadDependencyFile(mod);
                }
            }
        }

        private static void ReadDependencyFile(ModMetaData mod) {
            DependenciesFile file = DependenciesFile.TryParseFile(mod.RootDir.FullName);
            if (file != null) {
                foreach (var dependency in file.Dependencies) {
                    AddDependency(dependency, mod);
                }
            }
        }

        private static void ReadHugsLibVersionFile(ModMetaData mod) {
            HugsLibVersionFile hugslibVersion = HugsLibVersionFile.TryParseVersionFile(mod.RootDir.FullName);
            if (hugslibVersion != null) {
                ulong hugslibID = HugsLibVersionFile.SteamIDA17;
                if (hugslibVersion.RequiredLibraryVersion <= HugsLibVersionFile.A16Version) {
                    hugslibID = HugsLibVersionFile.SteamIDA16;
                }
                AddDependency(new DependencyMetaData(HugsLibVersionFile.Identifier, hugslibID, "HugsLib", hugslibVersion.RequiredLibraryVersion), mod);
            }
        }
    }
}