using System.Globalization;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IcicleFramework.Renderables.Animation
{
    public class SpriteAnimation : IAnimation
    {
        private static int animIndex = 0;

        protected int startIndex;

        public Rectangle[] Frames { get; protected set; }

        public string Name { get; set; }

        public int TotalFrames { get; protected set; }

        public float FramesPerSecond { get; set; }

        public bool Looping { get; set; }

        public string CompletedValue { get; set; }

        public SpriteAnimation() { }

        protected SpriteAnimation(SpriteAnimation old)
        {
            Frames = new Rectangle[old.Frames.Length];
            old.Frames.CopyTo(Frames, 0);

            FramesPerSecond = old.FramesPerSecond;
            TotalFrames = old.TotalFrames;
            Looping = old.Looping;
            CompletedValue = old.CompletedValue;
        }
        
        /// <summary>
        /// Builds up the list of sources for each frame using the given texture.
        /// </summary>
        /// <param name="texture">The texture containing each frame for the AnimatedImage.</param>
        /// <param name="numRows">The number of rows of frames in the texture.</param>
        /// <param name="numCols">The number of columns of frames in the texture.</param>
        public void BuildFrames(Texture2D texture, int numRows, int numCols)
        {
            //We know how many Rectangles/Frames we need by the number of rows and columns
            Frames = new Rectangle[TotalFrames];

            int frame = 0;
            int width = (texture.Width / numCols);
            int height = (texture.Height / numRows);

            //The starting row we're at is defined by our start index / numCols
            int startRow = startIndex / numCols, 
                startCol = startIndex % numCols;

            int numFramesUsed = 0;

            //Build up each frame by row then column
            for (int row = startRow; row < numRows && numFramesUsed < TotalFrames; row++)
            {
                int yStart = row * height;

                for (int col = startCol; col < numCols; col++)
                {
                    int xStart = col * width;
                    Frames[frame++] = new Rectangle(xStart, yStart, width, height);
                    numFramesUsed++;

                    if (numFramesUsed >= TotalFrames)
                        break;
                }

                startCol = 0;
            }
        }

        public IAnimation DeepClone()
        {
            return new SpriteAnimation(this);
        }

        public void Deserialize(XElement element)
        {
            XElement curr = element.Element("startIndex");
            int value = 0;

            if (curr != null)
            {
                int.TryParse(curr.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
            }
            startIndex = value;

            curr = element.Element("framesPerSecond");
            if (curr != null)
            {
                int.TryParse(curr.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
            }
            FramesPerSecond = value;

            curr = element.Element("totalFrames");
            if (curr != null)
            {
                int.TryParse(curr.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
            }
            TotalFrames = value;

            bool looping = true;
            curr = element.Element("looping");
            if (curr != null)
            {
                bool.TryParse(curr.Value, out looping);
            }
            Looping = looping;

            var attrib = element.Attribute("name");
            string name = animIndex.ToString(CultureInfo.InvariantCulture);
            if (attrib != null)
            {
                name = attrib.Value;
            }
            Name = name;

            curr = element.Element("completedValue");
            string completedValue = "";
            if (curr != null)
            {
                completedValue = curr.Value;
            }
            CompletedValue = completedValue;
        }
    }
}
