using System.Diagnostics;
using System.Xml.Linq;
using ExampleGame.Components.Score;
using ExampleGame.GameSystems;
using IcicleFramework.Components.Behaviors;
using IcicleFramework.Components.Renderable;
using IcicleFramework.Entities;
using IcicleFramework.GameServices;
using IcicleFramework.Renderables;

namespace ExampleGame.Components.Behaviors.HUD
{
    public class HUDScoreBehavior : BaseBehavior
    {
        protected string textRenderableName;

        protected TextRenderable textRenderable;

        public string BaseText { get; set; }

        public string TextRenderableName
        {
            get { return textRenderableName; }
            set { textRenderableName = value; }
        }

        public override void Initialize()
        {
            var renderable = ParentGameObject.GetComponent<IRenderComponent>();

            if (renderable != null)
            {
                textRenderable = renderable.GetRenderable(textRenderableName) as TextRenderable;

                if (textRenderable != null)
                {
                    textRenderable.Text = string.Format(BaseText, 0);
                }
            }

            base.Initialize();
        }

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
            if (textRenderable != null)
            {
                textRenderable.Text = string.Format(BaseText, source.Score);
            }
        }

        public override void Deserialize(XElement element)
        {
            string baseText = "Player Score: {0}";
            if (element.Element("baseText") != null)
                baseText = element.Element("baseText").Value;
            BaseText = baseText;

            string renderableName = "";
            if (element.Element("renderableName") != null)
                renderableName = element.Element("renderableName").Value;
            textRenderableName = renderableName;

            base.Deserialize(element);
        }

        public override void CopyInto(IBehavior newObject)
        {
            var hudBehavior = newObject as HUDScoreBehavior;

            Debug.Assert(hudBehavior != null, "hudBehavior == null");

            hudBehavior.TextRenderableName = TextRenderableName;
            hudBehavior.BaseText = BaseText;

            base.CopyInto(newObject);
        }
    }
}
