using IcicleFramework;
using IcicleFramework.Cameras;
using IcicleFramework.Collision;
using IcicleFramework.Collision.QuadTree;
using IcicleFramework.DebugComponents;
using IcicleFramework.Entities;
using IcicleFramework.GameServices;
using IcicleFramework.GameServices.Factories;
using IcicleFramework.GameServices.HelperServices;
using IcicleFramework.GameServices.ParticleServices;
using IcicleFramework.Inputs;
using IcicleFramework.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TestBed
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class TestBed : Microsoft.Xna.Framework.Game
    {
        public static GraphicsDeviceManager Graphics { get; private set; }

        private SpriteBatch spriteBatch;

        private IGameObject test;

        private Renderer renderer;

        public TestBed()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Graphics.PreferredBackBufferWidth = 1920;
            Graphics.PreferredBackBufferHeight = 1080;

            Graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;

            Graphics.ApplyChanges();

            Components.Add(new FrameRateCounter(this, "Content\\Fonts\\fpsfont"));
            Components.Add(new GameObjectInfoDisplay(this, 5, "Content\\Fonts\\debugfont", new Vector2(0f, 25f)));


            ComponentFactory componentFactory = new ComponentFactory();
            GameObjectFactory gameObjectFactory = new GameObjectFactory(this.Content);
            gameObjectFactory.PathToXML = "EntityDefinitions\\entity.xml";

            InputHandler inputHandler = new InputHandler(false);
            PlayerManager playerManager = new PlayerManager();

            var collisionManager = new CollisionManager(new RectangleF(0f, 0f, Constants.INTERNAL_SCREEN_WIDTH, Constants.INTERNAL_SCREEN_HEIGHT));

            var particleManager = new ParticleManager(Content, "ParticleEffects\\", "Textures\\");

            GameServiceManager.AddService(typeof(IGameObjectFactory), gameObjectFactory);
            GameServiceManager.AddService(typeof(IComponentFactory), componentFactory);
            GameServiceManager.AddService(typeof(RenderableFactory), new RenderableFactory(this.Content));
            GameServiceManager.AddService(typeof(IInputHandler), inputHandler);
            GameServiceManager.AddService(typeof(PlayerManager), playerManager);
            GameServiceManager.AddService(typeof(IGameObjectManager), new GameObjectManager());
            GameServiceManager.AddService(typeof(ICollisionManager), collisionManager);
            GameServiceManager.AddService(typeof(IBehaviorFactory), new BehaviorFactory());
            GameServiceManager.AddService(typeof(IActionFactory), new ActionFactory());
            GameServiceManager.AddService(typeof(IRandomGenerator), new RandomGenerator());
            GameServiceManager.AddService(typeof(IParticleManager), particleManager);
            GameServiceManager.AddService(typeof(ILayerManager), new LayerManager("CollisionDefinitions\\layers.xml", this.Content));
            GameServiceManager.AddService(typeof(IActionManager), new ActionManager());

            //Initialize the GameServices););
            foreach (IGameService service in GameServiceManager.Services)
            {
                service.Initialize();
            }

            CameraController.Initialize(this);
            CameraController.MoveCamera(new Vector2(GraphicsDevice.Viewport.Width / 2f, GraphicsDevice.Viewport.Height / 2f));

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            PlayerManager manager = (PlayerManager)GameServiceManager.GetService(typeof(PlayerManager));
                manager.SetPlayer(LogicalPlayerIndex.One, PlayerIndex.One);
            
            renderer = new Renderer(Graphics, spriteBatch);
            renderer.SpriteSortMode = SpriteSortMode.Deferred;
            renderer.SetInternalResolution(1920, 1080);
            renderer.SetScreenResolution(1920, 1080, false);

            renderer.Initialize();

            GameServiceManager.AddService(typeof(IRenderer), renderer);

            var gameObjectFactory = GameServiceManager.GetService<IGameObjectFactory>();

            test = gameObjectFactory.GetGameObject("player");
            test.Position = new Vector2(250, 250);
            test.Active = true;
            
            var go = gameObjectFactory.GetGameObject("brick");
            go.Position = new Vector2(500, 500);
            go.Active = true;

            go = gameObjectFactory.GetGameObject("brick");
            go.Position = new Vector2(510, 500);
            go.Active = true;

            go = gameObjectFactory.GetGameObject("brick");
            go.Position = new Vector2(800, 700);
            go.Active = true;

            go = gameObjectFactory.GetGameObject("brick2");
            go.Position = new Vector2(1520, 100);
            go.Active = true;
            
            Player player = manager.GetPlayer(LogicalPlayerIndex.One);
            test.UpdateMetadata("player", player);
            test.PostInitialize();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Escape))
                Exit();

            GameServiceManager.Update(gameTime);

            var gameObjects = GameServiceManager.GetService<IGameObjectManager>().GetAll();
            foreach (IGameObject gameObject in gameObjects)
            {
                if (gameObject.Active)
                    gameObject.Update(gameTime);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Matrix projection = CameraController.GetProjection();
            Matrix view = CameraController.GetView();

            renderer.PrepareDraw(view);

            renderer.ClearScreen();

            renderer.Draw();

            renderer.EndDraw();
            
            base.Draw(gameTime);
        }
    }
}
