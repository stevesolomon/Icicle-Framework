using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using IcicleFramework.Entities;
using IcicleFramework.Pooling;
using Microsoft.Xna.Framework;

namespace IcicleFramework.Collision
{
    /// <summary>
    /// Interface to define BoundingBox2D, so that QuadTree knows how to store the object.
    /// </summary>
    public interface IQuadStorable
    {
        /// <summary>
        /// The rectangle that defines the object's boundaries.
        /// </summary>
        BoundingBox2D BoundingBox2D { get; }

        /// <summary>
        /// This should return True if the object has moved during the last update, false otherwise
        /// </summary>
        bool HasMoved { get; }
    }

    /// <summary>
    /// Used internally to attach an Owner to each object stored in the QuadTree
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QuadTreeObject<T> : IPoolable<QuadTreeObject<T>>, IDisposable
        where T : IQuadStorable //, IComparable<QuadTreeObject<T>>
    {
        public event DestroyedHandler<QuadTreeObject<T>> OnDestroyed;

        private bool destroyed;

        public bool Destroyed
        {
            get { return destroyed; }
            set
            {
                destroyed = value;

                if (destroyed)
                {
                    OnQuadTreeObjectDestroyed();
                }
            }
        }

        /// <summary>
        /// The wrapped data value
        /// </summary>
        public T Data
        {
            get;
            set;
        }

        public bool Active { get; set; }

        /// <summary>
        /// The QuadTreeNode that owns this object
        /// </summary>
        public QuadTreeNode<T> Owner
        {
            get;
            set;
        }

        /// <summary>
        /// Wraps the data value
        /// </summary>
        /// <param name="data">The data value to wrap</param>
        public QuadTreeObject(T data)
        {
            Data = data;
        }

        public QuadTreeObject() { }

        public void Dispose()
        {
            Data = default(T);
            Owner = null;
        }

        protected virtual void OnQuadTreeObjectDestroyed()
        {
            if (OnDestroyed != null)
            {
                OnDestroyed(this);
            }
        }

        public virtual void Reallocate()
        {
            Destroyed = false;
            Data = default(T);
            Owner = null;
        }

        public virtual void Destroy()
        {
            Destroyed = true;
        }
    }

    /// <summary>
    /// A QuadTree Object that provides fast and efficient storage of objects in a world space.
    /// </summary>
    /// <typeparam name="T">Any object implementing IQuadStorable.</typeparam>
    public class QuadTree<T> : ICollection<T> where T : IQuadStorable
    {
        #region Private Members

        private readonly Dictionary<T, QuadTreeObject<T>> wrappedDictionary = new Dictionary<T, QuadTreeObject<T>>();

        // The root of this quad tree
        private readonly QuadTreeNode<T> quadTreeRoot;

        private PoolNew<QuadTreeObject<T>> quadTreeObjectPool;

