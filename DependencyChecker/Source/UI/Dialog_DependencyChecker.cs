using UnityEngine;
using Verse;

namespace DependencyChecker.UI {

    internal class Dialog_DependencyChecker : Window {
        public override Vector2 InitialSize { get { return new Vector2(300, 400); } }

        public override void DoWindowContents(Rect inRect) {
        }

        public override void PreOpen() {
            base.PreOpen();
        }

        private void DetermineInitialSize() {
        }

        public Dialog_DependencyChecker() {
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
        }

        private static void CreateDialog() {
            Find.WindowStack.Add(new Dialog_DependencyChecker());
        }
    }
}