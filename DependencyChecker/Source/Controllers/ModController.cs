using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DependencyChecker.Controllers {
    internal class ModController {
        public ModMetaData GetMod(string identifier, bool onlyIfActive = false) {
            if(onlyIfActive)
                return ModsConfig.ActiveModsInLoadOrder.First(mod => mod.Identifier == identifier);
            return ModLister.AllInstalledMods.First(mod=>mod.Identifier == identifier);
        }
    }
}