        private long objectRemovedCount;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a QuadTree for the specified area.
        /// </summary>
        /// <param name="rect">The area this QuadTree object will encompass.</param>
        public QuadTree(BoundingBox2D rect)
        {
            quadTreeRoot = new QuadTreeNode<T>(rect);
            quadTreeObjectPool =  new PoolNew<QuadTreeObject<T>>(typeof(QuadTreeObject<T>), 1000);
            objectRemovedCount = 0;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the rectangle that bounds this QuadTree
        /// </summary>
        public BoundingBox2D QuadRect
        {
            get { return quadTreeRoot.QuadRect; }
        }

        /// <summary>
        /// Get the objects in this tree that intersect with the specified rectangle.
        /// </summary>
        /// <param name="rect">The rectangle to find objects in.</param>
        public List<T> GetObjects(BoundingBox2D rect)
        {
            return quadTreeRoot.GetObjects(rect);
        }


        /// <summary>
        /// Get the objects in this tree that intersect with the specified rectangle.
        /// </summary>
        /// <param name="rect">The rectangle to find objects in.</param>
        /// <param name="results">A reference to a list that will be populated with the results.</param>
        public void GetObjects(BoundingBox2D rect, ref List<T> results)
        {
            quadTreeRoot.GetObjects(rect, ref results);
        }


        /// <summary>
        /// Get all objects in this Quad, and it's children.
        /// </summary>
        public IEnumerator<T> GetAllObjects()
        {
            return wrappedDictionary.Keys.GetEnumerator();
        }


        /// <summary>
        /// Moves the object in the tree
        /// </summary>
        /// <param name="item">The item that has moved</param>
        public bool Move(T item)
        {
            if (Contains(item))
            {
                quadTreeRoot.Move(wrappedDictionary[item]);
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region ICollection<T> Members

        ///<summary>
        ///Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
        ///</summary>
        ///
        ///<param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        ///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</exception>
        public void Add(T item)
        {
            QuadTreeObject<T> wrappedObject = quadTreeObjectPool.New();
            wrappedObject.Data = item;
            wrappedObject.Active = true;
            wrappedDictionary.Add(item, wrappedObject);
            quadTreeRoot.Insert(wrappedObject);
        }


        ///<summary>
        ///Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        ///</summary>
        ///
        ///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only. </exception>
        public void Clear()
        {
            wrappedDictionary.Clear();
            quadTreeRoot.Clear();
        }


        ///<summary>
        ///Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.
        ///</summary>
        ///
        ///<returns>
        ///true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.
        ///</returns>
        ///
        ///<param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        public bool Contains(T item)
        {
            return wrappedDictionary.ContainsKey(item);
        }


        ///<summary>
        ///Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
        ///</summary>
        ///
        ///<param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
        ///<param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        ///<exception cref="T:System.ArgumentNullException"><paramref name="array" /> is null.</exception>
        ///<exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex" /> is less than 0.</exception>
        ///<exception cref="T:System.ArgumentException"><paramref name="array" /> is multidimensional.-or-<paramref name="arrayIndex" /> is equal to or greater than the length of <paramref name="array" />.-or-The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1" /> is greater than the available space from <paramref name="arrayIndex" /> to the end of the destination <paramref name="array" />.-or-Type <paramref name="T" /> cannot be cast automatically to the type of the destination <paramref name="array" />.</exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            wrappedDictionary.Keys.CopyTo(array, arrayIndex);
        }

        ///<summary>
        ///Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
        ///</summary>
        ///<returns>
        ///The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
        ///</returns>
        public int Count
        {
            get { return wrappedDictionary.Count; }
        }

        ///<summary>
        ///Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        ///</summary>
        ///
        ///<returns>
        ///true if the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only; otherwise, false.
        ///</returns>
        ///
        public bool IsReadOnly
        {
            get { return false; }
        }

        ///<summary>
        ///Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        ///</summary>
        ///
        ///<returns>
        ///true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.
        ///</returns>
        ///
        ///<param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        ///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</exception>
        public bool Remove(T item)
        {
            if (Contains(item))
            {
                quadTreeRoot.Delete(wrappedDictionary[item], true);
                wrappedDictionary[item].Active = false;
                wrappedDictionary.Remove(item);

                quadTreeObjectPool.CleanUp();

                return true;
            }

            return false;
        }

        #endregion

        #region IEnumerable<T> and IEnumerable Members

        ///<summary>
        ///Returns an enumerator that iterates through the collection.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        ///</returns>
        ///<filterpriority>1</filterpriority>
        public IEnumerator<T> GetEnumerator()
        {
            return wrappedDictionary.Keys.GetEnumerator();
        }


        ///<summary>
        ///Returns an enumerator that iterates through a collection.
        ///</summary>
        ///
        ///<returns>
        ///An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion


        /// <summary>
        /// The top left child for this QuadTree, only usable in debug mode
        /// </summary>
        public QuadTreeNode<T> RootQuad
        {
            get { return quadTreeRoot; }
        }
    }

    /// <summary>
    /// A QuadTree Object that provides fast and efficient storage of objects in a world space.
    /// </summary>
    /// <typeparam name="T">Any object implementing IQuadStorable.</typeparam>
    public class QuadTreeNode<T> where T : IQuadStorable
    {
        #region Constants

        // How many objects can exist in a QuadTree before it sub divides itself
        private const int maxObjectsPerNode = 10;

        #endregion

        #region Private Members

        //private List<T> m_objects = null;       // The objects in this QuadTree
        private List<QuadTreeObject<T>> objects = null;
        private BoundingBox2D rect; // The area this QuadTree represents

        private QuadTreeNode<T> parent = null; // The parent of this quad

        private QuadTreeNode<T> childTL = null; // Top Left Child
        private QuadTreeNode<T> childTR = null; // Top Right Child
        private QuadTreeNode<T> childBL = null; // Bottom Left Child
        private QuadTreeNode<T> childBR = null; // Bottom Right Child

        #endregion

        #region Public Properties

        /// <summary>
        /// The area this QuadTree represents.
        /// </summary>
        public BoundingBox2D QuadRect
        {
            get { return rect; }
        }

        /// <summary>
        /// The top left child for this QuadTree
        /// </summary>
        public QuadTreeNode<T> TopLeftChild
        {
            get { return childTL; }
        }

        /// <summary>
        /// The top right child for this QuadTree
        /// </summary>
        public QuadTreeNode<T> TopRightChild
        {
            get { return childTR; }
        }

        /// <summary>
        /// The bottom left child for this QuadTree
        /// </summary>
        public QuadTreeNode<T> BottomLeftChild
        {
            get { return childBL; }
        }

        /// <summary>
        /// The bottom right child for this QuadTree
        /// </summary>
        public QuadTreeNode<T> BottomRightChild
        {
            get { return childBR; }
        }

        /// <summary>
        /// This QuadTree's parent
        /// </summary>
        public QuadTreeNode<T> Parent
        {
            get { return parent; }
        }

        /// <summary>
        /// The objects contained in this QuadTree at it's level (ie, excludes children)
        /// </summary>
        internal List<QuadTreeObject<T>> Objects
        {
            get { return objects; }
        }

        /// <summary>
        /// How many total objects are contained within this QuadTree (ie, includes children)
        /// </summary>
        public int Count
        {
            get { return ObjectCount(); }
        }

        /// <summary>
        /// Returns true if this is a empty leaf node
        /// </summary>
        public bool IsEmptyLeaf
        {
            get { return Count == 0 && childTL == null; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a QuadTree for the specified area.
        /// </summary>
        /// <param name="rect">The area this QuadTree object will encompass.</param>
        public QuadTreeNode(BoundingBox2D rect)
        {
            this.rect = rect;
        }


        /// <summary>
        /// Creates a QuadTree for the specified area.
        /// </summary>
        /// <param name="x">The top-left position of the area rectangle.</param>
        /// <param name="y">The top-right position of the area rectangle.</param>
        /// <param name="width">The width of the area rectangle.</param>
        /// <param name="height">The height of the area rectangle.</param>
        public QuadTreeNode(int x, int y, int width, int height)
        {
            rect = new BoundingBox2D(new RectangleF(x, y, width, height));
        }


        private QuadTreeNode(QuadTreeNode<T> parent, BoundingBox2D rect)
            : this(rect)
        {
            this.parent = parent;
        }

        #endregion

        #region Private Members

        /// <summary>
        /// Add an item to the object list.
        /// </summary>
        /// <param name="item">The item to add.</param>
        private void Add(QuadTreeObject<T> item)
        {
            if (objects == null)
            {
                //m_objects = new List<T>();
                objects = new List<QuadTreeObject<T>>();
            }

            Debug.Print("Adding one into Node! Count is: " + Count.ToString() + "\n" + Environment.StackTrace + "\n\n-----------------------------------------------------------\n\n");

            item.Owner = this;
            objects.Add(item);
        }


        /// <summary>
        /// Remove an item from the object list.
        /// </summary>
        /// <param name="item">The object to remove.</param>
        private void Remove(QuadTreeObject<T> item)
        {
            if (objects != null)
            {
                int removeIndex = objects.IndexOf(item);
                if (removeIndex >= 0)
                {
                    objects[removeIndex] = objects[objects.Count - 1];
                    objects.RemoveAt(objects.Count - 1);
                    item.Active = false;
                }
            }
        }


        /// <summary>
        /// Get the total for all objects in this QuadTree, including children.
        /// </summary>
        /// <returns>The number of objects contained within this QuadTree and its children.</returns>
        private int ObjectCount()
        {
            int count = 0;

            // Add the objects at this level
            if (objects != null)
            {
                count += objects.Count;
            }

            // Add the objects that are contained in the children
            if (childTL != null)
            {
                count += childTL.ObjectCount();
                count += childTR.ObjectCount();
                count += childBL.ObjectCount();
                count += childBR.ObjectCount();
            }

            return count;
        }


        /// <summary>
        /// Subdivide this QuadTree and move it's children into the appropriate Quads where applicable.
        /// </summary>
        private void Subdivide()
        {
            // We've reached capacity, subdivide...
            Point size = new Point((int) rect.Width / 2, (int) rect.Height / 2);
            Point mid = new Point((int) rect.X + size.X, (int) rect.Y + size.Y);

            childTL = new QuadTreeNode<T>(this, new BoundingBox2D(rect.Left, rect.Top, size.X, size.Y));
            childTR = new QuadTreeNode<T>(this, new BoundingBox2D(mid.X, rect.Top, size.X, size.Y));
            childBL = new QuadTreeNode<T>(this, new BoundingBox2D(rect.Left, mid.Y, size.X, size.Y));
            childBR = new QuadTreeNode<T>(this, new BoundingBox2D(mid.X, mid.Y, size.X, size.Y));

            // If they're completely contained by the quad, bump objects down
            for (int i = 0; i < objects.Count; i++)
            {
                QuadTreeNode<T> destTree = GetDestinationTree(objects[i]);

                if (destTree != this)
                {
                    // Insert to the appropriate tree, remove the object, and back up one in the loop
                    destTree.Insert(objects[i]);
                    Remove(objects[i]);
                    i--;
                }
            }
        }


        /// <summary>
        /// Get the child Quad that would contain an object.
        /// </summary>
        /// <param name="item">The object to get a child for.</param>
        /// <returns></returns>
        private QuadTreeNode<T> GetDestinationTree(QuadTreeObject<T> item)
        {
            // If a child can't contain an object, it will live in this Quad
            QuadTreeNode<T> destTree = this;

            if (childTL.QuadRect.Contains(item.Data.BoundingBox2D))
            {
                destTree = childTL;
            }
            else if (childTR.QuadRect.Contains(item.Data.BoundingBox2D))
            {
                destTree = childTR;
            }
            else if (childBL.QuadRect.Contains(item.Data.BoundingBox2D))
            {
                destTree = childBL;
            }
            else if (childBR.QuadRect.Contains(item.Data.BoundingBox2D))
            {
                destTree = childBR;
            }

            return destTree;
        }


        private void Relocate(QuadTreeObject<T> item)
        {
            // Are we still inside our parent?
            if (QuadRect.Contains(item.Data.BoundingBox2D))
            {
                // Good, have we moved inside any of our children?
                if (childTL != null)
                {
                    QuadTreeNode<T> dest = GetDestinationTree(item);
                    if (item.Owner != dest)
                    {
                        // Delete the item from this quad and add it to our child
                        // Note: Do NOT clean during this call, it can potentially delete our destination quad
                        QuadTreeNode<T> formerOwner = item.Owner;
                        Delete(item, false);
                        dest.Insert(item);

                        // Clean up ourselves
                        //formerOwner.CleanUpwards();
                    }
                }
            }
            else
            {
                // We don't fit here anymore, move up, if we can
                if (parent != null)
                {
                    parent.Relocate(item);
                }
            }
        }


        private void CleanUpwards()
        {
            if (childTL != null)
            {
                // If all the children are empty leaves, delete all the children
                if (childTL.IsEmptyLeaf &&
                    childTR.IsEmptyLeaf &&
                    childBL.IsEmptyLeaf &&
                    childBR.IsEmptyLeaf)
                {
                    childTL = null;
                    childTR = null;
                    childBL = null;
                    childBR = null;

                    if (parent != null && Count == 0)
                    {
                        parent.CleanUpwards();
                    }
                }
            }
            else
            {
                // I could be one of 4 empty leaves, tell my parent to clean up
                if (parent != null && Count == 0)
                {
                    parent.CleanUpwards();
                }
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Clears the QuadTree of all objects, including any objects living in its children.
        /// </summary>
        internal void Clear()
        {
            // Clear out the children, if we have any
            if (childTL != null)
            {
                childTL.Clear();
                childTR.Clear();
                childBL.Clear();
                childBR.Clear();
            }

            // Clear any objects at this level
            if (objects != null)
            {
                objects.Clear();
                objects = null;
            }

            // Set the children to null
            childTL = null;
            childTR = null;
            childBL = null;
            childBR = null;
        }


        /// <summary>
        /// Deletes an item from this QuadTree. If the object is removed causes this Quad to have no objects in its children, it's children will be removed as well.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <param name="clean">Whether or not to clean the tree</param>
        internal void Delete(QuadTreeObject<T> item, bool clean)
        {
            if (item.Owner == null)
                return;

            if (item.Owner == this)
            {
                Remove(item);
                if (clean)
                {
                    CleanUpwards();
                }
            }
            else
            {
                item.Owner.Delete(item, clean);
            }
        }


        /// <summary>
        /// Insert an item into this QuadTree object.
        /// </summary>
        /// <param name="item">The item to insert.</param>
        internal void Insert(QuadTreeObject<T> item)
        {
            // If this quad doesn't contain the items rectangle, do nothing, unless we are the root
            if (!rect.Contains(item.Data.BoundingBox2D))
            {
                System.Diagnostics.Debug.Assert(parent == null, "We are not the root, and this object doesn't fit here. How did we get here?");
                if (parent == null)
                {
                    // This object is outside of the QuadTree bounds, we should add it at the root level
                    Add(item);
                }
                else
                {
                    return;
                }
            }

            if (objects == null ||
                (childTL == null && objects.Count < maxObjectsPerNode))
            {
                // If there's room to add the object, just add it
                Add(item);
            }
            else
            {
                // No quads, create them and bump objects down where appropriate
                if (childTL == null)
                {
                    Subdivide();
                }

                // Find out which tree this object should go in and add it there
                QuadTreeNode<T> destTree = GetDestinationTree(item);
                if (destTree == this)
                {
                    Add(item);
                }
                else
                {
                    destTree.Insert(item);
                }
            }
        }


        /// <summary>
        /// Get the objects in this tree that intersect with the specified rectangle.
        /// </summary>
        /// <param name="searchRect">The rectangle to find objects in.</param>
        internal List<T> GetObjects(BoundingBox2D searchRect)
        {
            List<T> results = new List<T>();
            GetObjects(searchRect, ref results);
            return results;
        }


        /// <summary>
        /// Get the objects in this tree that intersect with the specified rectangle.
        /// </summary>
        /// <param name="searchRect">The rectangle to find objects in.</param>
        /// <param name="results">A reference to a list that will be populated with the results.</param>
        internal void GetObjects(BoundingBox2D searchRect, ref List<T> results)
        {
            // We can't do anything if the results list doesn't exist
            if (results != null)
            {
                if (searchRect.Contains(this.rect))
                {
                    // If the search area completely contains this quad, just get every object this quad and all it's children have
                    GetAllObjects(ref results);
                }
                else if (searchRect.Intersects(this.rect))
                {
                    // Otherwise, if the quad isn't fully contained, only add objects that intersect with the search rectangle
                    if (objects != null)
                    {
                        for (int i = 0; i < objects.Count; i++)
                        {
                            if (searchRect.Intersects(objects[i].Data.BoundingBox2D))
                            {
                                results.Add(objects[i].Data);
                            }
                        }
                    }

                    // Get the objects for the search rectangle from the children
                    if (childTL != null)
                    {
                        childTL.GetObjects(searchRect, ref results);
                        childTR.GetObjects(searchRect, ref results);
                        childBL.GetObjects(searchRect, ref results);
                        childBR.GetObjects(searchRect, ref results);
                    }
                }
            }
        }


        /// <summary>
        /// Get all objects in this Quad, and it's children.
        /// </summary>
        /// <param name="results">A reference to a list in which to store the objects.</param>
        internal void GetAllObjects(ref List<T> results)
        {
            // If this Quad has objects, add them
            if (objects != null)
            {
                foreach (QuadTreeObject<T> qto in objects)
                {
                    results.Add(qto.Data);
                }
            }

            // If we have children, get their objects too
            if (childTL != null)
            {
                childTL.GetAllObjects(ref results);
                childTR.GetAllObjects(ref results);
                childBL.GetAllObjects(ref results);
                childBR.GetAllObjects(ref results);
            }
        }


        /// <summary>
        /// Moves the QuadTree object in the tree
        /// </summary>
        /// <param name="item">The item that has moved</param>
        internal void Move(QuadTreeObject<T> item)
        {
            if (item.Owner != null)
            {
                item.Owner.Relocate(item);
            }
            else
            {
                Relocate(item);
            }
        }

        #endregion
    }
}
