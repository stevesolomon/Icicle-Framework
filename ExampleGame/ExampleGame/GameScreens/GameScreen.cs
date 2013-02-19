using System;
using Microsoft.Xna.Framework;
using IcicleFramework.Inputs;

namespace ExampleGame.GameScreens
{
    /// <summary>
    /// The different states a screen can be in.
    /// 
    /// TransitionOn  - Screen is currently being transitioned into focus
    /// Active        - Screen is currently active
    /// TransitionOff - Screen is currently being transitioned out of focus
    /// Hidden        - Screen is hidden from view
    /// </summary>
    public enum ScreenState
    {
        TransitionOn,
        Active,
        TransitionOff,
        Hidden,
    }


    /// <summary>
    /// A screen is a single layer that has update and draw logic, and which
    /// can be combined with other layers to build up a complex menu system.
    /// For instance the main menu, the options menu, the "are you sure you
    /// want to quit" message box, and the main game itself are all implemented
    /// as screens.
    /// </summary>
    public abstract class GameScreen
    {

        #region Member Variables

        /// <summary>
        /// If the screen is a popup screen the screens underneath this screen
        /// will not transition off.
        /// </summary>
        protected bool isPopup = false;

        /// <summary>
        /// Amount of time to transition on when the screen becomes active
        /// </summary>
        protected TimeSpan transitionOnTime = TimeSpan.Zero;

        /// <summary>
        /// Amount of time to transition off when the screen is deactivated
        /// </summary>
        protected TimeSpan transitionOffTime = TimeSpan.Zero;

        /// <summary>
        /// The current state of the transition.
        /// </summary>
        protected float transitionPosition = 1;

        /// <summary>
        /// The current state of the screen.
        /// </summary>
        protected ScreenState screenState = ScreenState.TransitionOn;

        /// <summary>
        /// Whether or not the screen is exiting for good (true) or 
        /// merely transitioning to a hidden state (false).
        /// </summary>
        protected bool isExiting = false;

        /// <summary>
        /// True if the screen is not active and will not respond to user input
        /// False if otherwise.
        /// </summary>
        protected bool otherScreenHasFocus;

        /// <summary>
        /// The ScreenManager that controls this screen.
        /// </summary>
        protected ScreenManager screenManager;

        /// <summary>
        /// Reference back to the game object itself.
        /// </summary>
        protected ExampleGame myGame;

        #endregion


        #region Properties

        /// <summary>
        /// Normally when one screen is brought up over the top of another,
        /// the first screen will transition off to make room for the new
        /// one. This property indicates whether the screen is only a small
        /// popup, in which case screens underneath it do not need to bother
        /// transitioning off.
        /// </summary>
        public bool IsPopup
        {
            get { return isPopup; }
            protected set { isPopup = value; }
        }


        /// <summary>
        /// Indicates how long the screen takes to
        /// transition on when it is activated.
        /// </summary>
        public TimeSpan TransitionOnTime
        {
            get { return transitionOnTime; }
            protected set { transitionOnTime = value; }
        }


        /// <summary>
        /// Indicates how long the screen takes to
        /// transition off when it is deactivated.
        /// </summary>
        public TimeSpan TransitionOffTime
        {
            get { return transitionOffTime; }
            protected set { transitionOffTime = value; }
        }


        /// <summary>
        /// Gets the current position of the screen transition, ranging
        /// from zero (fully active, no transition) to one (transitioned
        /// fully off to nothing).
        /// </summary>
        public float TransitionPosition
        {
            get { return transitionPosition; }
            protected set { transitionPosition = value; }
        }


        /// <summary>
        /// Gets the current alpha of the screen transition, ranging
        /// from 255 (fully active, no transition) to 0 (transitioned
        /// fully off to nothing).
        /// </summary>
        public byte TransitionAlpha
        {
            get { return (byte)(255 - TransitionPosition * 255); }
        }


        /// <summary>
        /// Gets the current screen transition state.
        /// </summary>
        public ScreenState ScreenState
        {
            get { return screenState; }
            protected set { screenState = value; }
        }


        /// <summary>
        /// There are two possible reasons why a screen might be transitioning
        /// off. It could be temporarily going away to make room for another
        /// screen that is on top of it, or it could be going away for good.
        /// This property indicates whether the screen is exiting for real:
        /// if set, the screen will automatically remove itself as soon as the
        /// transition finishes.
        /// </summary>
        public bool IsExiting
        {
            get { return isExiting; }
            protected internal set { isExiting = value; }
        }


