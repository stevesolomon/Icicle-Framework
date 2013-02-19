using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;

namespace IcicleFramework.GameServices.PhysicsServices
{
    public interface IPhysicsFactory
    {
        Body CopyBody(Body body);

        Joint CopyJoint(Joint joint, Body bodyA, Body bodyB);
    }
}
