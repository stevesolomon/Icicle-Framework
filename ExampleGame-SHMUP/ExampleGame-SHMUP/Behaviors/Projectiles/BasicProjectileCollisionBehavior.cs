using ExampleGameSHMUP.Actions.Projectiles;
using IcicleFramework.Actions;
using IcicleFramework.Actions.Damage;
using IcicleFramework.Actions.Destruction;
using IcicleFramework.Behaviors;
using IcicleFramework.Components;
using IcicleFramework.Components.Collision;
using IcicleFramework.Components.Damage;

namespace ExampleGameSHMUP.Behaviors.Projectiles
{
    public class BasicProjectileCollisionBehavior : BaseBehavior
    {
        public ICollisionComponent CollisionComponent { get; protected set; }

        public IDamageComponent DamageComponent { get; protected set; }

        public override void Initialize()
        {
            //Subscribe to our ICollisionComponent.
            CollisionComponent = ParentGameObject.GetComponent<ICollisionComponent>();
            CollisionComponent.OnCollision += Collided;

            //Get a reference to our damage component.
            DamageComponent = ParentGameObject.GetComponent<IDamageComponent>();

            base.Initialize();
        }

        protected virtual void Collided(ICollisionComponent source, ICollisionComponent collided)
        {
            //Just attempt to damage the thing we collided with immediately.
            var damageAction = Parent.ActionFactory.GetAction<BasicDamageAction>();
            damageAction.Damage = DamageComponent.Damage;

            //Let's fire a particle destruction action while we're at it!
            var destroyAction = Parent.ActionFactory.GetAction<DestroyGameObjectAction>();

            var sequence = Parent.ActionFactory.GetAction<SequenceGameAction>();
            sequence.AddAction(damageAction);
            sequence.AddAction(destroyAction);

            Parent.FireAction(sequence, ((IBaseComponent) collided).Parent);

            //And now let's destroy ourself!
            var selfDestroy = Parent.ActionFactory.GetAction<DestroyGameObjectAction>();
            Parent.FireAction(selfDestroy, this.ParentGameObject);
        }

        public override void Cleanup()
        {
            CollisionComponent.OnCollision -= Collided;
            CollisionComponent = null;
            DamageComponent = null;

            base.Cleanup();
        }
    }
}
