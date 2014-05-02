using ExampleGameSHMUP.Actions.Projectiles;
using IcicleFramework.Inputs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ExampleGameSHMUP.Behaviors.Players
{
    public class PlayerShipFireBehavior : BasicProjectileFiringBehavior
    {
        protected Player player;

        public override void PostInitialize()
        {
            player = ParentGameObject.GetMetadata("player") as Player;

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            TimeSinceLastShot += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (TimeSinceLastShot <= TimeBetweenShots || !player.InputHandler.IsDown(Keys.LeftControl))
                return;

            TimeSinceLastShot = 0.0f;

            var fireProjAction = Parent.ActionFactory.GetAction<FireProjectileAction>();
            fireProjAction.ProjectileName = "projectile";
            fireProjAction.ProjectileLayer = ParentGameObject.Layer;
            fireProjAction.Direction = new Vector2(0, -1);
            fireProjAction.ProjectilePosition = ParentGameObject.Position;

            ParentGameObject.Rotation += 0.001f;
            
            Parent.FireAction(fireProjAction, ParentGameObject);
        }

        public override void Reallocate()
        {
            player = null;

            base.Reallocate();
        }
    }
}
