using System.Collections.Generic;
using System.Linq;
using Verse;

namespace DependencyChecker.Utils {

    internal static class RimWorldUtility {

        /// <summary>
        /// Schedules a Verse.Window to display
        /// </summary>
        /// <param name="dialog">The Verse.Window to show</param>
        /// <param name="tryRemoveOtherDialogs">Removes any other dialogs in the background for a cleaner look</param>
        internal static void ScheduleDialog(Window dialog, bool tryRemoveOtherDialogs = false) {
            // A better option would be to hold any background dialogs in memory and show them if/when the user closes
            // the dialog
            LongEventHandler.QueueLongEvent(() => {
                if (tryRemoveOtherDialogs) {
                    List<Window> toRemove = Find.WindowStack.Windows.ToList().FindAll(win => win.layer != WindowLayer.GameUI).ListFullCopy();
                    foreach (var win in toRemove) {
                        if (!Find.WindowStack.TryRemove(win, false))
                            Log.ErrorOnce(AssemblyUtility.CurrentAssemblyName + " failed to remove window of type" + win.GetType().Name, 156);
                    }
                    EditWindow_Log.wantsToOpen = false;
                }
                Find.WindowStack.Add(dialog);
            }, null, true, null);
        }
    }
}