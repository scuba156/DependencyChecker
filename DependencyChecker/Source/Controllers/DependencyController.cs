using DependencyChecker.SupportedFiles;
using DependencyChecker.UI;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace DependencyChecker.Controllers {

    public enum IssueType { Missing, Inactive, Outdated }

    public static class DependencyController {

        public static void Start(List<string> mods) {
            Log.Message("Starting with " + mods.Count + "mods");
            Dialog_MissingAndOutDatedMods.CreateDialog(ModsWithIssues(mods));
        }

        private static List<DependencyHolder> ModsWithIssues(List<string> mods) {
            List<DependencyHolder> dependencies = new List<DependencyHolder>();
            List<ModMetaData> currentActiveMods = ModsConfig.ActiveModsInLoadOrder.ToList();

            foreach (var item in mods) {
                ModMetaData checkerEnabledMod = ModsConfig.ActiveModsInLoadOrder.First(mod => mod.Identifier == item);

                if (checkerEnabledMod == null) {
                    Log.Message(item + " doesn't seem to be active");
                    continue;
                }
                Log.Message("Looking at " + checkerEnabledMod.Name + "'s dependency file");

                DependenciesFile dependencyFile = DependenciesFile.TryParseFile(checkerEnabledMod.RootDir.FullName);
                if (dependencyFile != null) {
                    Log.Message("There are " + dependencyFile.Dependencies.Count + " dependencies");
                    foreach (var dependency in dependencyFile.Dependencies) {
                        DependencyHolder depHolder = new DependencyHolder();//dependencies.Find(d => d.mod == dependency);
                        depHolder.mod = dependency;

                        foreach (var dep in dependencies.FindAll(dep => dep.mod.Identifier == dependency.Identifier)) {
                            depHolder.modsWithThisDependency.Add(checkerEnabledMod);
                        }
                        DependencyHolder existing = dependencies.Find(dep=>dep.mod == depHolder.mod);

                        if (existing == null) {
                            Log.Message("Adding " + depHolder.mod.Identifier + " to dependencies");
                            dependencies.Add(depHolder);
                        } else {
                            foreach (var mod in depHolder.modsWithThisDependency) {
                                if (!existing.modsWithThisDependency.Contains(mod)) {
                                    existing.modsWithThisDependency.Add(mod);
                                }
                            }
                        }

                    }
                }
                else {
                    Log.Message("No valid dependency file");
                }
            }
            return dependencies;
        }
    }

    public class DependencyHolder {
        public DependencySaveableData mod = new DependencySaveableData(); // this is the dependency that is causing the issue
        public List<ModMetaData> modsWithThisDependency = new List<ModMetaData>();
    }
}