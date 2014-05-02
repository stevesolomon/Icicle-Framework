using IcicleFramework.Components.Health;
using IcicleFramework.Entities;

namespace IcicleFramework.Components.Gauges
{
    /// <summary>
    /// A gauge that monitors the IHealthComponent of its parent GameObject and provides
    /// a visual representation of the remaining health.
    /// </summary>
    public class HealthGaugeComponent : ColorChangeGaugeComponent
    {
        public void HealthChanged(IHealthComponent sender, IGameObject damageInitiator, float oldHealth, float newHealth)
        {
            float maxHealth = sender.MaxHealth;

            Filled = newHealth / maxHealth;
        }

        public override void Initialize()
        {
            IHealthComponent healthComponent = null;

            //Track down the IHealthComponent for our parent object and attach ourselves to the health-changed 
            //event so we can correctly function when the health changes!
            if (Parent != null)
            {
                healthComponent = Parent.GetComponent<IHealthComponent>();

                if (healthComponent != null)
                    healthComponent.OnHealthChanged += HealthChanged;
            }

            base.Initialize();
        }

        public override void Reallocate()
        {
            if (Parent != null)
            {
                IHealthComponent healthComponent = Parent.GetComponent<IHealthComponent>();

                if (healthComponent != null)
                    healthComponent.OnHealthChanged -= HealthChanged;
            }

            base.Reallocate();
        }

    }
}
