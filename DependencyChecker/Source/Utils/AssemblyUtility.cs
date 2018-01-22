using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DependencyChecker.Utils {

    internal static class AssemblyUtility {

        private static Assembly executingAssemblyCached;

        /// <summary>
        /// Gets the executing assembly
        /// </summary>
        internal static Assembly CurrentAssembly { get { if (executingAssemblyCached == null) executingAssemblyCached = Assembly.GetExecutingAssembly();  return executingAssemblyCached; } }

        /// <summary>
        /// Gets the executing assemblies name
        /// </summary>
        internal static string CurrentAssemblyName { get { return CurrentAssembly.GetName().Name; } }

        /// <summary>
        /// Gets the executing assemblies version
        /// </summary>
        internal static Version CurrentAssemblyVersion { get { return CurrentAssembly.GetName().Version; } }

        /// <summary>
        /// Get all assemblies that match the executing assembly name
        /// </summary>
        internal static List<Assembly> AllTypesOfExecutingAssemblies { get { return AppDomain.CurrentDomain.GetAssemblies().ToList().FindAll((ass => ass.GetName().Name == CurrentAssemblyName)); } }

        /// <summary>
        /// Get all assemblies that match the executing assembly name that are not the executing assembly instance
        /// </summary>
        internal static List<Assembly> AllTypesOfExecutingAssembliesExceptCurrent { get { return AllTypesOfExecutingAssemblies.FindAll(ass => ass.FullName != CurrentAssembly.FullName); } }
    }
}