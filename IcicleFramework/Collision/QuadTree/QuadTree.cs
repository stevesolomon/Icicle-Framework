using System;
using System.Collections.Generic;

namespace IcicleFramework.Collision.QuadTree
{
    public class QuadTree<T> where T : IBoundingBox
    {
        protected QuadNode<IBoundingBox> RootNode { get; set; }

        /// <summary>
        /// The area of the world covered by this QuadTree
        /// </summary>
        protected RectangleF BoundingArea { get; set; }

        public QuadTree(RectangleF boundingArea, int maxDepth, int maxItemsPerNode)
        {
            BoundingArea = boundingArea;
            RootNode = new QuadNode<IBoundingBox>(BoundingArea, null, 0, maxDepth, maxItemsPerNode, true);
        }

        public bool Insert(T item)
        {
            return RootNode.Insert(item);
        }

        public bool Remove(T item)
        {
            return RootNode.Remove(item);
        }

        public void ItemMoved(T item, RectangleF previousBoundingRectangle)
        {
            RootNode.ItemMoved(item, previousBoundingRectangle);
        }

        public void GetCollidingWith(T item, ref Dictionary<Guid, IBoundingBox> collidingEntities)
        {
            RootNode.GetCollidingWith(item, ref collidingEntities);
        }
    }
}
