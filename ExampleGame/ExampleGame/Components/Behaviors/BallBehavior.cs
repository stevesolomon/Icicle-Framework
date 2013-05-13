using System;
using System.Diagnostics;
using System.Globalization;
using System.Xml.Linq;
using IcicleFramework;
using IcicleFramework.Actions.Physics;
using IcicleFramework.Behaviors;
using IcicleFramework.Components.Behaviors;
using IcicleFramework.Components.Physics;
using Microsoft.Xna.Framework;

namespace ExampleGame.Components.Behaviors
{
    public class BallBehavior : BaseBehavior
    {
        public float ZeroVelocityTimeAllowed { get; set; }

        protected float TimeWithZeroXVelocity
        {
            get; 
            set;
        }

        public float TimeWithZeroYVelocity { get; protected set; }

        public float HelperImpulse { get; set; }

        protected IPhysicsComponent ballPhysics;

        public override void Initialize()
        {
            ballPhysics = ParentGameObject.GetComponent<IPhysicsComponent>();
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            var primaryBody = ballPhysics.GetPrimaryBody();

            //Now, if our velocity along one dimension is zero, keep track of how long it's
            //been like this for...
            if (primaryBody != null)
            {
                float xVel = Math.Abs(primaryBody.LinearVelocity.X);
                float yVel = Math.Abs(primaryBody.LinearVelocity.Y);

                float xImpulse = 0.0f, yImpulse = 0.0f;

                if (xVel < 0.1f)
                    TimeWithZeroXVelocity += (float)gameTime.ElapsedGameTime.TotalSeconds;
                else
                    TimeWithZeroXVelocity = 0f;

                if (yVel < 0.1f)
                    TimeWithZeroYVelocity += (float)gameTime.ElapsedGameTime.TotalSeconds;
                else
                    TimeWithZeroYVelocity = 0f;

                //Give it an impulse!
                if (TimeWithZeroXVelocity > ZeroVelocityTimeAllowed)
                {
                    xImpulse = HelperImpulse;
                    TimeWithZeroXVelocity = 0f;
                }

                if (TimeWithZeroYVelocity > ZeroVelocityTimeAllowed)
                {
                    yImpulse = HelperImpulse;
                    TimeWithZeroYVelocity = 0f;
                }

                if (xImpulse != 0f || yImpulse != 0f)
                {
                    var impulseAction = Parent.ActionFactory.GetAction<PhysicsImpulseAction>();
                    impulseAction.Impulse = new Vector2(xImpulse, yImpulse);

                    Parent.FireAction(impulseAction, ParentGameObject, null, 0f);
                }

            }
            base.Update(gameTime);
        }

        public override void Deserialize(XElement element)
        {
            float zeroVelocityTime = float.PositiveInfinity;
            if (element.Element("zeroVelocityTime") != null)
                float.TryParse(element.Element("zeroVelocityTime").Value, NumberStyles.Float, CultureInfo.InvariantCulture, out zeroVelocityTime);
            ZeroVelocityTimeAllowed = zeroVelocityTime;

            float helperImpulse = 0f;
            if (element.Element("helperImpulse") != null)
                float.TryParse(element.Element("helperImpulse").Value, NumberStyles.Float, CultureInfo.InvariantCulture, out helperImpulse);
            HelperImpulse = ConvertUnits.ToSimUnits(helperImpulse);


            base.Deserialize(element);
        }

        public override void CopyInto(IBehavior newObject)
        {
            var behavior = newObject as BallBehavior;

            Debug.Assert(behavior != null);

            behavior.HelperImpulse = HelperImpulse;
            behavior.ZeroVelocityTimeAllowed = ZeroVelocityTimeAllowed;

            base.CopyInto(newObject);
        }
    }
}
