using IcicleFramework.Components.Renderable;
using IcicleFramework.Renderables;
using Microsoft.Xna.Framework;

namespace IcicleFramework.Components.Gauges
{
    public interface IGaugeComponent : IRenderComponent
    {
        /// <summary>
        /// Gets the ITexturedRenderable object used to display the Border of this IGaugeComponent.
        /// </summary>
        ITexturedRenderable Border { get; set; }

        /// <summary>
        /// Gets the ITexturedRenderable object used to the display the actual (progress) bar of this IGaugeComponent.
        /// </summary>
        ITexturedRenderable Bar { get; set; }

        Vector2 Offset { get; set; }
    }
}
