using System.Collections.Generic;
using System.Linq;
using Verse;

namespace DependencyChecker.Utils {

    internal static class CommonUtils {

        /// <summary>
        /// Schedules a dialog to display
        /// </summary>
        internal static void ScheduleDialog(Window dialog, bool tryRemoveOtherDialogs = false) {
            LongEventHandler.QueueLongEvent(() => {
                if (tryRemoveOtherDialogs) {
                    List<Window> toRemove = Find.WindowStack.Windows.ToList().FindAll(win => win.layer != WindowLayer.GameUI).ListFullCopy();
                    foreach (var win in toRemove) {
                        if (!Find.WindowStack.TryRemove(win))
                            Log.Message(AssemblyUtils.CurrentAssemblyName + " failed to remove window of type" + win.GetType().Name);
                    }
                }
                Find.WindowStack.Add(dialog);
            }, null, false, null);
        }
    }
}