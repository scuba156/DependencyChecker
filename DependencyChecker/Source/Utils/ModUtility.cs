using System.Linq;
using Verse;
using Verse.Steam;

namespace DependencyChecker.Utils {

    internal static class ModUtility {

        /// <summary>
        /// Checks if Steam is available
        /// </summary>
        /// <param name="steamID">I can't remember why this is needed exactly</param>
        /// <returns></returns>
        internal static bool CanSubscribe(ulong steamID) {
            if (steamID > 0) {
                if (SteamManager.Initialized && SteamManager.Active) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns a mods meta data if it exists, else returns null
        /// </summary>
        /// <param name="identifier">The identifier for the mod to get</param>
        /// <param name="onlyIfActive">Returns only mods that are currently active</param>
        /// <returns></returns>
        internal static ModMetaData GetModByIdentifier(string identifier, bool onlyIfActive = false) {
            if (onlyIfActive)
                return ModsConfig.ActiveModsInLoadOrder.ToList().Find(mod => mod.Identifier.ToLower() == identifier.ToLower());
            return ModLister.AllInstalledMods.ToList().Find(mod => mod.Identifier.ToLower() == identifier.ToLower());
        }

        /// <summary>
        /// Returns a mods meta data if it exists, else returns null
        /// </summary>
        /// <param name="name">The name of the mod to get</param>
        /// <param name="onlyIfActive">Returns only mods that are currently active</param>
        /// <returns></returns>
        internal static ModMetaData GetModByName(string name, bool onlyIfActive = false) {
            Log.Message(string.Format("Checking for '{0}', {1}", name, onlyIfActive));
            if (onlyIfActive)
                return ModsConfig.ActiveModsInLoadOrder.ToList().Find(mod => mod.Name.ToLower() == name.ToLower());
            return ModLister.AllInstalledMods.ToList().Find(mod => mod.Name.ToLower() == name.ToLower());
        }
    }
}