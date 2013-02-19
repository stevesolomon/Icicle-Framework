using System;
using IcicleFramework.Cameras;
using IcicleFramework.GameServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using IcicleFramework.Inputs;

namespace ExampleGame.GameScreens
{
    public class GameplayScreen : GameScreen
    {

        #region Member Variables

        /// <summary>
        /// Transition time in seconds when transitions this screen to be active.
        /// </summary>
        private const float TRANSITION_ON_TIME = 1.5f;

        /// <summary>
        /// Transition time in seconds when transitions this screen to be deactivated.
        /// </summary>
        private const float TRANSITION_OFF_TIME = 1.0f;

        /// <summary>
        /// The ContentManager for this screen.
        /// </summary>
        private ContentManager content;

        /// <summary>
        /// The SpriteBatch used to draw to the screen.
        /// </summary>
        private SpriteBatch spriteBatch;

        /// <summary>
        /// Font used for debug purposes
        /// </summary>
        public static SpriteFont debugFont;

        #endregion
        

        #region Properties


        #endregion


        #region Constructors

        /// <summary>
        /// Default Constructor 
        /// Sets the transition times as well as the viewports
        /// </summary>
        public GameplayScreen(ExampleGame game)
        {
            myGame = game;
            content = game.Content;
            TransitionOnTime = TimeSpan.FromSeconds(TRANSITION_ON_TIME);
            TransitionOffTime = TimeSpan.FromSeconds(TRANSITION_OFF_TIME);
            CameraController.Initialize(myGame);
            debugFont = game.Content.Load<SpriteFont>("debugFont");
        }

        #endregion


        /// <summary>
        /// Loads all necessary graphics content for the GameplayScreen
        /// </summary>
        public override void LoadContent()
        {
            spriteBatch = this.ScreenManager.SpriteBatch;
        }

        /// <summary>
        /// Updates the screen.
        /// </summary>
        /// <param name="gameTime">Time since last update.</param>
        /// <param name="otherScreenHasFocus">True if some other screen has the focus.</param>
        /// <param name="coveredByOtherScreen">True if this screen is covered.</param>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        /// <summary>
        /// Draws the GameplayScreen and all relevant items.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0, 0);
        }


        #region Input Handling

        /// <summary>
        /// Handles all input for the GameplayScreen.
        /// </summary>
        /// <param name="gameTime">Time since last updated.</param>
        /// <param name="inputHandler">InputHandler containing input information.</param>
        public override void HandleInput(GameTime gameTime, InputHandler inputHandler)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Check if we should exit this screen. Currently only for DEBUG PURPOSES!!!
            //if (PlayerManager.PlayerOne != null)
            //{
             //   if (inputHandler.MenuCancelPressed(PlayerManager.PlayerOne))
              //      ExitScreen();
            //}

        }

        #endregion

    }
}
