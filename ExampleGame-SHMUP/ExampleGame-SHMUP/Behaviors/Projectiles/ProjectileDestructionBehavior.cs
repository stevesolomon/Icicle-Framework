using IcicleFramework.Actions.Destruction;
using IcicleFramework.Behaviors;
using IcicleFramework.Components.Behaviors;
using IcicleFramework.Entities;
using IcicleFramework.GameServices;
using IcicleFramework.GameServices.CameraServices;
using Microsoft.Xna.Framework;

namespace ExampleGameSHMUP.Behaviors.Projectiles
{
    public class ProjectileDestructionBehavior : BaseBehavior
    {
        protected ICameraService CameraManager { get; set; }

        public override void Initialize()
        {
            CameraManager = GameServiceManager.GetService<ICameraService>();

            ParentGameObject.OnMove += OnMove;
            base.Initialize();
        }

        private void OnMove(IGameObject sender)
        {
            Vector2 scenePosition = CameraManager.TranslateWorldPointToScreen(ParentGameObject.Position);

            if (scenePosition.X < -100f || scenePosition.Y < -100f
                || scenePosition.X > 2100f || scenePosition.Y > 1200f)
            {
                var destroyAction = Parent.ActionFactory.GetAction<DestroyGameObjectAction>();
                Parent.FireAction(destroyAction, ParentGameObject);
            }
        }

        public override void Destroy()
        {
            CameraManager = null;
            ParentGameObject.OnMove -= OnMove;

            base.Destroy();
        }
    }
}
