using IcicleFramework.GameServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IcicleFramework.Rendering
{
    public interface IRenderer : IGameService 
    {
        SpriteBatch SpriteBatch { get; }

        Matrix ResolutionTransformMatrix { get; }

        bool Fullscreen { get; }

        void SetInternalResolution(int width, int height);

        void SetScreenResolution(int width, int height, bool fullscreen);

        void Draw();
        void PrepareDraw(Matrix view);
        void EndDraw();
        void ClearScreen();
    }
}
