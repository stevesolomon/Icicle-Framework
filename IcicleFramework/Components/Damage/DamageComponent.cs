using System.Diagnostics;
using System.Globalization;
using System.Xml.Linq;
using Microsoft.Xna.Framework;

namespace IcicleFramework.Components.Damage
{
    public class DamageComponent : BaseComponent, IDamageComponent
    {
        /// <summary>
        /// Gets or sets the damage caused by this DamageComponent.
        /// </summary>
        public float Damage { get; set; }

        public override void Update(GameTime gameTime) { }

        public override void Deserialize(XElement element)
        {
            //All we want is the damage caused!
            if (element.Element("damage") != null)
            {
                this.Damage = float.Parse(element.Element("damage").Value, NumberStyles.Float,
                                          CultureInfo.InvariantCulture);
            }
        }

        public override void CopyInto(IBaseComponent newObject)
        {
            var damageComp = newObject as DamageComponent;

            Debug.Assert(damageComp != null, "damageComp != null");

            damageComp.Damage = Damage;

            base.CopyInto(newObject);
        }
    }
}
