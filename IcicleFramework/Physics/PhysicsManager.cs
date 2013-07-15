using System;
using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using IcicleFramework.Components.Physics;
using IcicleFramework.Entities;
using IcicleFramework.GameServices;
using Microsoft.Xna.Framework;

namespace IcicleFramework.Physics
{
    public class PhysicsManager : GameService, IPhysicsManager
    {
        public World PhysicsWorld { get; protected set; }

        protected Dictionary<Guid, PhysicsCollisionHandler> collisionEvents;

        protected Dictionary<string, Body> bodyDictionary;

        public PhysicsManager()
        {
            this.PhysicsWorld = new World(Vector2.Zero);
            
            this.collisionEvents = new Dictionary<Guid, PhysicsCollisionHandler>();
            this.bodyDictionary = new Dictionary<string, Body>();
        }

        public override void Initialize()
        {
            var gameObjectManager = GameServiceManager.GetService<IGameObjectManager>();

            if (gameObjectManager != null)
                gameObjectManager.OnGameObjectAdded += OnGameObjectAddedHandler;

            PhysicsWorld.BodyRemoved += OnBodyRemoved;
        }

        private void OnGameObjectAddedHandler(IGameObject newObject)
        {
            var physicsComponent = newObject.GetComponent<IPhysicsComponent>();

            if (physicsComponent != null)
            {
                newObject.OnDestroyed += OnGameObjectDestroyed;

                //Subscribe to the collision event for each body.
                for (int i = 0; i < physicsComponent.NumBodies; i++)
                    physicsComponent.GetBody(i).OnCollision += OnCollision;
            }
        }

        private void OnGameObjectDestroyed(IGameObject gameObject)
        {
            var collisionComponent = gameObject.GetComponent<IPhysicsComponent>();

            if (collisionComponent != null)
            {
                for (int i = 0; i < collisionComponent.NumBodies; i++)
                  PhysicsWorld.RemoveBody(collisionComponent.GetBody(i));
            }
        }

        private bool OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            var completeCollision = true;

            var userDataA = fixtureA.Body.UserData as FarseerUserData;
            var userDataB = fixtureB.Body.UserData as FarseerUserData;

            if (userDataA != null && userDataB != null)
            {
                var collisionComponentA = userDataA.Owner;
                var collisionComponentB = userDataB.Owner;

                if (collisionComponentA != null && collisionComponentB != null && collisionEvents.ContainsKey(collisionComponentA.Parent.GUID))
                    collisionEvents[collisionComponentA.Parent.GUID](collisionComponentA, collisionComponentB, contact);

            }

            return completeCollision;
        }

        protected void OnBodyRemoved(Body body)
        {
            var userData = new FarseerUserData();

            if (body.UserData != null)
                userData = (FarseerUserData) body.UserData;

            if (bodyDictionary.ContainsKey(userData.Name))
                bodyDictionary.Remove(userData.Name);
        }

        public void RegisterBody(Body body)
        {
            var userData = (FarseerUserData)body.UserData;

            if (userData.Name != null && !bodyDictionary.ContainsKey(userData.Name))
                bodyDictionary.Add(userData.Name, body);
        }

        public Body GetBodyByName(string name)
        {
            return bodyDictionary.ContainsKey(name) ? bodyDictionary[name] : null;
        }

        public override void Update(GameTime gameTime)
        {
            PhysicsWorld.Step((float) gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);
        }

        public bool SubscribeCollisionEvent(Guid GUID, PhysicsCollisionHandler collisionHandler)
        {
            bool subscribed = false;

            if (collisionEvents.ContainsKey(GUID))
            {
                collisionEvents[GUID] += collisionHandler;
                subscribed = true;
            }
            else
            {
                collisionEvents.Add(GUID, collisionHandler);
                subscribed = true;
            }

            return subscribed;
        }
    }
}
