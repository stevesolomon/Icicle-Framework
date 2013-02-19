using IcicleFramework.Entities;

namespace IcicleFramework.GameServices.Factories
{
    public delegate void GameObjectCreatedHandler(IGameObject newObject);

    public interface IGameObjectFactory : IGameService
    {
        /// <summary>
        /// An event that fires whenever a new IGameObject is created by this IGameObjectFactory.
        /// </summary>
        event GameObjectCreatedHandler OnGameObjectCreated;

        /// <summary>
        /// Gets or set the path (relative to the Content directory) of the XML file containing entity definitions.
        /// </summary>
        string PathToXML { get; set; }

        /// <summary>
        /// Deep copies an existing <see cref="IGameObject"/> into a new one.
        /// </summary>
        /// <param name="toCopy">The <see cref="IGameObject"/> to deep copy.</param>
        /// <returns>A deep copy of the original <see cref="IGameObject"/>.</returns>
        IGameObject CopyExistingGameObject(IGameObject toCopy);

        IGameObject GetRawGameObject();

        IGameObject GetGameObject(string name, float x = 0f, float y = 0f, bool usedAsTemplate = false);
    }
}
