using IcicleFramework.Entities;
using IcicleFramework.GameServices;
using IcicleFramework.GameServices.Factories;
using IcicleFramework.Inputs;
using Microsoft.Xna.Framework;

namespace ExampleGameSHMUP.GameServices
{
    public class LevelLogicManager : GameService, ILevelLogicManager
    {
        protected IGameObjectFactory gameObjectFactory;

        public bool GameOver { get; protected set; }

        public override void PostInitialize()
        {
            gameObjectFactory = GameServiceManager.GetService<IGameObjectFactory>();

            LoadLevel("testLevel");

            base.PostInitialize();
        }

        public void LoadLevel(string levelName)
        {
            var test = gameObjectFactory.GetGameObject("player", 400f, 400f);

            var manager = (PlayerManager)GameServiceManager.GetService(typeof(PlayerManager));
            var player = manager.GetPlayer(LogicalPlayerIndex.One);
            test.UpdateMetadata("player", player);
            test.PostInitialize();
            test.Active = true;

            test = gameObjectFactory.GetGameObject("EnemySpawner");
            test.PostInitialize();
            test.Active = true;

            test = gameObjectFactory.GetGameObject("ScoreDisplay", 100f, 100f);
            test.PostInitialize();
            test.Active = true;
        }

        public override void Update(GameTime gameTime)
        {
            var gameObjects = GameServiceManager.GetService<IGameObjectManager>().GetAll();
            foreach (var gameObject in gameObjects)
            {
                if (gameObject.Active)
                    gameObject.Update(gameTime);
            }
        }
    }
}
