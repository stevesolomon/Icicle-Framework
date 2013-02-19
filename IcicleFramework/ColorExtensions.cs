using System.Globalization;
using Microsoft.Xna.Framework;

namespace IcicleFramework
{
    public static class ColorExtensions
    {
        public static Color ConvertColorFromString(this Color color, string ARGB)
        {
            if (ARGB != null)
            {
                //Fiddle with the ARGB string if there is no alpha value included...
                if (ARGB.Length == 6)
                    ARGB.Insert(0, "FF");

                bool passed = true;
                int[] vals = new int[4];
                passed &= int.TryParse(ARGB.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out vals[0]);
                passed &= int.TryParse(ARGB.Substring(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out vals[1]);
                passed &= int.TryParse(ARGB.Substring(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out vals[2]);
                passed &= int.TryParse(ARGB.Substring(6, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out vals[3]);

                color = passed ? Color.FromNonPremultiplied(vals[1], vals[2], vals[3], vals[0]) : Color.HotPink;
            }
            else
                color = Color.HotPink;

            return color;
        }
        
    }
}
