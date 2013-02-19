using System;
using System.Globalization;
using System.Xml.Linq;
using Microsoft.Xna.Framework;

namespace IcicleFramework.Collision
{
    public class BoundingBox2D : ICollision
    {
        private RectangleF boundingBox;

        private Vector2 offset;

        public Vector2 Offset
        {
            get { return offset; }
            set
            {
                offset = value;
                Position = Position;
                X = X;
                Y = Y;
            }
        }

        public Vector2 Position
        {
            get { return boundingBox.Position; }
            set 
            { 
                boundingBox.X = value.X + Offset.X;
                boundingBox.Y = value.Y + Offset.Y;
            }
        }

        public String Name { get; set; }

        public bool Active { get; set; }

        public float X
        {
            get { return boundingBox.X; }
            set { boundingBox.X = value + Offset.X; }
        }

        public float Y
        {
            get { return boundingBox.Y; }
            set { boundingBox.Y = value + Offset.Y; }
        }

        public float Width
        {
            get { return boundingBox.Width; }
            set { boundingBox.Width = value >= 0.0f ? value : 0.0f; }
        }

        public float Height
        {
            get { return boundingBox.Height; }
            set { boundingBox.Height = value >= 0.0f ? value : 0.0f; }
        }

        public float Right
        {
            get { return boundingBox.Right; }
        }

        public float Left
        {
            get { return boundingBox.Left; }
        }

        public float Top
        {
            get { return boundingBox.Top; }
        }

        public float Bottom
        {
            get { return boundingBox.Bottom; }
        }

        public static readonly BoundingBox2D Empty = new BoundingBox2D(RectangleF.Empty);

        public RectangleF Box
        {
            get { return boundingBox; }
        }

        public BoundingBox2D()
        {
            this.boundingBox = new RectangleF();
        }

        public BoundingBox2D(RectangleF boundingBox)
        {
            this.boundingBox = boundingBox;
        }

        public BoundingBox2D(float x, float y, float width, float height)
        {
            this.boundingBox = new RectangleF(x, y, width, height);
        }

        protected BoundingBox2D(BoundingBox2D old)
        {
            this.boundingBox = new RectangleF(old.boundingBox.X, old.boundingBox.Y, old.boundingBox.Width, old.boundingBox.Height);
            this.Offset = old.Offset;
            this.Name = old.Name;
            this.Active = old.Active;
            this.Position = new Vector2(old.Position.X, old.Position.Y);
        }

        public bool Intersects(ICollision collisionObject)
        {
            BoundingBox2D otherObject = collisionObject as BoundingBox2D;

            return otherObject != null && this.boundingBox.Intersects(otherObject.Box);
        }

        public bool Contains(ICollision collisionObject)
        {
            BoundingBox2D otherObject = collisionObject as BoundingBox2D;

            return otherObject != null && this.boundingBox.Contains(otherObject.Box);
        }

        public ICollision DeepCopy()
        {
            return new BoundingBox2D(this);
        }

        #region IXmlBuildable

        public void Deserialize(XElement element)
        {
            //Make sure we have the right element here!
            if (element.Name == "boundingBox")
            {
                int width = 0, height = 0, offsetX = 0, offsetY = 0;

                if (element.Element("width") != null)
                    width = int.Parse(element.Element("width").Value, NumberStyles.Integer, CultureInfo.InvariantCulture);

                if (element.Element("height") != null)
                    height = int.Parse(element.Element("height").Value, NumberStyles.Integer, CultureInfo.InvariantCulture);

                if (element.Element("offset") != null)
                {
                    XElement offsetElem = element.Element("offset");

                    if (offsetElem.Attribute("x") != null)
                        offsetX = int.Parse(offsetElem.Attribute("x").Value, NumberStyles.Integer,
                                            CultureInfo.InvariantCulture);

                    if (offsetElem.Attribute("y") != null)
                        offsetY = int.Parse(offsetElem.Attribute("y").Value, NumberStyles.Integer,
                                            CultureInfo.InvariantCulture);
                }

                this.boundingBox = new RectangleF(0f, 0f, width, height);
                this.Offset = new Vector2(offsetX, offsetY);
            }
        }

        #endregion

    }
}
