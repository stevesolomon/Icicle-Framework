using System.Collections.Generic;
using IcicleFramework.GameServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;

namespace IcicleFramework.Inputs
{
    public class InputHandler : GameService, IInputHandler
    {

        #region Internal Classes

        private class LastButtonInfo<T>
        {
            public T Button;
            public float TimePressed;
            public bool IsRepetitionPress;
        }

        #endregion


        #region Constants

        private const Buttons LeftThumbstickButtons = Buttons.LeftThumbstickUp | Buttons.LeftThumbstickDown | Buttons.LeftThumbstickLeft | Buttons.LeftThumbstickRight;

        private const Buttons RightThumbstickButtons = Buttons.RightThumbstickUp | Buttons.RightThumbstickDown | Buttons.RightThumbstickLeft | Buttons.RightThumbstickRight;

        private const Buttons ThumbstickButtons = LeftThumbstickButtons | RightThumbstickButtons;

        private const Buttons Triggers = Buttons.LeftTrigger | Buttons.RightTrigger;

        private const Buttons AnalogButtons = ThumbstickButtons | Triggers;

        #endregion


        #region Internal Variables

        private static Keys[] keys;

        private static Buttons[] buttons;

        private InputConfiguration inputConfiguration;

        private PlayerIndex?[] logicalPlayerMapping;

        private bool[] controllerHandled;
        
        private LastButtonInfo<Keys> lastKeyPressed;

        private LastButtonInfo<Buttons>[] lastButtonPressed;

        private List<Keys> pressedKeys;

        private KeyboardState currKeyboardState;

        private KeyboardState prevKeyboardState;

        private GamePadState[] currGamePadStates;

        private GamePadState[] prevGamePadStates;

        private static readonly GamePadState BlankGamePadState = new GamePadState();

        private bool gamerServicesEnabled = false;

        #endregion


        #region Properties

        public InputConfiguration Configuration
        {
            get { return inputConfiguration; }
            set
            {
                if (value != null)
                    inputConfiguration = value;
            }
        }

        public int MaxPlayers { get { return 4; } }
        
        public bool KeyboardInputConsumed { get; set; }

        /// <summary>
        /// Gets the current state of keyboard input for this frame.
        /// </summary>
        public KeyboardState KeyboardState
        {
            get { return currKeyboardState; }
        }

        /// <summary>
        /// Gets the state of keyboard input for the previous frame.
        /// </summary>
        public KeyboardState PreviousKeyboardState
        {
            get { return prevKeyboardState; }
        }

        public GamePadState GamePadState { get; protected set; }

        #endregion


        #region Constructors and Initialization

        public InputHandler()
        {
            int i;

            //Load up some default settings and the logical player index mapping.
            inputConfiguration = new InputConfiguration();
            logicalPlayerMapping = new PlayerIndex?[MaxPlayers];

            //Initialize the last button information.
            lastKeyPressed = new LastButtonInfo<Keys>();
            lastButtonPressed = new LastButtonInfo<Buttons>[MaxPlayers];

            for (i = 0; i < lastButtonPressed.Length; i++)
                lastButtonPressed[i] = new LastButtonInfo<Buttons>();

            pressedKeys = new List<Keys>(10);

            // Gamepads
            controllerHandled = new bool[MaxPlayers];
            prevGamePadStates = new GamePadState[MaxPlayers];
            currGamePadStates = new GamePadState[MaxPlayers];
        }

        public InputHandler(bool gamerServicesEnabled)
            :this()
        {
            this.gamerServicesEnabled = gamerServicesEnabled;
        }
        
        /// <summary>
        /// Initializes the InputHandler with the key/button listings.
        /// </summary>
        public override void Initialize()
        {
            int i;
            Keys[] keysValues = EnumHelper.EnumToArray<Keys>();
            int maxNumberOfKeys = keysValues.Length;

            keys = new Keys[maxNumberOfKeys];

            for (i = 0; i < maxNumberOfKeys; i++)
                keys[i] = keysValues[i];

            Buttons[] buttonsValues = EnumHelper.EnumToArray<Buttons>();
            var maxNumberOfButtons = buttonsValues.Length;

            buttons = new Buttons[maxNumberOfButtons];

            for (i = 0; i < maxNumberOfButtons; i++)
                buttons[i] = buttonsValues[i];
        }

        #endregion


        #region Keyboard Methods

