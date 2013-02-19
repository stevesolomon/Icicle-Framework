using IcicleFramework.Entities;
using IcicleFramework.GameServices;
using Microsoft.Xna.Framework;

namespace ExampleGameSHMUP.GameServices
{
    public interface IProjectileManager : IGameService
    {
        /// <summary>
        /// Spawns a new projectile of the given name at the given location, traveling in the provided initial direction.
        /// </summary>
        /// <param name="name">The name of the projectile to spawn, corresponding to a definition in the projectiles definition file.</param>
        /// <param name="location">The location that the projectile is to be spawned at.</param>
        /// <param name="initialDirection">The initial direction the projectile will travel in.</param>
        /// <returns>A new <see cref="IGameObject"/> encompassing the created projectile, or null if no projectile could be created.</returns>
        IGameObject SpawnProjectile(string name, Vector2 location, Vector2 initialDirection);
    }
}
