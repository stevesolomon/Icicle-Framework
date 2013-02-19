using IcicleFramework.Inputs;

namespace IcicleFramework.Components.Input
{
    public interface IPlayerInputComponent : IInputComponent
    {
        /// <summary>
        /// Gets or sets the Player that this <see cref="IPlayerInputComponent"/> is interested in.
        /// </summary>
        Player Player { get; set; }
    }
}
