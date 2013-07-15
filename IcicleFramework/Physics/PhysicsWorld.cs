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
    public class PhysicsWorld : World
    {
        protected Dictionary<Guid, PhysicsCollisionHandler> collisionEvents;

        protected Dictionary<string, Body> bodyDictionary;

        public PhysicsWorld(RectangleF worldSpace)
            : base(Vector2.Zero)
        {
            collisionEvents = new Dictionary<Guid, PhysicsCollisionHandler>();
            bodyDictionary =  new Dictionary<string, Body>();
        }

        public void Initialize()
        {
            var factory = GameServiceManager.GetService<IGameObjectManager>();

            if (factory != null)
            {
                factory.OnGameObjectAdded += OnGameObjectAdded;
                factory.OnGameObjectRemoved += OnGameObjectRemoved;
            }

            BodyRemoved += OnBodyRemoved;

        }

        protected void OnBodyRemoved(Body body)
        {
            var userData = new FarseerUserData();

            if (body.UserData != null)
                userData = (FarseerUserData) body.UserData;

            if (bodyDictionary.ContainsKey(userData.Name))
                bodyDictionary.Remove(userData.Name);
        }

        private void OnGameObjectAdded(IGameObject newObject)
        {
            var collisionComponent = newObject.GetComponent<IPhysicsComponent>();

            if (collisionComponent != null)
            {
                //Subscribe to the collision event for each body.
                for (int i = 0; i < collisionComponent.NumBodies; i++)
                     collisionComponent.GetBody(i).OnCollision += OnCollision;
            }
        }



        public Body GetBodyByName(string name)
        {
            return bodyDictionary.ContainsKey(name) ? bodyDictionary[name] : null;
        }

        public void RegisterBody(Body body)
        {
            var userData = (FarseerUserData) body.UserData;

            if (userData.Name != null && !bodyDictionary.ContainsKey(userData.Name))
                bodyDictionary.Add(userData.Name, body);
        }

        private bool OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            var completeCollision = true;

            var collisionComponentA = fixtureA.UserData as IPhysicsComponent;
            var collisionComponentB = fixtureB.UserData as IPhysicsComponent;
            
            if (collisionComponentA != null && collisionComponentB != null && collisionEvents.ContainsKey(collisionComponentA.Parent.GUID))
                collisionEvents[collisionComponentA.Parent.GUID](collisionComponentA, collisionComponentB, contact);

            return completeCollision;
        }

        private void OnGameObjectRemoved(IGameObject gameObject)
        {
            var collisionComponent = gameObject.GetComponent<IPhysicsComponent>();

            if (collisionComponent != null)
            {
                for (var i = 0; i < collisionComponent.NumBodies; i++)
                {
                    RemoveBody(collisionComponent.GetBody(i));
                }
            }
        }
        
        public void Update(GameTime gameTime)
        {
            Step((float) gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);
        }

        public bool SubscribeCollisionEvent(Guid GUID, PhysicsCollisionHandler collisionHandler)
        {
            var subscribed = false;

            if (collisionEvents.ContainsKey(GUID))
            {
                collisionEvents[GUID] += collisionHandler;
                subscribed = true;
            }

            return subscribed;
        }
    }
}
