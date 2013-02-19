using System.Collections.Generic;
using IcicleFramework.Components.Collision;
using Microsoft.Xna.Framework.Graphics;

namespace IcicleFramework.Collision
{
    /// <summary>
    /// Represents a single Node in a QuadTree.
    /// Non-leaf nodes have 4 children QuadNodes.
    /// </summary>
    public class QuadNode<T> where T : ICollisionComponent
    {
        #region Internal Variables
        
        /// <summary>
        /// Children QuadNodes covering small areas inside of this QuadNode.
        /// </summary>
        private QuadNode<T>[] children;

        /// <summary>
        /// List of T objects managed by this QuadNode.
        /// </summary>
        private List<T> collidableObjects;

        #endregion


        #region Properties

        /// <summary>
        /// Gets the children of this QuadNode.
        /// </summary>
        public QuadNode<T>[] Children
        {
            get 
            {
                if (!IsLeaf)
                    return children;
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets the Rectangle that represents the world-space covered by this QuadNode.
        /// </summary>
        public RectangleF BoundingBox
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the list of T objects in this QuadNode.
        /// </summary>
        public List<T> PlayerObjects
        {
            get { return collidableObjects; }
        }

        /// <summary>
        /// Gets the Parent QuadNode for this QuadNode.
        /// </summary>
        public QuadNode<T> Parent
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets whether or not this QuadNode (and/or its children) has any items stored within it.
        /// </summary>
        public bool HasItems 
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets whether or not this QuadNode is a leaf node in the QuadTree.
        /// </summary>
        public bool IsLeaf
        {
            get;
            private set;
        }

        /// <summary>
        /// Depth of this QuadNode within the QuadTree.
        /// </summary>
        public int MyDepth
        {
            get;
            private set;
        }

        #endregion


        #region Constructor

        /// <summary>
        /// Creates a new QuadNode and child nodes if this QuadNode is not a leaf.
        /// </summary>
        /// <param name="boundingBox">The Rectangle defining the world-space coverage area of this QuadNode.</param>
        /// <param name="parent">The QuadNode parent of this QuadNode.</param>
        /// <param name="isLeaf">Whether or not this QuadNode is a leaf node in the QuadTree.</param>
        /// <param name="myDepth">The depth of this QuadNode.</param>
        /// <param name="maxDepth">The maximum depth that the QuadTree supports.</param>
        public QuadNode(RectangleF boundingBox, QuadNode<T> parent, bool isLeaf, int myDepth, int maxDepth)
        {
            BoundingBox = boundingBox;
            Parent = parent;
            IsLeaf = isLeaf;
            HasItems = false;
            MyDepth = myDepth;

            collidableObjects = new List<T>();

            //Construct the children QuadNodes if this QuadNode is not a leaf.
            if (!isLeaf)
            {
                //Shouldn't have to worry about this...but we might as well play it safe.
                //If the depth of this QuadNode is the max depth then simply mark it as a leaf node and be happy.
                if (myDepth == maxDepth)
                    IsLeaf = true;
                else
                {
                    int newDepth = myDepth + 1;
                    float halfWidth = BoundingBox.Width / 2;
                    float halfHeight = BoundingBox.Height / 2;
                    bool areLeaves = false;

                    children = new QuadNode<T>[4];

                    if (newDepth == maxDepth)
                        areLeaves = true;

                    //Create the 4 child QuadNodes, each controlling a distinct quadrant of this QuadNode's Bounding Box.
                    children[0] = new QuadNode<T>(new RectangleF(BoundingBox.X, BoundingBox.Y, halfWidth, halfHeight),
                                               this, areLeaves, newDepth, maxDepth);   //Top-Left
                    children[1] = new QuadNode<T>(new RectangleF(BoundingBox.X + halfWidth, BoundingBox.Y, halfWidth, halfHeight),
                                               this, areLeaves, newDepth, maxDepth);  //Top-Right
                    children[2] = new QuadNode<T>(new RectangleF(BoundingBox.X, BoundingBox.Y + halfHeight, halfWidth, halfHeight),
                                               this, areLeaves, newDepth, maxDepth);  //Bottom-Left
                    children[3] = new QuadNode<T>(new RectangleF(BoundingBox.X + halfWidth, BoundingBox.Y + halfHeight, halfWidth, halfHeight),
                                               this, areLeaves, newDepth, maxDepth);  //Bottom-Right
                }
            }
        }

        #endregion


        #region Adding Objects

        /// <summary>
        /// Inserts an ICollidable object to this QuadNode, or one of its children.
        /// </summary>
        /// <param name="item">The CollidableGameObject to add to this QuadNode.</param>
        /// <returns>True if the CollidableGameObject was added, false if otherwise.</returns>
        public bool Insert(T item)
        {
            bool added;

            //Check if the object can fit entirely within the bounding box of any of our children.
            //If so, ask the child to add it. 
            added = InsertInChild(item);

            //If we could not add it to any of our children, then add it to ourself.
            if (!added)
                collidableObjects.Add(item);

            return added;
        }

        /// <summary>
        /// Inserts an item into one of this node's children
        /// </summary>
        /// <param name="item">The CollidableGameObject to insert in a child.</param>
        /// <returns>Whether or not the insert succeeded</returns>
        protected bool InsertInChild(T item)
        {
            if (IsLeaf) return false;

            if (children[0].BoundingBox.Contains(item.BoundingBox2D.Box))
                children[0].Insert(item);
            else if (children[1].BoundingBox.Contains(item.BoundingBox2D.Box))
                children[1].Insert(item);
            else if (children[2].BoundingBox.Contains(item.BoundingBox2D.Box))
                children[2].Insert(item);
            else if (children[3].BoundingBox.Contains(item.BoundingBox2D.Box))
                children[3].Insert(item);
            else return false; //Could not insert into one of our children.

            return true;
        }

        /// <summary>
        /// Removes the given CollidableGameObject from this or one of its children.
        /// </summary>
        /// <param name="item">The CollidiableGameObject to remove from this node.</param>
        /// <returns>True if the CollidableGameObject was removed, false if otherwise.</returns>
        public bool RemoveItem(T item)
        {
            bool removed = false;

            if (!IsLeaf && children[0].BoundingBox.Contains(item.BoundingBox2D.Box))
                removed = children[0].RemoveItem(item);
            else if (!IsLeaf && children[1].BoundingBox.Contains(item.BoundingBox2D.Box))
                removed = children[1].RemoveItem(item);
            else if (!IsLeaf && children[2].BoundingBox.Contains(item.BoundingBox2D.Box))
                removed = children[2].RemoveItem(item);
            else if (!IsLeaf && children[3].BoundingBox.Contains(item.BoundingBox2D.Box))
                removed = children[3].RemoveItem(item);
            else //Remove the item from ourself
            {
                removed = collidableObjects.Remove(item);
            }

            return removed;
        }

        /// <summary>
        /// Pushes an item down to one of this node's children
        /// </summary>
        /// <param name="item">The object to push down.</param>
        /// <returns>True if the CollidableGameObject could be pushed down.</returns>
        protected bool PushItemDown(T item)
        {
            if (InsertInChild(item))
            {
                RemoveItem(item);
                return true;
            }
            else 
                return false;
        }

        /// <summary>
        /// Pushes an item down to one of this node's children
        /// </summary>
        /// <param name="itemNum">The index of the CollidableGameObject to push down.</param>
        /// <returns>True if the CollidableGameObject could be pushed down.</returns>
        protected bool PushItemDown(int itemNum)
        {
            if (itemNum < collidableObjects.Count)
            {
                if (InsertInChild(collidableObjects[itemNum]))
                {
                    RemoveItem(collidableObjects[itemNum]);
                    return true;
                }
                else return false;
            }

            else return false;
        }

        /// <summary>
        /// Push an item up to this node's parent
        /// </summary>
        /// <param name="item">The CollidableGameObject to push up.</param>
        protected void PushItemUp(T item)
        {
            RemoveItem(item);
            Parent.Insert(item);
        }

        /// <summary>
        /// Push an item up to this node's parent
        /// </summary>
        /// <param name="itemNum">The index of the CollidableGameObject to push up.</param>
        protected void PushItemUp(int itemNum)
        {
            if (itemNum < collidableObjects.Count)
            {
                T theObject = collidableObjects[itemNum];

                RemoveItem(theObject);
                Parent.Insert(theObject);
            }
        }


        #endregion

        /// <summary>
        /// Handles item movement
        /// </summary>
        /// <param name="item">The item that moved</param>
        public bool MoveItem(T item)
        {
            bool done = false;
            int loc = collidableObjects.IndexOf(item);

            if (loc >= 0)
            {
                //Try to push the item down to a child
                if (!PushItemDown(loc))
                {
                    //Otherwise, try to push up if we're not the root.
                    if (Parent != null)
                        PushItemUp(loc);
                }

                done = true;
            }
            else if (children != null) //Check if any of our children have this object
            {
                for (int i = 0; i < children.Length && !done; i++)
                    done = children[i].MoveItem(item);
            }

            return done;
        }

        /// <summary>
        /// Adds to the list of CollidableGameObjects colliding with the given item.
        /// </summary>
        public void GetCollidingWith(T item, ref List<T> objectList)
        {
            int i;
            bool contained = false;

            //Check what objects are being collided with in this QuadNode.
            if (BoundingBox.Intersects(item.BoundingBox2D.Box))
            {
                contained = true;

                for (i = 0; i < collidableObjects.Count; i++)
                    if (item.BoundingBox2D.Intersects(collidableObjects[i].BoundingBox2D))
                        objectList.Add(collidableObjects[i]);
            }

            //Ask any of our children to check as well if their bounding areas cover any portion of the object.
            if (!IsLeaf && contained)
            {
                for (i = 0; i < children.Length; i++)
                {
                    if (children[i].BoundingBox.Intersects(item.BoundingBox2D.Box))
                        children[i].GetCollidingWith(item, ref objectList);
                }
            }
        }

        #region Drawing

        /// <summary>
        /// Draws this QuadNode's borders to the screen.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
        }        

        #endregion
    }
}
