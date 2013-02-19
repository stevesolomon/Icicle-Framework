namespace ExampleGameSHMUP.GameServices
{
    public interface ILevelLogicManager
    {
        bool GameOver { get; }

        void LoadLevel(string levelName);
    }
}
