using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DependencyChecker.Utils {

    public static class GameObjectUtility {

        /// <summary>
        /// Token that identifies our game object
        /// </summary>
        private static readonly string GeneratedUniqueToken = AssemblyUtility.CurrentAssemblyName + "Token";

        public static List<string> StoredModIdentifiers {
            get {
                var gameObject = GetGameObject();
                foreach (Assembly assembly in AssemblyUtility.AllTypesOfExecutingAssemblies) {
                    var component = gameObject.GetComponent(assembly.GetTypes().First(type => type.Name == typeof(ComponentModIdentifiers).Name));
                    if (component != null) {
                        return (List<string>)component.GetType().GetProperty("StoredModIdentifiers").GetValue(component, null);
                    }
                }
                return null;
            }

            set {
                var gameObject = GetGameObject();
                gameObject.SetActive(true);
                var component = gameObject.GetComponent<ComponentModIdentifiers>();
                if (component == null) {
                    gameObject.AddComponent(typeof(ComponentModIdentifiers));
                    component = gameObject.GetComponent<ComponentModIdentifiers>();
                }
                component.enabled = true;
                component.StoredModIdentifiers = value;
            }
        }

        public static Version StoredVersion {
            get {
                var gameObject = GetGameObject();
                foreach (Assembly assembly in AssemblyUtility.AllTypesOfExecutingAssembliesExceptCurrent) {
                    var component = gameObject.GetComponent(assembly.GetTypes().First(type => type.Name == typeof(ComponentVersion).Name));
                    if (component != null) {
                        return (Version)component.GetType().GetProperty("StoredVersion").GetValue(component, null);
                    }
                }
                return null;
            }

            set {
                var gameObject = GetGameObject();
                gameObject.SetActive(true);
                gameObject.AddComponent(typeof(ComponentVersion));
                var component = gameObject.GetComponent<ComponentVersion>();
                if (component == null) {
                    gameObject.AddComponent(typeof(ComponentVersion));
                    component = gameObject.GetComponent<ComponentVersion>();
                }
                component.enabled = true;
                component.StoredVersion = value;
            }
        }

        public static GameObject GetGameObject() {
            GameObject result = GameObject.Find(GeneratedUniqueToken);
            if (result == null) {
                return new GameObject(GeneratedUniqueToken);
            }
            return result;
        }

        public static void TryDestroyGameObject() {
            GameObject gameObject = GameObject.Find(GeneratedUniqueToken);
            if (gameObject == null)
                UnityEngine.Object.Destroy(gameObject);
        }
    }

    public class ComponentModIdentifiers : MonoBehaviour {
        public List<string> StoredModIdentifiers { get; set; }
    }

    public class ComponentVersion : MonoBehaviour {
        public Version StoredVersion { get; set; }
    }
}