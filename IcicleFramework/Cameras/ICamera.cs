using IcicleFramework.Entities;
using Microsoft.Xna.Framework;

namespace IcicleFramework.Cameras
{
    public interface ICamera : IUpdateable
    {
        /// <summary>
        /// Gets the name of the Camera set during instantiation.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the View Matrix.
        /// </summary>
        Matrix View { get; }

        /// <summary>
        /// Gets the Projection Matrix.
        /// </summary>
        Matrix Projection { get; }

        /// <summary>
        /// Gets the current Position of this ICamera.
        /// </summary>
        Vector2 Position { get; }

        /// <summary>
        /// Gets the current amount of Rotation for this ICamera.
        /// </summary>
        float Rotation { get; }

        /// <summary>
        /// Gets the current amount of Zoom for this ICamera.
        /// </summary>
        float Zoom { get; }

        /// <summary>
        /// Moves this ICamera to the new position.
        /// </summary>
        /// <param name="amount">The new position to move this ICamera to.</param>
        /// <param name="seconds">The amount of time, in seconds, the ICamera will take to move from its current position to the new position.</param>
        void MoveCamera(Vector2 position);

        /// <summary>
        /// Rotates this ICamera by the provided amount in radians.
        /// </summary>
        /// <param name="amount">The amount in radians to rotate this ICamera.</param>
        void RotateCamera(float amount);

        /// <summary>
        /// Zooms this ICamera to the provided zoom factor.
        /// </summary>
        /// <param name="amount">The zoom factor for this ICamera.</param>
        void ZoomCamera(float amount);

        /// <summary>
        /// Resets all of this ICamera's settings to their default values.
        /// </summary>
        void Reset();

        /// <summary>
        /// Converts a camera-relative point to world-relative coordinates.
        /// </summary>
        /// <param name="screenPoint">The point relative to the camera.</param>
        /// <returns>A <see cref="Vector2"/> containing the translated point in world space.</returns>
        Vector2 ConvertCameraPointToWorld(Vector2 screenPoint);

        /// <summary>
        /// Converts a world-relative point to camera-relative coordinates.
        /// </summary>
        /// <param name="worldPoint">The point relative to the world.</param>
        /// <returns>A <see cref="Vector2"/> containing the translated point in camera space.</returns>
        Vector2 ConvertWorldPointToCamera(Vector2 worldPoint);
    }
}
