using System;
using IcicleFramework.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IcicleFramework.Renderables
{
    public interface IRenderable : IDrawableObject, IUpdateable, IXmlBuildable, IDisposable
    {
        IBaseComponent Parent { get; set; }
        float Scale { get; set; }
        Color ColorChannel { get; set; }
        Vector2 Offset { get; set; }
        float Layer { get; set; }
        float Rotation { get; set; }
        Vector2 Origin { get; set; }
        Vector2 Position { get; set; }

        int Width { get; }
        int Height { get; }

        void Initialize();

        SpriteEffects Effects { get; set; }

        IRenderable DeepClone();
    }
}
