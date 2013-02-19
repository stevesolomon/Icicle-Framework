using System.Collections.Generic;
using System.Xml.Linq;
using IcicleFramework.Components.Renderable;
using IcicleFramework.Renderables;

namespace IcicleFramework.GameServices.Factories
{
    public interface IRenderableFactory : IGameService
    {
        /// <summary>
        /// Generates the IRenderable object(s) corresponding to the provided XElement definition and assigns them to
        /// the given IRenderComponent.
        /// </summary>
        /// <param name="element">The XElement containing the descriptions of the IRenderable objects to create.</param>
        /// <param name="parent">The IRenderComponent all generated IRenderable objects will be parented to.</param>
        /// <returns>A Dictionary of the IRenderable objects generated, keyed by their name defined in the provided XElement.</returns>
        Dictionary<string, IRenderable> GenerateRenderables(XElement element, IRenderComponent parent);

        /// <summary>
        /// Generates a single IRenderable object corresponding to the provided XElement definition and assigns it to the given IRenderComponent.
        /// </summary>
        /// <param name="element">The XElement containing the description of the IRenderable object to create.</param>
        /// <param name="parent">The IRenderComponent the IRenderable object will be parented to.</param>
        /// <returns>The generated IRenderable object, or null if the provided XElement did not contain a valid description.</returns>
        IRenderable GenerateRenderable(XElement element, IRenderComponent parent);
    }
}
