using FarseerPhysics.Dynamics.Contacts;
using IcicleFramework.Actions;
using IcicleFramework.Actions.Damage;
using IcicleFramework.Actions.Particles;
using IcicleFramework.Behaviors.Collision;
using IcicleFramework.Components.Damage;
using IcicleFramework.Components.Physics;

namespace ExampleGame.Components.Behaviors
{
    public class BallCollisionBehavior : PhysicsCollisionBehavior
    {
        protected IDamageComponent damageComponent;

        public override void Initialize()
        {
            damageComponent = ParentGameObject.GetComponent<IDamageComponent>();
            base.Initialize();
        }

        protected override void OnCollision(IPhysicsComponent physicsComponent, IPhysicsComponent colliding, Contact contact)
        {
            //Fire the damage action.
            var damageAction = Parent.ActionFactory.GetAction<BasicDamageAction>();
            damageAction.Damage = damageComponent.Damage;

            var particleAction = Parent.ActionFactory.GetAction<EmitParticleAction>();
            particleAction.ParticleName = "BallCollisionParticle";
            particleAction.TargetPosition = ParentGameObject.Position;

            particleAction.Initialize();

            var compositeAction = Parent.ActionFactory.GetAction<ParallelGameAction>();
            compositeAction.AddAction(damageAction);
            compositeAction.AddAction(particleAction);

            Parent.FireAction(compositeAction, colliding.Parent);
        }
    }
}
