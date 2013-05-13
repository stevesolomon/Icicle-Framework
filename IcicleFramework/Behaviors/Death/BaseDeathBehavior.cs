using IcicleFramework.Components.Behaviors;
using IcicleFramework.Components.Health;
using IcicleFramework.Entities;

namespace IcicleFramework.Behaviors.Death
{
    public abstract class BaseDeathBehavior : BaseBehavior
    {
        public override void Initialize()
        {
            IHealthComponent healthComponent = ParentGameObject.GetComponent<IHealthComponent>();
            healthComponent.OnHealthDepleted += OnDeath;
            base.Initialize();
        }

        protected abstract void OnDeath(IHealthComponent sender, IGameObject damageInitator);
    }
}
