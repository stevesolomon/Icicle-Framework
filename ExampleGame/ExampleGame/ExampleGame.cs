using System.Collections.Generic;
using ExampleGame.GameSystems;
using FarseerPhysics;
using FarseerPhysics.DebugViews;
using IcicleFramework;
using IcicleFramework.Cameras;
using IcicleFramework.Collision;
using IcicleFramework.Components.Renderable;
using IcicleFramework.DebugComponents;
using IcicleFramework.Entities;
using IcicleFramework.GameServices.Factories;
using IcicleFramework.GameServices.HelperServices;
using IcicleFramework.GameServices.ParticleServices;
using IcicleFramework.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using IcicleFramework.GameServices;
using IcicleFramework.Inputs;
using IcicleFramework.Rendering;
using ProjectMercury.Renderers;

namespace ExampleGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class ExampleGame : Game
    {
        public static GraphicsDeviceManager Graphics { get; private set; }
        SpriteBatch spriteBatch;

        private DebugViewXNA debugView; 
        
        private Level level;
        
        private Renderer renderer;

        private IParticleManager particleManager;

        private SpriteBatchRenderer particleRenderer;
        
        public static ContentManager ContentManager { get; private set; }

        private IInputHandler inputHandler;

        private PlayerManager playerManager;
        
        public ExampleGame()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            Graphics.SynchronizeWithVerticalRetrace = false;

            IsFixedTimeStep = false;

            ContentManager = Content;
        }

        /// <summary> 
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Graphics.PreferredBackBufferWidth = 1280;
            Graphics.PreferredBackBufferHeight = 720;
            Graphics.ApplyChanges();
            
            Components.Add(new FrameRateCounter(this, "Content\\debugfont", 1f));
            Components.Add(new GameObjectInfoDisplay(this, 5, "Content\\debugfont", new Vector2(0f, 25f)));

            ComponentFactory componentFactory = new ComponentFactory();
            TiledGameObjectFactory gameObjectFactory = new TiledGameObjectFactory(this.Content);
            gameObjectFactory.PathToXML = "EntityDefinitions\\entity.xml";
            
            inputHandler = new InputHandler(false);
            playerManager = new PlayerManager();

            particleManager = new ParticleManager(Content, "ParticleEffects\\", "Textures\\");

            PhysicsManager physicsManager = new PhysicsManager();
            particleRenderer = new SpriteBatchRenderer();
            particleRenderer.GraphicsDeviceService = Graphics;

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            renderer = new Renderer(Graphics, spriteBatch);
            renderer.SetInternalResolution(1280, 720);
            renderer.SetScreenResolution(1280, 720, false);

            GameServiceManager.AddService(typeof(IGameObjectFactory), gameObjectFactory);
            GameServiceManager.AddService(typeof(IComponentFactory), componentFactory);
            GameServiceManager.AddService(typeof(RenderableFactory), new RenderableFactory(this.Content));
            GameServiceManager.AddService(typeof(IInputHandler), inputHandler);
            GameServiceManager.AddService(typeof(PlayerManager), playerManager);
            GameServiceManager.AddService(typeof(IScoreManager), new ScoreManager());
            GameServiceManager.AddService(typeof(IGameObjectManager), new GameObjectManager());
            GameServiceManager.AddService(typeof(IPhysicsManager), physicsManager);
            GameServiceManager.AddService(typeof(IBehaviorFactory), new BehaviorFactory());
            GameServiceManager.AddService(typeof(IActionFactory), new ActionFactory());
            GameServiceManager.AddService(typeof(IActionManager), new ActionManager());
            GameServiceManager.AddService(typeof(ILevelLogicManager), new LevelLogicManager(this.Content));
            GameServiceManager.AddService(typeof(IRandomGenerator), new RandomGenerator());
            GameServiceManager.AddService(typeof(IParticleManager), particleManager);
            GameServiceManager.AddService(typeof(IRenderer), renderer);

            debugView = new DebugViewXNA(GameServiceManager.GetService<IPhysicsManager>().PhysicsWorld);

            //Initialize the GameServices
            GameServiceManager.Initialize();
            
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
            particleRenderer.LoadContent(Content);

            debugView.LoadContent(this.GraphicsDevice, this.Content);
            debugView.AppendFlags(DebugViewFlags.Shape);
            debugView.AppendFlags(DebugViewFlags.PolygonPoints);

            if (inputHandler.GetPlayerMapping(LogicalPlayerIndex.One) == null)
            {
                playerManager.SetPlayer(LogicalPlayerIndex.One, PlayerIndex.One);
            }

            ILevelLogicManager logicManager = GameServiceManager.GetService<ILevelLogicManager>();
            logicManager.LoadLevel("trialmap");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            
        }
        bool buildLevel = false;
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            ILevelLogicManager logicManager = GameServiceManager.GetService<ILevelLogicManager>();

            if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Escape))
                this.Exit();

            logicManager.Update(gameTime);

            

            //Hey wow this is suuuper bad code positioning but it's not always going to be like this okay ;)
            /*if (!buildLevel)
            {
                LevelBuilder levelBuilder = new LevelBuilder();
                levelBuilder.Initialize();
                level = levelBuilder.BuildLevel(Content, "trialmap");

                buildLevel = true;

                //level.Initialize(new RectangleF(0, 0, 1280, 720));
            }*/

            //TEMPORARY code for showing how to register a controller with a 'player' in the InputHandler.
            /*if (inputHandler.GetPlayerMapping(LogicalPlayerIndex.One) == null)
            {
                //Wait until any of the controllers have pressed the A or Start buttons.
                for (PlayerIndex controller = PlayerIndex.One; controller <= PlayerIndex.Four; controller++)
                {
                    if (inputHandler.IsDown(Buttons.A, controller) || inputHandler.IsDown(Buttons.Start, controller))
                    {
                        //Someone pressed A or Start. Assign the controller to the first logical player for the time being.
                        playerManager.SetPlayer(LogicalPlayerIndex.One, controller);
                        buildLevel = true;
                         
                        break;
                    }
                }
            }*/

            

            //Update any of the game services that can actually be updated...
            GameServiceManager.Update(gameTime);

            CameraController.Update(gameTime);

           // if (level != null)
            //    level.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Matrix projection = CameraController.GetProjection();
            Matrix view = CameraController.GetView();

            renderer.PrepareDraw(view);
            
            renderer.ClearScreen();

            renderer.Draw();
            
            renderer.EndDraw();

            foreach (var effect in particleManager.GetAllActiveParticleEffects())
            {
                var test = Vector3.Zero;
                particleRenderer.Transformation = renderer.ResolutionTransformMatrix;
                particleRenderer.RenderEffect(effect, ref view, ref view, ref projection, ref test);
            }
            
            debugView.RenderDebugData(ref projection, ref view);


            base.Draw(gameTime);
        }
    }
}
