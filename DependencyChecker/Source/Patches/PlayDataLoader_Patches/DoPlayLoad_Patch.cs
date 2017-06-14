using Harmony;
using Verse;

namespace DependencyChecker.Patches.PlayDataLoader_Patches {

    [HarmonyPatch]
    [HarmonyPatch(typeof(PlayDataLoader))]
    [HarmonyPatch("DoPlayLoad")]
    public static class DoPlayLoad_Patch {

        public static void Prefix() {
        }
    }
}