using System;

namespace IcicleFramework
{
    /// <summary>
    /// A simple static class for providing access to common, generic methods.
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// Finds the matching named interface for a type.
        /// </summary>
        /// <param name="classType">The type to find the specific interface from.</param>
        /// <param name="stringInterface">A string representing the basic name of the interface to find.</param>
        /// <returns>The interface type found, null if no matching interface was located.</returns>
        public static Type FindInterface(Type classType, string stringInterface)
        {
            Type type = null;
            int i;

            Type[] types = classType.GetInterfaces();

            for (i = 0; i < types.Length; i++)
            {
                if (types[i].Name.Equals(stringInterface, StringComparison.InvariantCultureIgnoreCase))
                {
                    type = types[i];
                    break;
                }
            }

            return type;
        }

        public static string GetLookupName(Type type, string name)
        {
            return string.Format("{0}.{1}", name, type.FullName);
        }
    }
}
