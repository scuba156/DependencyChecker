using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace DependencyChecker.Dependencies.SupportedFiles {

    public class DependenciesFile {
        public List<DependencyMetaData> Dependencies { get; private set; }

        public const string Dir = "about";
        public const string FileName = "dependencies.xml";

        public static string RelativePath { get { return Path.Combine(Dir, FileName); } }

        internal DependenciesFile(XDocument doc) {
            Dependencies = new List<DependencyMetaData>();
            ParseXmlDocument(doc);
        }

        public static DependenciesFile TryParseFile(string modRootDir) {
            string filePath = Path.Combine(modRootDir, RelativePath);
            if (!File.Exists(filePath)) return null;
            try {
                XDocument doc = XDocument.Load(filePath);
                return new DependenciesFile(doc);
            }
            catch (Exception e) {
                throw e;
                //Log.Error("[DependencyChecker] Exception while parsing dependencies file at path: " + filePath + " Exception was: " + e);
            }
        }

        private void ParseXmlDocument(XDocument doc) {
            if (doc.Root == null) throw new Exception("Missing root node");
            if (doc.Element("Dependencies") != null) {
                foreach (var dependenciesElement in doc.Element("Dependencies").Elements("Dependency")) {
                    DependencyMetaData dependency = new DependencyMetaData();

                    foreach (var element in dependenciesElement.Elements()) {
                        switch (element.Name.LocalName.ToLower()) {
                            case "identifier":
                                if (element.Value != null)
                                    dependency.Identifier = element.Value;
                                break;

                            case "requiredversion":
                                if (element.Value != null && element.Value != string.Empty)
                                    dependency.RequiredVersion = new Version(element.Value);
                                break;

                            case "steamid":
                                if (element.Value != null)
                                    dependency.SteamID = element.Value;

                                break;

                            default:
                                //Log.Message(string.Format("ignoring '{0}' as it is not a valid dependency element", element.Name.LocalName));
                                break;
                        }
                    }
                    if (dependency.Identifier != string.Empty) {
                        Dependencies.Add(dependency);
                    }
                }
            }


            
        }
    }
}