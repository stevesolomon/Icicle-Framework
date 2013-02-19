
using System.Diagnostics;
using System.Globalization;
using System.Xml.Linq;
using IcicleFramework.Entities;
using Microsoft.Xna.Framework;

namespace IcicleFramework.Components.Health
{
    /// <summary>
    /// An IntervalInvincibleHealthComponent enforced a period of invincibility following
    /// the application of damage.
    /// </summary>
    public class IntervalInvincibleHealthComponent : InvincibleHealthComponent
    {
        /// <summary>
        /// The remaining amount of time this IntervalInvincibleHealthComponent has to remain invincible.
        /// </summary>
        private float currInvincibleTime;

        /// <summary>
        /// Gets or sets the remaining amount of time this IntervalInvincibleHealthComponent has to remain invincible.
        /// </summary>
        private float CurrInvincibleTime
        {
            get { return currInvincibleTime; }
            set
            {
                if (value > 0.0f)
                {
                    currInvincibleTime = value;
                    IsInvincible = true;
                }
                else
                {
                    IsInvincible = false;
                }
            }
        }

        /// <summary>
        /// The interval this IntervalInvincibleHealthComponent remains invincible for when damaged.
        /// </summary>
        public float InvincibleInterval { get; set; }

        public override void Damage(float damage, IGameObject damageInitiator)
        {
            if (!IsInvincible)
            {
                base.Damage(damage, damageInitiator);
                CurrInvincibleTime = InvincibleInterval;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (Paused)
                return;

            if (CurrInvincibleTime > 0.0f)
            {
                CurrInvincibleTime -= (float) gameTime.ElapsedGameTime.TotalSeconds;

                if (CurrInvincibleTime <= 0.0f)
                    CurrInvincibleTime = 0.0f;
            }
            base.Update(gameTime);
        }

        public override void Deserialize(XElement element)
        {
            if (element.Element("interval") != null)
            {
                InvincibleInterval = float.Parse(element.Element("interval").Value, NumberStyles.Float,
                                                      CultureInfo.InvariantCulture);
                CurrInvincibleTime = 0.0f;
            }

            base.Deserialize(element);
        }

        public override void CopyInto(IBaseComponent newObject)
        {
            var healthComp = newObject as IntervalInvincibleHealthComponent;

            Debug.Assert(healthComp != null, "intervalHealthComponent != null");

            healthComp.InvincibleInterval = InvincibleInterval;
            healthComp.CurrInvincibleTime = CurrInvincibleTime;

            base.CopyInto(newObject);
        }
    }
}
