using System;
using System.Linq;

namespace IcicleFramework
{
    public static class TypeExtensions
    {
        public static Type[] GetTopLevelInterfaces(this Type t)
        {
            Type[] allInterfaces = t.GetInterfaces();

            var selection = allInterfaces
                .Where(x => !allInterfaces.Any(y => y.GetInterfaces().Contains(x)))
                .Except(t.BaseType.GetInterfaces());

            return selection.ToArray();
        }
    }
}
