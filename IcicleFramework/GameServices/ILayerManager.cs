namespace IcicleFramework.GameServices
{
    public interface ILayerManager : IGameService
    {
        /// <summary>
        /// Returns whether or not the two layers provided can interact with one another.
        /// </summary>
        /// <returns>True if the layers can interact, false if otherwise.</returns>
        bool LayersInteract(string layer1, string layer2);
    }
}
