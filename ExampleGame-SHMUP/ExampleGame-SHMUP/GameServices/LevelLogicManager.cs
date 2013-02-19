using IcicleFramework.GameServices;

namespace ExampleGameSHMUP.GameServices
{
    public class LevelLogicManager : GameService, ILevelLogicManager
    {
        public bool GameOver { get; protected set; }

        public void LoadLevel(string levelName)
        {
            
        }
    }
}
