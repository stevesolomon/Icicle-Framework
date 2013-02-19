using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IcicleFramework.Renderables.Animation
{
    public interface IAnimation : IXmlBuildable
    {
        Rectangle[] Frames { get; }

        string Name { get; set; }

        int TotalFrames { get; }

        bool Looping { get; set; }

        string CompletedValue { get; set; }

        float FramesPerSecond { get; set; }

        void BuildFrames(Texture2D texture, int numRows, int numCols);

        IAnimation DeepClone();
    }
}
