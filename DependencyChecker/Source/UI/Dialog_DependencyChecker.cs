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

    internal class Dialog_DependencyChecker : Window {
        private Vector2 scrollPosition;
        public Dialog_DependencyChecker(List<DependencyContainer> dependencies) {
            this.absorbInputAroundWindow = true;
            this.closeOnEscapeKey = false;

            AllDependencies = dependencies;
        }

        public override Vector2 InitialSize { get { return new Vector2(500, 600); } }
        private List<DependencyContainer> AllDependencies { get; set; }
        private Vector2 ItemSize { get { return new Vector2(400f, 90f); } }
        private Dictionary<string, string> truncatedDependencyNamesCache = new Dictionary<string, string>();
        private Dictionary<string, string> truncatedRequiredByModsCache = new Dictionary<string, string>();

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
            Rect descRect = new Rect(17f, titleRect.yMax + 15f, inRect.width - 33f , 90f);
            Widgets.Label(descRect, "MainDescription".Translate());

            // Dependency list
            Rect listRect = new Rect(7f, descRect.yMax + 5f, inRect.width, inRect.height - descRect.yMax - 55f);
            DrawList(listRect);

            // Ignore button
            Rect ignoreButtonRect = new Rect(17f, inRect.yMax - 40f, this.CloseButSize.x, this.CloseButSize.y);
            if (Widgets.ButtonText(ignoreButtonRect, "IgnoreButton".Translate())) {
                //TODO: Garbage collection
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

        private void DrawItem(Rect inRect, DependencyContainer current) {
            Color origColor = GUI.color;
            Widgets.DrawMenuSection(inRect, true);
            Rect contentRect = inRect.ContractedBy(5f);

            Text.Font = GameFont.Medium;
            string title = current.RequiredMod.PrettyName.Truncate(130f, truncatedDependencyNamesCache);
            string version = string.Empty;
            if (current.RequiredMod.RequiredVersion != null) {
                version = "v" + current.RequiredMod.RequiredVersion;
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
            string issueString = current.CurrentStatus.ToString().Translate();
            GUI.color = Color.red;
            Text.Anchor = TextAnchor.UpperRight;
            if (current.IssueResolved) {
                GUI.color = Color.green;
                if (current.CurrentStatus == StatusType.RequiredDisabled) {
                    issueString = "Enabled".Translate();
                } else if (current.CurrentStatus == StatusType.RequiredMissing) {
                    issueString = "Installed".Translate();
                }
            }
            Widgets.Label(issueRect, issueString);
            GUI.color = origColor;
            Text.Anchor = TextAnchor.UpperLeft;

            // Fix action button
            Rect fixButtonRect = new Rect(contentRect.xMax - 90f, contentRect.yMax - 30f, 90f, 30f);
            DrawFixButton(fixButtonRect, current);

            // Find button
            Rect findButtonRect = new Rect(fixButtonRect.xMin - 95f, fixButtonRect.y, 90f, 30f);
            if (Widgets.ButtonText(findButtonRect, "Find".Translate())) {
                List<FloatMenuOption> options = new List<FloatMenuOption> {
                    new FloatMenuOption("FindOn".Translate("LudeonForums".Translate()), new Action(() => { Application.OpenURL("https://ludeon.com/forums/index.php?action=search2&advanced=1&searchtype=1&sort=relevance|desc&brd[15]=15&brd[16]=16&search=" + WWW.EscapeURL(current.RequiredMod.Name)); })),
                    new FloatMenuOption("FindOn".Translate("Github".Translate()), new Action(() => { Application.OpenURL("https://github.com/search?utf8=%E2%9C%93&type=Repositories&q=" + WWW.EscapeURL(current.RequiredMod.Name)); }))
                };
                if (ModUtility.CanSubscribe(current.RequiredMod.SteamID)) {
                    options.Add(new FloatMenuOption("OpenOnSteam".Translate(), new Action(() => { SteamUtility.OpenWorkshopPage(new PublishedFileId_t(current.RequiredMod.SteamID)); })));
                } else {
                    options.Add(new FloatMenuOption("FindOn".Translate("Steam".Translate()), new Action(() => { Application.OpenURL("http://steamcommunity.com/workshop/browse/?appid=294100&browsesort=textsearch&searchtext=" + WWW.EscapeURL(current.RequiredMod.Name)); })));
                }
                if (current.RequiredMod.RelatedModMetaData != null && !current.RequiredMod.RelatedModMetaData.Url.NullOrEmpty()) {
                    options.Add(new FloatMenuOption("OpenModURL".Translate(), new Action(() => { Application.OpenURL(WWW.EscapeURL(current.RequiredMod.RelatedModMetaData.Url)); })));
                }

                Find.WindowStack.Add(new FloatMenu(options));
            }

            // Required by Label
            if (current.DependentMods != null) {
                Text.Font = GameFont.Tiny;
                Text.Anchor = TextAnchor.UpperLeft;
                Rect requiredLabelRect = new Rect(contentRect.xMin, contentRect.yMax - 15f, 120f, 20f);
                TooltipHandler.TipRegion(requiredLabelRect, "TooltipDependentsCount".Translate(current.RequiredMod.PrettyName, string.Join("\n", current.DependentMods.Select(m => m.Name).ToArray())));
                if (Widgets.ButtonText(requiredLabelRect, "RequiredCount".Translate(current.DependentMods.Count).Truncate(requiredLabelRect.width, truncatedRequiredByModsCache), false)) {
                    Find.WindowStack.Add(new Dialog_RequiredBy(current));
                }
            }

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.color = origColor;
        }

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
                DrawItem(listing.GetRect(ItemSize.y), item);
                listing.Gap();
            }

            listing.End();
            Widgets.EndScrollView();
        }
    }
}