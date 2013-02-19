using ExampleGame.Components.Score;
using IcicleFramework.Components.Renderable;

namespace ExampleGame.Components.HUD
{
    interface IHUDScoreComponent : IRenderComponent
    {
        void OnScoreChanged(IScoreComponent source);
    }
}
