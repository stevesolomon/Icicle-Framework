using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using System.Linq;
using Microsoft.Xna.Framework;

namespace IcicleFramework.Components.Gauges
{
    /// <summary>
    /// A Gauge component that changes the color of the gauge bar based on the
    /// percentage that the gauge is filled.
    /// </summary>
    public class ColorChangeGaugeComponent : GaugeComponent
    {
        private List<Color> colors;

        private List<float> transitionThresholds;

        public override float Filled
        {
            set
            {
                FilledColor = SearchColor(value);
                base.Filled = value;
            }
        }

        public ColorChangeGaugeComponent()
        {
            colors = new List<Color>();
            transitionThresholds = new List<float>();
        }

        /// <summary>
        /// Finds the correct (Filled) Color to use for the Bar based on the provided value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>The Color to use for the Bar. Color.White if no matching transitional color value was found.</returns>
        private Color SearchColor(float value)
        {
            Color color = Color.White;

            for (int i = transitionThresholds.Count - 1; i >= 0 && i < colors.Count; i--)
            {
                //Threshold values are sorted. If the current threshold value is less than our value,
                //we want the *previous* threshold value's color.
                if (transitionThresholds[i] < value)
                {
                    //Safety check/massage!
                    //if (i == 0)
                    //    i = 1;

                    color = colors[i];
                    break;
                }
            }

            return color;
        }

        public override void Deserialize(XElement element)
        {
            if (element.Element("transitions") != null)
            {
                XElement[] elements =
                    element.Element("transitions").Descendants("transition")
                        .OrderBy(x => (int) x.Attribute("threshold"))
                        .ToArray();

                for (int i = 0; i < elements.Length; i++)
                {
                    float val = 100f;
                    bool parsed = false;

                    //We support threshold values in the 0 - 1 range, or the 0 - 100% range.
                    //We'll assume anything <= 1.0 is the former.
                    //If it's the 0 - 100% range then convert the number!
                    if (elements[i].Attribute("threshold") != null)
                    {
                        parsed = float.TryParse(elements[i].Attribute("threshold").Value, NumberStyles.Float,
                                                CultureInfo.InvariantCulture, out val);

                        if (parsed && val > 1.0f)
                            val /= 100;
                        else if (!parsed)
                            val = 1.0f;
                            
                    }

                    

                    Color color = new Color();

                    if (elements[i].Element("color") != null)
                        color = color.ConvertColorFromString(elements[i].Element("color").Value);
                    else
                        color = Color.HotPink;

                    transitionThresholds.Add(val);
                    colors.Add(color);
                }
            }

            base.Deserialize(element);
        }
    }
}
