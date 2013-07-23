using System.Collections.Generic;
using IcicleFramework.Components.Collision;

namespace IcicleFramework.Collision
{
    public class QuadTreeOld<T> where T : ICollisionComponent 
    {

        #region Internal Variables

        /// <summary>
        /// Root node of this QuadTree.
        /// </summary>
        private QuadNode<T> root;

        /// <summary>
        /// The maximum depth of the QuadTree.
        /// </summary>
        private int maxDepth;

        #endregion


        #region Properties

        /// <summary>
        /// Gets the root QuadNode for this this QuadTree.
        /// </summary>
        public QuadNode<T> Root
        {
            get { return root; }
        }

        #endregion


        #region Constructor

        /// <summary>
        /// Creates a new QuadTree covering the given area.
        /// </summary>
        /// <param name="maxDepth">The maximum depth of the QuadTree.</param>
        /// <param name="worldSpace">The world-space area covered by this QuadTree.</param>
        public QuadTreeOld(int maxDepth, RectangleF worldSpace)
        {
            root = new QuadNode<T>(worldSpace, null, false, 0, maxDepth);
            this.maxDepth = maxDepth;
        }

        #endregion


        #region Adding Objects

        /// <summary>
        /// Adds an CollidableGameObject Object to the QuadTree.
        /// </summary>
        /// <param name="collidableObject">The T-type Object to add to the QuadTree.</param>
        /// <returns>True if the CollidableGameObject Object was added, false if otherwise.</returns>
        public bool AddObject(T collidableObject)
        {
            return root.Insert(collidableObject);
        }

        /// <summary>
        /// Removes the given T-type Object from the QuadTree.
        /// </summary>
        /// <param name="collidableObject">The T-type Object to remove from the QuadTree.</param>
        /// <returns>True if the T-type Object was removed, false if otherwise.</returns>
        public bool RemoveObject(T collidableObject)
        {
            return root.RemoveItem(collidableObject);
        }

        /// <summary>
        /// Assumes the given object has moved and relocated the object to the correct position.
        /// </summary>
        /// <param name="collidableObject">The T-type object that has moved.</param>
        /// <returns>True if the object was moved in the QuadTree, false if it did not have to be.</returns>
        public bool ObjectMoved(T collidableObject)
        {
            return root.MoveItem(collidableObject);
        }


        #endregion


        #region Collision Testing

        /// <summary>
        /// Tests for collisions against a given T object and adds to the list of T objects current colliding.
        /// </summary>
        /// <param name="item">The T object to test for collisions against.</param>
        /// <param name="collidingObjects">The T objects currently colliding against the provided T object.</param>
        public void GetCollidingWith(T item, ref List<T> collidingObjects)
        {
            if (root != null && collidingObjects != null)
            {
                root.GetCollidingWith(item, ref collidingObjects);
                collidingObjects.Remove(item);
            }
        }

        #endregion

    }
}
