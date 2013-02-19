using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using IcicleFramework.Components.Renderable;
using IcicleFramework.Renderables;
using Microsoft.Xna.Framework.Content;

namespace IcicleFramework.GameServices.Factories
{
    /// <summary>
    /// A Factory for generating IRenderable objects.
    /// </summary>
    public class RenderableFactory : GameService, IRenderableFactory
    {
        private ContentManager contentManager;

        public RenderableFactory(ContentManager content)
        {
            this.contentManager = content;
        }

        public Dictionary<string, IRenderable> GenerateRenderables(XElement element, IRenderComponent parent)
        {
            Dictionary<string, IRenderable> renderables = new Dictionary<string, IRenderable>();
            IEnumerable<XElement> renderableElements = element.Elements("renderable");

            int i = 0;

            foreach (XElement renderableElement in renderableElements)
            {
                IRenderable renderable = GenerateRenderable(renderableElement, parent);

                if (renderable != null)
                {
                    string name;

                    XAttribute nameAttrib = renderableElement.Attribute("name");
                    if (nameAttrib != null)
                        name = nameAttrib.Value;
                    else
                        name = i.ToString(CultureInfo.InvariantCulture);
                    
                    renderables.Add(name, renderable);

                    i++;
                }
            }
            
            return renderables;
        }

        public IRenderable GenerateRenderable(XElement element, IRenderComponent parent)
        {
            IRenderable renderable = null;

            XAttribute classAttrib = element.Attribute("class"), 
                        typeAttrib = element.Attribute("type");

            if (classAttrib != null && typeAttrib != null)
            {
                Type classType = Type.GetType(classAttrib.Value);

                renderable = (IRenderable) Activator.CreateInstance(classType);
                renderable.Deserialize(element);
                renderable.Load(contentManager);
                renderable.Visible = true;
                renderable.Parent = parent;
            }
            return renderable;
        }
    }
}
