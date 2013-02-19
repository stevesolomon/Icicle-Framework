using IcicleFramework.Cameras;
using Microsoft.Xna.Framework;

namespace IcicleFramework.GameServices.CameraServices
{
    public interface ICameraService : IGameService
    {
        /// <summary>
        /// Gets the Main Camera for this ICameraService. 
        /// </summary>
        ICamera MainCamera { get; }

        /// <summary>
        /// Adds a new ICamera to be managed by this ICameraService.
        /// </summary>
        /// <param name="camera">The new camera to be added to this ICameraService.</param>
        /// <param name="main">Whether or not this added ICamera should be registered as the main camera for this ICameraService.</param>
        void AddCamera(ICamera camera, bool main = false);

        /// <summary>
        /// Removes the named ICamera from this ICameraService.
        /// </summary>
        /// <param name="name">The name of the ICamera to remove.</param>
        void RemoveCamera(string name);

        /// <summary>
        /// Gets the ICamera managed by this ICameraService under the given name.
        /// </summary>
        /// <param name="name">The name of the ICamera to retrieve.</param>
        /// <returns>The ICamera under the given name, or null if no Camera was found</returns>
        ICamera GetCamera(string name);

        Vector2 TranslateWorldPointToScreen(Vector2 worldPoint, string cameraName = "");
    }
}
