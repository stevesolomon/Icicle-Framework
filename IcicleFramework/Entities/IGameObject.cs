using System;
using System.Collections.Generic;
using IcicleFramework.Components;
using IcicleFramework.Pooling;
using Microsoft.Xna.Framework;

namespace IcicleFramework.Entities
{
    public delegate void MoveHandler(IGameObject sender);

    public delegate void InitializeHandler(IGameObject sender);

    public interface IGameObject : IPoolable<IGameObject>, IXmlBuildable, IDeepCopyable<IGameObject>, IDisposable
    {
        /// <summary>
        /// Gets or sets whether or not this <see cref="IGameObject"/> is Active.
        /// </summary>
        bool Active { get; set; }

        /// <summary>
        /// Gets the GUID for this IGameObject.
        /// </summary>
        Guid GUID { get; }

        /// <summary>
        /// Gets or sets the position of this IGameObject and its components.
        /// </summary>
        Vector2 Position { get; set; }

        /// <summary>
        /// Gets the amount along the x and y axes that this IGameObject has moved the last time it moved.
        /// </summary>
        Vector2 LastMovementAmount { get; }

        /// <summary>
        /// Gets the position this IGameObject was in during the last frame.
        /// </summary>
        Vector2 LastFramePosition { get; }

        /// <summary>
        /// Gets or sets the rotation of this IGameObject and its components.
        /// </summary>
        float Rotation { get; set; }

        /// <summary>
        /// Gets the change in rotation the last time this IGameObject rotated.
        /// </summary>
        float LastRotationAmount { get; }

        /// <summary>
        /// The generic name for this <see cref="IGameObject"/>. This may be defined in the XML definition for a Game Object or set manually.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets whether or not this IGameObject has moved in the last frame.
        /// </summary>
        bool HasMoved { get; }

        /// <summary>
        /// Gets whether or not this IGameObject has rotated in the last frame.
        /// </summary>
        bool HasRotated { get; }

        /// <summary>
        /// Gets the Metadata Dictionary for this IGameObject.
        /// </summary>
        Dictionary<string, object> Metadata { get; }

        /// <summary>
        /// Gets or sets the Layer for this IGameObject.
        /// </summary>
        string Layer { get; set; } 

        /// <summary>
        /// An event that fires whenever this IGameObject changes its position.
        /// </summary>
        event MoveHandler OnMove;

        /// <summary>
        /// An event that fires when this <see cref="IGameObject"/> has completed its initialization and is almost ready for use.
        /// </summary>
        event InitializeHandler OnInitialize;

        /// <summary>
        /// Initializes this IGameObject and all of its IBaseComponents.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Performs any post-initialization tasks on request.
        /// </summary>
        void PostInitialize();

        /// <summary>
        /// Updates this IGameObject and all of its components.
        /// </summary>
        /// <param name="gameTime">Game time-related information, including the time since the last Update.</param>
        void Update(GameTime gameTime);
        
        /// <summary>
        /// Retrieves the component of type T if this IGameObject contains a component
        /// of the matching type.
        /// </summary>
        /// <typeparam name="T">The type of component to be retrieved. Use the base interface for each type of
        /// component in order to properly retrieve the component.</typeparam>
        /// <returns>The component of type T if found, null if otherwise.</returns>
        T GetComponent<T>() where T : IBaseComponent;

        /// <summary>
        /// Adds a new component to this IGameObject.
        /// </summary>
        /// <param name="baseInterfaceType">The base interface type of the component we are adding.</param>
        /// <param name="component">The actual component to be added to this IGameObject.</param>
        /// <remarks>Currently, IGameObjects support only one component of each base interface type at a time.</remarks>
        void AddComponent(Type baseInterfaceType, IBaseComponent component);

        /// <summary>
        /// Retrieves a list of all components attached to this IGameObject.
        /// </summary>
        /// <returns>A list of all components currently attached to this IGameObject.</returns>
        IEnumerable<IBaseComponent> AllComponents();

        bool AddMetadata(string name, object value);

        bool RemoveMetadata(string name);

        bool UpdateMetadata(string name, object newValue);

        object GetMetadata(string name);

        bool HasMetadata(string name);
    }
}
