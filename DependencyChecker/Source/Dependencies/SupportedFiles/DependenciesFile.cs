using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace DependencyChecker.Dependencies.SupportedFiles {

    /// <summary>
    /// Handles a dependencies.xml file
    /// </summary>
    internal class DependenciesFile {
        internal List<DependencyMetaData> Dependencies { get; private set; }

        internal const string Directory = "about";
        internal const string FileName = "dependencies.xml";

        internal static string RelativePath { get { return Path.Combine(Directory, FileName); } }

        internal DependenciesFile(XDocument doc) {
            Dependencies = new List<DependencyMetaData>();
            ParseXmlDocument(doc);
        }

        internal static DependenciesFile TryParseFile(string modRootDir) {
            string filePath = Path.Combine(modRootDir, RelativePath);
            if (!File.Exists(filePath)) return null;
            try {
                XDocument doc = XDocument.Load(filePath);
                return new DependenciesFile(doc);
            }
            catch (Exception) {
                throw;
            }
        }

        private void ParseXmlDocument(XDocument doc) {
            if (doc.Element("Dependencies") != null) {
                foreach (var dependenciesElement in doc.Element("Dependencies").Elements("Dependency")) {
                    DependencyMetaData dependency = new DependencyMetaData();

                    foreach (var element in dependenciesElement.Elements()) {
                        if (!string.IsNullOrEmpty(element.Value)) {
                            switch (element.Name.LocalName.ToLower()) {
                                case "name":
                                        dependency.Name = element.Value;
                                    break;

                                case "requiredversion":
                                        dependency.RequiredVersion = new Version(element.Value);
                                    break;

                                case "steamid":
                                    if (ulong.TryParse(element.Value, out ulong parsedValue)) {
                                        dependency.SteamID = parsedValue;
                                    }
                                    break;

                                case "friendlyname":
                                        dependency.FriendlyName = element.Value;
                                    break;

                                default:
                                    //Log.Message(string.Format("ignoring '{0}' as it is not a valid dependency element", element.Name.LocalName));
                                    break;
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(dependency.Name)) {
                        dependency.RelatedModMetaData = Utils.ModUtility.GetModByName(dependency.Name);
                        Dependencies.Add(dependency);
                    }
                }
            }
        }
    }
}