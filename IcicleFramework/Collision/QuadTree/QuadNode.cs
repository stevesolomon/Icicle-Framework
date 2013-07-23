using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace IcicleFramework.Collision.QuadTree
{
    public class QuadNode<T> where T : IBoundingBox
    {
        protected QuadNode<T>[] children;

        protected QuadNode<T> parent; 

        protected List<T> entities; 

        protected int maximumEntities;

        protected bool isLeaf;

        protected RectangleF boundingBox;

        protected int currDepth;

        protected int maxDepth;

        public QuadNode(RectangleF boundingBox, QuadNode<T> parent, int currDepth, int maxDepth, int maximumEntities, bool isLeaf)
        {
            this.boundingBox = boundingBox;
            this.parent = parent;
            this.currDepth = currDepth;
            this.maxDepth = maxDepth;
            this.maximumEntities = maximumEntities;
            this.isLeaf = isLeaf;

            if (isLeaf)
                entities = new List<T>();
        }

        public bool Insert(T newItem)
        {
            var inserted = false;

            //If we are a leaf then insert into our list...
            if (isLeaf && boundingBox.Intersects(newItem.BoundingBox2D.Box))
            {
                //If we still have room for another...
                if (entities.Count < maximumEntities)
                {
                    entities.Add(newItem);
                    inserted = true;
                }
                //Otherwise, split this QuadNode and insert into our children.
                else
                {
                    Split();
                    inserted = InsertIntoChildren(newItem);
                }
            }
            else if (!isLeaf) //Just insert into our children as we are not a leaf node.
            {
                inserted = InsertIntoChildren(newItem);
            }

            return inserted;

        }

        protected bool InsertIntoChildren(T newItem)
        {
            var inserted = false;

            //Insert into each of our children so long as the newItem's bounding rectangle intersects the child quad node's bounding rectangle.
            foreach (var child in children)
            {
                if (child.boundingBox.Intersects(newItem.BoundingBox2D.Box))
                    inserted |= child.Insert(newItem);
            }

            return inserted;
        }

        public bool Remove(T item)
        {
            var removed = false;

            //If we are a leaf then just remove the item.
            if (isLeaf)
            {
                removed = entities.Remove(item);
            }
            else //Otherwise look through all of our children for the item.
            {
                foreach (var child in children)
                {
                    if (child.boundingBox.Intersects(item.BoundingBox2D.Box))
                        removed |= child.Remove(item);
                }
            }

            return removed;
        }

        private void Remove(T item, RectangleF previousBoundingRectangle)
        {
            if (isLeaf)
            {
                entities.Remove(item);
            }
            else
            {
                foreach (var child in children)
                {
                    if (child.boundingBox.Intersects(previousBoundingRectangle))
                        child.Remove(item, previousBoundingRectangle);
                }
            }
        }

        public void ItemMoved(T item, RectangleF previousBoundingRectangle)
        {
            //First, we need to remove the item from where it was in the past.
            Remove(item, previousBoundingRectangle);

            //Now we merely insert it again!
            Insert(item);
        }

        public void GetCollidingWith(T item, ref Dictionary<Guid, T> collidingEntities)
        {
            if (isLeaf)
            {
                foreach (var entity in entities)
                {
                    //If the item is intersecting with the entity, and we have not already recorded this entity, 
                    //then add it to the collision dictionary.
                    if (entity.BoundingBox2D.Intersects(item.BoundingBox2D) && item.Source != null && entity.Source.GUID != item.Source.GUID && 
                        !collidingEntities.ContainsKey(entity.Source.GUID))
                    {
                        collidingEntities.Add(entity.Source.GUID, entity);
                    }
                }
            }
            else
            {
                foreach (var child in children)
                {
                    if (child.boundingBox.Intersects(item.BoundingBox2D.Box))
                    {
                        child.GetCollidingWith(item, ref collidingEntities);
                    }
                }
            }
        }

        protected void Split()
        {
            Debug.Print("Splitting...");
            var nextDepth = currDepth + 1;

            //If this next level will be at the maximum depth, then set its maximum entities to a very large number.
            var nextMaxEntities = nextDepth == maxDepth ? int.MaxValue : maximumEntities;

            var halfWidth = boundingBox.Width / 2;
            var halfHeight = boundingBox.Height / 2;

            children = new QuadNode<T>[4];

            //Create our children nodes.
            children[0] = new QuadNode<T>(new RectangleF(boundingBox.X, boundingBox.Y, halfWidth, halfHeight), 
                                          this, nextDepth, maxDepth, nextMaxEntities, true);   //Top-Left
            children[1] = new QuadNode<T>(new RectangleF(boundingBox.X + halfWidth, boundingBox.Y, halfWidth, halfHeight), 
                                          this, nextDepth, maxDepth, nextMaxEntities, true);   //Top-right
            children[2] = new QuadNode<T>(new RectangleF(boundingBox.X, boundingBox.Y + halfHeight, halfWidth, halfHeight), 
                                          this, nextDepth, maxDepth, nextMaxEntities, true);   //Bottom-left
            children[3] = new QuadNode<T>(new RectangleF(boundingBox.X + halfWidth, boundingBox.Y + halfHeight, halfWidth, halfHeight), 
                                          this, nextDepth, maxDepth, nextMaxEntities, true);   //Bottom-right

            //Now insert all of our children...
            foreach (var entity in entities)
            {
                InsertIntoChildren(entity);
            }

            entities.Clear();
            entities = null;

            isLeaf = false;
        }

    }
}
