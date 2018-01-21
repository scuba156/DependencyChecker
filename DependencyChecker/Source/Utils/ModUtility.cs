using System.Linq;
using Verse;
using Verse.Steam;

namespace DependencyChecker.Utils {

    internal static class ModUtility {

        public static bool CanSubscribe(ulong steamID) {
            if (steamID > 0) {
                if (SteamManager.Initialized && SteamManager.Active) {
                    return true;
                }
            }
            return false;
        }

        public static ModMetaData GetModByIdentifier(string identifier, bool onlyIfActive = false) {
            if (onlyIfActive)
                return ModsConfig.ActiveModsInLoadOrder.ToList().Find(mod => mod.Identifier.ToLower() == identifier.ToLower());
            return ModLister.AllInstalledMods.ToList().Find(mod => mod.Identifier.ToLower() == identifier.ToLower());
        }

        public static ModMetaData GetModByName(string name, bool onlyIfActive = false) {
            if (onlyIfActive)
                return ModsConfig.ActiveModsInLoadOrder.ToList().Find(mod => mod.Name.ToLower() == name.ToLower());
            return ModLister.AllInstalledMods.ToList().Find(mod => mod.Name.ToLower() == name.ToLower());
        }
    }
}