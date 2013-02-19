using ExampleGameSHMUP.Actions.Projectiles;
using Microsoft.Xna.Framework;

namespace ExampleGameSHMUP.Behaviors.Enemies
{
    public class BasicEnemyFiringBehavior : BasicProjectileFiringBehavior
    {
        public override void Update(GameTime gameTime)
        {
            TimeSinceLastShot += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (TimeSinceLastShot <= TimeBetweenShots)
                return;

            TimeSinceLastShot = 0.0f;

            var fireProjAction = Parent.ActionFactory.GetAction<FireProjectileAction>();
            fireProjAction.ProjectileName = "projectile";
            fireProjAction.ProjectileLayer = ParentGameObject.Layer;
            fireProjAction.Direction = new Vector2(0, 1);
            fireProjAction.ProjectilePosition = ParentGameObject.Position;

            Parent.FireAction(fireProjAction, ParentGameObject);
        }
    }
}
