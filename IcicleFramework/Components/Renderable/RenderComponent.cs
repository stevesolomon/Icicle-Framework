using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using IcicleFramework.GameServices;
using IcicleFramework.GameServices.Factories;
using IcicleFramework.Renderables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace IcicleFramework.Components.Renderable
{
    public class RenderComponent : BaseComponent, IRenderComponent
    {
        protected bool visible;

        protected int width;

        protected int height;

        public override bool Active
        {
            get { return Visible; }
            set { Visible = value; }
        }

        public virtual bool Visible
        {
            get { return visible; }
            set
            {
                if (value != visible)
                {
                    foreach (IRenderable renderable in renderables.Values)
                        renderable.Visible = value;

                    visible = value;
                }
            }
        }

        public virtual bool VisibleOnScreen
        {
            get; 
            protected set;
        }
        
        [JsonProperty]
        protected Dictionary<string, IRenderable> renderables;

        public int NumRenderables
        {
            get { return renderables.Count; }
        }

        public int Width
        {
            get { return width; }
        }

        public int Height
        {
            get { return height;  }
        }

        public IRenderable GetRenderable(string name)
        {
            return renderables.ContainsKey(name) ? renderables[name] : null;
        }

        public IRenderable GetRenderable(int index)
        {
            return renderables.Count > index ? renderables.Values.ToArray()[index] : null;
        }

        public void SetShading(Color shading)
        {
            foreach (IRenderable renderable in renderables.Values)
                renderable.ColorChannel = shading;
        }

        public void SetScale(float scale)
        {
            foreach (IRenderable renderable in renderables.Values)
                renderable.Scale = scale;
        }

        public void SetLayer(float layer)
        {
            foreach (IRenderable renderable in renderables.Values)
                renderable.Layer = layer;
        }
        
        public RenderComponent()
        {
            renderables = new Dictionary<string, IRenderable>();
            visible = true;
        }
        
        public bool AddRenderable(IRenderable renderable, string name)
        {
            if (!renderables.ContainsKey(name))
            {
                renderables.Add(name, renderable);
                renderable.Parent = this;

                UpdateDimensions();

                return true;
            }

            return false;
        }

        public override void Update(GameTime gameTime)
        {
            foreach (IRenderable renderable in renderables.Values)
                renderable.Update(gameTime);
        }

        public override void Initialize()
        {
            foreach (IRenderable renderable in renderables.Values)
                renderable.Initialize();

            base.Initialize();
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            bool visibleOnScreen = true;

            foreach (var renderable in renderables.Values)
            {
                if (Visible)
                {
                    renderable.Draw(spriteBatch);
                }

                visibleOnScreen &= renderable.VisibleOnScreen;
            }

            VisibleOnScreen = visibleOnScreen;
        }

        public virtual void Load(ContentManager content)
        {
            foreach (IRenderable renderable in renderables.Values)
                renderable.Load(content);
        }


        public override void Deserialize(XElement element)
        {
            //Read in all of our renderables.
            if (element.Element("renderables") != null)
            {
                RenderableFactory factory = (RenderableFactory)GameServiceManager.GetService(typeof(RenderableFactory));

                if (factory != null)
                    renderables = factory.GenerateRenderables(element.Element("renderables"), this);
            }
        }

        public override void Reallocate()
        {
            foreach (var renderable in renderables.Values)
                renderable.Dispose();

            renderables.Clear();

            base.Reallocate();
        }

        public override void CopyInto(IBaseComponent newObject)
        {
            var renderComponent = newObject as RenderComponent;

            Debug.Assert(renderComponent != null, "renderComponent != null");

            renderComponent.Visible = true;

            foreach (var renderableKeyValue in renderables)
            {
                var newRenderable = renderableKeyValue.Value.DeepClone();
                newRenderable.Visible = true;
                renderComponent.AddRenderable(newRenderable, renderableKeyValue.Key);
            }

            base.CopyInto(newObject);
        }

        protected void UpdateDimensions()
        {
            int leftMost = int.MinValue,
                rightMost = int.MaxValue,
                topMost = int.MinValue,
                bottomMost = int.MaxValue;

            foreach (var renderable in renderables.Values)
            {
                if (renderable.Position.X > leftMost)
                {
                    leftMost = (int) renderable.Position.X;
                }

                if (renderable.Position.X + renderable.Width < rightMost)
                {
                    rightMost = (int) (renderable.Position.X + renderable.Width);
                }

                if (renderable.Position.Y > topMost)
                {
                    topMost = (int) renderable.Position.Y;
                }

                if (renderable.Position.Y + renderable.Height < bottomMost)
                {
                    bottomMost = (int) (renderable.Position.Y + renderable.Height);
                }
            }

            //Now that we know the greatest extents of the renderables, use these to calculate the width and height.
            width = rightMost - leftMost;
            height = bottomMost - topMost;
        }
    }
}
