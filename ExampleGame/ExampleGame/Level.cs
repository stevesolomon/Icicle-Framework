using System.Collections.Generic;
using System.Linq;
using IcicleFramework;
using IcicleFramework.GameServices;
using Microsoft.Xna.Framework;
using IcicleFramework.Entities;

namespace ExampleGame
{
    public class Level
    {
        protected IGameObjectManager gameObjectManager;

        public Level()
        { }

        public void Initialize(RectangleF worldSpace)
        {
            gameObjectManager = GameServiceManager.GetService<IGameObjectManager>();
        }
        

        public void Update(GameTime gameTime)
        {
            if (gameObjectManager != null)
            {
                IEnumerable<IGameObject> enumerable = gameObjectManager.GetAll();
 
                foreach (IGameObject gameObject in enumerable)
                {
                    if (gameObject.Active)
                        gameObject.Update(gameTime);
                }
            }
        }

        
    }
}
