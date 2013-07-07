using System;
using IcicleFramework.Entities;
using IcicleFramework.Pooling;
using Microsoft.Xna.Framework;

namespace IcicleFramework.Actions
{
    public delegate void OnGameActionFinishedHandler(IGameAction source);

    public interface IGameAction : IPauseable, IPoolable<IGameAction>, IDeepCopyable<IGameAction>, IDisposable
    {
        /// <summary>
        /// An event that fires when this <see cref="IGameAction"/> has finished its execution.
        /// </summary>
        event OnGameActionFinishedHandler OnGameActionFinished;

        /// <summary>
        /// Gets whether or not this <see cref="IGameAction"/> has completed execution.
        /// </summary>
        bool Finished { get; }

        /// <summary>
        /// Gets or sets the <see cref="IGameObject"/> that invoked this <see cref="IGameAction"/>.
        /// </summary>
        IGameObject Parent { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IGameObject"/> that is the TargetPosition of this <see cref="IGameAction"/>.
        /// </summary>
        IGameObject Target { get; set; }

        /// <summary>
        /// Updates this <see cref="IGameAction"/>.
        /// </summary>
        /// <param name="gameTime">Information related to the time since the last Update</param>
        /// <remarks>An <see cref="IGameAction"/> may mark itself as finished during its Update.</remarks>
        void Update(GameTime gameTime);

        void Initialize();
    }
}
