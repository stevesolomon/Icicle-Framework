using System.Collections.Generic;
using IcicleFramework.Components;
using IcicleFramework.Entities;

namespace IcicleFramework.GameServices
{
    public delegate void ManagerGameObjectChanged(IGameObject gameObject);

    public interface IGameObjectManager : IGameService
    {
        /// <summary>
        /// Gets the total number of GameObjects current managed by this <see cref="IGameObjectManager"/>
        /// </summary>
        int TotalGameObjects { get; }

        /// <summary>
        /// Gets the total number of Active GameObjects currently managed by this <see cref="IGameObjectManager"/>
        /// </summary>
        int TotalActiveGameObjects { get; }

        /// <summary>
        /// Gets the total number of Destroyed GameObjects currently managed by this <see cref="IGameObjectManager"/>
        /// </summary>
        int TotalDestroyedGameObjects { get; }

        /// <summary>
        /// Adds a new IGameObject to this IGameObjectCollection.
        /// </summary>
        /// <param name="gameObject">The IGameObject to add to this collection.</param>
        void Add(IGameObject gameObject);

        /// <summary>
        /// Removes the given IGameObject from this IGameObjectCollection.
        /// </summary>
        /// <param name="gameObject">The IGameObject to remove from this collection.</param>
        void Remove(IGameObject gameObject);

        /// <summary>
        /// An event that fires when IGameObjectManager adds a new IGameObject to be managed.
        /// </summary>
        event ManagerGameObjectChanged OnGameObjectAdded;

        /// <summary>
        /// An event that fires when this IGameObjectManager stops managing an IGameObject.
        /// </summary>
        event ManagerGameObjectChanged OnGameObjectRemoved;

        /// <summary>
        /// Gets all of the IGameObjects currently managed by this IGameObjectManager service.
        /// </summary>
        /// <returns>A ReadOnlyCollection of all IGameObjects managed by this IGameObjectManager service.</returns>
        IEnumerable<IGameObject> GetAll();

        /// <summary>
        /// Finds all IGameObjects with the given Metadata property.
        /// </summary>
        /// <param name="metaName">The metadata property name to search for.</param>
        /// <returns>A List of IGameObjects with the matching Metadata property.</returns>
        List<IGameObject> FindAllWithMetadata(string metaName);

        /// <summary>
        /// Finds all IGameObjects containing a component of the given Type T
        /// </summary>
        /// <typeparam name="T">The base type of the component to find.</typeparam>
        /// <returns>A List of IGameObjects containing the base type of component we are searching for.</returns>
        List<IGameObject> FindAllWithComponent<T>() where T : IBaseComponent;

        /// <summary>
        /// Finds the first IGameObject with the given Metadata property.
        /// </summary>
        /// <param name="metaName">The metadata property name to search for.</param>
        /// <returns>The first IGameObject found with the matching Metadata property, or null if no matching IGameObjects were found.</returns>
        IGameObject FindWithMetadata(string metaName);

        /// <summary>
        /// Finds the first IGameObject containing a component of the given Type T
        /// </summary>
        /// <typeparam name="T">The base type of the component to find.</typeparam>
        /// <returns>The first IGameObject found containing the base type of component we are searching for, or null if no matching IGameObjects were found.</returns>
        IGameObject FindWithComponent<T>() where T : IBaseComponent;
    }
}