        private void UpdateKeyboard(float deltaTime)
        {
            //Update the state of the keyboard and record all the pressed keys...
            prevKeyboardState = currKeyboardState;
            currKeyboardState = Keyboard.GetState();
            
            //Record explicitly pressed keys...
            pressedKeys.Clear();

            foreach (Keys key in keys)
            {
                if (currKeyboardState.IsKeyDown(key) && prevKeyboardState.IsKeyUp(key))
                {
                    pressedKeys.Add(key);
                }
            }

            //Record a press that was caused by repetition iff no new keys
            //were physically pressed this frame.
            lastKeyPressed.IsRepetitionPress = false;

            if (pressedKeys.Count == 0)
            {
                //Make sure the previously pressed key is still being
                //pressed down before we do anything further...
                if (IsDown(lastKeyPressed.Button))
                {
                    lastKeyPressed.TimePressed += deltaTime;

                    //Generate a repetition key press if the key has been pressed long enough...
                    if (lastKeyPressed.TimePressed >= inputConfiguration.RepetitionTimeDelay)
                    {
                        pressedKeys.Add(lastKeyPressed.Button);
                        lastKeyPressed.IsRepetitionPress = true;

                        lastKeyPressed.TimePressed -= inputConfiguration.RepetitionTimeDelay;
                    }
                }
                else
                {
                    //Old key is no longer being depressed, reset the timer...
                    lastKeyPressed.TimePressed = 0;
                }
            }
            else
            {
                //New keys have been pressed, reset the timer and the last key pressed.
                lastKeyPressed.Button = pressedKeys[0];
                lastKeyPressed.TimePressed = 0;
            }
        }

        public bool IsDown(Keys key)
        {
            return currKeyboardState.IsKeyDown(key);
        }

        public bool IsUp(Keys key)
        {
            return currKeyboardState.IsKeyUp(key);
        }

        public bool IsNewlyPressed(Keys key, bool useKeyRepetition)
        {
            if (useKeyRepetition)
            {
                if (lastKeyPressed.Button == key && lastKeyPressed.IsRepetitionPress)
                {
                    return true;
                }
            }

            return currKeyboardState.IsKeyDown(key) && prevKeyboardState.IsKeyUp(key);
        }

        public bool IsReleased(Keys key)
        {
            return currKeyboardState.IsKeyUp(key) && prevKeyboardState.IsKeyDown(key);
        }

        #endregion


        #region Controller Methods

        /// <summary>  
        /// Gets the (controller) PlayerIndex for the player mapped to the (internal) logical player index.
        /// </summary>
        /// <param name="player">The logical player index to get a PlayerIndex mapping for.</param>
        /// <returns>The PlayerIndex corresponding to the provided logical player index, or null if no mapping exists.</returns>
        public PlayerIndex? GetPlayerMapping(LogicalPlayerIndex player)
        {
            return logicalPlayerMapping[(int) player];
        }

        /// <summary>
        /// Maps an internal logical player index to a controller index.
        /// </summary>
        /// <param name="player">The logical/internal index of the player to map.</param>
        /// <param name="controller">The index of the controller itself.</param>
        public void SetPlayerMapping(LogicalPlayerIndex player, PlayerIndex? controller)
        {
            logicalPlayerMapping[(int) player] = controller;
        }

        public void SetGamePadHandled(LogicalPlayerIndex player, bool value)
        {
            if (player == LogicalPlayerIndex.All)
            {
                foreach (PlayerIndex? controller in logicalPlayerMapping)
                {
                    if (controller.HasValue)
                        controllerHandled[(int)controller.Value] = value;
                }
            }
            else
            {
                var controller = GetPlayerMapping(player);
                if (controller.HasValue)
                    controllerHandled[(int)controller.Value] = value;
            }
        }

        public bool GamepadInputConsumed(LogicalPlayerIndex player)
        {
            if (player == LogicalPlayerIndex.All)
            {
                foreach (PlayerIndex? controller in logicalPlayerMapping)
                {
                    if (controller.HasValue && controllerHandled[(int)controller.Value])
                        return true;
                }
            }
            else
            {
                PlayerIndex? controller = GetPlayerMapping(player);

                if (controller.HasValue)
                    return controllerHandled[(int)controller.Value];
            }

            return false;
        }

