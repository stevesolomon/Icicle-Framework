using System;
using System.Diagnostics;
using System.Globalization;
using System.Xml.Linq;
using IcicleFramework.Collision;
using IcicleFramework.Entities;
using IcicleFramework.GameServices;
using Microsoft.Xna.Framework;

namespace IcicleFramework.Components.Collision
{
    public class CollisionComponent : BaseComponent, ICollisionComponent
    {
        public event OnCollisionHandler OnCollision;

        public event OnCollisionStoppedHandler OnCollisionStopped;

        public int CollisionPriority { get; set; }

        protected BoundingBox2D boundingBox2D;

        public BoundingBox2D BoundingBox2D
        {
            get { return boundingBox2D; }
            set
            {
                boundingBox2D = value;
                
                if (Parent != null && BoundingBox2D != null)
                {
                    boundingBox2D.Position = Parent.Position;
                }
            }
        }

        public bool Solid { get; set; }

        protected ICollisionManager collisionManager;

        public override IGameObject Parent
        {
            get
            {
                return base.Parent;
            }
            set
            {
                base.Parent = value;

                if (Parent != null && BoundingBox2D != null)
                {
                    boundingBox2D.Position = Parent.Position;
                }
            }
        }

        public IGameObject Source { get { return Parent; } }

        public CollisionComponent()
        {
            CollisionPriority = int.MaxValue;
            Solid = true;
            BoundingBox2D = new BoundingBox2D();
        }

        public CollisionComponent(BoundingBox2D boundingBox)
            : this()
        {
            BoundingBox2D = boundingBox;
        }

        public override void Initialize()
        {
            BoundingBox2D.Position = Parent.Position;
            collisionManager = GameServiceManager.GetService<ICollisionManager>();
            base.Initialize();
        }

        public override void PostInitialize()
        {
            collisionManager.SubscribeCollisionEvent(Parent.GUID, OnCollisionEvent);
            collisionManager.SubscribeCollisionStoppedEvent(Parent.GUID, OnCollisionStoppedEvent);
            base.PostInitialize();
        }

        public override void Update(GameTime gameTime)
        {
            BoundingBox2D.Position = Parent.Position;
        }

        protected virtual void OnCollisionEvent(ICollisionComponent source, ICollisionComponent collided)
        {
            if (OnCollision != null)
                OnCollision(source, collided);
        }

        private void OnCollisionStoppedEvent(ICollisionComponent source, ICollisionComponent previousCollided)
        {
            if (OnCollisionStopped != null)
                OnCollisionStopped(source, previousCollided);
        }

        public CorrectionVector2 GetCorrectionVector(ICollisionComponent B)
        {
            CorrectionVector2 vector = new CorrectionVector2();

            float x1 = Math.Abs(BoundingBox2D.Right - B.BoundingBox2D.Left);
            float x2 = Math.Abs(BoundingBox2D.Left - B.BoundingBox2D.Right);
            float y1 = Math.Abs(BoundingBox2D.Bottom - B.BoundingBox2D.Top);
            float y2 = Math.Abs(BoundingBox2D.Top - B.BoundingBox2D.Bottom);

            /*
            IMovementComponent movement = this.Parent.GetComponent<IMovementComponent>();

            //If both objects have the same priority, then the correction vector only rolls back along the velocity
            if (this.CollisionPriority == B.CollisionPriority)
            {
                if (movement.Velocity.X != 0.0f)
                {
                    //Then roll back the velocity...
                    if (x1 < x2)
                    {
                        vector.X = x1;
                        vector.DirectionX = DirectionX.Left;
                    }
                    else if (x1 > x2)
                    {
                        vector.X = x2;
                        vector.DirectionX = DirectionX.Right;
                    }
                }
                else
                {
                    vector.X = 0.0f;
                    vector.DirectionX = DirectionX.Right;
                }

                if (movement.Velocity.Y != 0.0f)
                {
                    //Calculate the displacement along Y-axis
                    if (y1 < y2)
                    {
                        vector.Y = y1;
                        vector.DirectionY = DirectionY.Up;
                    }
                    else if (y1 > y2)
                    {
                        vector.Y = y2;
                        vector.DirectionY = DirectionY.Down;
                    }
                }
                else
                {
                    vector.Y = 0.0f;
                    vector.DirectionY = DirectionY.Down;
                }
            }
            else
            {

                //Calculate the displacement along X-axis
                if (x1 < x2)
                {
                    vector.X = x1;
                    vector.DirectionX = DirectionX.Left;
                }
                else if (x1 > x2)
                {
                    vector.X = x2;
                    vector.DirectionX = DirectionX.Right;
                }

                //Calculate the displacement along Y-axis
                if (y1 < y2)
                {
                    vector.Y = y1;
                    vector.DirectionY = DirectionY.Up;
                }
                else if (y1 > y2)
                {
                    vector.Y = y2;
                    vector.DirectionY = DirectionY.Down;
                }
            } */

            return vector;
        }

        public override void Deserialize(XElement element)
        {
            if (element.Element("boundingBox") != null)
            {
                if (BoundingBox2D == null)
                    BoundingBox2D = new BoundingBox2D();

                BoundingBox2D.Deserialize(element.Element("boundingBox"));
            }

            int collisionPriority = int.MaxValue;
            if (element.Element("priority") != null)
            {
                int.TryParse(element.Element("priority").Value, NumberStyles.Integer,
                                                   CultureInfo.InvariantCulture, out collisionPriority);
            }
            CollisionPriority = collisionPriority;

            bool solid = true;
            if (element.Element("solid") != null)
            {
                bool.TryParse(element.Element("solid").Value, out solid);
            }
            Solid = solid;
        }

        public override void Reallocate()
        {
            OnCollision = null;
            OnCollisionStopped = null;
            Solid = false;
            CollisionPriority = 0;
            BoundingBox2D = null;

            base.Reallocate();
        }
        
        public bool HasMoved
        {
            get { return Parent.HasMoved; }
        }

        public override void CopyInto(IBaseComponent newObject)
        {
            var collisionComp = newObject as CollisionComponent;

            Debug.Assert(collisionComp != null, "collisionComp != null");

            collisionComp.BoundingBox2D = (BoundingBox2D) BoundingBox2D.DeepCopy();
            collisionComp.CollisionPriority = CollisionPriority;

            base.CopyInto(newObject);
        }
    }
}
