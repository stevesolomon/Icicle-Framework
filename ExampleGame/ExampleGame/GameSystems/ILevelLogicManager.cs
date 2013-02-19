using IcicleFramework.Entities;
using IcicleFramework.GameServices;

namespace ExampleGame.GameSystems
{
    public delegate void LivesChangedHandler(IGameObject gameObject, int lives);

    public interface ILevelLogicManager : IGameService
    {
        event LivesChangedHandler OnPlayerLivesChanged;

        int Lives { get; }

        bool GameOver { get; }

        void LoadLevel(string levelName);
    }
}
