using System;
using IcicleFramework.Components.Collision;
using IcicleFramework.GameServices;

namespace IcicleFramework.Collision
{
    public interface ICollisionManager : IGameService
    {
        /// <summary>
        /// Gets or sets whether this <see cref="ICollisionManager"/> should resolve collisions.
        /// </summary>
        bool ResolveCollisions { get; set; }

        bool SubscribeCollisionEvent(Guid GUID, CollisionHandler handler);

        bool SubscribeCollisionEvent(Guid GUID, OnCollisionHandler handler);

        void UnsubscribeCollisionEvent(Guid GUID, OnCollisionHandler handler);

        bool SubscribeCollisionStoppedEvent(Guid GUID, OnCollisionStoppedHandler handler);

        void UnsubscribeCollisionStoppedEvent(Guid GUID, OnCollisionStoppedHandler handler);
    }
}
