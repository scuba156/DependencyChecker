using DependencyChecker.Dependencies;
using DependencyChecker.Utils;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace DependencyChecker.UI {

    internal class Dialog_MissingAndOutDatedMods : Window {
        public override Vector2 InitialSize { get { return new Vector2(500, 764); } }

        private List<DependencyContainer> dependencies = new List<DependencyContainer>();
        private Vector2 scrollPosition;

        public override void DoWindowContents(Rect inRect) {
            GUI.BeginGroup(inRect);

            // Description
            string description = "There are active mods which depend on other mods that are either outdated, inactive or not installed.\n\nPlease resolve all issues before continuing.";
            Rect descRect = new Rect(0f, 0f, inRect.width, 90f);
            Widgets.Label(descRect, description);


            Rect listRect = new Rect(0f, descRect.yMax + 5f, inRect.width, inRect.height - descRect.height - 38f - 5f);
            DrawList(listRect);






            GUI.EndGroup();

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
        }

        private void DrawList(Rect rect) {
            float height = dependencies.Count * 34 + 300f;
            Rect scrollOuter = rect;
            Rect scrollInner = new Rect(0f, 26f, scrollOuter.width - 16f, height);
            Rect content = scrollInner.ContractedBy(4f);
            Widgets.DrawMenuSection(scrollOuter, true);
            Widgets.BeginScrollView(rect, ref scrollPosition, scrollInner, true);

            Listing_Standard listing = new Listing_Standard();
            listing.Begin(content);

            foreach (var dep in dependencies) {
                DrawDependencyItem(listing, dep);
            }

            listing.End();
            Widgets.EndScrollView();
        }

        private void DrawDependencyItem(Listing_Standard listing, DependencyContainer dependency) {
            listing.Label(dependency.RequiredMod.Identifier);
        }

        public override void PreOpen() {
            base.PreOpen();
        }

        public Dialog_MissingAndOutDatedMods(List<DependencyContainer> mods) {
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

            dependencies = mods;
        }

        public static void CreateDialog(List<DependencyContainer> dependencies) {
            Log.Message("Creating dialog with " + dependencies.Count + " mods");
            CommonUtils.ScheduleDialog(new Dialog_MissingAndOutDatedMods(dependencies), true);
        }
    }
}