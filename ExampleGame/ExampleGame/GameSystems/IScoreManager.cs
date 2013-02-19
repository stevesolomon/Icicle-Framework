using IcicleFramework.Entities;
using IcicleFramework.GameServices;

namespace ExampleGame.GameSystems
{
    public interface IScoreManager : IGameService
    {
        float GetScore(IGameObject gameObject);

        void AddPoints(IGameObject target, float points);
      
        void SubscribeToScoreChanged(IGameObject objectOfInterest, ScoreChangedHandler scoreChangedDelegate);
    }
}
