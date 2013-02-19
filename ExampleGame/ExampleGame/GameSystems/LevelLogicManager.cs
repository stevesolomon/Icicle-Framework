using System.Globalization;
using FarseerPhysics.Dynamics.Contacts;
using IcicleFramework.Components.Physics;
using IcicleFramework.Entities;
using IcicleFramework.GameServices;
using IcicleFramework.Physics;
using IcicleFramework.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ExampleGame.GameSystems
{
    public class LevelLogicManager : GameService, ILevelLogicManager
    {
        public event LivesChangedHandler OnPlayerLivesChanged; 
        private IGameObject player;

        private bool subscribedToBall;

        private IPhysicsManager physicsManager;

        private IGameObjectManager gameObjectManager;

        private IRenderer renderer;

        private Level level;

        private LevelBuilder levelBuilder;

        private Vector2 ballStartPosition;

        private ContentManager content;

        public bool GameOver { get; private set; }

        public int Lives
        {
            get
            {
                if (player != null) //We should really have some better life management here but, hey, this is just a quick demo.
                {
                    return int.Parse(player.GetMetadata("lives").ToString());
                }

                return 3;
            }
        }

        public LevelLogicManager(ContentManager content)
        {
            this.content = content;
        }

        public override void Initialize()
        {
            //Let's grab a reference to the other services that we'll need!
            gameObjectManager = GameServiceManager.GetService<IGameObjectManager>();
            physicsManager = GameServiceManager.GetService<IPhysicsManager>();
            renderer = GameServiceManager.GetService<IRenderer>();

            levelBuilder = new LevelBuilder();
            levelBuilder.Initialize();

            GameOver = false;
        }

        public override void PostInitialize()
        {
            if (gameObjectManager != null && physicsManager != null)
            {
                player = gameObjectManager.FindWithMetadata("player");

                if (!subscribedToBall)
                {
                    IGameObject ball = gameObjectManager.FindWithMetadata("ball");

                    if (ball != null)
                    {
                        physicsManager.SubscribeCollisionEvent(ball.GUID, CollisionHandler);
                        subscribedToBall = true;
                    }
                }
            }

            base.PostInitialize();
        }

        public void LoadLevel(string levelName)
        {
            level = levelBuilder.BuildLevel(content, levelName);
            ballStartPosition = levelBuilder.ballStartLocation;
            GameOver = false;
        }

        public override void Update(GameTime gameTime)
        {
            level.Update(gameTime);

            base.Update(gameTime);
        }

        private void StartWin()
        {
            IGameObject youWinObject = gameObjectManager.FindWithMetadata("youwin");

            foreach (IGameObject gameObject in gameObjectManager.GetAll())
                gameObject.Active = false;

            if (youWinObject != null)
                youWinObject.Active = true;
        }

        private void StartGameOver()
        {
            IGameObject gameOverObject = gameObjectManager.FindWithMetadata("gameover");

            foreach (IGameObject gameObject in gameObjectManager.GetAll())
                gameObject.Active = false;


            if (gameOverObject != null)
                gameOverObject.Active = true;
        }
        
        private void CollisionHandler(IPhysicsComponent source, IPhysicsComponent colliding, Contact contact)
        {
            if (colliding.Parent.HasMetadata("bottom"))
            {
                if (player != null)
                {
                    int lives;
                    int.TryParse(player.GetMetadata("lives").ToString(), NumberStyles.Integer,CultureInfo.InvariantCulture, out lives);

                    if (lives == 0)
                    {
                        GameOver = true;
                        StartGameOver();
                    }
                    else
                    {
                        player.UpdateMetadata("lives", lives - 1);

                        if (OnPlayerLivesChanged != null)
                            OnPlayerLivesChanged(player, lives - 1);

                        ResetBall();
                    }
                }
            }

            if (colliding.Parent.HasMetadata("brick")) //We've collided with a brick, check if they're all gone and, if so, you win!
            {
                IGameObject brick = gameObjectManager.FindWithMetadata("brick");

                if (brick == null)
                    StartWin();
            }
        }

        private void ResetBall()
        {
            IGameObject ball = GameServiceManager.GetService<IGameObjectManager>().FindWithMetadata("ball");
            IPhysicsComponent ballPhysics = ball.GetComponent<IPhysicsComponent>();

            ballPhysics.Velocity = Vector2.Zero;

            ball.Position = ballStartPosition;
        }
    }
}
