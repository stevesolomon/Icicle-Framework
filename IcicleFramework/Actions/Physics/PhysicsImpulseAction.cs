using System.Diagnostics;
using IcicleFramework.Components.Physics;
using Microsoft.Xna.Framework;

namespace IcicleFramework.Actions.Physics
{
    public class PhysicsImpulseAction : GameAction
    {
        public Vector2 Impulse { get; set; }

        public override void Update(GameTime gameTime)
        {
            var physics = Target.GetComponent<IPhysicsComponent>();

            if (physics != null)
            {
                physics.ApplyImpulse(Impulse);
            }

            Finished = true;

            base.Update(gameTime);
        }

        public override void CopyInto(IGameAction newObject)
        {
            var action = newObject as PhysicsImpulseAction;

            Debug.Assert(action != null);

            action.Impulse = Impulse;

            base.CopyInto(newObject);
        }
    }
}
