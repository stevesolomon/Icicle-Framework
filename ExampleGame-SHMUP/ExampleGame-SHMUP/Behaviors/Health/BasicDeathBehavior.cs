using IcicleFramework.Behaviors;
using IcicleFramework.Components.Behaviors;
using IcicleFramework.Components.Health;
using IcicleFramework.Entities;

namespace ExampleGameSHMUP.Behaviors.Health
{
    public class BasicDeathBehavior : BaseBehavior
    {
        protected IHealthComponent healthComponent;

        public override void Initialize()
        {
            healthComponent = ParentGameObject.GetComponent<IHealthComponent>();

            if (healthComponent != null)
            {
                healthComponent.OnHealthDepleted += OnHealthDepleted;    
            }

            base.Initialize();
        }

        protected virtual void OnHealthDepleted(IHealthComponent sender, IGameObject damageInitator)
        {
            healthComponent.OnHealthDepleted -= OnHealthDepleted;

            ParentGameObject.Destroy();
        }
    }
}
