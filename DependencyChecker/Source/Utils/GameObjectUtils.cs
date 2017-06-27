using DependencyChecker.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;

namespace DependencyChecker.Utils {

    public static class GameObjectUtils {

        /// <summary>
        /// Token that identifies our game object
        /// </summary>
        private static readonly string GameObjectName = AssemblyUtils.CurrentAssemblyName + "Token";

        public static List<string> StoredModIdentifiers {
            get {
                var gameObject = GetGameObject(GameObjectName);
                foreach (Assembly assembly in AssemblyUtils.AllTypesOfExecutingAssemblies) {
                    var component = gameObject.GetComponent(assembly.GetTypes().First(type => type.Name == typeof(ComponentModIdentifiers).Name));
                    if (component != null) {
                        return (List<string>)component.GetType().GetProperty("StoredModIdentifiers").GetValue(component, null);
                    }
                }
                return null;
            }

            set {
                var gameObject = GetGameObject(GameObjectName);
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
                var gameObject = GetGameObject(GameObjectName);
                foreach (Assembly assembly in AssemblyUtils.AllTypesOfExecutingAssembliesExceptCurrent) {
                    var component = gameObject.GetComponent(assembly.GetTypes().First(type => type.Name == typeof(ComponentVersion).Name));
                    if (component != null) {
                        return (Version)component.GetType().GetProperty("StoredVersion").GetValue(component, null);
                    }
                }
                return null;
            }

            set {
                var gameObject = GetGameObject(GameObjectName);
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

        public static GameObject GetGameObject(string tokenName) {
            GameObject result = GameObject.Find(tokenName);
            if (result == null) {
                Log.Message("Creating new game object");
                return new GameObject(tokenName);
            }
            return result;
        }

        public static void TryDestroyGameObject() {
            TryDestroyGameObject(GameObjectName);
        }

        public static void TryDestroyGameObject(string gameObjectName) {
            GameObject gameObject = GameObject.Find(gameObjectName);
            if (gameObject == null)
                UnityEngine.Object.Destroy(gameObject);
        }
    }
}