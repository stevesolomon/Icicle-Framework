using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace IcicleFramework.Actions.Movement
{
    public class DirectAngularMovementAction : DirectMovementAction
    {
        public float MovementSpeed { get; set; }

        public float MovementAngle { get; set; }

        public override void Update(GameTime gameTime)
        {
            var scaleX = (float)Math.Cos(Math.PI / 180.0 * MovementAngle);
            var scaleY = (float)Math.Sin(Math.PI / 180.0 * MovementAngle);

            MovementAmount = new Vector2(scaleX * MovementSpeed, scaleY * MovementSpeed);

            base.Update(gameTime);
        }

        public override void CopyInto(IGameAction newObject)
        {
            var action = newObject as DirectAngularMovementAction;

            Debug.Assert(action != null);

            action.MovementSpeed = MovementSpeed;
            action.MovementAngle = MovementAngle;

            base.CopyInto(newObject);
        }
    }
}
