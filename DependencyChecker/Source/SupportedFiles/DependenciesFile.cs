using System;
using System.IO;
using System.Xml.Linq;

namespace DependencyChecker.SupportedFiles {

    public class DependenciesFile {
        public string Identifier { get; private set; }
        public string Name { get; private set; }
        public string Steamid { get; private set; }
        public Version RequiredVersion { get; private set; }
        private const string aboutDir = "about";
        private const string dependenciesFileName = "dependencies.xml";

        internal DependenciesFile(XDocument doc) {
            ParseXmlDocument(doc);
        }

        public static DependenciesFile TryParseFile(string modRootDir) {
            string filepath = Path.Combine(modRootDir, Path.Combine(aboutDir, dependenciesFileName));
            if (!File.Exists(filepath))
                return null;
            try {
                XDocument doc = XDocument.Load(filepath);
                return new DependenciesFile(doc);
            }
            catch (Exception ex) {
                throw (ex);
            }
        }

        private void ParseXmlDocument(XDocument doc) {
            if (doc.Root == null) throw new Exception("Missing root node");
            foreach (var element in doc.Root.Elements("dependency")) {
                switch (element.Name.LocalName.ToLower()) {
                    case "name":
                        if (element.Value != null)
                            this.Name = element.Value;
                        break;

                    case "identifier":
                        if (element.Value != null)
                            this.Identifier = element.Value;
                        break;

                    case "requiredversion":
                        if (element.Value != null)
                            this.RequiredVersion = new Version(element.Value);
                        break;

                    case "steamid":
                        if (element.Value != null)
                            this.Steamid = element.Value;

                        break;

                    default:
                        //Logger.Message("ignoring '{0}' as it is not a valid dependency element", element.Name.LocalName, doc);
                        break;
                }
            }
        }
    }
}