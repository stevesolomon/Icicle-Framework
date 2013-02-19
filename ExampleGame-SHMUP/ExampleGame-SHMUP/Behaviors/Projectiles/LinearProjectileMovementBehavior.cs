﻿using System.Diagnostics;
using System.Globalization;
using System.Xml.Linq;
using IcicleFramework.Actions.Movement;
using IcicleFramework.Components.Behaviors;

namespace ExampleGameSHMUP.Behaviors.Projectiles
{
    public class LinearProjectileMovementBehavior : BaseProjectileMovementBehavior
    {
        public float Angle { get; set; }

        protected bool ActionFiring
        {
            get;
            set;
        }

        public override void PostInitialize()
        {
            //Fire the action in PostInitialize and we're done!
            MovementComponent.MoveInDirection(InitialDirection);

            base.PostInitialize();
        }

        public override void CopyInto(IBehavior newObject)
        {
            var behavior = newObject as LinearProjectileMovementBehavior;

            Debug.Assert(behavior != null, "LinearProjectileMovementBehavior was null in CopyInto");

            behavior.Angle = Angle;
            behavior.ActionFiring = false;

            base.CopyInto(newObject);
        }

        public override void Dispose()
        {
            ActionFiring = false;
            Angle = 0f;
            base.Dispose();
        }

        public override void Deserialize(XElement element)
        {
            float angle = 0.0f;
            var dirElem = element.Element("angle");

            if (dirElem != null)
            {
                float.TryParse(dirElem.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out angle);
            }

            Angle = angle;

            base.Deserialize(element);
        }
    }
}
