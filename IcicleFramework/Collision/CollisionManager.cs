using System;
using System.Collections.Generic;
using IcicleFramework.Collision.QuadTree;
using IcicleFramework.Components;
using IcicleFramework.Components.Collision;
using IcicleFramework.Entities;
using IcicleFramework.GameServices;
using IcicleFramework.Pooling;
using Microsoft.Xna.Framework;

namespace IcicleFramework.Collision
{
    public class CollisionManager : GameService, ICollisionManager
    {
        protected Dictionary<Guid, OnCollisionHandler> onCollisionEventListeners;

        protected Dictionary<Guid, OnCollisionStoppedHandler> onCollisionStoppedEventListeners;

        protected QuadTree.QuadTree<ICollisionComponent> collisionTree;

        protected Dictionary<Guid, CollisionList> currentCollisions;

        protected PoolNew<CollisionList> listPool; 

        protected Dictionary<Guid, IBoundingBox> quadTreeReturnList;

        protected List<ICollisionComponent> tempList;

        protected List<IBoundingBox> currentCollisionsList; 

        protected Dictionary<Guid, ICollisionComponent> movedLastFrame;

        protected ILayerManager layerManager;

        public bool ResolveCollisions { get; set; }

        public bool Active { get; set; }

        #region Constructors

        public CollisionManager(RectangleF worldSpace)
        {
            listPool = new PoolNew<CollisionList>(typeof(CollisionList), 1000);
            currentCollisions = new Dictionary<Guid, CollisionList>();
            currentCollisionsList = new List<IBoundingBox>(128);
            tempList = new List<ICollisionComponent>();
            quadTreeReturnList = new Dictionary<Guid, IBoundingBox>(64);
            movedLastFrame = new Dictionary<Guid, ICollisionComponent>(512);
            onCollisionEventListeners = new Dictionary<Guid, OnCollisionHandler>();
            onCollisionStoppedEventListeners = new Dictionary<Guid, OnCollisionStoppedHandler>();

            collisionTree = new QuadTree.QuadTree<ICollisionComponent>(new RectangleF(worldSpace.X, worldSpace.Y, worldSpace.Width, worldSpace.Height), 4, 2);
        }

        #endregion

        public override void Initialize()
        {
            var gameObjectManager = GameServiceManager.GetService<IGameObjectManager>();

            if (gameObjectManager != null)
            {
                gameObjectManager.OnGameObjectAdded += OnGameObjectAdded;
                gameObjectManager.OnGameObjectRemoved += OnGameObjectRemoved;
            }

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
            var collisionComp = sender.GetComponent<ICollisionComponent>();

            if (Active && collisionComp != null)
            {
                //Add this component to the list of components that have moved this frame.
                if (!movedLastFrame.ContainsKey(sender.GUID))
                {
                    movedLastFrame.Add(sender.GUID, collisionComp);
                }
            }
        }

        private void OnGameObjectAdded(IGameObject newObject)
        {
            if (newObject.GetComponent<ICollisionComponent>() != null)
                RegisterObject(newObject);
        }

        private void OnGameObjectRemoved(IGameObject gameObject)
        {
            var collisionComponent = gameObject.GetComponent<ICollisionComponent>();

            if (collisionComponent != null)
                collisionTree.Remove(collisionComponent);

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
        }

        private void RegisterObject(IGameObject theObject)
        {
            //Link ourselves to the OnMove event for the game object itself, and the OnDestroyed event for the collision component.
            theObject.OnMove += OnMove;

            var collisionComponent = theObject.GetComponent<ICollisionComponent>();

            collisionTree.Insert(collisionComponent);

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
            if (source == null || ((IBaseComponent) source).Parent == null || collider == null || ((IBaseComponent) collider).Parent == null)
                return;

            var sourceGUID = ((IBaseComponent) source).Parent.GUID;
            var colliderGUID = ((IBaseComponent) collider).Parent.GUID;

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
            if (source == null || ((IBaseComponent) source).Parent == null || previousCollider == null || ((IBaseComponent) previousCollider).Parent == null)
                return;

            var sourceGUID = ((IBaseComponent) source).Parent.GUID;
            var colliderGUID = ((IBaseComponent) previousCollider).Parent.GUID;

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
            foreach (var collisionComponent in movedLastFrame.Values)
            {
                collisionTree.ItemMoved(collisionComponent, 
                                        new RectangleF(((IBaseComponent) collisionComponent).Parent.LastFramePosition.X,
                                                       ((IBaseComponent) collisionComponent).Parent.LastFramePosition.Y,
                                                       collisionComponent.BoundingBox2D.Width,
                                                       collisionComponent.BoundingBox2D.Height));
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
                quadTreeReturnList.Clear();
                collisionTree.GetCollidingWith(moved.Value, ref quadTreeReturnList);
                quadTreeReturnList.Remove(moved.Value.Source.GUID);

                currentCollisionsList.Clear();

                foreach (var item in quadTreeReturnList.Values)
                {
                    currentCollisionsList.Add(item);
                }

                CullLayers(moved.Value, currentCollisionsList);
                
                //Add any *new* collisions to the lists for each entity. 
                foreach (var collision in currentCollisionsList)
                {
                    var collisionComponent = collision.Source.GetComponent<ICollisionComponent>();

                    if (currentCollisions.ContainsKey(moved.Key) && !currentCollisions[moved.Key].Contains(collisionComponent))
                    {
                        currentCollisions[moved.Key].Add(collisionComponent);
                        NotifyCollision(moved.Value, collisionComponent);
                    }

                    if (currentCollisions.ContainsKey(collision.Source.GUID) && !currentCollisions[collision.Source.GUID].Contains(moved.Value))
                    {
                        currentCollisions[collision.Source.GUID].Add(moved.Value);
                        NotifyCollision(collisionComponent, moved.Value);
                    }
                }  
            }
        }

        protected void CullLayers(ICollisionComponent moved, List<IBoundingBox> list)
        {
            for (int i = list.Count - 1; i >= 0 && i < list.Count; i--)
            {
                if (layerManager != null && !layerManager.LayersInteract(moved.Parent.Layer, list[i].Source.Layer))
                {
                    list.RemoveRange(i, 1);
                }
            }
        }
    }
}
