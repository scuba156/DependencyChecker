using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Verse;

namespace DependencyChecker.SupportedFiles {

    public class DependenciesFile {
        public List<DependencySaveableData> Dependencies { get; private set; }

        private const string aboutDir = "about";
        private const string dependenciesFileName = "dependencies.xml";

        internal DependenciesFile(XDocument doc) {
            Dependencies = new List<DependencySaveableData>();
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
                DependencySaveableData dependency = new DependencySaveableData();
                switch (element.Name.LocalName.ToLower()) {
                    case "name":
                        if (element.Value != null)
                            dependency.Name = element.Value;
                        break;

                    case "identifier":
                        if (element.Value != null)
                            dependency.Identifier = element.Value;
                        break;

                    case "requiredversion":
                        if (element.Value != null)
                            dependency.RequiredVersion = new Version(element.Value);
                        break;

                    case "steamid":
                        if (element.Value != null)
                            dependency.Steamid = element.Value;

                        break;

                    default:
                        //Logger.Message("ignoring '{0}' as it is not a valid dependency element", element.Name.LocalName, doc);
                        break;
                }
                Dependencies.Add(dependency);
            }
        }
    }

    public class DependencySaveableData {
        public string Identifier { get; internal set; }
        public string Name { get; internal set; }
        public string Steamid { get; internal set; }
        public Version RequiredVersion { get; internal set; }
    }
}