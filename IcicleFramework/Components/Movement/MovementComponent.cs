using System;
using System.Diagnostics;
using System.Xml.Linq;
using Microsoft.Xna.Framework;

namespace IcicleFramework.Components.Movement
{
    public class MovementComponent : BaseComponent, IMovementComponent
    {
        protected Vector2 velocity;

        public Vector2 VelocityDampingPercent { get; set; }

        public Vector2 MaxVelocity { get; set; }

        public Vector2 MoveVelocity { get; set; }

        public Vector2 Velocity
        {
            get { return velocity; }
            set
            {
                if (value.X > MaxVelocity.X)
                    value.X = MaxVelocity.X;
                else if (value.X < -MaxVelocity.X)
                    value.X = -MaxVelocity.X;

                if (value.Y > MaxVelocity.Y)
                    value.Y = MaxVelocity.Y;
                else if (value.Y < -MaxVelocity.Y)
                    value.Y = -MaxVelocity.Y;

                velocity = value;
            }
        }
        
        public override void Update(GameTime gameTime)
        {
            if (Velocity != Vector2.Zero)
            {
                Parent.Position += Velocity*(float) gameTime.ElapsedGameTime.TotalSeconds;

                float xDecay, yDecay;

                xDecay = (velocity.X*VelocityDampingPercent.X); //* (float)gameTime.ElapsedGameTime.TotalSeconds;
                yDecay = (velocity.Y*VelocityDampingPercent.Y); //* (float)gameTime.ElapsedGameTime.TotalSeconds;

                velocity.X -= xDecay;
                velocity.Y -= yDecay;
            }

            base.Update(gameTime);
        }

        public void MoveInDirection(Vector2 direction)
        {
            Vector2 newVelocity = new Vector2();

            newVelocity.X = Velocity.X + direction.X * MoveVelocity.X;
            newVelocity.Y = Velocity.Y + direction.Y * MoveVelocity.Y;

            Velocity = newVelocity;
        }
        
        public override void Deserialize(XElement element)
        {
            XElement velElem = element.Element("maxVelocity");

            Vector2 parsedVec = Vector2.Zero;
            if (velElem != null)
            {
                parsedVec = parsedVec.DeserializeOffset(velElem);
            }
            MaxVelocity = parsedVec;

            parsedVec = Vector2.Zero;
            velElem = element.Element("velocityDampingPercent");
            if (velElem != null)
            {
                parsedVec = parsedVec.DeserializeOffset(velElem);
            }
            VelocityDampingPercent = parsedVec / 100f;

            parsedVec = Vector2.Zero;
            velElem = element.Element("moveVelocity");
            if (velElem != null)
            {
                parsedVec = parsedVec.DeserializeOffset(velElem);
            }
            MoveVelocity = parsedVec;
        }

        public override void Reallocate()
        {
            Velocity = Vector2.Zero;
            VelocityDampingPercent = Vector2.Zero;
            MaxVelocity = Vector2.Zero;
            MoveVelocity = Vector2.Zero;

            base.Reallocate();
        }

        public override void CopyInto(IBaseComponent newObject)
        {
            var movementComponent = newObject as MovementComponent;

            Debug.Assert(movementComponent != null, "movementComponent != null");

            movementComponent.MaxVelocity = MaxVelocity;
            movementComponent.MoveVelocity = MoveVelocity;
            movementComponent.VelocityDampingPercent = VelocityDampingPercent;

            base.CopyInto(newObject);
        }
    }
}
