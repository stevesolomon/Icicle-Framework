using ExampleGame.Actions;
using IcicleFramework.Actions;
using IcicleFramework.Behaviors;
using IcicleFramework.Components.Health;
using IcicleFramework.Entities;
using IcicleFramework.GameServices;
using IcicleFramework.GameServices.HelperServices;

namespace ExampleGame.Components.Behaviors
{
    public class BrickBehavior : BaseBehavior
    {
        private IRandomGenerator randGen;

        public override void Initialize()
        {
            var health = ParentGameObject.GetComponent<IHealthComponent>();
            health.OnHealthDepleted += HealthOnOnHealthDepleted;

            randGen = GameServiceManager.GetService<IRandomGenerator>();
            
            base.Initialize();
        }

        public override void PostInitialize()
        {
            PrepareColorChangeAction();

            base.PostInitialize();
        }

        private void PrepareColorChangeAction()
        {
            var action = Parent.ActionFactory.GetAction<RandomColorChangeAction>();
            
            Parent.FireAction(action, ParentGameObject, ColorChangeActionCompleted, randGen.GenerateRandomInt(1, 100) / 10f);
        }

        private void ColorChangeActionCompleted(IGameAction action)
        {
            if (Parent == null || ParentGameObject.Destroyed)
                return;

            PrepareColorChangeAction();
        }

        private void HealthOnOnHealthDepleted(IHealthComponent sender, IGameObject damageInitator)
        {
            var pointsAction = Parent.ActionFactory.GetAction<GivePointsAction>();
            pointsAction.Points = 10f;

            IGameObject playerPaddle = GameServiceManager.GetService<IGameObjectManager>().FindWithMetadata("player");

            Parent.FireAction(pointsAction, playerPaddle);

            ParentGameObject.Destroy();
        }
    }
}
