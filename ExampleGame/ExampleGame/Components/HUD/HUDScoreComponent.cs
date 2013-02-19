using System.Xml.Linq;
using ExampleGame.Components.Score;
using ExampleGame.GameSystems;
using IcicleFramework.Components.Renderable;
using IcicleFramework.Entities;
using IcicleFramework.GameServices;
using IcicleFramework.Renderables;

namespace ExampleGame.Components.HUD
{
    public class HUDScoreComponent : RenderComponent, IHUDScoreComponent
    {
        public string BaseText { get; protected set; }

        public override void PostInitialize()
        {
            //Request the player object from the Game Object Manager...
            IGameObjectManager objectManager = GameServiceManager.GetService<IGameObjectManager>();

            if (objectManager != null)
            {
                IGameObject objectOfInterest = objectManager.FindWithMetadata("player");

                //Tell the Score Subsystem we are interested in linking up with the Score component for the player object.
                IScoreManager scoreManager = GameServiceManager.GetService<IScoreManager>();

                if (scoreManager != null && objectOfInterest != null)
                {
                    scoreManager.SubscribeToScoreChanged(objectOfInterest, OnScoreChanged);
                }
            }
        }
            
        public void OnScoreChanged(IScoreComponent source)
        {
            ITextRenderable textRenderable = null;

            if (NumRenderables > 0)
                textRenderable = this.renderables["text"] as ITextRenderable;
            
            if (textRenderable != null)
                textRenderable.Text = string.Format(BaseText, source.Score);
        }
        
        public override void Deserialize(XElement element)
        {
            string baseText = "Player Score: {0}";
            if (element.Element("baseText") != null)
                baseText = element.Element("baseText").Value;
            this.BaseText = baseText;

            base.Deserialize(element);
        }
    }
}
