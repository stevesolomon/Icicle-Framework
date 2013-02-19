using System.Collections.Generic;
using IcicleFramework.Inputs;
using Microsoft.Xna.Framework;

namespace IcicleFramework.GameServices
{
    /// <summary>
    /// PlayerManager controls access to individual Players.
    /// This class relies on outside functionality to tie Players to controller (indices).
    /// </summary>
    public class PlayerManager : GameService
    {

        #region Internal Variables
        
        /// <summary>
        /// A Dictionary of Players currently in the game, keyed by the in-game (logical) player index.
        /// </summary>
        private Dictionary<LogicalPlayerIndex, Player> players;
        
        #endregion
        

        #region Properties

        /// <summary>
        /// Gets the number of Players currently registered as playing.
        /// </summary>
        public int NumPlayers
        {
            get { return players.Count; }
        }

        public IInputHandler InputHandler { get; set; }
        
        #endregion


        public PlayerManager()
        {
            this.players = new Dictionary<LogicalPlayerIndex, Player>();
        }
        

        public override void Initialize()
        {
            this.InputHandler = (IInputHandler) GameServiceManager.GetService(typeof (IInputHandler));
        }

        public Player GetPlayer(LogicalPlayerIndex index)
        {
            if (players.ContainsKey(index))
                return players[index];
            else
                return null;
        }

        public void SetPlayer(LogicalPlayerIndex index, PlayerIndex controller)
        {
            if (!players.ContainsKey(index))
            {
                Player player = new Player(index, controller);
                player.InputHandler = InputHandler;

                if (InputHandler != null)
                    InputHandler.SetPlayerMapping(index, controller);

                players.Add(index, player);
            }
        }
    }
}
