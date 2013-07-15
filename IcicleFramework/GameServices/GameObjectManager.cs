using System;
using System.Collections.Generic;
using System.Linq;
using IcicleFramework.Components;
using IcicleFramework.Entities;
using IcicleFramework.GameServices.Factories;
using Microsoft.Xna.Framework;

namespace IcicleFramework.GameServices
{
    public class GameObjectManager : GameService, IGameObjectManager
    {
        protected Dictionary<Guid, IGameObject> gameObjects;

        protected List<IGameObject> newGameObjects;

        protected List<Guid> destroyedGameObjects;

        public event ManagerGameObjectChanged OnGameObjectAdded;

        public event ManagerGameObjectChanged OnGameObjectRemoved;

        public int TotalGameObjects
        {
            get { return gameObjects.Count; }
        }

        public int TotalActiveGameObjects
        {
            get
            { 
                return gameObjects.Count(val => val.Value.Active);
            }
        }

        public int TotalDestroyedGameObjects
        {
            get
            {
                return gameObjects.Count(val => val.Value.Destroyed);
            }
        }

        public GameObjectManager()
        {
            gameObjects = new Dictionary<Guid, IGameObject>(2048);
            newGameObjects = new List<IGameObject>(256);
            destroyedGameObjects = new List<Guid>(256);
        }
 
        public override void Initialize()
        {
            var factory = GameServiceManager.GetService<IGameObjectFactory>();

            if (factory != null)
                factory.OnGameObjectCreated += OnGameObjectCreatedHandler;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (newGameObjects.Count == 0 && destroyedGameObjects.Count == 0)
                return;

            foreach (var gameObject in newGameObjects)
            {
                gameObjects.Add(gameObject.GUID, gameObject);
            }

            foreach (var gameObject in destroyedGameObjects)
            {
                gameObjects.Remove(gameObject);
            }

            destroyedGameObjects.Clear();
            newGameObjects.Clear();
        }
        
        protected void OnGameObjectCreatedHandler(IGameObject newObject)
        {
            Add(newObject);
        }

        public void Add(IGameObject gameObject)
        {
            newGameObjects.Add(gameObject);

            //Subscribe to the OnDestroyed event so we know when the IGameObject can be released.
            gameObject.OnDestroyed += OnGameObjectDestroyed;

            if (OnGameObjectAdded != null)
                OnGameObjectAdded(gameObject);
        }

        protected void OnGameObjectDestroyed(IGameObject gameObject)
        {
            Remove(gameObject);
        }

        public void Remove(IGameObject gameObject)
        {
            var removed = false;

            //We should have this game object listed, but let's just check to be sure.
            //We also need to remember to unsubscribe from the OnDestroyed event.
            if (gameObjects.ContainsKey(gameObject.GUID))
            {
                destroyedGameObjects.Add(gameObject.GUID);
                gameObject.OnDestroyed -= OnGameObjectDestroyed;

                removed = true;
            }
            else //Try to remove it from the list of new game objects
            {
                removed = newGameObjects.Remove(gameObject);
            }

            if (removed && OnGameObjectRemoved != null)
                OnGameObjectRemoved(gameObject);
        }

        public IEnumerable<IGameObject> GetAll()
        {
            return gameObjects.Values;
        }

        public List<IGameObject> FindAllWithMetadata(string metaName)
        {
            var foundObjects = new List<IGameObject>();

            //For now, we have to scan through all of the game objects in order to figure out
            //which ones may or may not contain the given metadata.
            //A future improvement may be to store indices on metadata for quicker lookup.
            foreach (var gameObject in gameObjects.Values)
            {
                if (gameObject.HasMetadata(metaName))
                    foundObjects.Add(gameObject);
            }

            foreach (var gameObject in newGameObjects)
            {
                if (gameObject.HasMetadata(metaName))
                    foundObjects.Add(gameObject);
            }
            
            return foundObjects;
        }

        public List<IGameObject> FindAllWithComponent<T>() where T : IBaseComponent
        {
            var foundObjects = new List<IGameObject>();

            //Like searching for all game objects with a given metadata item, this requires us to look
            //at all of the gameobjects, and check if they have the given component.
            //A future improvement may be to store indices on components for quicker lookup.
            foreach (var gameObject in gameObjects.Values)
            {
                IBaseComponent component = gameObject.GetComponent<T>();

                //We found the component of type T within this game object!
                if (component != null)
                {
                    foundObjects.Add(gameObject);
                }
            }

            foreach (var gameObject in newGameObjects)
            {
                IBaseComponent component = gameObject.GetComponent<T>();

                //We found the component of type T within this game object!
                if (component != null)
                {
                    foundObjects.Add(gameObject);
                }
            }

            return foundObjects;
        }

        public IGameObject FindWithMetadata(string metaName)
        {
            IGameObject found = null;

            //In the worst case we'll have to loop through all game objects, but we'll break the loop short
            //when we find the first game object with the given metadata.
            //We could still improve this in the future with an index on metadata and simply grab the first listed game object there.
            foreach (var gameObject in gameObjects.Values)
            {
                if (gameObject.HasMetadata(metaName))
                {
                    found = gameObject;
                    break;
                }
            }

            if (found == null)
            {
                foreach (var gameObject in newGameObjects)
                {
                    if (gameObject.HasMetadata(metaName))
                    {
                        found = gameObject;
                        break;
                    }
                }
            }

            return found;
        }

        public IGameObject FindWithComponent<T>() where T : IBaseComponent
        {
            IGameObject found = null;

            foreach (var gameObject in gameObjects.Values)
            {
                IBaseComponent component = gameObject.GetComponent<T>();

                if (component != null)
                {
                    found = gameObject;
                    break;
                }
            }

            return found;
        }
    }
}
