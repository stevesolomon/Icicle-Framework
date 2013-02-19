using IcicleFramework.Components.Behaviors;
using IcicleFramework.Components.Collision;
using Microsoft.Xna.Framework;
using TestBed.Actions;

namespace TestBed.Components.Behaviors
{
    public class PlayerCollisionBehavior : BaseBehavior
    {
        public override void Initialize()
        {
            //Subscribe to our Collision Components OnCollision event
            var collisionComponent = ParentGameObject.GetComponent<ICollisionComponent>();
            collisionComponent.OnCollision += CollisionHandler;
            collisionComponent.OnCollisionStopped += CollisionStoppedHandler;

            base.Initialize();
        }

        private void CollisionStoppedHandler(ICollisionComponent source, ICollisionComponent previousCollided)
        {
            Parent.FireAction(new ColorChangeAction() { Color = Color.Purple }, ParentGameObject);
        }

        private void CollisionHandler(ICollisionComponent source, ICollisionComponent collided)
        {
            Parent.FireAction(new ColorChangeAction() { Color = Color.Red }, ParentGameObject);
        }
    }
}
