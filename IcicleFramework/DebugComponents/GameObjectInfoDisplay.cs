using System.Text;
using IcicleFramework.GameServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace IcicleFramework.DebugComponents
{

    /// <summary>
    /// GameObjectInfoDisplay provides debug information related to Game Objects.
    /// </summary>
    public class GameObjectInfoDisplay : DrawableGameComponent
    {

        #region Internal Members

        /// <summary>
        /// ContentManager for this GameObjectInfoDisplay so we can load fonts required.
        /// </summary>
        private ContentManager content;

        /// <summary>
        /// SpriteBatch used to draw this GameObjectInfoDisplay to the screen.
        /// </summary>
        private SpriteBatch spriteBatch;

        /// <summary>
        /// SpriteFont used for display.
        /// </summary>
        private SpriteFont spriteFont;

        /// <summary>
        /// TitleSafeArea for drawing information to ensure it does not "overscan" out of the screen.
        /// </summary>
        private Rectangle titleSafeArea;

        /// <summary>
        /// The path to the font to load.
        /// </summary>
        private string pathToFont;

        /// <summary>
        /// Stores the current information.
        /// </summary>
        private StringBuilder infoString;

        /// <summary>
        /// Frames elapsed between subsequent calls to Update().
        /// </summary>
        private int elapsedFrames;

        /// <summary>
        /// Amount of time between updates.
        /// </summary>
        private float secondsBetweenUpdates = 1f;

        private float secondsSinceLastUpdate = 0f;

        private IGameObjectManager gameObjectManager;

        private Vector2 position;

        #endregion


        #region Constructors

        /// <summary>
        /// Creates a new display object for showing game object information
        /// </summary>
        /// <param name="game">The game this GameObjectInfoDisplay belongs to</param>
        /// <param name="frameUpdate"></param>
        /// <param name="pathToFont">The path to the font to use. (Content\...)</param>
        public GameObjectInfoDisplay(Game game, float secondsBetweenUpdates, string pathToFont, Vector2 position)
            : base(game)
        {
            content = new ContentManager(game.Services);
            titleSafeArea = game.GraphicsDevice.Viewport.TitleSafeArea;
            this.pathToFont = pathToFont;
            this.secondsBetweenUpdates = secondsBetweenUpdates;
            this.position = position;
        }

        #endregion


        #region Initialization

        public override void Initialize()
        {
            base.Initialize();
            LoadContent();
        }

        /// <summary>
        /// Loads all necessary assets for the GameObjectInfoDisplay
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteFont = content.Load<SpriteFont>(pathToFont);

            gameObjectManager = GameServiceManager.GetService<IGameObjectManager>();
            infoString = new StringBuilder();

            base.LoadContent();
        }

        /// <summary>
        /// Unloads all loaded assets for this GameObjectInfoDisplay
        /// </summary>
        protected override void UnloadContent()
        {
            content.Unload();
            base.UnloadContent();
        }

        #endregion


        #region Update and Draw

        /// <summary>
        /// Updates the GameObjectInfoDisplay and determines the current frame rate.
        /// </summary>
        /// <param name="gameTime">Game timing information.</param>
        public override void Update(GameTime gameTime)
        {
            secondsSinceLastUpdate += (float) gameTime.ElapsedGameTime.TotalSeconds;

            if (secondsSinceLastUpdate > secondsBetweenUpdates)
            {
                secondsSinceLastUpdate = 0f;
                infoString.Clear();

                infoString.AppendLine(string.Format("Total Number of Game Objects: {0}",
                                                    gameObjectManager.TotalGameObjects));
                infoString.AppendLine(string.Format("\tActive Game Objects: {0}",
                                                    gameObjectManager.TotalActiveGameObjects));
                infoString.AppendLine(string.Format("\tDestroyed Game Objects: {0}",
                                                    gameObjectManager.TotalDestroyedGameObjects));
            }
        }

        /// <summary>
        /// Draws the current Game Object information
        /// </summary>
        /// <param name="gameTime">Game timing information.</param>
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(spriteFont, infoString.ToString(), position, Color.White);
            spriteBatch.End();
        }

        #endregion

    }

}
