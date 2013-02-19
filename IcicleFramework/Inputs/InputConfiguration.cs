using Microsoft.Xna.Framework.Input;

namespace IcicleFramework.Inputs
{
    public class InputConfiguration
    {
        #region Internal Variables

        private float triggerThreshold;

        #endregion


        #region Properties

        /// <summary>
        /// Gets or sets the time delay (in seconds) before repetition of a key/button press begins.
        /// </summary>
        public float RepetitionTimeDelay { get; set; }
        
        /// <summary>
        /// Gets or sets the time delay (in seconds) between 'virtual' repetition of a key/button press.
        /// </summary>
        public float RepetitionTimeInterval { get; set; }

        /// <summary>
        /// Gets or sets the deadzone before trigger presses are recognized.
        /// </summary>
        /// <remarks>The value must be between 0 and 1.</remarks>
        public float TriggerThreshold
        {
            get { return triggerThreshold; }
            set
            {
                if (value < 0.0f) value = 0.0f;
                else if (value > 1.0f) value = 1.0f;

                triggerThreshold = value;
            }
        }

        /// <summary>
        /// Gets or sets the DeadZone settings for the analog sticks on a GamePad.
        /// </summary>
        public GamePadDeadZone AnalogDeadZone { get; set; }

        /// <summary>
        /// Gets or sets the dead zone settings for the analog sticks when we consider them as being 'buttons'
        /// along the four cardinal directions.
        /// </summary>
        public float AnalogCardinalDeadZone { get; set; }

        #endregion


        #region Constructor

        /// <summary>
        /// Creates a new instance of the InputConfiguration class with default settings.
        /// </summary>
        public InputConfiguration()
        {
            RepetitionTimeDelay = 0.5f;
            RepetitionTimeInterval = 0.15f;
            TriggerThreshold = 0.0f;
            AnalogDeadZone = GamePadDeadZone.None;
            AnalogCardinalDeadZone = 0.45f;
        }

        #endregion
    }
}
