using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using IcicleFramework.Inputs;

namespace ExampleGame.GameScreens
{
    /// <summary>
    /// ScreenManager manages all the screens that make up the game.
    /// A list of current screens is maintained, and these are updated
    /// (via their Update and Draw methods) when the ScreenManager is
    /// to be Drawn to Updated.
    /// </summary>
    public class ScreenManager : DrawableGameComponent
    {

        public Game myGame;

        //List of screens currently in the Manager
        private List<GameScreen> screens = new List<GameScreen>();

        //List of screens currently requiring an Update
        private List<GameScreen> screensToUpdate = new List<GameScreen>();

        //SpriteBatch and Font that may carry over to all screens in the
        //manager, so that each individual screen does not have to worry
        //about creating their own.
        private SpriteBatch spriteBatch;
        private SpriteFont font;
        private SpriteFont titleFont;

        private Texture2D blankTexture;

        private Boolean isInitialized = false;

        private Boolean traceEnabled = false;


        #region Properties

        //Properties for getting at the spriteBatch and spriteFont
        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }

        public SpriteFont Font
        {
            get { return font; }
        }

        public SpriteFont TitleFont
        {
            get { return titleFont; }
        }

        public Boolean IsInitialized
        {
            get { return isInitialized; }
        }

        #endregion


        #region Initialization

        public ScreenManager(Game game)
            : base(game)
        {
            myGame = game;
        }


        public override void Initialize()
        {
            base.Initialize();

            isInitialized = true;
        }


        /// <summary>
        /// Loads all required graphics content.
        /// </summary>
        protected override void LoadContent()
        {
            // Load content belonging to the screen manager.
            ContentManager content = Game.Content;

            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Load our SpriteFont as well as a default blank texture
            font = content.Load<SpriteFont>("menuFont");
            titleFont = content.Load<SpriteFont>("titleFont");
            blankTexture = content.Load<Texture2D>("blank");

            // Tell each of the screens to load their content.
            foreach (GameScreen screen in screens)
            {
                screen.LoadContent();
            }
        }


        /// <summary>
        /// Unloads graphics content from each screen.
        /// </summary>
        protected override void UnloadContent()
        {
            // Tell each of the screens to unload their content.
            foreach (GameScreen screen in screens)
            {
                screen.UnloadContent();
            }
        }

        #endregion


        public override void Update(GameTime gameTime)
        {
            //If a different screen has focus, we want to know.
            bool otherScreenHasFocus = !Game.IsActive;

            //This is true if the screen we are currently looking at is not the top level
            //screen in the current game. IE: It may be a menu screen or a message screen
            //above the gameplay screen, for example.
            bool coveredByOtherScreen = false;

            //Copy the current list of screens as updating one screen
            //may remove screens from the current list and we don't
            //want theis random activity to impact the update procedure!
            screensToUpdate.Clear();

            foreach (GameScreen screen in screens)
                screensToUpdate.Add(screen);

            //Now update all the screens currently in the list of screens
            while (screensToUpdate.Count > 0)
            {
                //Grab the highest level screen from the list. This is assumed to be
                //the "top most" screen currently active in the game. Make sure we
                //remove it from this list as well, so we don't update it again.
                GameScreen screen = screensToUpdate[screensToUpdate.Count - 1];
                screensToUpdate.RemoveAt(screensToUpdate.Count - 1);

                //Now update this screen
                screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

                //Check if we should give the screen a chance to handle
                //input or if subsequent screens should consider themselves
                //to be "covered" by this screen.
                if (screen.ScreenState == ScreenState.TransitionOn ||
                    screen.ScreenState == ScreenState.Active)
                {
                    // If this is the first active screen we came across,
                    // give it a chance to handle input.
                    if (!otherScreenHasFocus)
                    {
                        //screen.HandleInput(gameTime, inputHandler);
                        otherScreenHasFocus = true;
                    }

                    // If this is an active non-popup, inform any subsequent
                    // screens that they are covered by it.
                    if (!screen.IsPopup)
                        coveredByOtherScreen = true;
                }

                // Print debug trace?
                if (traceEnabled)
                    TraceScreens();

            }//while...
        }//Update



        /// <summary>
        /// Prints a list of all the screens, for debugging.
        /// </summary>
        void TraceScreens()
        {
#if WINDOWS
            Trace.WriteLine(string.Join(", ", screens.Select(screen => screen.GetType().Name).ToArray()));
#endif
        }



        /// <summary>
        /// Tells each screen to draw itself.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            foreach (GameScreen screen in screens)
            {
                if (screen.ScreenState != ScreenState.Hidden)
                    screen.Draw(gameTime);
            }
        }//Draw


        /// <summary>
        /// Adds a new screen to the screen manager.
        /// </summary>
        /// <param name="screen">The new GameScreen to add to the Manager.</param>
        public void AddScreen(GameScreen screen)
        {
            screen.ScreenManager = this;
            screen.IsExiting = false;

            //If we have a graphics device, tell the screen to load content.
            if (isInitialized)
            {
                screen.LoadContent();
            }

            screens.Add(screen);
        }//AddScreen


        /// <summary>
        /// Removes a screen from the screen manager. You should normally
        /// use GameScreen.ExitScreen instead of calling this directly, so
        /// the screen can gradually transition off rather than just being
        /// instantly removed.
        /// </summary>
        /// <param name="screen">The GameScreen to remove from the Manager.</param>
        public void RemoveScreen(GameScreen screen)
        {
            //If we have a graphics device, tell the screen to unload content.
            if (isInitialized)
            {
                screen.UnloadContent();
            }

            screens.Remove(screen);
            screensToUpdate.Remove(screen);
        }//RemoveScreen


        /// <summary>
        /// Returns a copy of all the screens in the ScreenManager.
        /// A copy, as screens should never be removed or added outside of 
        /// the ScreenManager itself.
        /// </summary>
        /// <returns>An array of all screens in the Manager.</returns>
        public GameScreen[] GetScreens()
        {
            return screens.ToArray();
        }


        /// <summary>
        /// Helper draws a translucent black sprite in the given area, used for fading
        /// screens in and out, and for darkening the background behind popups.
        /// </summary>
        /// <param name="alpha">Amount of transparency to use in the black sprite.</param>
        /// <param name="position">The upper left position to start the sprite size.</param>
        /// <param name="area">The area to apply the translucent sprite to.</param>
        public void FadeBackBufferToBlack(int alpha, Vector2 position, Rectangle area)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(blankTexture,
                             new Rectangle((int)position.X, (int)position.Y, area.Width, area.Height),
                             new Color(0, 0, 0, (byte)alpha));

            spriteBatch.End();
        }


    }//Class ScreenManager

}
