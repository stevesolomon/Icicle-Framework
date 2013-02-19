using IcicleFramework.GameServices;
using IcicleFramework.GameServices.HelperServices;
using Microsoft.Xna.Framework;

namespace ExampleGame.Actions
{
    public class RandomColorChangeAction : ColorChangeAction
    {
        public override void Update(GameTime gameTime)
        {
            var randomGenerator = GameServiceManager.GetService<IRandomGenerator>();
            
                int r = randomGenerator.GenerateRandomInt(0, 255),
                    g = randomGenerator.GenerateRandomInt(0, 255),
                    b = randomGenerator.GenerateRandomInt(0, 255);

            Color = new Color(r, g, b);

            base.Update(gameTime);
        }
    }
}
