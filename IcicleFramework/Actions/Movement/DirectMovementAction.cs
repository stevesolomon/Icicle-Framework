using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace IcicleFramework.Actions.Movement
{
    public class DirectMovementAction : GameAction
    {
        public Vector2 MovementAmount { get; set; }
        
        public override void Update(GameTime gameTime)
        {
            Target.Position += MovementAmount;
            Finished = true;

            base.Update(gameTime);
        }

        public override void CopyInto(IGameAction newObject)
        {
            var action = newObject as DirectMovementAction;

            Debug.Assert(action != null);

            action.MovementAmount = MovementAmount;

            base.CopyInto(newObject);
        }
    }
}
