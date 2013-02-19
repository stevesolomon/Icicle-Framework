using System.Collections.Generic;
using ExampleGameSHMUP.Behaviors.Projectiles;
using IcicleFramework.Components.Behaviors;
using IcicleFramework.Entities;
using IcicleFramework.GameServices;
using IcicleFramework.GameServices.Factories;
using Microsoft.Xna.Framework;

namespace ExampleGameSHMUP.GameServices
{
    public class ProjectileManager : GameService, IProjectileManager
    {
        protected Dictionary<string, IGameObject> cachedProjectiles;

        protected IGameObjectFactory gameObjectFactory;

        public IGameObject SpawnProjectile(string name, Vector2 location, Vector2 initialDirection)
        {
            IGameObject projectile = null;

            if (!cachedProjectiles.ContainsKey(name))
            {
                projectile = gameObjectFactory.GetGameObject(name, usedAsTemplate: true);
                projectile.Active = false;
                cachedProjectiles.Add(name, projectile);
            }

            projectile = gameObjectFactory.CopyExistingGameObject(cachedProjectiles[name]);
            projectile.Position = location;

            var projMoveBehavior =
                projectile.GetComponent<IBehaviorComponent>().GetBehavior("moveBehavior") as
                BaseProjectileMovementBehavior;

            projMoveBehavior.InitialDirection = initialDirection;

            projectile.PostInitialize();

            return projectile;
        }

        public override void Initialize()
        {
            cachedProjectiles = new Dictionary<string, IGameObject>(32);
            gameObjectFactory = GameServiceManager.GetService<IGameObjectFactory>();
        }
    }
}
