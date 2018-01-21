using System;
using Verse;

namespace DependencyChecker.Dependencies {

    public class DependencyMetaData {

        public DependencyMetaData() {
        }

        public DependencyMetaData(string name, ulong steamID, string friendlyName, Version requiredVersion = null) {
            //Required
            this.Name = name;

            //Optional
            this.SteamID = steamID;
            this.RequiredVersion = requiredVersion;
            this.FriendlyName = friendlyName;
            this.RelatedModMetaData = Utils.ModUtility.GetModByName(name);
        }

        public string FriendlyName { get; internal set; }
        public string Name { get; internal set; }
        public Version InstalledVersion { get; internal set; }
        public string PrettyName { get { if (FriendlyName.NullOrEmpty()) return this.Name; return FriendlyName; } }
        public ModMetaData RelatedModMetaData { get; internal set; }
        public Version RequiredVersion { get; internal set; }
        public ulong SteamID { get; internal set; }
    }
}