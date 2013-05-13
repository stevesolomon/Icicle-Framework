using ExampleGameSHMUP.GameServices;
using IcicleFramework;
using IcicleFramework.Cameras;
using IcicleFramework.Collision;
using IcicleFramework.DebugComponents;
using IcicleFramework.Entities;
using IcicleFramework.GameServices;
using IcicleFramework.GameServices.CameraServices;
using IcicleFramework.GameServices.Factories;
using IcicleFramework.GameServices.HelperServices;
using IcicleFramework.GameServices.ParticleServices;
using IcicleFramework.Inputs;
using IcicleFramework.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectMercury.Renderers;

namespace ExampleGameSHMUP
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class ExampleSHMUP : Game
    {
        public static GraphicsDeviceManager Graphics { get; private set; }

        private SpriteBatch spriteBatch;

        private SpriteBatchRenderer particleRenderer;

        private ParticleManager particleManager;

        private ICameraService cameraService;

        private IGameObject test;

        private Renderer renderer;

        public ExampleSHMUP()
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
            Graphics.PreferredBackBufferWidth = Constants.InternalResolutionWidth;
            Graphics.PreferredBackBufferHeight = Constants.InternalResolutionHeight;

            Graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;

            Graphics.ApplyChanges();

            //Components.Add(new FrameRateCounter(this, "Content\\Fonts\\fpsfont", 1f));
            Components.Add(new GameObjectInfoDisplay(this, 0.5f, "Content\\Fonts\\debugfont", new Vector2(0f, 25f)));


            ComponentFactory componentFactory = new ComponentFactory();
            GameObjectFactory gameObjectFactory = new GameObjectFactory(this.Content);
            gameObjectFactory.PathToXML = "EntityDefinitions\\entity.xml";

            InputHandler inputHandler = new InputHandler(false);
            PlayerManager playerManager = new PlayerManager();

            var collisionManager = new CollisionManager(new RectangleF(0f, 0f, Constants.InternalResolutionWidth, Constants.InternalResolutionHeight));

            particleManager = new ParticleManager(Content, "ParticleEffects\\", "Textures\\");

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            renderer = new Renderer(Graphics, spriteBatch);
            renderer.SpriteSortMode = SpriteSortMode.FrontToBack;
            renderer.SetInternalResolution(Constants.InternalResolutionWidth, Constants.InternalResolutionHeight);
            renderer.SetScreenResolution(1920, 1080, false);

            particleRenderer = new SpriteBatchRenderer();
            particleRenderer.GraphicsDeviceService = Graphics;

            cameraService =  new CameraManager();

            BasicCamera2D camera = new BasicCamera2D(GraphicsDevice, "main");

            cameraService.AddCamera(camera);

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
            GameServiceManager.AddService(typeof(IProjectileManager), new ProjectileManager());
            GameServiceManager.AddService(typeof(IActionManager), new ActionManager());
            GameServiceManager.AddService(typeof(ILayerManager), new LayerManager("layers.xml", this.Content));
            GameServiceManager.AddService(typeof(IRenderer), renderer);
            GameServiceManager.AddService(typeof(ICameraService), cameraService);
            GameServiceManager.AddService(typeof(ILevelLogicManager), new LevelLogicManager());

            //Initialize the GameServices););
            foreach (IGameService service in GameServiceManager.Services)
            {
                service.Initialize();
            }

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            particleRenderer.LoadContent(Content);

            PlayerManager manager = (PlayerManager)GameServiceManager.GetService(typeof(PlayerManager));
            manager.SetPlayer(LogicalPlayerIndex.One, PlayerIndex.One);

            foreach (IGameService service in GameServiceManager.Services)
            {
                service.PostInitialize();
            }
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

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            renderer.PrepareDraw(cameraService.MainCamera.View);

            renderer.ClearScreen();

            renderer.Draw();

            renderer.EndDraw();

            foreach (var effect in particleManager.GetAllActiveParticleEffects())
            {
                var view = cameraService.MainCamera.View;
                var projection = cameraService.MainCamera.Projection;

                var position = new Vector3(cameraService.MainCamera.Position, 0f);

                particleRenderer.Transformation = renderer.ResolutionTransformMatrix * view;
                particleRenderer.RenderEffect(effect, ref view, ref view, ref projection, ref position); //The matrices don't actually do anything in 2D.
            }
            
            base.Draw(gameTime);
        }
    }
}