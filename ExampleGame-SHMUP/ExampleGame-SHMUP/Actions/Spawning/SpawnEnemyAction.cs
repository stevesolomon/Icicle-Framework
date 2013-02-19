using System.Diagnostics;
using IcicleFramework.Actions;
using IcicleFramework.GameServices;
using IcicleFramework.GameServices.Factories;
using Microsoft.Xna.Framework;

namespace ExampleGameSHMUP.Actions.Spawning
{
    public class SpawnEnemyAction : GameAction
    {
        public string EnemyName { get; set; }

        public Vector2 SpawnPosition { get; set; }

        protected IGameObjectFactory gameObjectFactory;

        public override void Initialize()
        {
            gameObjectFactory = GameServiceManager.GetService<IGameObjectFactory>();

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (EnemyName != null)
            {
                var enemy  = gameObjectFactory.GetGameObject(EnemyName, SpawnPosition.X, SpawnPosition.Y);
                enemy.PostInitialize();
                enemy.Active = true;
            }
#if DEBUG
            else
            {
                Trace.WriteLine("The enemy name passed into SpawnEnemyAction was null!");
            }
#endif

            Finished = true;

            base.Update(gameTime);
        }
    }
}
