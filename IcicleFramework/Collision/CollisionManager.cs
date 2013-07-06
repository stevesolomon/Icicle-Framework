using System;
using System.Collections.Generic;
using System.Diagnostics;
using IcicleFramework.Components;
using IcicleFramework.Components.Collision;
using IcicleFramework.Entities;
using IcicleFramework.GameServices;
using IcicleFramework.GameServices.Factories;
using IcicleFramework.Pooling;
using Microsoft.Xna.Framework;

namespace IcicleFramework.Collision
{
    public class CollisionManager : GameService, ICollisionManager
    {
        protected Dictionary<Guid, OnCollisionHandler> onCollisionEventListeners;

        protected Dictionary<Guid, OnCollisionStoppedHandler> onCollisionStoppedEventListeners;

        protected QuadTreeOld<ICollisionComponent> collisionTree;

        protected Dictionary<Guid, CollisionList> currentCollisions;

        protected PoolNew<CollisionList> listPool; 

        protected List<ICollisionComponent> tempList;

        protected Dictionary<Guid, ICollisionComponent> movedLastFrame;

        protected ILayerManager layerManager;

        public bool ResolveCollisions { get; set; }

        public bool Active { get; set; }

        #region Constructors

        public CollisionManager(RectangleF worldSpace)
        {
            listPool = new PoolNew<CollisionList>(typeof(CollisionList), 1000);
            currentCollisions = new Dictionary<Guid, CollisionList>();
            tempList = new List<ICollisionComponent>(64);
            movedLastFrame = new Dictionary<Guid, ICollisionComponent>(512);
            onCollisionEventListeners = new Dictionary<Guid, OnCollisionHandler>();
            onCollisionStoppedEventListeners = new Dictionary<Guid, OnCollisionStoppedHandler>();

            collisionTree = new QuadTreeOld<ICollisionComponent>(4, new RectangleF(worldSpace.X, worldSpace.Y, worldSpace.Width, worldSpace.Height));
        }

        #endregion

        public override void Initialize()
        {
            IGameObjectFactory factory = GameServiceManager.GetService<IGameObjectFactory>();

            if (factory != null)
                factory.OnGameObjectCreated += OnGameObjectCreated;

            layerManager = GameServiceManager.GetService<ILayerManager>();

            Active = true;
        }



        #region Subscribing and Unsubscribing

        public bool SubscribeCollisionEvent(Guid GUID, CollisionHandler handler)
        {
            return true;
        }

        public bool SubscribeCollisionEvent(Guid GUID, OnCollisionHandler handler)
        {
            bool subscribed = false;

            if (onCollisionEventListeners.ContainsKey(GUID))
            {
                onCollisionEventListeners[GUID] += handler;
                subscribed = true;
            }

            return subscribed;
        }

        public void UnsubscribeCollisionEvent(Guid GUID, OnCollisionHandler handler)
        {
            if (onCollisionEventListeners.ContainsKey(GUID))
            {
                onCollisionEventListeners[GUID] -= handler;
            }
        }

        public bool SubscribeCollisionStoppedEvent(Guid GUID, OnCollisionStoppedHandler handler)
        {
            bool subscribed = false;

            if (onCollisionStoppedEventListeners.ContainsKey(GUID))
            {
                onCollisionStoppedEventListeners[GUID] += handler;
                subscribed = true;
            }

            return subscribed;
        }

        public void UnsubscribeCollisionStoppedEvent(Guid GUID, OnCollisionStoppedHandler handler)
        {
            if (onCollisionStoppedEventListeners.ContainsKey(GUID))
            {
                onCollisionStoppedEventListeners[GUID] -= handler;
            }
        }

        #endregion
        

        #region Internal Event Handlers

        /// <summary>
        /// Fired whenever a moveable object has changed its position. We will test to
        /// ensure that the movement is valid and then tell its QuadTree that it has moved.
        /// </summary>
        /// <param name="sender">The IMoveable object that has moved, triggering this OnMove event.</param>
        private void OnMove(IGameObject sender)
        {
            ICollisionComponent collisionComp = sender.GetComponent<ICollisionComponent>();

            if (Active && collisionComp != null)
            {
                //collisionTree.ObjectMoved(collisionComp);

                //Add this component to the list of components that have moved this frame.
                if (!movedLastFrame.ContainsKey(sender.GUID))
                {
                    movedLastFrame.Add(sender.GUID, collisionComp);
                }
            }
        }

        private void OnGameObjectCreated(IGameObject newObject)
        {
            if (newObject.GetComponent<ICollisionComponent>() != null)
                RegisterObject(newObject);
        }

        private void OnCollisionComponentDestroyed(IBaseComponent baseComponent)
        {
            var collisionComponent = baseComponent as ICollisionComponent;
            var gameObject = baseComponent.Parent;

            if (collisionComponent != null)
                collisionTree.RemoveObject(collisionComponent);

            if (onCollisionEventListeners.ContainsKey(gameObject.GUID))
                onCollisionEventListeners.Remove(gameObject.GUID);

            if (onCollisionStoppedEventListeners.ContainsKey(gameObject.GUID))
                onCollisionStoppedEventListeners.Remove(gameObject.GUID);

            currentCollisions[gameObject.GUID].Destroyed = true;
            currentCollisions.Remove(gameObject.GUID);
            movedLastFrame.Remove(gameObject.GUID);

            //foreach (var value in currentCollisions)
            //{
            //    foreach (var collision in value.Value.Values)
            //    {
            //        if (collision.Parent.GUID == baseComponent.Parent.GUID)
            //        {
            //            toRemove.Add(value.Key, collision);
            //        }
            //    }
            //}

            //foreach (var value in toRemove)
            //{
            //    currentCollisions[value.Key].Remove(value.Value.Parent.GUID);
            //}

            gameObject.OnMove -= OnMove;
            collisionComponent.OnDestroyed -= OnCollisionComponentDestroyed;
        }

