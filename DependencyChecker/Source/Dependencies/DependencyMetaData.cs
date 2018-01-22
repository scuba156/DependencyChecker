using System;
using Verse;

namespace DependencyChecker.Dependencies {

    internal class DependencyMetaData {

        internal DependencyMetaData() {
        }

        internal DependencyMetaData(string name, ulong steamID, string friendlyName, Version requiredVersion = null) {
            //Required
            this.Name = name;

            //Optional
            this.SteamID = steamID;
            this.RequiredVersion = requiredVersion;
            this.FriendlyName = friendlyName;
            this.RelatedModMetaData = Utils.ModUtility.GetModByName(name);
        }

        internal string FriendlyName { get; set; }
        internal Version InstalledVersion { get; set; }
        internal string Name { get; set; }
        internal string PrettyName { get { if (FriendlyName.NullOrEmpty()) return this.Name; return FriendlyName; } }
        internal ModMetaData RelatedModMetaData { get; set; }
        internal Version RequiredVersion { get; set; }
        internal ulong SteamID { get; set; }
    }
}