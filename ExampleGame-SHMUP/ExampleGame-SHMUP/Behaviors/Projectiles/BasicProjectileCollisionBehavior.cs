using ExampleGameSHMUP.Actions.Projectiles;
using IcicleFramework.Actions;
using IcicleFramework.Actions.Damage;
using IcicleFramework.Behaviors;
using IcicleFramework.Components.Behaviors;
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
            var destroyAction = Parent.ActionFactory.GetAction<BaseDestroyProjectileAction>();

            var sequence = Parent.ActionFactory.GetAction<SequenceGameAction>();
            sequence.AddAction(damageAction);
            sequence.AddAction(destroyAction);

            Parent.FireAction(sequence, collided.Parent);
        }

        public override void Dispose()
        {
            CollisionComponent.OnCollision -= Collided;
            CollisionComponent = null;
            DamageComponent = null;

            base.Dispose();
        }
    }
}
