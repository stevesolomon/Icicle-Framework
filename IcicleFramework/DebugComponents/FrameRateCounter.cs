using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace IcicleFramework.DebugComponents
{

    /// <summary>
    /// FrameRateCounter implements a stand-alone counter for displaying current frame rates in the game.
    /// </summary>
    public class FrameRateCounter : DrawableGameComponent
    {

        #region Internal Members

        /// <summary>
        /// ContentManager for this FrameRateCounter so we can load fonts required.
        /// </summary>
        private ContentManager content;

        /// <summary>
        /// SpriteBatch used to draw this FrameRateCounter to the screen.
        /// </summary>
        private SpriteBatch spriteBatch;

        /// <summary>
        /// SpriteFont used for displaying the Frame Rate information.
        /// </summary>
        private SpriteFont spriteFont;

        /// <summary>
        /// TitleSafeArea for drawing FPS information to ensure it does not "overscan" out of the screen.
        /// </summary>
        private Rectangle titleSafeArea;

        /// <summary>
        /// The path to the font to load for displaying the FPS information on-screen.
        /// </summary>
        private string pathToFont;

        /// <summary>
        /// Stores the current frame rate of the game.
        /// </summary>
        private int frameRate = 0;

        /// <summary>
        /// Counts the number of frames per second total.
        /// </summary>
        private int frameCounter = 0;

        private string fpsString;


        private float timeBetweenUpdates = 0f;

        private TimeSpan timeSinceLastRedraw = TimeSpan.MaxValue;

        /// <summary>
        /// Time elapsed between subsequent calls to Update().
        /// </summary>
        private TimeSpan elapsedTime = TimeSpan.Zero;

        #endregion


        #region Properties

        public float TimeBetweenUpdates
        {
            get { return timeBetweenUpdates; }
            set { timeBetweenUpdates = value; }
        }

        #endregion


        #region Constructors

        /// <summary>
        /// Creates a new counter to track game frame rates.
        /// </summary>
        /// <param name="game">The game this FrameRateCounter belongs to</param>
        /// <param name="pathToFont">The path to the font to use for displaying FPS. (Content\...)</param>
        public FrameRateCounter(Game game, string pathToFont, float timeBetweenUpdates = 0f)
            : base(game)
        {
            content = new ContentManager(game.Services);
            titleSafeArea = game.GraphicsDevice.Viewport.TitleSafeArea;
            this.pathToFont = pathToFont;
            this.timeBetweenUpdates = timeBetweenUpdates;
        }

        #endregion


        #region Initialization

        public override void Initialize()
        {
            base.Initialize();
            LoadContent();
        }

        /// <summary>
        /// Loads all necessary assets for the FrameRateCounter
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteFont = content.Load<SpriteFont>(pathToFont);
            base.LoadContent();
        }

        /// <summary>
        /// Unloads all loaded assets for this FrameRateCounter
        /// </summary>
        protected override void UnloadContent()
        {
            content.Unload();
            base.UnloadContent();
        }

        #endregion


        #region Update and Draw

        /// <summary>
        /// Updates the FrameRateCounter and determines the current frame rate.
        /// </summary>
        /// <param name="gameTime">Game timing information.</param>
        public override void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }
        }

        /// <summary>
        /// Draws the current frames per second to the screen.
        /// </summary>
        /// <param name="gameTime">Game timing information.</param>
        public override void Draw(GameTime gameTime)
        {
            frameCounter++;
            timeSinceLastRedraw += gameTime.ElapsedGameTime;

            if (timeSinceLastRedraw.TotalSeconds > timeBetweenUpdates)
            {
                fpsString = string.Format("fps: {0}", frameRate);

                timeSinceLastRedraw = TimeSpan.Zero;
            }
            spriteBatch.Begin();
            spriteBatch.DrawString(spriteFont, fpsString,
                                   new Vector2(titleSafeArea.Left, titleSafeArea.Top), Color.White);
            spriteBatch.End();
        }

        #endregion

    }

}
