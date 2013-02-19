using IcicleFramework.Images;

namespace IcicleFramework.Components.Image
{
    public interface IRenderableComponent : IBaseComponent, IDrawableObject
    {
        IImage Image { get; set; }
    }
}
