using IcicleFramework.GameServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace IcicleFramework.Inputs
{
    public interface IInputHandler : IGameService, IUpdateable
    {
        #region Properties

        InputConfiguration Configuration { get; set; }

        /// <summary>
        /// Gets or sets whether or not input has been wholly handled for this frame and should no longer
        /// be considered until the next frame's update.
        /// </summary>
        bool KeyboardInputConsumed { get; set; }

        /// <summary>
        /// Gets the current state of the keyboard for this frame.
        /// </summary>
        KeyboardState KeyboardState { get; }

        /// <summary>
        /// Gets the state of the keyboard from the previous frame.
        /// </summary>
        KeyboardState PreviousKeyboardState { get; }

        /// <summary>
        /// Gets the collection of keys that have been freshly pressed this frame.
        /// </summary>
        //ReadOnlyCollection<Keys> KeysPressed { get; }

        #endregion


        /// <summary>
        /// Gets the actual PlayerIndex that maps to the provided LogicalPlayerIndex.
        /// </summary>
        /// <param name="playerIndex">The logical player index that identifies the internal player number.</param>
        /// <returns>The actual PlayerIndex corresponding to the provided LogicalPlayerIndex, or null if no mapping was found.</returns>
        PlayerIndex? GetPlayerMapping(LogicalPlayerIndex playerIndex);
        
        /// <summary>
        /// Maps a Logical Player to a physical controller.
        /// </summary>
        void SetPlayerMapping(LogicalPlayerIndex player, PlayerIndex? controller);

        /// <summary>
        /// Determines whether or not the input for a given logical player has already been handled for this frame and should not be considered.
        /// </summary>
        /// <param name="player">The logical player index that identified the internal player number</param>
        /// <returns>True if the input has already been handled for this player, false if otherwise.</returns>
        bool GamepadInputConsumed(LogicalPlayerIndex player);
        
        /// <summary>
        /// Gets the state of the gamepad for this frame of the controller used by the player provided.
        /// </summary>
        /// <param name="player">The logical player index that identified the internal player number</param>
        /// <returns>The resulting GamePadState corresponding to the correct controller.</returns>
        GamePadState GetGamePadState(LogicalPlayerIndex player);

        /// <summary>
        /// Gets the state of the gamepad from the previous frame of the controller used by the player provided.
        /// </summary>
        /// /// <param name="player">The logical player index that identified the internal player number.</param>
        /// <returns>The resulting GamePadState corresponding to the correct controller.</returns>
        GamePadState GetPreviousGamePadState(LogicalPlayerIndex player);
        
        /// <summary>
        /// Checks if a given key is currently being pressed down.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns> True if the specified key is down, false if otherwise.</returns>
        bool IsDown(Keys key);
        
        /// <summary>
        /// Checks if a given key is currently not being pressed.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns> True if the specified key is not being pressed, false if otherwise.</returns>
        bool IsUp(Keys key);
        
        /// <summary>
        /// Checks if a given key was previously not pressed down, but is now being pressed.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <param name="considerRepetition">Whether or not to consider a key 'press' via repetition.</param>
        /// <returns> True if the specified key was previously not pressed down, but currently is pressed down, false if otherwise.</returns>
        bool IsNewlyPressed(Keys key, bool considerRepetition);
        
        /// <summary>
        /// Checks if a given key was previously pressed down, but is now not pressed down.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns> True if the specified key was previously pressed down, but currently is not pressed down, false if otherwise.</returns>
        bool IsReleased(Keys key);
        
        /// <summary>
        /// Checks if a given button is currently being pressed down by the player.
        /// </summary>
        /// <param name="button">The button to check.</param>
        /// <param name="player">The logical player index that identified the internal player number.</param>
        /// <returns> True if the specified button is down, false if otherwise.</returns>
        bool IsDown(Buttons button, LogicalPlayerIndex player);

        bool IsDown(Buttons button, PlayerIndex controller);
        
        /// <summary>
        /// Checks if a given button is not currently being pressed down by the player.
        /// </summary>
        /// <param name="button">The button to check.</param>
        /// <param name="player">The logical player index that identified the internal player number.</param>
        /// <returns> True if the specified button is not being pressed, false if otherwise.</returns>
        bool IsUp(Buttons button, LogicalPlayerIndex player);

        /// <summary>
        /// Checks if a given button was previously not pressed, but is now being pressed.
        /// </summary>
        /// <param name="button">The button to check.</param>
        /// <param name="considerRepetition">Whether or not to consider a button 'press' via repetition.</param>
        /// <param name="player">The logical player index that identified the internal player number.</param>
        /// <returns> True if the specified button was previously not pressed but is now pressed, false if otherwise.</returns>
        bool IsNewlyPressed(Buttons button, bool considerRepetition, LogicalPlayerIndex player);
        
        /// <summary>
        /// Checks if a given button was previous pressed, and is not not pressed by the player.
        /// </summary>
        /// <param name="button">The button to check.</param>
        /// <param name="player">The logical player index that identified the internal player number.</param>
        /// <returns> True if the specified button was previously pressed but is now not pressed, false if otherwise.</returns>
        bool IsReleased(Buttons button, LogicalPlayerIndex player);
    }
}
