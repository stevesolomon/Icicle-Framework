using IcicleFramework.Entities;

namespace IcicleFramework.Components.Health
{
    public delegate void HealthChangedHandler(IHealthComponent sender, IGameObject damageInitiator, float oldHealth, float newHealth);

    public delegate void HealthDepletedHandler(IHealthComponent sender, IGameObject damageInitator);

    public interface IHealthComponent : IBaseComponent
    {
        event HealthChangedHandler OnHealthChanged;

        event HealthDepletedHandler OnHealthDepleted;

        /// <summary>
        /// Gets the current health status for this IHealthComponent.
        /// </summary>
        float CurrHealth { get; }

        /// <summary>
        /// Gets the maximum amount of health possible for this IHealthComponent.
        /// </summary>
        float MaxHealth { get; }

        /// <summary>
        /// Gets whether or not this IHealthComponent is signaling that it is dead (zero health).
        /// </summary>
        bool Dead { get; }

        /// <summary>
        /// Applies damage to this IHealthComponent.
        /// </summary>
        /// <param name="damage">The amount of damage to apply, must be greater than zero.</param>
        /// <param name="damageInitiator">The IGameObject that initiated the damage against this IHealthComponent.</param>
        void Damage(float damage, IGameObject damageInitiator);

        /// <summary>
        /// Heals this IHealthComponent (without exceeding MaxHealth).
        /// </summary>
        /// <param name="heal">The amount to heal, must be greater than zero.</param>
        void Heal(float heal);
    }
}
