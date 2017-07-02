using System;

namespace DependencyChecker.Dependencies {

    public class DependencyMetaData {
        public DependencyMetaData() {
        }

        public DependencyMetaData(string identifier, string steamID, string friendlyName, Version requiredVersion = null) {
            this.Identifier = identifier;
            this.SteamID = steamID;
            this.RequiredVersion = requiredVersion;
            this.FriendlyName = friendlyName;
        }

        public string FriendlyName { get; internal set; }
        public string Identifier { get; internal set; }
        public Version RequiredVersion { get; internal set; }
        public string SteamID { get; internal set; }
    }
}