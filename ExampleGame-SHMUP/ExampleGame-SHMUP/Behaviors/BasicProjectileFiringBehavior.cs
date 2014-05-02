using System.Diagnostics;
using System.Globalization;
using System.Xml.Linq;
using IcicleFramework.Behaviors;
using IcicleFramework.Components.Behaviors;

namespace ExampleGameSHMUP.Behaviors
{
    public class BasicProjectileFiringBehavior : BaseBehavior
    {
        public float TimeSinceLastShot { get; protected set; }

        public float TimeBetweenShots { get; set; }

        public override void CopyInto(IBehavior newObject)
        {
            var projectileFiring = newObject as BasicProjectileFiringBehavior;

            Debug.Assert(projectileFiring != null, "BasicProjectileFiringBehavior is null in CopyInto");

            projectileFiring.TimeBetweenShots = TimeBetweenShots;

            base.CopyInto(newObject);
        }

        public override void Reallocate()
        {
            TimeSinceLastShot = 0.0f;
            TimeBetweenShots = 0.0f;

            base.Reallocate();
        }

        public override void Deserialize(XElement element)
        {
            var timeBetweenElem = element.Element("timeBetweenShots");

            var value = 0.5f;
            if (timeBetweenElem != null)
            {
                var parsed = float.TryParse(timeBetweenElem.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out value);

                value = parsed ? value : 0.5f;
            }

            TimeBetweenShots = value;

            base.Deserialize(element);
        }
    }
}
