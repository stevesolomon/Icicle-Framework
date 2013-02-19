using IcicleFramework.Components.Health;
using Microsoft.Xna.Framework;

namespace IcicleFramework.Actions.Damage
{
    public class BasicDamageAction : GameAction
    {
        public float Damage { get; set; }

        public override void Update(GameTime gameTime)
        {
            var healthComponent = target.GetComponent<IHealthComponent>();

            if (healthComponent != null)
            {
                healthComponent.Damage(Damage, Parent);
            }

            Finished = true;

            base.Update(gameTime);
        }

        public override void Dispose()
        {
            Damage = 0.0f;
            base.Dispose();
        }
    }
}
