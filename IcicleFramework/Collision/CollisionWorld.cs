using System;
using System.Collections.Generic;
using IcicleFramework.Components;
using IcicleFramework.Components.Collision;
using IcicleFramework.Entities;
using IcicleFramework.GameServices;
using IcicleFramework.GameServices.Factories;
using Microsoft.Xna.Framework;

namespace IcicleFramework.Collision
{
    
    #region MoveResponse Structure

    /// <summary>
    /// Lightweight structure containing responses when a CollidableGameObject requests the ability to move.
    /// </summary>
    public struct MoveResponse
    {
        public bool moved;
        public bool collided;
        public Vector2 finalPosition;
        public int damage;

        public MoveResponse(bool moved)
        {
            this.moved = moved;
            collided = false;
            finalPosition = Vector2.Zero;
            damage = 0;
        }
    }

    #endregion


    #region CorrectionVector2 Structure

    /// <summary>
    /// A structure for managing the correction required in the event of collisions.
    /// </summary>
    public struct CorrectionVector2
    {
        public DirectionX DirectionX;
        public DirectionY DirectionY;
        public float X;
        public float Y;
    }

    #endregion


    #region ResolvedCollision Structure 

    /// <summary>
    /// A structure for managing an object that has already been considered in collision resolution.
    /// </summary>
    public struct ResolvedCollision
    {
        public ICollision collisionObject;
        public int axis;
    }

    #endregion


    #region Enumerations

    /// <summary>
    /// Enumeration containing all possible X-axis push out directions.
    /// </summary>
    public enum DirectionX
    {
        Left = -1,
        None = 0,
        Right = 1
    }

    /// <summary>
    /// Enumeration containing all possible Y-axis push out directions.
    /// </summary>
    public enum DirectionY
    {
        Up = -1,
        None = 0,
        Down = 1
    }
    
    #endregion

    


    /// <summary>
    /// Implements a simple collision/physics managements system.
    /// </summary>
    public class CollisionWorld : GameService, ICollisionManager
    {

        private const int XAXIS = 0;
        private const int YAXIS = 1;

        protected Dictionary<Guid, CollisionHandler> collisionEvents;

        #region Internal Variables

        /// <summary>
        /// Temporary list for object collisions that stays in memory to improve performance.
        /// </summary>
        private List<ICollisionComponent> tempCollisionList;

        /// <summary>
        /// Temporary list for object collisions above the priority of some source object.
        /// </summary>
        private List<ICollisionComponent> higherPriorityCollisionList;

        /// <summary>
        /// Temporary list for object collisions below the priority of some source object.
        /// </summary>
        private List<ICollisionComponent> lowerPriorityCollisionList;

        /// <summary>
        /// Temporary list for resolved collisions against objects for some source object.
        /// </summary>
        private Dictionary<ICollisionComponent, int> resolvedCollisions;

        /// <summary>
        /// Temporary list of correction vectors that stays in memory to improve performance.
        /// </summary>
        private List<CorrectionVector2> correctionVectors;

        private CorrectionVector2 finalCorrectionVector;

        private QuadTreeOld<ICollisionComponent> collisionTree;

        #endregion

        public bool Active { get; protected set; }

        public bool ResolveCollisions { get; set; }


        #region Constructors

        /// <summary>
        /// Creates a new CollisionWorld covering the provided world space.
        /// </summary>
        /// <param name="worldSpace">The total world space covered by this CollisionWorld.</param>
        public CollisionWorld(RectangleF worldSpace)
        {
            correctionVectors = new List<CorrectionVector2>(8);

            collisionTree = new QuadTreeOld<ICollisionComponent>(4, worldSpace);

            collisionEvents = new Dictionary<Guid, CollisionHandler>();

            tempCollisionList = new List<ICollisionComponent>();
            higherPriorityCollisionList = new List<ICollisionComponent>();
            lowerPriorityCollisionList = new List<ICollisionComponent>();
            resolvedCollisions = new Dictionary<ICollisionComponent, int>();
        }