        private void UpdateGamePads(float deltaTime)
        {
            for (int player = 0; player < MaxPlayers; player++)
            {
                PlayerIndex playerIndex = (PlayerIndex)player;

                //Update the current and previous states of the controllers...
                prevGamePadStates[player] = currGamePadStates[player];
                currGamePadStates[player] = GamePad.GetState(playerIndex, inputConfiguration.AnalogDeadZone);

                GamePadState newGamePadState = currGamePadStates[player];
                GamePadState previousGamePadState = prevGamePadStates[player];
                LastButtonInfo<Buttons> lastGamePadButton = lastButtonPressed[player];

                if (!newGamePadState.IsConnected)
                    lastGamePadButton.TimePressed = 0;
                else
                {
                    //Record a button if it was pressed.
                    Buttons? pressedButton = null;

                    foreach (Buttons button in buttons)
                    {
                        if (IsDown(ref newGamePadState, button) && !IsDown(ref previousGamePadState, button))
                        {
                            //Found a new button press!
                            pressedButton = button;
                            break;
                        }
                    }

                    lastGamePadButton.IsRepetitionPress = false;

                    //If we didn't find a new button press, then figure out if we need to add a button
                    //repetition if a button has been held...
                    if (!pressedButton.HasValue)
                    {
                        //If the last button pressed is still pressed then add to the timers and consider whether or
                        //not we have to generate a repetition press.
                        if (IsDown(lastGamePadButton.Button, playerIndex))
                        {
                            lastGamePadButton.TimePressed += deltaTime;

                            //Generate a repetition press if the button has been pressed for long enough.
                            if (lastGamePadButton.TimePressed >= inputConfiguration.RepetitionTimeDelay)
                            {
                                lastGamePadButton.IsRepetitionPress = true;
                                lastGamePadButton.TimePressed -= inputConfiguration.RepetitionTimeInterval;
                            }
                        }
                        else //Last pressed button is no longer down, just reset the timer.
                        {
                            lastGamePadButton.TimePressed = 0;
                        }
                    }
                    else
                    {
                        lastGamePadButton.Button = pressedButton.Value;
                        lastGamePadButton.TimePressed = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the current state of the GamePad controlled by the player.
        /// </summary>
        /// <param name="player">The player controlling the GamePad to retrieve the state of.</param>
        /// <returns>The GamePadState of the GamePad controlled by the player.</returns>
        public GamePadState GetGamePadState(LogicalPlayerIndex player)
        {
            PlayerIndex? controller = logicalPlayerMapping[(int) player];

            if (controller.HasValue)
                return currGamePadStates[(int) controller.Value];

            return BlankGamePadState;
        }
        
        public GamePadState GetPreviousGamePadState(LogicalPlayerIndex player)
        {
            PlayerIndex? controller = logicalPlayerMapping[(int) player];

            if (controller.HasValue)
                return prevGamePadStates[(int) controller.Value];

            return BlankGamePadState;
        }

        private bool IsDown(ref GamePadState state, Buttons button)
        {
            // Important: button can contain several button flags. 
            // All buttons must be pressed to return true.

            if ((button & AnalogButtons) != 0)
            {
                // For triggers and thumbsticks we apply our custom thresholds!
                if ((button & ThumbstickButtons) != 0)
                {
                    float thumbstickThreshold = inputConfiguration.AnalogCardinalDeadZone;

                    if ((button & LeftThumbstickButtons) != 0)
                    {
                        if ((button & Buttons.LeftThumbstickLeft) != 0)
                        {
                            // Abort if this button is not pressed (below threshold).
                            if (state.ThumbSticks.Left.X > -thumbstickThreshold)
                                return false;

                            // button might contain more flags. Remove the flag that was already handled.
                            button = button & ~Buttons.LeftThumbstickLeft;
                        }

                        if ((button & Buttons.LeftThumbstickRight) != 0)
                        {
                            if (state.ThumbSticks.Left.X < thumbstickThreshold)
                                return false;

                            button = button & ~Buttons.LeftThumbstickRight;
                        }

                        if ((button & Buttons.LeftThumbstickUp) != 0)
                        {
                            if (state.ThumbSticks.Left.Y < thumbstickThreshold)
                                return false;

                            button = button & ~Buttons.LeftThumbstickUp;
                        }

                        if ((button & Buttons.LeftThumbstickDown) != 0)
                        {
                            if (state.ThumbSticks.Left.Y > -thumbstickThreshold)
                                return false;

                            button = button & ~Buttons.LeftThumbstickDown;
                        }
                    }

                    if ((button & RightThumbstickButtons) != 0)
                    {
                        if ((button & Buttons.RightThumbstickLeft) != 0)
                        {
                            if (state.ThumbSticks.Right.X > -thumbstickThreshold)
                                return false;

                            button = button & ~Buttons.RightThumbstickLeft;
                        }

                        if ((button & Buttons.RightThumbstickRight) != 0)
                        {
                            if (state.ThumbSticks.Right.X < thumbstickThreshold)
                                return false;

                            button = button & ~Buttons.RightThumbstickRight;
                        }

                        if ((button & Buttons.RightThumbstickUp) != 0)
                        {
                            if (state.ThumbSticks.Right.Y < thumbstickThreshold)
                                return false;

                            button = button & ~Buttons.RightThumbstickUp;
                        }

                        if ((button & Buttons.RightThumbstickDown) != 0)
                        {
                            if (state.ThumbSticks.Right.Y > -thumbstickThreshold)
                                return false;

                            button = button & ~Buttons.RightThumbstickDown;
                        }
                    }
                }

                if ((button & Triggers) != 0)
                {
                    var triggerThreshold = inputConfiguration.TriggerThreshold;
                    if ((button & Buttons.LeftTrigger) != 0)
                    {
                        if (state.Triggers.Left < triggerThreshold)
                            return false;

                        button = button & ~Buttons.LeftTrigger;
                    }

                    if ((button & Buttons.RightTrigger) != 0)
                    {
                        if (state.Triggers.Right < triggerThreshold)
                            return false;

                        button = button & ~Buttons.RightTrigger;
                    }
                }
            }

            if ((int)button == 0)
            {
                // Buttons were handled in the check above.
                // All required buttons are down.
                return true;
            }

            return state.IsButtonDown(button);
        }


        public bool IsDown(Buttons button, LogicalPlayerIndex player)
        {
            if (player == LogicalPlayerIndex.All)
            {
                // Check game controllers of all players.
                foreach (PlayerIndex? controller in logicalPlayerMapping)
                    if (controller.HasValue && IsDown(ref currGamePadStates[(int)controller.Value], button))
                        return true;

                return false;
            }
            else
            {
                PlayerIndex? controller = logicalPlayerMapping[(int)player];
                return controller.HasValue && IsDown(ref currGamePadStates[(int)controller.Value], button);
            }
        }

        public bool IsDown(Buttons button, PlayerIndex controller)
        {
            int index = (int)controller;
            return IsDown(ref currGamePadStates[index], button);
        }


        public bool IsUp(Buttons button, LogicalPlayerIndex player)
        {
            if (player == LogicalPlayerIndex.All)
            {
                // Check game controllers of all players.
                bool isUp = true;
                foreach (PlayerIndex? controller in logicalPlayerMapping)
                {
                    if (controller.HasValue)
                    {
                        if (!IsDown(ref currGamePadStates[(int)controller.Value], button))
                            return true;

                        isUp = false;
                    }
                }

                return isUp;
            }
            else
            {
                PlayerIndex? controller = logicalPlayerMapping[(int)player];
                if (controller.HasValue)
                    return !IsDown(ref currGamePadStates[(int)controller.Value], button);

                return true;
            }
        }

        public bool IsUp(Buttons button, PlayerIndex controller)
        {
            int index = (int)controller;
            return !IsDown(ref currGamePadStates[index], button);
        }

        public bool IsNewlyPressed(Buttons button, bool useButtonRepetition, LogicalPlayerIndex player)
        {
            if (player == LogicalPlayerIndex.All)
            {
                // Check game controllers of all players.
                foreach (PlayerIndex? controller in logicalPlayerMapping)
                {
                    if (controller.HasValue && IsNewlyPressed(button, useButtonRepetition, controller.Value))
                        return true;
                }

                return false;
            }
            else
            {
                PlayerIndex? controller = logicalPlayerMapping[(int)player];
                return controller.HasValue && IsNewlyPressed(button, useButtonRepetition, controller.Value);
            }
        }

        private bool IsNewlyPressed(Buttons button, bool useButtonRepetition, PlayerIndex controller)
        {
            int index = (int) controller;

            if (useButtonRepetition)
            {
                if (lastButtonPressed[index].Button == button && lastButtonPressed[index].IsRepetitionPress)
                {
                    return true;
                }
            }

            return IsDown(ref currGamePadStates[index], button) && !IsDown(ref prevGamePadStates[index], button);
        }


        
        public bool IsReleased(Buttons button, LogicalPlayerIndex player)
        {
            if (player == LogicalPlayerIndex.All)
            {
                //Check if any player has released the provided button...
                foreach (PlayerIndex? controller in logicalPlayerMapping)
                {
                    if (controller.HasValue && IsReleased(button, controller.Value))
                        return true;
                }

                return false;
            }
            else
            {
                //Check if the given player has released the provided button...
                PlayerIndex? controller = logicalPlayerMapping[(int) player];
                return controller.HasValue && IsReleased(button, controller.Value);
            }
        }

        public bool IsReleased(Buttons button, PlayerIndex controller)
        {
            int index = (int)controller;
            return !IsDown(ref currGamePadStates[index], button) && IsDown(ref prevGamePadStates[index], button);
        }


        #endregion


        #region Update

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float) gameTime.ElapsedGameTime.TotalSeconds;

            //If the guide is currently being shown then the keyboard and controllers 
            //are currently being handled, so we don't want the input getting to the game itself!
            bool isHandled = (gamerServicesEnabled && Guide.IsVisible); 

            KeyboardInputConsumed = isHandled;

            for (int i = 0; i < controllerHandled.Length; i++)
                controllerHandled[i] = isHandled;

            // Update input devices.
            UpdateKeyboard(deltaTime);
            UpdateGamePads(deltaTime);

            // Update commands.
           // foreach (var command in Commands)
            //    command.Detect();
        }

        #endregion

    }
}
