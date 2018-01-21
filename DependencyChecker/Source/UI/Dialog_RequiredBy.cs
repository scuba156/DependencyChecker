using DependencyChecker.Dependencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace DependencyChecker.UI {
    public class Dialog_RequiredBy : Window {
        DependencyContainer Current { get; set; }

        public override void DoWindowContents(Rect inRect) {

        }

        public Dialog_RequiredBy(DependencyContainer dependency) {
            this.closeOnEscapeKey = false;
            Current = dependency;
        }
    }
}
