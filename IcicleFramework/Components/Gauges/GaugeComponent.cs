using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using IcicleFramework.Components.Renderable;
using IcicleFramework.GameServices;
using IcicleFramework.GameServices.Factories;
using IcicleFramework.Renderables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace IcicleFramework.Components.Gauges
{
    public class GaugeComponent : RenderComponent, IGaugeComponent
    {
        private float filled;

        private Color filledColor;

        private Vector2 offset;

        public ITexturedRenderable Border { get; set; }

        public ITexturedRenderable Bar { get; set; }

        public Vector2 Offset
        { 
            get { return offset; } 
            set
            {
                if (Bar != null)
                    Bar.Offset = value;

                if (Border != null)
                    Border.Offset = value;

                offset = value;
            } 
        }

        public Color FilledColor
        {
            get { return filledColor; }
            set
            {
                filledColor = value;

                if (Bar != null)
                    Bar.ColorChannel = filledColor;
            }
        }

        public Color EmptyColor { get; set; }

        public virtual float Filled
        {
            get { return filled; }
            set
            {
                value = value >= 0.0f ? value : 0.0f;
                filled = value;

                Bar.Source = new Rectangle((int) Parent.Position.X + (int) Offset.X,
                                           (int) Parent.Position.Y + (int) Offset.Y, 
                                           (int) (Bar.Texture.Width * filled), Bar.Texture.Height);
            }
        }

        public override bool Visible
        {
            get { return visible; }
            set
            {
                visible = value;
                Border.Visible = visible;
                Bar.Visible = visible;
            }
        }
        
        public override void Draw(SpriteBatch spriteBatch)
        {
            Border.Draw(spriteBatch);
            Bar.Draw(spriteBatch);
        }

        public override void Load(ContentManager content)
        {
            Border.Load(content);
            Bar.Load(content);
        }

        public override void Initialize()
        {
            Filled = 1f;
        }
        
        public override void Deserialize(XElement element)
        {
            Color color = new Color();

            if (element.Element("filledColor") != null)
                color = color.ConvertColorFromString(element.Element("filledColor").Value);

            Offset = Offset.DeserializeOffset(element.Element("offset"));

            //Retrieve the asset elements
            List<XElement> elements = element.Elements("renderable").ToList();

            RenderableFactory factory = (RenderableFactory)GameServiceManager.GetService(typeof(RenderableFactory));

            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i].Attribute("name") != null)
                {
                    //Loading the bar element...
                    if (elements[i].Attribute("name").Value.Equals("bar", StringComparison.InvariantCultureIgnoreCase))
                    {
                        Bar = (ITexturedRenderable) factory.GenerateRenderable(elements[i], this);
                        Bar.Parent = this;
                        Bar.Offset = Offset;
                    }
                    else if (elements[i].Attribute("name").Value.Equals("border",
                                                                        StringComparison.InvariantCultureIgnoreCase))
                    {
                        Border = (ITexturedRenderable) factory.GenerateRenderable(elements[i], this);
                        Border.Parent = this;
                        Border.Offset = Offset;
                    }
                }
            }

            FilledColor = color;
            this.Visible = true;
        }

        
    }
}
