using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DependencyChecker.Dependencies {
    public class DependencyMetaData {
        public string Identifier { get; internal set; }
        public string SteamID { get; internal set; }
        public Version RequiredVersion { get; internal set; }

        public DependencyMetaData() { }

        public DependencyMetaData(string identifier, string steamID, Version requiredVersion = null) {
            this.Identifier = identifier;
            this.SteamID = steamID;
            this.RequiredVersion = requiredVersion;
        }
    }
}
