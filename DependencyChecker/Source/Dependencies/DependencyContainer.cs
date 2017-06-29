using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DependencyChecker.Dependencies {
    public enum IssueType { Missing, Inactive, Outdated }

    public class DependencyContainer {
        public DependencyMetaData RequiredMod { get; private set; }
        public List<ModMetaData> DepentantMods { get; private set; }
        public IssueType Issue { get; private set; }

        public DependencyContainer(DependencyMetaData requiredMod) {
            RequiredMod = requiredMod;
            DepentantMods = new List<ModMetaData>();

            //TODO: determine issue here
        }

        public void AddDependent(ModMetaData mod) {
            if (!DepentantMods.Contains(mod)) {
                DepentantMods.Add(mod);
            }
        }
    }
}
