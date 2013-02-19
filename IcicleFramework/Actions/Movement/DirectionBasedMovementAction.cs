using System.Diagnostics;
using IcicleFramework.Components.Movement;
using Microsoft.Xna.Framework;

namespace IcicleFramework.Actions.Movement
{
    public class DirectionBasedMovementAction : GameAction
    {
        /// <summary>
        /// Gets or sets the direction along the X and Y axis to move.
        /// </summary>
        public Vector2 Direction { get; set; }

        public override void Update(GameTime gameTime)
        {
            var movementComp = Target.GetComponent<IMovementComponent>();

            if (movementComp != null)
            {
                movementComp.MoveInDirection(Direction);
            }

            Finished = true;

            base.Update(gameTime);
        }

        public override void CopyInto(IGameAction newObject)
        {
            var action = newObject as DirectionBasedMovementAction;

            Debug.Assert(action != null);

            action.Direction = Direction;

            base.CopyInto(newObject);
        }
    }
}
