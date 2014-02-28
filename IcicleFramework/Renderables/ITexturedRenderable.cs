using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IcicleFramework.Renderables
{
    public interface ITexturedRenderable : IRenderable
    {
        string AssetName { get; set; }

        Texture2D Texture { get; }

        Rectangle Source { get; set; }
    }
}
