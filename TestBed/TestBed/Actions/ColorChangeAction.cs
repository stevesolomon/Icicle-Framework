using IcicleFramework.Actions;
using IcicleFramework.Components.Renderable;
using IcicleFramework.Renderables;
using Microsoft.Xna.Framework;

namespace TestBed.Actions
{
    public class ColorChangeAction : GameAction 
    {
        public Color Color { get; set; }

        public override void Update(GameTime gameTime)
        {
            var renderComp = Target.GetComponent<IRenderComponent>();
            var renderable = (SingleColorRenderable)renderComp.GetRenderable(0);

            //Change the color
            renderable.ColorChannel = Color;

            Finished = true;
        }
    }
}
