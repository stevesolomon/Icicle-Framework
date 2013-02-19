using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IcicleFramework.Cameras
{
    public class BasicCamera2D : ICamera
    {
        protected Matrix view;

        protected Matrix projection;

        protected Vector2 position;

        protected GraphicsDevice graphicsDevice;

        protected readonly string name;

        public virtual string Name
        {
            get { return name; }
        }

        public Matrix View
        {
            get { return view; }
        }

        public Matrix Projection
        {
            get { return projection; }
        }

        public virtual Vector2 Position
        {
            get { return position; } 
            protected set
            {
                position = value;
                SetView();
            }
        }

        public virtual float Rotation { get; protected set; }

        public virtual float Zoom { get; protected set; }

        public BasicCamera2D(GraphicsDevice graphics, string name)
        {
            graphicsDevice = graphics;
            this.name = name;
            Zoom = 1f;
            Rotation = 0f;

            projection = Matrix.CreateOrthographicOffCenter(0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height, 0, 0, 1);
            Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);
            projection = halfPixelOffset * projection;

            SetView();
        }

        public virtual void Update(GameTime gameTime)
        {
            //if (Position != targetPosition)
            //{
            //    var dt = (float) gameTime.ElapsedGameTime.TotalSeconds;

            //    //Advance the x and y positions by the movementPerSecond multiplied by the normalized x/y movement units, multiplied by dt
            //    Vector2 newPosition;
            //    newPosition.X = (movementPerSecond*normalizedMovement.X)*dt;
            //    newPosition.Y = (movementPerSecond*normalizedMovement.Y)*dt;
            //}

            SetView();
        }

        public virtual void MoveCamera(Vector2 newPosition)
        {
            Position = newPosition;

            /* if (seconds <= 0f)
            {
                Position = newPosition;
                return;
            }
            
            targetPosition = newPosition;

            //Compute how far we have to move along the x and y axis in total.
            totalMovementRemaining = Position - targetPosition;

            //Next figure out how many units in each direction we have to move to move one unit along the line
            normalizedMovement = totalMovementRemaining;
            normalizedMovement.Normalize();

            //Now, we know we have x seconds to move totalMovementRemaining, and we know the ratios of
            //movement along the x and y axis...so figure let's record the total distance.
            float totalDistance = totalMovementRemaining.Length();

            //We know we need to move this far, and we have x seconds to move so...
            //Let's determine how far we have to move each second.
            //In the Update method we'll use this value, multiplied by the frame time, multiplied by the x or y axis normalized unit movement
            //to figure out how far we have to move.
            movementPerSecond = totalDistance / seconds;
            */
        }

        public virtual void RotateCamera(float amount)
        {
            Rotation = amount % MathHelper.TwoPi;
        }

        public virtual void ZoomCamera(float amount)
        {
            Zoom = amount;
        }

        public virtual void Reset()
        {
            Rotation = 0f;
            Zoom = 1f;
            Position = Vector2.Zero;

            SetView();
        }

        protected void SetView()
        {
            Matrix rotationMatrix = Matrix.CreateRotationZ(Rotation);
            Matrix zoomMatrix = Matrix.CreateScale(Zoom);
            Vector3 translateBody = new Vector3(-Position, 0f);

            view = Matrix.CreateTranslation(translateBody) * rotationMatrix * zoomMatrix; 
        }

        public Vector2 ConvertCameraPointToWorld(Vector2 screenPoint)
        {
            Vector3 t = new Vector3(screenPoint, 0);

            t = graphicsDevice.Viewport.Unproject(t, Projection, View, Matrix.Identity);

            return new Vector2(t.X, t.Y);
        }

        public Vector2 ConvertWorldPointToCamera(Vector2 worldPoint)
        {
            Vector3 t = new Vector3(worldPoint, 0);

            t = graphicsDevice.Viewport.Project(t, Projection, View, Matrix.Identity);

            return new Vector2(t.X, t.Y);
        }
    }
}
