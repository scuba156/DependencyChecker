using DependencyChecker.SupportedFiles;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace DependencyChecker.UI {

    internal class Dialog_MissingAndOutDatedMods : Window {
        public override Vector2 InitialSize { get { return new Vector2(350, 400); } }

        private List<ModMetaData> workingMods = new List<ModMetaData>();
        private Dictionary<string, DependenciesFile> dependencyFiles;

        public override void DoWindowContents(Rect inRect) {
            GUI.BeginGroup(inRect);

            // Description
            string description = "There are active mods which depend on other mods that are either outdated, inactive or not installed.\n\nPlease resolve all issues before continuing.";
            Rect descRect = new Rect(0f, 0f, inRect.width, 90f);
            Widgets.Label(descRect, description);

            // Need per mod:
            //      - mod name
            //      - list of inactive/missing/outdated dependencies
            //      - button to disable mod
            //
            // Need per mod dependency
            //      if inactive
            //          - Button to activate
            //      if missing/outdated
            //          - version required
            //          - FloatMenu with download links
            //                  - Ludeon forum topic link
            //                  - GitHub link
            //                  - Steam workshop link (if steam is available)
            //
            // Only allow user to continue and restart once all issues are resolved

            GUI.EndGroup();
        }

        public override void PreOpen() {
            base.PreOpen();

            dependencyFiles = new Dictionary<string, DependenciesFile>();
            foreach (var mod in workingMods) {
                DependenciesFile file = DependenciesFile.TryParseFile(mod.RootDir.FullName);
                if (file != null) {
                    dependencyFiles.Add(mod.Identifier, file);
                }
            }
        }

        private void DetermineInitialSize() {
        }

        public Dialog_MissingAndOutDatedMods(List<ModMetaData> mods) {
            this.optionalTitle = "Dependency issues need to be resolved";
            this.absorbInputAroundWindow = true;
            this.closeOnClickedOutside = false;
            this.closeOnEscapeKey = false;
            this.doCloseButton = false;
            this.doCloseX = false;
            this.draggable = false;
            this.focusWhenOpened = true;
            this.forcePause = true;
            this.layer = WindowLayer.Dialog;
            this.onlyOneOfTypeAllowed = true;

            workingMods = mods;
        }

        public static void CreateDialog(List<string> identifiers) {
            if (ModLister.AllInstalledMods == null) {
                Log.Message("Installedmods is null");
            }

            CreateDialog(ModLister.AllInstalledMods.ToList().FindAll(m => identifiers.Contains(m.Identifier)));
        }

        public static void CreateDialog(List<ModMetaData> mods) {
            Main.ScheduleDialog(new Dialog_MissingAndOutDatedMods(mods), true);
        }
    }
}