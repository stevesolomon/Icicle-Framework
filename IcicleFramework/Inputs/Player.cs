using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace IcicleFramework.Inputs
{
    /// <summary>
    /// The Player class contains some information about each player in the game.
    /// </summary>
    public class Player
    {
        #region Member Variables
        
        /// <summary>
        /// The in-game logical player index for this player.
        /// </summary>
        /// <remarks>This is not the actual index of a controller for this player.</remarks>
        [JsonIgnoreAttribute]
        private LogicalPlayerIndex index;

        #endregion


        #region Properties

        /// <summary>
        /// Gets or sets the current controller used by the Player.
        /// </summary>
        [JsonIgnoreAttribute]
        public PlayerIndex Controller { get; set; }

        /// <summary>
        /// Gets the in-game (logical) index for this Player.
        /// </summary>
        [JsonIgnoreAttribute]
        public LogicalPlayerIndex Index
        {
            get { return index; }
        }

        /// <summary>
        /// Gets or sets the IInputHandler this Player uses for handling input.
        /// </summary>
        [JsonIgnoreAttribute]
        public IInputHandler InputHandler { get; set; }
        
        #endregion


        #region Constructors

        public Player(LogicalPlayerIndex index, PlayerIndex controller)
        {
            this.index = index;
            this.Controller = controller;
        }

        #endregion

    }
}
