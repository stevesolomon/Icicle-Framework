using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace IcicleFramework.Renderables
{
    public class SingleColorRenderable : TexturedRenderable
    {
        public SingleColorRenderable() { }

        protected SingleColorRenderable(SingleColorRenderable old)
            :base(old)
        {
            
        }
        
        public override void Load(ContentManager content)
        {
            IGraphicsDeviceService gService =
                ((IGraphicsDeviceService) content.ServiceProvider.GetService(typeof (IGraphicsDeviceService)));

            this.Texture = new Texture2D(gService.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            this.Texture.SetData(new[] {Color.White});
            //this.Parent = this.Texture.Bounds;
        }


        public override IRenderable DeepClone()
        {
            return new SingleColorRenderable(this);
        }
        
    }
}
