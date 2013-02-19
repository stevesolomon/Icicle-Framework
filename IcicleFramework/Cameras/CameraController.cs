using Microsoft.Xna.Framework;

namespace IcicleFramework.Cameras
{
    /// <summary>
    /// CameraController implements a class that controls the Camera2D and access to it.
    /// </summary>
    public static class CameraController
    {
        /// <summary>
        /// The Camera2D that this CameraController controls.
        /// </summary>
        private static Camera2DFS camera;

        /// <summary>
        /// True if the camera has been initialized, false if otherwise.
        /// </summary>
        private static bool initialized;

        /// <summary>
        /// Initializes the CameraController.
        /// </summary>
        /// <param name="game">The Game object that this CameraController works under.</param>
        /// <param name="position">The initial starting position in world coordinates for the Camera controlled by the CameraController.</param>
        public static void Initialize(Game game)
        {
            camera = new Camera2DFS(game.GraphicsDevice);
            initialized = true;
        }

        public static Vector3 CameraPosition
        {
            get { return new Vector3(camera.Position, 0); }
        }

        /// <summary>
        /// Gets the transformation matrix for the Camera2D to transform from world coordinates to relative Camera coordinates.
        /// </summary>
        /// <returns>The transformation matrix.</returns>
        public static Matrix GetView()
        {
            if (initialized)
                return camera.SimView;
            else
                return Matrix.Identity;
        }

        public static Matrix GetProjection()
        {
            if (initialized)
                return camera.SimProjection;
            else
                return Matrix.Identity;
        }

        public static bool IsPointVisisble(Vector2 point)
        {
            if (initialized)
            {
                var screenPoint = camera.ConvertWorldToScreen(point);


            }

            return true;
        }

        /// <summary>
        /// Rotates the camera a certain number of radians.
        /// </summary>
        /// <param name="rotation">The amount of rotation, in radians, to rotate the camera.</param>
        public static void RotateCamera(float rotation)
        {
            if (initialized)
                camera.Rotation = rotation;
        }

        /// <summary>
        /// Zooms the camera along the X and Y axes.
        /// </summary>
        /// <param name="zoom">A Vector 2 describing the amount of zoom present along the X and Y axes.</param>
        public static void ZoomCamera(float zoom)
        {
            if (initialized)
                camera.Zoom = zoom;
        }

        public static void MoveCamera(Vector2 offset)
        {
            if (initialized)
                camera.MoveCamera(ConvertUnits.ToSimUnits(offset));
        }

        public static void Update(GameTime gameTime)
        {
            camera.Update(gameTime);
        }

    }
}
