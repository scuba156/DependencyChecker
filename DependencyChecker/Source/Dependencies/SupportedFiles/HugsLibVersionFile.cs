using System;
using System.IO;
using System.Xml.Linq;
using Verse;

namespace DependencyChecker.Dependencies.SupportedFiles {

    public class HugsLibVersionFile {
        public const string VersionFileDir = "About";
        public const string VersionFileName = "Version.xml";

        public static readonly string Identifier = "HugsLib";
        public static readonly string SteamIDA16 = "";
        public static readonly string SteamIDA17 = "";
        public static readonly Version A16Version = new Version(2, 4, 5);

        private HugsLibVersionFile(XDocument doc) {
            ParseXmlDocument(doc);
        }

        public Version OverrideVersion { get; private set; }

        public Version RequiredLibraryVersion { get; private set; }

        public static HugsLibVersionFile TryParseVersionFile(string rootDir) {
            var filePath = Path.Combine(rootDir, Path.Combine(VersionFileDir, VersionFileName));
            if (!File.Exists(filePath)) return null;
            try {
                var doc = XDocument.Load(filePath);
                return new HugsLibVersionFile(doc);
            } catch (Exception e) {
                Log.Error("[DependencyChecker] Exception while parsing version file at path: " + filePath + " Exception was: " + e);
            }
            return null;
        }
        private void ParseXmlDocument(XDocument doc) {
            if (doc.Root == null) throw new Exception("Missing root node");
            var overrideVersionElement = doc.Root.Element("overrideVersion");
            if (overrideVersionElement != null) {
                OverrideVersion = new Version(overrideVersionElement.Value);
            }
            var requiredLibraryVersionElement = doc.Root.Element("requiredLibraryVersion");
            if (requiredLibraryVersionElement != null) {
                RequiredLibraryVersion = new Version(requiredLibraryVersionElement.Value);
            }
        }
    }
}