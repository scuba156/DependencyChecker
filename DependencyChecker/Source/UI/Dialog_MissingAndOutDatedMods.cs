using DependencyChecker.Dependencies;
using DependencyChecker.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Steam;

namespace DependencyChecker.UI {

    internal class Dialog_MissingAndOutDatedMods : Window {
        private List<DependencyContainer> AllDependencies = new List<DependencyContainer>();
        private Vector2 scrollPosition;

        public Dialog_MissingAndOutDatedMods(List<DependencyContainer> dependencies) {
            this.optionalTitle = "Dependency issues need to be resolved";
            this.absorbInputAroundWindow = true;
            this.closeOnClickedOutside = false;
            this.closeOnEscapeKey = false;
            this.doCloseButton = true;
            this.doCloseX = false;
            this.draggable = false;
            this.focusWhenOpened = true;
            this.forcePause = true;
            this.layer = WindowLayer.Dialog;
            this.onlyOneOfTypeAllowed = true;

            AllDependencies = dependencies;
        }

        public override Vector2 InitialSize { get { return new Vector2(500, 700); } }
        private Vector2 ItemSize { get { return new Vector2(400f, 120f); } }

        public static void CreateDialog(List<DependencyContainer> dependencies) {
            CommonUtils.ScheduleDialog(new Dialog_MissingAndOutDatedMods(dependencies), true);
        }

        public override void Close(bool doCloseSound = true) {
            GenCommandLine.Restart();
        }

        public override void DoWindowContents(Rect inRect) {
            GUI.BeginGroup(inRect);

            // Description
            string description = "There are active mods which depend on other mods that are either outdated, inactive or not installed.\n\nPlease resolve all issues before continuing.";
            Rect descRect = new Rect(0f, 0f, inRect.width, 90f);
            Widgets.Label(descRect, description);

            // Dependency list
            Rect listRect = new Rect(7f, descRect.yMax + 5f, inRect.width, inRect.height - descRect.height - 38f - 5f);
            DrawList(listRect);

            GUI.EndGroup();


            // Need per mod dependency
            //      - list of dependant mods
            //      if inactive
            //          - Button to activate
            //      if missing/outdated
            //          - version required
            //          - FloatMenu with download links
            //                  - Ludeon forum topic link
            //                  - GitHub link
            //                  - Steam workshop link (if steam is available)
            //
            //
            // Need per mod:
            //      - mod name
            //      - button to disable mod
            // Only allow user to continue and restart once all issues are resolved
        }

        public override void PreOpen() {
            base.PreOpen();
        }

        private Action ActivateModAction(DependencyMetaData dependency) {
            return new Action(() => {
                ModMetaData mod = ModLister.AllInstalledMods.First(m => m.Identifier == dependency.Identifier | m.Identifier == dependency.SteamID);
                ModsConfig.SetActive(mod, !mod.Active);
            });
        }

        private Action DownloadlinksAction(DependencyMetaData dependency) {
            return new Action(() => {
                List<FloatMenuOption> options = new List<FloatMenuOption>();

                options.Add(new FloatMenuOption("Ludeon link?", null));
                options.Add(new FloatMenuOption("Github link?", null));
                if (SteamManager.Initialized && SteamManager.Active) {
                    options.Add(new FloatMenuOption("Steam link", null));
                }

                FloatMenu linksMenu = new FloatMenu(options);
                Find.WindowStack.Add(linksMenu);
            });
        }

        private void DrawDependencyItem(Rect itemRect, DependencyContainer dependency) {
            GameFont origFont = Text.Font;
            TextAnchor origAnchor = Text.Anchor;
            Widgets.DrawMenuSection(itemRect, true);
            Rect contentRect = itemRect.ContractedBy(5f);

            // Title
            Text.Font = GameFont.Medium;
            Rect titleRect = new Rect(contentRect.xMin, contentRect.yMin, contentRect.width, 32f);
            Widgets.Label(titleRect, dependency.RequiredMod.Identifier);
            Widgets.DrawLineHorizontal(titleRect.xMin, titleRect.yMax, Text.CalcSize(dependency.RequiredMod.Identifier).x + 10f);
            titleRect.y += 30f;
            Text.Font = GameFont.Small;
            Widgets.Label(titleRect, dependency.Issue.ToString());

            // Required version
            Text.Font = GameFont.Tiny;
            if (dependency.RequiredMod.RequiredVersion != null) {
                Text.Anchor = TextAnchor.UpperRight;
                Rect requiredVersionRect = new Rect(contentRect.xMax - 90f, contentRect.yMin, 90f, 23f);
                Widgets.Label(requiredVersionRect, "Requires v" + dependency.RequiredMod.RequiredVersion);
            }

            // Required by
            Text.Anchor = origAnchor;
            string s = "Required by:";
            foreach (var item in dependency.DepentantMods) {
                s += " " + item.Name + ",";
            }

            Rect dependentsRect = new Rect(contentRect.xMin, contentRect.yMax - 23f, contentRect.width, 23f);
            Widgets.Label(dependentsRect, s.TrimEnd(','));

            Text.Font = origFont;
            Text.Anchor = origAnchor;
        }

        private void DrawList(Rect rect) {
            float height = AllDependencies.Count * 34 + 300f;
            Rect scrollOuter = rect;
            Rect scrollInner = new Rect(0f, 26f, scrollOuter.width - 16f, AllDependencies.Count * (ItemSize.y + 12f));
            Rect content = scrollInner.ContractedBy(4f);
            Widgets.BeginScrollView(rect, ref scrollPosition, scrollInner, true);

            Listing_Standard listing = new Listing_Standard();
            listing.Begin(content);

            foreach (var item in from d in AllDependencies
                                 orderby d.Issue == IssueType.Inactive
                                 select d) {
                DrawDependencyItem(listing.GetRect(ItemSize.y), item);
                listing.Gap();
            }
            listing.End();
            Widgets.EndScrollView();
        }
    }
}