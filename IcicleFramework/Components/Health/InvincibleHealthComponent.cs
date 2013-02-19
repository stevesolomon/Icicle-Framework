
using System.Diagnostics;
using IcicleFramework.Entities;

namespace IcicleFramework.Components.Health
{
    /// <summary>
    /// An IHealthComponent that supports invincibility.
    /// </summary>
    public class InvincibleHealthComponent : HealthComponent
    {
        /// <summary>
        /// Gets whether or not this InvincibleHealthComponent is, in fact, invincible.
        /// </summary>
        public bool IsInvincible { get; set; }

        public override void Damage(float damage, IGameObject damageInitator)
        {
            if (!IsInvincible)
                base.Damage(damage, damageInitator);
        }

        public override void CopyInto(IBaseComponent newObject)
        {
            var healthComp = newObject as InvincibleHealthComponent;

            Debug.Assert(healthComp != null, "healthComp != null");

            healthComp.IsInvincible = IsInvincible;

            base.CopyInto(newObject);
        }
    }
}
