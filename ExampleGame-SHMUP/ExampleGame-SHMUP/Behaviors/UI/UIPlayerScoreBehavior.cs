using System.Globalization;
using IcicleFramework.Behaviors;
using IcicleFramework.Components.Behaviors;
using IcicleFramework.Components.Health;
using IcicleFramework.Components.Renderable;
using IcicleFramework.Entities;
using IcicleFramework.GameServices;
using IcicleFramework.Renderables;

namespace ExampleGameSHMUP.Behaviors.UI
{
    public class UIPlayerScoreBehavior : BaseBehavior
    {
        private ITextRenderable scoreText;

        private IGameObject player;

        public override void Initialize()
        {
            scoreText = ParentGameObject.GetComponent<IRenderComponent>().GetRenderable("score") as ITextRenderable;

            base.Initialize();
        }

        public override void PostInitialize()
        {
            var goManager = GameServiceManager.GetService<IGameObjectManager>();
            goManager.OnGameObjectAdded += OnGameObjectAdded;

            player = goManager.FindWithMetadata("player");

            base.PostInitialize();
        }

        private void OnGameObjectAdded(IGameObject gameObject)
        {
            //If it's an enemy, and has an IHealthComponent let's watch for its death!
            if (!gameObject.Layer.ToLowerInvariant().Equals("enemylayer")) 
                return;
            
            var healthComp = gameObject.GetComponent<IHealthComponent>();

            if (healthComp != null)
            {
                healthComp.OnHealthDepleted += OnEnemyHealthDepleted;
            }
        }

        private void OnEnemyHealthDepleted(IHealthComponent sender, IGameObject damageInitator)
        {
            //Make sure that the damage initiator was something owned by the player...
            if (!damageInitator.Layer.ToLowerInvariant().Equals("playerlayer")) 
                return;

            sender.OnHealthDepleted -= OnEnemyHealthDepleted;

            if (player.HasMetadata("score"))
            {
                var metadataValue = int.Parse(player.GetMetadata("score").ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture);
                metadataValue++;
                player.UpdateMetadata("score", metadataValue);

                scoreText.Text = metadataValue.ToString(CultureInfo.InvariantCulture);
            }
        }

    }
}
