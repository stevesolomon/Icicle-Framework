namespace IcicleFramework.GameServices.HelperServices
{
    public interface IRandomGenerator : IGameService
    {
        int GenerateRandomInt();

        int GenerateRandomInt(int min, int max);

        float GenerateRandomFloat();

        float GenerateRandomFloat(float min, float max);
    }
}
