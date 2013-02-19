using System.Xml.Linq;
using ExampleGame.GameSystems;
using IcicleFramework.Components;
using IcicleFramework.Components.Renderable;
using IcicleFramework.Entities;
using IcicleFramework.GameServices;
using IcicleFramework.Renderables;

namespace ExampleGame.Components.HUD
{
    public class HUDLivesComponent : RenderComponent, IHUDLivesComponent
    {
        public string BaseText { get; protected set; }

        public override void PostInitialize()
        {
            ILevelLogicManager logicManager = GameServiceManager.GetService<ILevelLogicManager>();
            ITextRenderable textRenderable = null;

            if (NumRenderables > 0)
                textRenderable = this.renderables["text"] as ITextRenderable;

            if (logicManager != null)
            {
                logicManager.OnPlayerLivesChanged += OnOnPlayerLivesChanged;
                if (textRenderable != null) textRenderable.Text = string.Format(BaseText, logicManager.Lives);
            }


            base.PostInitialize();
        }
        
        private void OnOnPlayerLivesChanged(IGameObject gameObject, int lives)
        {
            ITextRenderable textRenderable = null;

            if (NumRenderables > 0)
                textRenderable = this.renderables["text"] as ITextRenderable;

            if (textRenderable != null)
                textRenderable.Text = string.Format(BaseText, lives);
        }

        public override void Deserialize(XElement element)
        {
            string baseText = "Player Lives: {0}";
            if (element.Element("baseText") != null)
                baseText = element.Element("baseText").Value;
            this.BaseText = baseText;

            base.Deserialize(element);
        }
    }
}
