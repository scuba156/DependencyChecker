using DependencyChecker.Dependencies.SupportedFiles;
using DependencyChecker.UI;
using DependencyChecker.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace DependencyChecker.Dependencies {


    public static class DependencyController {

        static List<DependencyContainer> Dependencies = new List<DependencyContainer>();


        public static void Start(List<string> mods) {
            ParseRelevantModFiles(mods);

            if (Dependencies.Count > 0) {
                Dialog_MissingAndOutDatedMods.CreateDialog(Dependencies.FindAll(d=>d.Issue != IssueType.None));
            }
        }

        static void ParseRelevantModFiles(List<string> relevantMods) {
            foreach (var identifier in relevantMods) {
                var mod = CommonUtils.GetMod(identifier);
                if (mod != null) {
                    ReadHugsLibVersionFile(mod);
                    ReadDependencyFile(mod);
                }
            }
        }

        static void ReadDependencyFile(ModMetaData mod) {
            DependenciesFile file = DependenciesFile.TryParseFile(mod.RootDir.FullName);
            if (file != null) {
                foreach (var dependency in file.Dependencies) {
                    AddDependency(dependency, mod);
                }
            }
        }

        static void ReadHugsLibVersionFile(ModMetaData mod) {
            HugsLibVersionFile hugslibVersion = HugsLibVersionFile.TryParseVersionFile(mod.RootDir.FullName);
            if (hugslibVersion != null) {
                string steamID = HugsLibVersionFile.SteamIDA17;
                if (hugslibVersion.RequiredLibraryVersion <= HugsLibVersionFile.A16Version) {
                    steamID = HugsLibVersionFile.SteamIDA16;
                }
                AddDependency(new DependencyMetaData(HugsLibVersionFile.Identifier, steamID, hugslibVersion.RequiredLibraryVersion), mod);
            }
        }

        static void AddDependency(DependencyMetaData dependency, ModMetaData dependent) {
            DependencyContainer existing = Dependencies.Find(d => d.RequiredMod.Identifier == dependency.Identifier);
            if (existing == null) {
                DependencyContainer newDependency = new DependencyContainer(dependency);
                newDependency.DepentantMods.Add(dependent);
                Dependencies.Add(newDependency);
            } else {
                if (!existing.DepentantMods.Contains(dependent)) {
                    existing.DepentantMods.Add(dependent);
                }
            }
        }
    }
}