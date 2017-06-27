using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DependencyChecker.Utils {

    internal static class AssemblyUtils {

        /// <summary>
        /// Gets the executing assemblies name
        /// </summary>
        internal static string CurrentAssemblyName { get { return Assembly.GetExecutingAssembly().GetName().Name; } }

        /// <summary>
        /// Gets the executing assemblies version
        /// </summary>
        internal static Version CurrentAssemblyVersion { get { return Assembly.GetExecutingAssembly().GetName().Version; } }

        /// <summary>
        /// Get all assemblies that match the executing type
        /// </summary>
        internal static List<Assembly> AllTypesOfExecutingAssemblies { get { return AppDomain.CurrentDomain.GetAssemblies().ToList().FindAll((ass => ass.GetName().Name == CurrentAssemblyName)); } }

        /// <summary>
        /// Get all assemblies that match the executing type that are not the executing assembly
        /// </summary>
        internal static List<Assembly> AllTypesOfExecutingAssembliesExceptCurrent { get { return AllTypesOfExecutingAssemblies.FindAll(ass => ass.FullName != Assembly.GetExecutingAssembly().FullName); } }
    }
}