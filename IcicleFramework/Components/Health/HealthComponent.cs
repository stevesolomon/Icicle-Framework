using System.Diagnostics;
using System.Globalization;
using System.Xml.Linq;
using IcicleFramework.Entities;
using Microsoft.Xna.Framework;

namespace IcicleFramework.Components.Health
{
    public class HealthComponent : BaseComponent, IHealthComponent
    {
        public event HealthChangedHandler OnHealthChanged;

        public event HealthDepletedHandler OnHealthDepleted;
        
        public float CurrHealth { get; protected set; }

        public float PrevHealth { get; protected set; }

        public float MaxHealth { get; protected set; }

        public bool Dead
        {
            get { return CurrHealth < 0; }
        }
        
        /// <summary>
        /// Updates the HealthComponent. Does nothing for a basic HealthComponent.
        /// </summary>
        /// <param name="gameTime">Information related to the current game time.</param>
        public override void Update(GameTime gameTime) {}

        public virtual void Damage(float damage, IGameObject damageInitiator)
        {
            if (damage > 0)
                ApplyDamage(damage, damageInitiator);
        }

        protected virtual void ApplyDamage(float damage, IGameObject damageInitiator)
        {
            PrevHealth = CurrHealth;
            CurrHealth -= damage;

            if (OnHealthChanged != null)
                OnHealthChanged(this, damageInitiator, PrevHealth, CurrHealth);

            if (CurrHealth <= 0.0f)
            {
                if (OnHealthDepleted != null)
                    OnHealthDepleted(this, damageInitiator);
            }
        }

        public virtual void Heal(float heal)
        {
            if (heal > 0)
            {
                CurrHealth += heal;
                CurrHealth = CurrHealth > MaxHealth ? MaxHealth : CurrHealth;
            }
        }

        public override void Deserialize(XElement element)
        {
            //We need to grab the maximum health and, if it exists, a current health...
            if (element.Element("maxHealth") != null)
                MaxHealth = float.Parse(element.Element("maxHealth").Value, NumberStyles.Float,
                                             CultureInfo.InvariantCulture);

            if (element.Element("currHealth") != null)
                CurrHealth = float.Parse(element.Element("currHealth").Value, NumberStyles.Float,
                                              CultureInfo.InvariantCulture);
            else
                CurrHealth = MaxHealth;
        }

        public override void Reallocate()
        {
            OnHealthChanged = null;
            OnHealthDepleted = null;

            base.Reallocate();
        }

        public override void CopyInto(IBaseComponent newObject)
        {
            var healthComp = newObject as HealthComponent;

            Debug.Assert(healthComp != null, "healthComp != null");

            healthComp.MaxHealth = MaxHealth;
            healthComp.CurrHealth = CurrHealth;

            base.CopyInto(newObject);
        }

    }
}
