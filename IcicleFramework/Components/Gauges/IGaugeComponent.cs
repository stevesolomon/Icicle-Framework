using IcicleFramework.Components.Renderable;
using IcicleFramework.Renderables;
using Microsoft.Xna.Framework;

namespace IcicleFramework.Components.Gauges
{
    public interface IGaugeComponent : IRenderComponent
    {
        /// <summary>
        /// Gets the ITextured object used to display the Border of this IGaugeComponent.
        /// </summary>
        ITextured Border { get; set; }

        /// <summary>
        /// Gets the ITextured object used to the display the actual (progress) bar of this IGaugeComponent.
        /// </summary>
        ITextured Bar { get; set; }

        Vector2 Offset { get; set; }
    }
}
