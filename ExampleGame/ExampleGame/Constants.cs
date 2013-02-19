using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ExampleGame
{
    public static class Constants
    {
        public const string COMPONENT_NS = "Monochrome.Entities.Components";

        /// <summary>
        /// Structure that stores resolution related information.
        /// </summary>
        public struct Resolution
        {
            /// <summary>
            /// Width, in pixels, of the screen.
            /// </summary>
            public static int width = 1280;

            /// <summary>
            /// Height, in pixels, of the screen.
            /// </summary>
            public static int height = 720;
        }

        /// <summary>
        /// The maximum number of Players in this game.
        /// </summary>
        public const int MAX_PLAYERS = 1;

        /// <summary>
        /// The maximum number of Controllers that can be used in the game.
        /// </summary>
        public const int MAX_CONTROLLERS = 1;

        /// <summary>
        /// True if we are debugging.
        /// </summary>
        public const Boolean DEBUG = true;

        /// <summary>
        /// Access to the game's random number generator.
        /// </summary>
        public static Random rand = new Random();

        public static int ENVIRONMENT_OBJECT_COLLISION_PRIORITY = int.MaxValue;

        public static int ENTITY_OBJECT_COLLISION_PRIORITY = int.MinValue;

        public readonly static Keys[] menuAcceptKeys = new Keys[] { Keys.Enter };
        public readonly static Buttons[] menuAcceptButtons = new Buttons[] { Buttons.A, Buttons.Start };

        public readonly static Keys[] menuCancelKeys = new Keys[] { Keys.Back, Keys.Escape, Keys.B };
        public readonly static Buttons[] menuCancelButtons = new Buttons[] { Buttons.B, Buttons.Back };


        #region DirectionKeysAndButtons

        public readonly static Keys[] moveLeftKeys = new Keys[] { Keys.Left, Keys.A };
        public readonly static Buttons[] moveLeftButtons = new Buttons[] { Buttons.DPadLeft, Buttons.LeftThumbstickLeft };

        public readonly static Keys[] moveRightKeys = new Keys[] { Keys.Right, Keys.D };
        public readonly static Buttons[] moveRightButtons = new Buttons[] { Buttons.DPadRight, Buttons.LeftThumbstickRight };

        public readonly static Keys[] moveUpKeys = new Keys[] { Keys.Up, Keys.W };
        public readonly static Buttons[] moveUpButtons = new Buttons[] { Buttons.DPadUp, Buttons.LeftThumbstickUp };

        public readonly static Keys[] moveDownKeys = new Keys[] { Keys.Down, Keys.S };
        public readonly static Buttons[] moveDownButtons = new Buttons[] { Buttons.DPadDown, Buttons.LeftThumbstickDown };

        #endregion


        #region Helper Functions

        /// <summary>
        /// Generates a random float value between min and max.
        /// </summary>
        /// <param name="min">The minimum value to generate.</param>
        /// <param name="max">The maximum value to generate.</param>
        /// <returns>A random float value between min and max.</returns>
        public static float RandomBetween(float min, float max)
        {
            return min + (float)rand.NextDouble() * (max - min);
        }

        #endregion

    }
}