        #endregion

        public override void Initialize()
        {
            var gameObjectManager = GameServiceManager.GetService<IGameObjectManager>();

            if (gameObjectManager != null)
                gameObjectManager.OnGameObjectAdded += OnGameObjectAdded;

            ResumeSimulation();
        }

        public void ResumeSimulation()
        {
            Active = true;
        }

        public void PauseSimulation()
        {
            Active = false;
        }


        public bool SubscribeCollisionEvent(Guid GUID, CollisionHandler handler)
        {
            bool subscribed = false;

            if (collisionEvents.ContainsKey(GUID))
            {
                collisionEvents[GUID] += handler;
                subscribed = true;
            }

            return subscribed;
        }

        public bool SubscribeCollisionEvent(Guid GUID, OnCollisionHandler handler)
        {
            return true;
        }

        public void UnsubscribeCollisionEvent(Guid GUID, OnCollisionHandler handler)
        {
            throw new NotImplementedException();
        }

        public bool SubscribeCollisionStoppedEvent(Guid GUID, OnCollisionStoppedHandler handler)
        {
            throw new NotImplementedException();
        }

        public void UnsubscribeCollisionStoppedEvent(Guid GUID, OnCollisionStoppedHandler handler)
        {
            throw new NotImplementedException();
        }

        #region Event Handlers

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
                collisionTree.ObjectMoved(collisionComp);
                RequestMove(collisionComp);
            }
        }

        private void OnGameObjectAdded(IGameObject newObject)
        {
            if (newObject.GetComponent<ICollisionComponent>() != null)
                RegisterObject(newObject);
        }

        private void OnGameObjectDestroyed(IGameObject gameObject)
        {
            ICollisionComponent collisionComponent = gameObject.GetComponent<ICollisionComponent>();

            if (collisionComponent != null)
                collisionTree.RemoveObject(collisionComponent);

            if (collisionEvents.ContainsKey(gameObject.GUID))
                collisionEvents.Remove(gameObject.GUID);

            gameObject.OnMove -= OnMove;
            gameObject.OnDestroyed -= OnGameObjectDestroyed;
        }

        #endregion

        
        #region Adding new objects

        private void RegisterObject(IGameObject theObject)
        {
            //Link ourselves to the OnMove and OnDestroyed events
            theObject.OnMove += OnMove;
            theObject.OnDestroyed += OnGameObjectDestroyed;

            collisionTree.AddObject(theObject.GetComponent<ICollisionComponent>());
            collisionEvents.Add(theObject.GUID, null);
        }

        #endregion


        #region Checking and handling collisions

        /// <summary>
        /// Requests that the CollidableGameObject be moved to the location where its ProjectedBoundingBox currently resides.
        /// </summary>
        /// <param name="objectToMove">The IGameObject to be moved.</param>
        /// <returns>A MoveResponse object providing information related to the movement.</returns>
        protected MoveResponse RequestMove(ICollisionComponent objectToMove)
        {
            MoveResponse response = new MoveResponse(false);            

            HandleObjectMovement(objectToMove);

            return response;
        }

        #region Push Out Collision Response

        /// <summary>
        /// Gets the correction vector to resolve a collision between two ICollidable objects.
        /// </summary>
        /// <param name="A">The ICollidable object we want to push out.</param>
        /// <param name="B">The ICollidable object we resolve the collision against.</param>
        /// <returns>A CorrectionVector2 that details the movement required to resolve the collision.</returns>
        private CorrectionVector2 GetCorrectionVector(ICollisionComponent A, ICollisionComponent B)
        {
            return A.GetCorrectionVector(B);
        }

        /// <summary>
        /// Sums up the horizontal components of the correction vectors.
        /// </summary>
        /// <param name="correctionVectors">A list of CorrectionVector2's that will have their horizontal components summed.</param>
        /// <returns>The sum of the horizontal components.</returns>
        private int SumHorizontal(List<CorrectionVector2> correctionVectors)
        {
            int sum = 0;
            int i;

            for (i = 0; i < correctionVectors.Count; i++)
                sum += (int) correctionVectors[i].DirectionX;

            return sum;
        }

        /// <summary>
        /// Sums up the vertical components of the correction vectors.
        /// </summary>
        /// <param name="correctionVectors">A list of CorrectionVector2's that will have their vertical components summed.</param>
        /// <returns>The sum of the vertical components.</returns>
        private int SumVertical(List<CorrectionVector2> correctionVectors)
        {
            int sum = 0;
            int i;

            for (i = 0; i < correctionVectors.Count; i++)
                sum += (int)correctionVectors[i].DirectionY;

            return sum;
        }

        /// <summary>
        /// Returns a CorrectionVector2 containing the directions on the X and Y axis that the collision response is to be resolved.
        /// </summary>
        /// <param name="correctionVectors">A list of CorrectionVector2's, one for each of the collisions that need to be resolved.</param>
        /// <returns>A CorrectionVector2 with the DirectionX and DirectionY components set in the direction to perform the resolution.</returns>
        private CorrectionVector2 GetDirections(List<CorrectionVector2> correctionVectors)
        {
            CorrectionVector2 directions = new CorrectionVector2();
            int horizontalSum = SumHorizontal(correctionVectors);
            int verticalSum = SumVertical(correctionVectors);

            DirectionX dirX = DirectionX.None;
            DirectionY dirY = DirectionY.None;

            if (horizontalSum <= (int) DirectionX.Left)
                dirX = DirectionX.Left;
            else if (horizontalSum >= (int) DirectionX.Right)
                dirX = DirectionX.Right;
            else
                dirX = DirectionX.None; //If they cancel each other out, i.e 2 Left and 2 Right
            
            if (verticalSum <= (int) DirectionY.Up)
                dirY = DirectionY.Up;
            else if (verticalSum >= (int) DirectionY.Down)
                dirY = DirectionY.Down;
            else
                dirY = DirectionY.None; //If they cancel each other out, i.e 1 Up and 1 Down

            directions.DirectionX = dirX;
            directions.DirectionY = dirY;

            //If both directions are None, then push out along the smallest for each
            if (directions.DirectionX == DirectionX.None && directions.DirectionY == DirectionY.None)
            {
                float smallestX = float.MaxValue;
                float smallestY = float.MaxValue;

                //Find the smallest X-axis and Y-axis correction...
                for (int i = 0; i < correctionVectors.Count; i++)
                {
                    if (correctionVectors[i].X < smallestX)
                    {
                        smallestX = correctionVectors[i].X;
                        directions.DirectionX = correctionVectors[i].DirectionX;
                    }

                    if (correctionVectors[i].Y < smallestY)
                    {
                        smallestY = correctionVectors[i].Y;
                        directions.DirectionY = correctionVectors[i].DirectionY;
                    }
                }
            }

            return directions;
        }

        /// <summary>
        /// Checks if an ICollidable object is colliding against a list of other ICollidable objects.
        /// </summary>
        /// <param name="item">The ICollidable object we want to test for collisions.</param>
        /// <param name="collisionList">The list of ICollidable objects to test for collisions against.</param>
        /// <returns>True if there are collisions, false if otherwise.</returns>
        private bool CheckCollisions(ICollisionComponent item, List<ICollisionComponent> collisionList)
        {
            int i;

            for (i = 0; i < collisionList.Count; i++)
            {
                if (item.BoundingBox2D.Intersects(collisionList[i].BoundingBox2D))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Corrects the movement of the ICollidable object and places it in a position where it will no longer be colliding
        /// with any of the ICollidable objects in the provided list.
        /// </summary>
        /// <param name="item">The ICollidable object to move/position to resolve collisions.</param>
        /// <param name="collisionList">The list of ICollidable objects we no longer want to be colliding against</param>
        /// <param name="corrections">The list of CorrectionVector2's detailing how to resolve each individual collision.</param>
        private void CorrectMovement(ICollisionComponent item, List<ICollisionComponent> collisionList, CorrectionVector2 corrections)
        {
            CorrectionVector2 smallestCorrectionX = GetSmallestCorrectionX(corrections.DirectionX, correctionVectors);
            CorrectionVector2 smallestCorrectionY = GetSmallestCorrectionY(corrections.DirectionY, correctionVectors);

            int fixedAxis = 0;
            bool found = false;
            int foundIndex = 0;

            for (int i = 0; i < collisionList.Count && !found; i++)
            {
                if (resolvedCollisions.ContainsKey(collisionList[i]))
                {
                    found = true;
                    foundIndex = i;
                }
            }

            if (!found) 
            {
                if (smallestCorrectionX.X > smallestCorrectionY.Y) //Then start correcting Y first
                {
                    CorrectCollision(item, smallestCorrectionY, false);

                    fixedAxis = YAXIS;

 //                   if (CheckCollisions(item, collisionList))
   //                 {
     //                   CorrectCollision(item, smallestCorrectionX, true);
       //                 fixedAxis = BOTHAXIS;
         //           }

                }
                else
                {
                    CorrectCollision(item, smallestCorrectionX, true);

                    fixedAxis = XAXIS;

 //                  if (CheckCollisions(item, collisionList))
   //                 {
     //                   CorrectCollision(item, smallestCorrectionY, false);
       //                 fixedAxis = BOTHAXIS;
         //           }
                }
            }
            else //We've already used this before, so do the opposite!
            {
                if (resolvedCollisions[collisionList[foundIndex]] == XAXIS)
                {
                    CorrectCollision(item, smallestCorrectionY, false);
                }
                else if (resolvedCollisions[collisionList[foundIndex]] == YAXIS)
                {
                    CorrectCollision(item, smallestCorrectionX, true);
                }
                else
                {
                    CorrectCollision(item, smallestCorrectionY, false);
                    CorrectCollision(item, smallestCorrectionX, true);
                }
            }


            for (int i = 0; i < collisionList.Count; i++)
            {
                if (!resolvedCollisions.ContainsKey(collisionList[i]))
                    resolvedCollisions.Add(collisionList[i], fixedAxis);
            }
        }

        /// <summary>
        /// Performs the actual movement of the ICollidable object's position in order to resolve a collision.
        /// </summary>
        /// <param name="item">The ICollidable object to move.</param>
        /// <param name="correction">A CorrectionVector2 detailing the X and Y axis movements required to resolve the collisions.</param>
        /// <param name="correctHorizontal">True if we are resolving along the X axis, false if we are resolving along the Y axis.</param>
        private void CorrectCollision(ICollisionComponent item, CorrectionVector2 correction, bool correctHorizontal)
        {
            Vector2 newVector = ((IBaseComponent) item).Parent.Position;
            //Vector2 newVelocity;

            //Vector2 originalPosition = item.Parent.Position;
           // Vector2 originalVelocity = item.Parent.Velocity;

            finalCorrectionVector = correction;

            if (correctHorizontal)
            {
                //newVelocity = new Vector2(0f, item.Parent.Velocity.Y);
                newVector.X += correction.X * (int)correction.DirectionX;
            }
            else
            {
                //newVelocity = new Vector2(item.Parent.Velocity.X, 0f);
                newVector.Y += correction.Y*(int) correction.DirectionY;
            }

            ((IBaseComponent) item).Parent.Position = newVector;
            //item.Parent.Velocity = newVelocity;

            item.BoundingBox2D.Position = newVector;

            collisionTree.ObjectMoved(item);

            /*
            //Check if we're still colliding with anything else...
            tempCollisionList.Clear();
            collisionTree.GetCollidingWith(item, ref tempCollisionList);

            //If we are colliding with new items, then correct along the other axis instead
            if (tempCollisionList.Count > 0)
            {
                newVector = originalPosition;

                if (correctHorizontal)
                {
                    newVelocity = new Vector2(originalVelocity.X, 0f);
                    newVector.Y += correction.Y * (int)correction.DirectionY;
                }
                else
                {
                    newVelocity = new Vector2(0f, originalVelocity.Y);
                    newVector.X += correction.X * (int)correction.DirectionX;
                }

                item.Position = newVector;
                item.Velocity = newVelocity;
                collisionTree.ObjectMoved(item);
            }
             * */
        }

        /// <summary>
        /// Gets the smallest correction required to resolve collisions along the X axis.
        /// </summary>
        /// <param name="directionX">The direction along the X axis to resolve.</param>
        /// <param name="correctionVectors">A list of CollisionVector2's detailing the resolutions required for all collisions that are occurring.</param>
        /// <returns>A CorrectionVector2 containing the smallest correction required.</returns>
        private CorrectionVector2 GetSmallestCorrectionX(DirectionX directionX, List<CorrectionVector2> correctionVectors)
        {
            CorrectionVector2 smallest = new CorrectionVector2();
            int i;
            
            smallest.X = int.MaxValue;

            for (i = 0; i < correctionVectors.Count; i++)
            {
                if (correctionVectors[i].DirectionX == directionX && correctionVectors[i].X != 0f && correctionVectors[i].X < smallest.X)
                    smallest = correctionVectors[i];
            }

            return smallest;
        }

        /// <summary>
        /// Gets the smallest correction required to resolve collisions along the Y axis.
        /// </summary>
        /// <param name="directionY">The direction along the Y axis to resolve.</param>
        /// <param name="correctionVectors">A list of CollisionVector2's detailing the resolutions required for all collisions that are occurring.</param>
        /// <returns>A CorrectionVector2 containing the smallest correction required.</returns>
        private CorrectionVector2 GetSmallestCorrectionY(DirectionY directionY, List<CorrectionVector2> correctionVectors)
        {
            CorrectionVector2 smallest = new CorrectionVector2();
            int i;

            smallest.Y = int.MaxValue;

            for (i = 0; i < correctionVectors.Count; i++)
            {
                if (correctionVectors[i].DirectionY == directionY && correctionVectors[i].Y != 0f && correctionVectors[i].Y < smallest.Y)
                    smallest = correctionVectors[i];
            }

            return smallest;
        }

        #endregion

        #endregion      
  
        private void NotifyCollision(ICollisionComponent source, ICollisionComponent collider, CorrectionVector2 collisionInfo)
        {
            var sourceGUID = ((IBaseComponent) source).Parent.GUID;
            var colliderGUID = ((IBaseComponent) collider).Parent.GUID;

            //Notify any listeners of the source that it has collided with something
            if (collisionEvents.ContainsKey(sourceGUID) && collisionEvents[sourceGUID] != null)
            {
                collisionEvents[sourceGUID](source, collider, collisionInfo);
            }

            //Notify any listeners of the collider that is has collided with something
            if (collisionEvents.ContainsKey(colliderGUID) && collisionEvents[colliderGUID] != null)
            {
                collisionEvents[colliderGUID](collider, source, collisionInfo);
            }
        }

        protected MoveResponse ResolveAllCollisions(ICollisionComponent source)
        {
            MoveResponse response = new MoveResponse();

            //As long as at least one collision occurred attempt to resolve it.
            //And, of course, only resolve collisions if we're supposed to!
            if (higherPriorityCollisionList.Count > 0)
            {
                correctionVectors.Clear();
                
                for (int i = 0; i < tempCollisionList.Count; i++)
                {
                    var vector = GetCorrectionVector(source, tempCollisionList[i]);
                    correctionVectors.Add(vector);

                    //Notify of a collision
                    NotifyCollision(source, tempCollisionList[i], vector);
                }

                //Now determine the direction for the X and Y axis.
                CorrectionVector2 directions = GetDirections(correctionVectors);

                //Correct the movement of this object if we're supposed to
                if (ResolveCollisions)
                {
                    CorrectMovement(source, tempCollisionList, directions);

                    //Mark this object as having moved and put it back into the QuadTree.
                    response.moved = true;
                }
            }

            return response;
        }

        protected void ResolveIndividualCollision(ICollisionComponent source, ICollisionComponent toMove)
        {
            List<ICollisionComponent> temp = new List<ICollisionComponent>();
            correctionVectors.Clear();

            temp.Add(source);

            var vector = GetCorrectionVector(toMove, source);
            correctionVectors.Add(vector);

            //Now determine the direction for the X and Y axis.
            CorrectionVector2 directions = GetDirections(correctionVectors);

            NotifyCollision(source, toMove, vector);

            //Correct the movement of this object if we're supposed to!
            if (ResolveCollisions)
            {
                CorrectMovement(toMove, temp, directions);
            }
        }


        /// <summary>
        /// Resolves all collisions against the source ICollidable object.
        /// </summary>
        /// <param name="source">The source ICollidable object that has moved and requires collision testing.</param>
        protected MoveResponse HandleObjectMovement(ICollisionComponent source)
        {
            tempCollisionList.Clear();
            higherPriorityCollisionList.Clear();
            lowerPriorityCollisionList.Clear();
            resolvedCollisions.Clear();

            finalCorrectionVector = new CorrectionVector2();

            MoveResponse response = new MoveResponse();

            collisionTree.GetCollidingWith(source, ref tempCollisionList);

            while (tempCollisionList.Count > 0)
            {
                if (source.Solid)
                {
                    for (int i = 0; i < tempCollisionList.Count; i++)
                    {
                        ICollisionComponent currObject = tempCollisionList[i];

                        //Check if the object we are colliding with has a priority greater than ours.
                        //If so, then add it to the list of collisions we need to resolve!
                        //We will also consider objects with the same priority so long as both of use are flagged as being able to collide with 
                        //objects of the same priority.
                        if (currObject.Solid)
                        {
                            if (currObject.CollisionPriority > source.CollisionPriority ||
                                (
                                 (currObject.CollisionPriority == source.CollisionPriority)))
                            {
                                higherPriorityCollisionList.Add(currObject);
                            }

                                //If we've collided with anything of lower priority then we need to push IT out of the way.
                            else if (currObject.CollisionPriority < source.CollisionPriority)
                            {
                                lowerPriorityCollisionList.Add(currObject);
                            }
                        }
                    }
                }

                //Now let's resolve those collisions!
                ResolveAllCollisions(source);

                //Resolve all of the individual collisions that we are pushing away.
                for (int j = 0; j < lowerPriorityCollisionList.Count; j++)
                {
                    ResolveIndividualCollision(source, lowerPriorityCollisionList[j]);
                }

                //Notify all the ICollisionComponents that they have collided with something.
                //for (int j = 0; j < tempCollisionList.Count; j++)
                //{
                   // if (collisionEvents.ContainsKey(source.Parent.GUID) && collisionEvents[source.Parent.GUID] != null)
                   //     collisionEvents[source.Parent.GUID](source, tempCollisionList[j], finalCorrectionVector);

                   // if (collisionEvents.ContainsKey(tempCollisionList[j].Parent.GUID) && collisionEvents[tempCollisionList[j].Parent.GUID] != null)
                   //     collisionEvents[tempCollisionList[j].Parent.GUID](tempCollisionList[j], source, finalCorrectionVector);
               // }

                tempCollisionList.Clear();
                higherPriorityCollisionList.Clear();
                lowerPriorityCollisionList.Clear();

                //Only check again if we're resolving collisions!
                if (ResolveCollisions)
                {
                    collisionTree.GetCollidingWith(source, ref tempCollisionList);
                }
            }

            return response;
        }
    }
}
