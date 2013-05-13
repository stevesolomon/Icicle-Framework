using IcicleFramework.Entities;

namespace IcicleFramework.Behaviors.Destruction
{
    public class BaseDestructionBehavior : BaseBehavior
    {
        public override void Initialize()
        {
            ParentGameObject.OnDestroyed += OnParentDestroyed;
            base.Initialize();
        }

        protected virtual void OnParentDestroyed(IGameObject sender)
        {
            ParentGameObject.OnDestroyed -= OnParentDestroyed;
        }
    }
}
