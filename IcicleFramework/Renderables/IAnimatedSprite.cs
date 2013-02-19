namespace IcicleFramework.Renderables
{
    public interface IAnimatedSprite
    {
        event OnAnimationCompletedHandler OnAnimationCompleted;

        void StartAnimation(string name);
        void StopAnimation();
        void PauseAnimation();
    }
}
