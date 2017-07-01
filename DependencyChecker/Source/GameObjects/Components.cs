using System;
using System.Collections.Generic;
using UnityEngine;

namespace DependencyChecker.GameObjects {

    public class ComponentModIdentifiers : MonoBehaviour {
        public List<string> StoredModIdentifiers { get; set; }
    }

    public class ComponentVersion : MonoBehaviour {
        public Version StoredVersion { get; set; }
    }
}