        /// <summary>
        /// Checks whether this screen is active and can respond to user input.
        /// </summary>
        public bool IsActive
        {
            get
            {
                return !otherScreenHasFocus &&
                       (screenState == ScreenState.TransitionOn ||
                        screenState == ScreenState.Active);
            }
        }

        /// <summary>
        /// Gets the manager that this screen belongs to.
        /// </summary>
        public ScreenManager ScreenManager
        {
            get { return screenManager; }
            internal set { screenManager = value; }
        }


        #endregion


        #region Initialization


        /// <summary>
        /// Load content for the screen.
        /// </summary>
        public virtual void LoadContent() { }


        /// <summary>
        /// Unload content for the screen.
        /// </summary>
        public virtual void UnloadContent() { }


        #endregion


        #region Update and Draw


        /// <summary>
        /// Allows the screen to run logic, such as updating the transition position.
        /// Unlike HandleInput, this method is called regardless of whether the screen
        /// is active, hidden, or in the middle of a transition.
        /// It should be noted that Input is *NOT* handled in this method.
        /// </summary>
        public virtual void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                      bool coveredByOtherScreen)
        {
            this.otherScreenHasFocus = otherScreenHasFocus;

            //Handle the case where the screen is going to be disposed of.
            if (isExiting)
            {
                // If the screen is going away to die, it should transition off.
                screenState = ScreenState.TransitionOff;

                //When the transition finishes, remove the screen.
                if (!UpdateTransition(gameTime, transitionOffTime, 1))
                    ScreenManager.RemoveScreen(this);

            }
            else if (coveredByOtherScreen) //We are covered by another screen, transition to off (hidden)
            {
                if (UpdateTransition(gameTime, transitionOffTime, 1)) //Still transitioning
                {
                    screenState = ScreenState.TransitionOff;
                }
                else //Finished transitioning, hide the screen
                {
                    screenState = ScreenState.Hidden;
                }
            }
            else //Transition on, if we are not already, and show the screen as active
            {
                if (UpdateTransition(gameTime, transitionOnTime, -1)) //Still transitioning on
                {
                    screenState = ScreenState.TransitionOn;
                }
                else //Finished transitioning, show the screen
                {
                    screenState = ScreenState.Active;
                }
            }
        }

        /// <summary>       
        /// Helper for updating the screen transition position.    
        /// </summary>
        /// <param name="gameTime">Time elapsed since last Update()</param>
        /// <param name="time">Time the transition is to take</param>
        /// <param name="direction">1 for Off, -1 for On</param>
        /// <returns>True if the transition is still going, false if we're done.</returns>
        bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
        {
            // How much should we move by?
            float transitionDelta;

            if (time == TimeSpan.Zero)
                transitionDelta = 1;
            else //Transition position depends on how much time elapsed since the last Update.
                transitionDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                          time.TotalMilliseconds);

            //Update the transition position.
            transitionPosition += transitionDelta * direction;

            //Did we reach the end of the transition?
            if ((transitionPosition <= 0) || (transitionPosition >= 1))
            {
                transitionPosition = MathHelper.Clamp(transitionPosition, 0, 1);
                return false;
            }

            //We are still transitioning.
            return true;
        }


        /// <summary>
        /// Gives the screen the chance to handle user input.
        /// This may not be called every time like Update(), but only in
        /// cases where the screen is active.
        /// </summary>
        /// <param name="gameTime">Information related to this time since the last Update.</param>
        /// <param name="input">The InputManager controlling input for the game.</param>
        public virtual void HandleInput(GameTime gameTime, InputHandler input) { }


        /// <summary>
        /// This is called when the screen should draw itself.
        /// </summary>
        public virtual void Draw(GameTime gameTime) { }


        #endregion


        #region Public Methods


        /// <summary>
        /// Tells the screen to go away. Unlike ScreenManager.RemoveScreen, which
        /// instantly kills the screen, this method respects the transition timings
        /// and will give the screen a chance to gradually transition off.
        /// </summary>
        public void ExitScreen()
        {
            if (TransitionOffTime == TimeSpan.Zero) //If the screen has a zero transition time, remove it immediately.
            {
                ScreenManager.RemoveScreen(this);
            }
            else // Otherwise flag that it should transition off and then exit.
            {
                isExiting = true;
            }
        }

        #endregion


    }
}
