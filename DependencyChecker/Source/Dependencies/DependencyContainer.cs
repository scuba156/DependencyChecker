using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DependencyChecker.Dependencies {
    public enum IssueType { Missing, Inactive, Outdated, None }

    public class DependencyContainer {
        public DependencyMetaData RequiredMod { get; private set; }
        public List<ModMetaData> DepentantMods { get; private set; }
        public IssueType Issue { get; private set; }
        public bool SetActive { get; set; }
        public bool FixIssue { get; set; }
        public bool DisableDependends { get; set; }

        public DependencyContainer(DependencyMetaData requiredMod) {
            RequiredMod = requiredMod;
            DepentantMods = new List<ModMetaData>();

            //TODO: more work needed

            var mod = LoadedModManager.RunningMods.ToList().Find(m => m.Name.ToLower() == requiredMod.Identifier.ToLower());
            if (mod != null) {
                Issue = IssueType.None;
            } else {

                //foreach (var item in ModLister.AllInstalledMods) {
                //    Log.Message("Mod " + item.Identifier + ":" + item.Name);
                //}


                var meta = ModLister.AllInstalledMods.ToList().Find(m => m.Identifier.ToLower() == requiredMod.Identifier.ToLower() | m.Identifier == requiredMod.SteamID);
                if (meta != null) {
                    //TODO: find assembly and compare version
                    Issue = IssueType.Inactive;
                } else {
                    Issue = IssueType.Missing;
                }
            }
        }

        public void AddDependent(ModMetaData mod) {
            if (!DepentantMods.Contains(mod)) {
                DepentantMods.Add(mod);
            }
        }
    }
}
