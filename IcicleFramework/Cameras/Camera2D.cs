using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IcicleFramework.Cameras
{
    public class Camera2D
    {

        #region Member Variables

        /// <summary>
        /// The current position of the Camera2D relative to world coordinates.
        /// </summary>
        protected Vector2 position = Vector2.Zero;

        /// <summary>
        /// Offset from the position that the Camera2D is focused on.
        /// </summary>
        protected Vector2 offset = Vector2.Zero;

        /// <summary>
        /// The area of the screen that is visible from this Camera2D.
        /// </summary>
        protected Rectangle visibleArea;

        /// <summary>
        /// The current rotation of the Camera2D.
        /// </summary>
        protected float rotation = 0.0f;

        /// <summary>
        /// The current zoom aspect of the Camera2D.
        /// </summary>
        protected Vector2 zoom = Vector2.One;

        /// <summary>
        /// The current GraphicsDevice being used by this Camera2D.
        /// </summary>
        private GraphicsDevice graphicsDevice;

        #endregion


        #region Properties

        /// <summary>
        /// Gets or sets the current position of the Camera2D relative to world coordinates.
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        /// <summary>
        /// Gets the area that is visible to the Camera2D.
        /// </summary>
        public Rectangle VisibleArea
        {
            get { return visibleArea; }
        }

        /// <summary>
        /// Gets or sets the width of the area visible to the Camera2D.
        /// </summary>
        public int ViewingWidth
        {
            get { return visibleArea.Width; }
            set { visibleArea.Width = value; }
        }

        /// <summary>
        /// Gets or sets the height of the area visible to the Camera2D.
        /// </summary>
        public int ViewingHeight
        {
            get { return visibleArea.Height; }
            set { visibleArea.Height = value; }
        }

        /// <summary>
        /// Gets or sets the current rotation of the Camera2D.
        /// </summary>
        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        /// <summary>
        /// Gets or sets the current amount of zoom for the Camera2D.
        /// </summary>
        public Vector2 Zoom
        {
            get { return zoom; }
            set
            {
                zoom = value;

                if (value.X < 0)
                    zoom.X = 0;

                if (value.Y < 0)
                    zoom.Y = 0;
            }
        }

        /// <summary>
        /// Gets the current screen position of the Camera2D.
        /// </summary>
        public Vector2 ScreenPosition
        {
            get { return new Vector2(graphicsDevice.Viewport.Width / 2, graphicsDevice.Viewport.Height / 2); }
        }

        #endregion


        #region Constructors

        /// <summary>
        /// Creates a new Camera2D at position (0, 0).
        /// </summary>
        /// <param name="game">The Game object that uses this Camera2D/</param>
        public Camera2D(Game game)
        {
            graphicsDevice = game.GraphicsDevice;
            visibleArea = new Rectangle(0, 0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height);
            position = ScreenPosition;
        }

        /// <summary>
        /// Creates a new Camera2D that looks at the given world position.
        /// </summary>
        /// <param name="game">The Game object that uses this Camera2D/</param>
        /// <param name="position">The world coordinates to place the Camera2D at.</param>
        public Camera2D(Game game, Vector2 position)
        {
            graphicsDevice = game.GraphicsDevice;
            this.position = position;
            visibleArea = new Rectangle(0, 0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height);
        }

        /// <summary>
        /// Creates a new Camera2D that looks at the given world position and has a viewable area of the given width/height.
        /// </summary>
        /// <param name="game">The Game object that uses this Camera2D/</param>
        /// <param name="position">The world coordinates to place the Camera2D at.</param>
        /// <param name="width">The width of the viewable area.</param>
        /// <param name="height">The height of the viewable area.</param>
        public Camera2D(Game game, Vector2 position, int width, int height)
        {
            graphicsDevice = game.GraphicsDevice;
            this.position = position;
            visibleArea = new Rectangle((int)(position.X - (width / 2)), (int)(position.Y - (height / 2)), width, height);
        }

        #endregion


        /// <summary>
        /// Returns a transformation matrix based on the camera’s position, rotation, and zoom.
        /// </summary>
        /// <returns>A transformation matrix based on the current attributes of the Camera2D.</returns>
        public virtual Matrix ViewTransformationMatrix()
        {
            Vector3 matrixRotOrigin = new Vector3(Position + offset, 0);
            Vector3 matrixScreenPos = new Vector3(ScreenPosition, 0.0f);
            
            //Translate back to the origin based on the camera's offset position (as we are rotating around the camera's "real" position).
            //From there, we scale and rotate around the origin point and then translate to screen coordinates (from world coordinates).
            return Matrix.CreateTranslation(-matrixRotOrigin) *
                Matrix.CreateScale(zoom.X, zoom.Y, 1.0f) *
                Matrix.CreateRotationZ(rotation) *
                Matrix.CreateTranslation(matrixScreenPos);
        }


    }
}