        private void RegisterObject(IGameObject theObject)
        {
            //Link ourselves to the OnMove event for the game object itself, and the OnDestroyed event for the collision component.
            theObject.OnMove += OnMove;

            var collisionComponent = theObject.GetComponent<ICollisionComponent>();

            collisionComponent.OnDestroyed += OnCollisionComponentDestroyed;

            collisionTree.AddObject(collisionComponent);

            //Add an entry to the current collision dictionary...
            currentCollisions.Add(theObject.GUID, listPool.New());
            currentCollisions[theObject.GUID].Source = collisionComponent;
            currentCollisions[theObject.GUID].Destroyed = false;

            //..and the collision event dictionaries
            onCollisionEventListeners.Add(theObject.GUID, null);
            onCollisionStoppedEventListeners.Add(theObject.GUID, null);
        }

        #endregion


        #region External Events

        private void NotifyCollision(ICollisionComponent source, ICollisionComponent collider)
        {
            if (source == null || source.Parent == null || collider == null || collider.Parent == null)
                return;

            var sourceGUID = source.Parent.GUID;
            var colliderGUID = collider.Parent.GUID;

            //Notify any listeners of the source that it has collided with something
            if (onCollisionEventListeners.ContainsKey(sourceGUID) && onCollisionEventListeners[sourceGUID] != null)
            {
                onCollisionEventListeners[sourceGUID](source, collider);
            }

            //Notify any listeners of the collider that is has collided with something
            if (onCollisionEventListeners.ContainsKey(colliderGUID) && onCollisionEventListeners[colliderGUID] != null)
            {
                onCollisionEventListeners[colliderGUID](collider, source);
            }
        }

        private void NotifyCollisionStopped(ICollisionComponent source, ICollisionComponent previousCollider)
        {
            if (source == null || source.Parent == null || previousCollider == null || previousCollider.Parent == null)
                return;

            var sourceGUID = source.Parent.GUID;
            var colliderGUID = previousCollider.Parent.GUID;

            //Notify any listeners of the source that it has stopped collided with what it was previously colliding with.
            if (onCollisionStoppedEventListeners.ContainsKey(sourceGUID) && onCollisionStoppedEventListeners[sourceGUID] != null)
            {
                onCollisionStoppedEventListeners[sourceGUID](source, previousCollider);
            }

            //Notify any listeners of the collider that is has stopped colliding with the source.
            if (onCollisionStoppedEventListeners.ContainsKey(colliderGUID) && onCollisionStoppedEventListeners[colliderGUID] != null)
            {
                onCollisionStoppedEventListeners[colliderGUID](previousCollider, source);
            }
        }

        #endregion
        

        public override void Update(GameTime gameTime)
        {
            foreach (var gameObject in movedLastFrame)
            {
                collisionTree.ObjectMoved(gameObject.Value);
            }

            //Update any of the existing collisions to ensure that:
            // (a) The objects in question are still colliding, and,
            // (b) OnCollisionOccurred events are fired if they are still colliding.
            UpdateExistingCollisions();

            //Add any new collisions from movedLastFrame that do not already appear in the existing collisions.
            UpdateNewCollisions();

            //Clear the moved list
            movedLastFrame.Clear();

            listPool.CleanUp();
            
            base.Update(gameTime);
        }

        protected void UpdateExistingCollisions()
        {
            //Check any collision dictionaries that have elements and have them remove any objects no longer colliding with their source object.
            foreach (var collisionList in currentCollisions.Values)
            {
                if (collisionList.Count <= 0) continue;

                tempList.Clear();

                foreach (var oldCollision in collisionList)
                {
                    //if (oldCollision.Parent == null || oldCollision.Parent.Destroyed)
                    //{
                    //    tempList.Add(oldCollision);
                    //    continue;
                   // }

                    //If the collision elements are still intersecting then the collision is ongoing.
                    if (collisionList.Source.BoundingBox2D.Intersects(oldCollision.BoundingBox2D))
                    {
                        NotifyCollision(collisionList.Source, oldCollision);
                    }
                    else //There is no longer a collision
                    {
                        NotifyCollisionStopped(collisionList.Source, oldCollision);
                        tempList.Add(oldCollision);
                    }
                }

                //Remove all old collisions that are no longer colliding.
                foreach (var old in tempList)
                {
                    collisionList.Remove(old);
                }
            }
        }


        protected void UpdateNewCollisions()
        {
            foreach (var moved in movedLastFrame)
            {
                tempList.Clear();
                collisionTree.GetCollidingWith(moved.Value, ref tempList);
                tempList.Remove(moved.Value);

                CullLayers(moved.Value, tempList);
                
                //Add any *new* collisions to the lists for each entity. 
                foreach (var collision in tempList)
                {
                    if (!currentCollisions[moved.Key].Contains(collision))
                    {
                        currentCollisions[moved.Key].Add(collision);
                        NotifyCollision(moved.Value, collision);
                    }

                    if (!currentCollisions[collision.Parent.GUID].Contains(moved.Value))
                    {
                        currentCollisions[collision.Parent.GUID].Add(moved.Value); 
                        NotifyCollision(collision, moved.Value);
                    }
                }  
            }
        }

        protected void CullLayers(ICollisionComponent moved, List<ICollisionComponent> list)
        {
            for (int i = list.Count - 1; i >= 0 && i < list.Count; i--)
            {
                if (layerManager != null && !layerManager.LayersInteract(moved.Parent.Layer, list[i].Parent.Layer))
                {
                    list.RemoveRange(i, 1);
                }
            }
        }
    }
}
