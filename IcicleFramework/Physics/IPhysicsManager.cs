using System;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using IcicleFramework.Components.Physics;
using IcicleFramework.GameServices;

namespace IcicleFramework.Physics
{
    public delegate void PhysicsCollisionHandler(IPhysicsComponent source, IPhysicsComponent colliding, Contact contact);

    public interface IPhysicsManager : IGameService, IUpdateable
    {
        World PhysicsWorld { get; }

        bool SubscribeCollisionEvent(Guid GUID, PhysicsCollisionHandler collisionHandler);

        void RegisterBody(Body body);

        Body GetBodyByName(string name);
    }
}
