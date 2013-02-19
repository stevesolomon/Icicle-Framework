using System.Globalization;
using System.Xml.Linq;
using Microsoft.Xna.Framework;

namespace IcicleFramework
{
    public static class Vector2Extensions
    {
        public static Vector2 DeserializeOffset(this Vector2 vector2, XElement element)
        {
            Vector2 vec = Vector2.Zero;

            if (element != null)
            {
                if (element.Attribute("x") != null)
                    float.TryParse(element.Attribute("x").Value, NumberStyles.Integer, CultureInfo.InvariantCulture,
                                   out vec.X);

                if (element.Attribute("y") != null)
                    float.TryParse(element.Attribute("y").Value, NumberStyles.Integer, CultureInfo.InvariantCulture,
                                   out vec.Y);
            }

            return vec;
        }
    }
}
