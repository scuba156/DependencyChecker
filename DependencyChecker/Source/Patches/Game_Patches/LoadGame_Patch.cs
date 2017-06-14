using Harmony;
using Verse;

namespace DependencyChecker.Patches.Game_Patches {

    [HarmonyPatch]
    [HarmonyPatch(typeof(Game))]
    [HarmonyPatch("LoadGame")]
    public static class LoadGame_Patch {

        public static void Prefix(this Game __instance) {
        }
    }
}