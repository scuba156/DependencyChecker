using DependencyChecker.Dependencies;
using DependencyChecker.Utils;
using RimWorld;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace DependencyChecker.UI {

    /// <summary>
    /// Our main dialog window
    /// </summary>
    internal class Dialog_DependencyChecker : Window {
        private Vector2 scrollPosition;
        private Dictionary<string, string> truncatedDependencyNamesCache = new Dictionary<string, string>();
        private Dictionary<string, string> truncatedRequiredByModsCache = new Dictionary<string, string>();

        /// <summary>
        /// A Verse.Window to display a list of mods with issues and suggested fixes
        /// </summary>
        /// <param name="dependencies"></param>
        public Dialog_DependencyChecker(List<DependencyContainer> dependencies) {
            this.absorbInputAroundWindow = true;
            this.closeOnEscapeKey = false;

            AllDependencies = dependencies;
        }

        public override Vector2 InitialSize { get { return new Vector2(500, 600); } }
        private List<DependencyContainer> AllDependencies { get; set; }
        private Vector2 ItemSize { get { return new Vector2(400f, 90f); } }

        public override void DoWindowContents(Rect inRect) {
            GUI.BeginGroup(inRect);

            // Title
            Rect titleRect = new Rect(0f, 0f, inRect.width, 30f);
            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.UpperCenter;
            Widgets.Label(titleRect, "MainTitle".Translate());
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;

            // Description
            Rect descRect = new Rect(17f, titleRect.yMax + 15f, inRect.width - 33f, 90f);
            Widgets.Label(descRect, "MainDescription".Translate());

            // Dependency list
            Rect listRect = new Rect(7f, descRect.yMax + 5f, inRect.width, inRect.height - descRect.yMax - 55f);
            DrawList(listRect);

            // Ignore button
            Rect ignoreButtonRect = new Rect(17f, inRect.yMax - 40f, this.CloseButSize.x, this.CloseButSize.y);
            if (Widgets.ButtonText(ignoreButtonRect, "IgnoreButton".Translate())) {
                GameObjectUtility.TryDestroyGameObject();
                this.Close();
            }

            // Restart Button
            Rect restartButtonRect = new Rect((inRect.width - this.CloseButSize.x) / 2f, inRect.yMax - 40f, this.CloseButSize.x, this.CloseButSize.y);
            if (Widgets.ButtonText(restartButtonRect, "RestartButton".Translate())) {
                GenCommandLine.Restart();
            }

            // Mod Menu Button
            Rect modMenuButtonRect = new Rect(inRect.xMax - this.CloseButSize.x - 17f, inRect.yMax - 40f, this.CloseButSize.x, this.CloseButSize.y);
            if (Widgets.ButtonText(modMenuButtonRect, "ModMenuButton".Translate())) {
                Find.WindowStack.Add(new Page_ModsConfig());
            }

            GUI.EndGroup();
        }

        /// <summary>
        /// Draws a dependency row into the list
        /// </summary>
        /// <param name="inRect">The rect to draw into</param>
        /// <param name="dependency">The dependency the row item is for</param>
        private void DrawDependencyRow(Rect inRect, DependencyContainer dependency) {
            Color origColor = GUI.color;
            Widgets.DrawMenuSection(inRect);
            Rect contentRect = inRect.ContractedBy(5f);

            // Dependency Name
            Text.Font = GameFont.Medium;
            string title = dependency.RequiredMod.PrettyName.Truncate(130f, truncatedDependencyNamesCache);
            string version = string.Empty;
            if (dependency.RequiredMod.RequiredVersion != null) {
                version = "v" + dependency.RequiredMod.RequiredVersion;
            }
            float titleWidth = Text.CalcSize(title).x;
            float versionWidth = Text.CalcSize(version).x;
            Rect titleRect = new Rect(contentRect.xMin, contentRect.yMin, titleWidth, 32f);
            Widgets.Label(titleRect, title);

            Widgets.DrawLineHorizontal(titleRect.xMin, titleRect.yMax, titleWidth + versionWidth + 5f);

            // Version
            Text.Font = GameFont.Small;
            if (!version.NullOrEmpty()) {
                Rect versionRect = new Rect(titleRect.xMax + 2f, titleRect.yMax - 25f, 60f, 23f);
                Widgets.Label(versionRect, version);
            }

            // Issue label
            Rect issueRect = new Rect(contentRect.xMax - 110f, contentRect.yMin - 3f, 110f, 23f);
            string issueString = dependency.CurrentStatus.ToString().Translate();
            Text.Anchor = TextAnchor.UpperRight;
            if (dependency.IssueResolved) {
                GUI.color = Color.green;
            } else {
                GUI.color = Color.red;
            }

            switch (dependency.CurrentStatus) {
                case StatusType.RequiredDisabled:
                    issueString = "Enabled".Translate();
                    break;

                case StatusType.RequiredMissing:
                    issueString = "Installed".Translate();
                    break;

                case StatusType.DependentsDisabled:
                    issueString = "Disabled".Translate();
                    break;
            }
            Widgets.Label(issueRect, issueString);
            GUI.color = origColor;
            Text.Anchor = TextAnchor.UpperLeft;

            // Fix action button
            Rect fixButtonRect = new Rect(contentRect.xMax - 90f, contentRect.yMax - 30f, 90f, 30f);
            DrawFixButton(fixButtonRect, dependency);

            // Find button
            Rect findButtonRect = new Rect(fixButtonRect.xMin - 95f, fixButtonRect.y, 90f, 30f);
            if (Widgets.ButtonText(findButtonRect, "Find".Translate())) {
                List<FloatMenuOption> options = new List<FloatMenuOption> {
                    new FloatMenuOption("FindOn".Translate("LudeonForums".Translate()), new Action(() => { Application.OpenURL("https://ludeon.com/forums/index.php?action=search2&advanced=1&searchtype=1&sort=relevance|desc&brd[15]=15&brd[16]=16&search=" + WWW.EscapeURL(dependency.RequiredMod.Name)); })),
                    new FloatMenuOption("FindOn".Translate("Github".Translate()), new Action(() => { Application.OpenURL("https://github.com/search?utf8=%E2%9C%93&type=Repositories&q=" + WWW.EscapeURL(dependency.RequiredMod.Name)); }))
                };
                if (ModUtility.CanSubscribe(dependency.RequiredMod.SteamID)) {
                    options.Add(new FloatMenuOption("OpenOnSteam".Translate(), new Action(() => { SteamUtility.OpenWorkshopPage(new PublishedFileId_t(dependency.RequiredMod.SteamID)); })));
                } else {
                    options.Add(new FloatMenuOption("FindOn".Translate("Steam".Translate()), new Action(() => { Application.OpenURL("http://steamcommunity.com/workshop/browse/?appid=294100&browsesort=textsearch&searchtext=" + WWW.EscapeURL(dependency.RequiredMod.Name)); })));
                }
                if (dependency.RequiredMod.RelatedModMetaData != null && !dependency.RequiredMod.RelatedModMetaData.Url.NullOrEmpty()) {
                    options.Add(new FloatMenuOption("OpenModURL".Translate(), new Action(() => { Application.OpenURL(WWW.EscapeURL(dependency.RequiredMod.RelatedModMetaData.Url)); })));
                }

                Find.WindowStack.Add(new FloatMenu(options));
            }

            // Required by Label
            if (dependency.DependentMods != null) {
                Text.Font = GameFont.Tiny;
                Text.Anchor = TextAnchor.UpperLeft;
                Rect requiredLabelRect = new Rect(contentRect.xMin, contentRect.yMax - 15f, 120f, 20f);
                TooltipHandler.TipRegion(requiredLabelRect, "TooltipDependentsCount".Translate(dependency.RequiredMod.PrettyName, string.Join("\n", dependency.DependentMods.Select(m => m.Name).ToArray())));
                if (Widgets.ButtonText(requiredLabelRect, "RequiredCount".Translate(dependency.DependentMods.Count).Truncate(requiredLabelRect.width, truncatedRequiredByModsCache), false)) {
                    Find.WindowStack.Add(new Dialog_RequiredBy(dependency));
                }
            }

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.color = origColor;
        }

        /// <summary>
        /// Draws the button that lists suggested fixes and handles clicks
        ///
        /// **Needs work
        /// Currently does actually do any fixes, they are easy to add. I had not yet decided
        /// on the design so its unfinished
        /// </summary>
        /// <param name="rect">The Rect to draw into</param>
        /// <param name="dependency">The dependency the button is for</param>
        private void DrawFixButton(Rect rect, DependencyContainer dependency) {
            Color orig = GUI.color;
            Color buttonColor = Color.green;
            string label = string.Empty;
            bool buttonDisabled = false;

            List<FloatMenuOption> options = new List<FloatMenuOption>();

            //    fixAction = new Action(() => { current.IssueResolved = !current.IssueResolved; ModsConfig.SetActive(current.RequiredMod.RelatedModMetaData, current.IssueResolved); ModsConfig.Save(); });
            //} else if (ModUtility.CanSubscribe(current.RequiredMod.SteamID)) {
            //    fixAction = new Action(() => { SteamUGC.SubscribeItem(new PublishedFileId_t(current.RequiredMod.SteamID)); current.IssueResolved = true; });
            //}

            if (dependency.CurrentStatus != StatusType.RequiredMissing && Widgets.ButtonText(rect, "FixButton".Translate(), true, true, !buttonDisabled)) {
                if (dependency.CurrentStatus == StatusType.DependentsDisabled) {
                    options.Add(new FloatMenuOption("Enable Dependents", null));
                } else {
                    options.Add(new FloatMenuOption("Disable Dependents", null));
                    if (dependency.CurrentStatus == StatusType.RequiredDisabled) {
                        options.Add(new FloatMenuOption("Enable Required", null));
                    } else {
                        options.Add(new FloatMenuOption("Disable Required", null));
                    }
                }
                Find.WindowStack.Add(new FloatMenu(options));
            }
            GUI.color = orig;
        }

        /// <summary>
        /// Draws the list of dependencies
        /// </summary>
        /// <param name="rect">The Rect to draw into</param>
        private void DrawList(Rect rect) {
            Rect scrollOuter = rect;
            Rect scrollInner = new Rect(0f, 26f, scrollOuter.width - 16f, AllDependencies.Count * (ItemSize.y + 12f));

            if (scrollInner.height > rect.height) {
                scrollOuter.width -= 7f;
                scrollInner.width -= 7f;
            }

            Rect content = scrollInner.ContractedBy(6f);
            Listing_Standard listing = new Listing_Standard();
            Widgets.BeginScrollView(scrollOuter, ref scrollPosition, scrollInner, true);
            listing.Begin(content);

            foreach (var item in from d in AllDependencies
                                 orderby d.CurrentStatus == StatusType.RequiredDisabled
                                 select d) {
                DrawDependencyRow(listing.GetRect(ItemSize.y), item);
                listing.Gap();
            }

            listing.End();
            Widgets.EndScrollView();
        }
    }
}