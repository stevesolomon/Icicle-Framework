using FarseerPhysics.Dynamics.Contacts;
using IcicleFramework.Components.Behaviors;
using IcicleFramework.Components.Physics;
using IcicleFramework.GameServices;
using IcicleFramework.Physics;

namespace IcicleFramework.Behaviors.Collision
{
    public abstract class PhysicsCollisionBehavior : BaseBehavior
    {
        public override void Initialize()
        {
            //Tell the collision manager we want to subscribe...
            IPhysicsManager collisionManager = GameServiceManager.GetService<IPhysicsManager>();

            if (collisionManager != null)
                collisionManager.SubscribeCollisionEvent(this.ParentGameObject.GUID, OnCollision);

            base.Initialize();
        }

        protected abstract void OnCollision(IPhysicsComponent physicsComponent, IPhysicsComponent colliding, Contact contact);
    }
}
