using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace IcicleFramework.Components.Physics
{
    public class SingleBodyPhysicsComponent : PhysicsComponent
    {
        public override Body GetPrimaryBody()
        {
            if (NumBodies > 0)
                return GetFirstBody();
            
            return null;
        }

        public override void ApplyImpulse(Vector2 impulse, string name)
        {
            base.ApplyImpulse(impulse);
        }
    }
}
