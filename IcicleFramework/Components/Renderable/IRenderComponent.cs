using IcicleFramework.Renderables;
using Microsoft.Xna.Framework;

namespace IcicleFramework.Components.Renderable
{
    public interface IRenderComponent : IBaseComponent, IDrawableObject
    {
        /// <summary>
        /// Gets the number of IRenderables managed by this IRenderComponent.
        /// </summary>
        int NumRenderables { get; }

        /// <summary>
        /// Gets the maximum Width that contains all IRenderables managed by this IRenderComponent.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the maximum Height that contains all IRenderables managed by this IRenderComponent.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Gets the IRenderable with the given name.
        /// </summary>
        /// <param name="name">The name of the IRenderable to retrieve.</param>
        /// <returns>The IRenderable with the given name, null if no matching IRenderable was found.</returns>
        IRenderable GetRenderable(string name);

        /// <summary>
        /// Gets the IRenderable at the given index.
        /// </summary>
        /// <param name="index">The index of the IRenderable to retrieve.</param>
        /// <returns>The IRenderable at the given index, null if thise IRenderComponent does not contain at least index IRenderables.</returns>
        IRenderable GetRenderable(int index);

        /// <summary>
        /// Adds the IRenderable with the given name to this IRenderComponent. This IRenderComponent must not contain an IRenderable
        /// with the given name, or the IRenderable will not be added.
        /// </summary>
        /// <param name="renderable">The IRenderable to add to this IRenderComponent.</param>
        /// <param name="name">The name associated with the IRenderable to add.</param>
        /// <returns>True if the IRenderable was added to this IRenderComponent, false if otherwise.</returns>
        bool AddRenderable(IRenderable renderable, string name);

        /// <summary>
        /// Sets the shading/color channel for all IRenderables managed by this IRenderComponent.
        /// </summary>
        /// <param name="shading">The new shading/color channel to use.</param>
        void SetShading(Color shading);

        /// <summary>
        /// Sets the scale for all IRenderables managed by this IRenderComponent.
        /// </summary>
        /// <param name="scale">The new scale value to use.</param>
        void SetScale(float scale);

        /// <summary>
        /// Sets the layer for all IRenderables managed by this IRenderComponent.
        /// </summary>
        /// <param name="layer">The new layer value to use.</param>
        void SetLayer(float layer);
    }
}
