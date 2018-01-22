using System.Collections.Generic;
using Verse;

namespace DependencyChecker.Dependencies {

    internal enum StatusType { Enabled, DependentsDisabled, RequiredDisabled, RequiredMissing }

    /// <summary>
    /// A class to hold a dependency, mods that depend on it, and what the issue is
    /// </summary>
    internal class DependencyContainer {

        internal DependencyContainer(DependencyMetaData requiredMod) {
            RequiredMod = requiredMod;
            DependentMods = new List<ModMetaData>();
            UpdateStatus();
        }

        /// <summary>
        /// Determines the current status of the RequiredMod
        /// </summary>
        internal void UpdateStatus() {
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

        internal List<ModMetaData> DependentMods { get; private set; }
        internal StatusType CurrentStatus { get; set; }
        internal bool IssueResolved { get; set; }
        internal DependencyMetaData RequiredMod { get; private set; }

        /// <summary>
        /// Add a mod that depends on RequiredMod to the current list
        /// </summary>
        /// <param name="mod"></param>
        internal void AddDependent(ModMetaData mod) {
            if (!DependentMods.Contains(mod)) {
                DependentMods.Add(mod);
            }
        }
    }
}