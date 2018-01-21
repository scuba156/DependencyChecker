using System.Collections.Generic;
using System.Linq;
using Verse;

namespace DependencyChecker.Dependencies {

    public enum StatusType { Enabled, DependentsDisabled, RequiredDisabled, RequiredMissing }

    public class DependencyContainer {
        public DependencyContainer(DependencyMetaData requiredMod) {
            RequiredMod = requiredMod;
            DependentMods = new List<ModMetaData>();
            UpdateStatus();
        }

        public void UpdateStatus() {
            if (CurrentStatus != StatusType.DependentsDisabled) {
                if (RequiredMod.RelatedModMetaData == null) {
                    CurrentStatus = StatusType.RequiredMissing;
                } else {
                    if (!RequiredMod.RelatedModMetaData.Active) {
                        CurrentStatus = StatusType.RequiredDisabled;
                    } else {
                        CurrentStatus = StatusType.Enabled;
                    }
                }
            }
        }

        public List<ModMetaData> DependentMods { get; private set; }
        public StatusType CurrentStatus { get; set; }
        public bool IssueResolved { get; set; }
        public DependencyMetaData RequiredMod { get; private set; }

        public void AddDependent(ModMetaData mod) {
            if (!DependentMods.Contains(mod)) {
                DependentMods.Add(mod);
            }
        }
    }
}