using System;
using System.Collections.Generic;
using System.Linq;

namespace IcicleFramework
{
    public static class EnumHelper
    {
        public static T[] EnumToArray<T>()
        {
            Type enumType = typeof(T);
            if (enumType.BaseType != typeof(Enum))
            {
                throw new ArgumentException("T must be a System.Enum");
            }
            return (Enum.GetValues(enumType) as IEnumerable<T>).ToArray();
        }
    }
}
