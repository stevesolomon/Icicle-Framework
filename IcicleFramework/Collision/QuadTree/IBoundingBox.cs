using IcicleFramework.Entities;

namespace IcicleFramework.Collision.QuadTree
{
    /// <summary>
    /// An interface for any object that can be stored in a QuadTree.
    /// </summary>
    public interface IBoundingBox
    {
        /// <summary>
        /// Gets the BoundingBox2D that helps position this IBoundingBox within a QuadTree.
        /// </summary>
        BoundingBox2D BoundingBox2D { get; }

        /// <summary>
        /// Gets the IGameObject that is the source of this IBoundingBox, if applicable.
        /// </summary>
        IGameObject Source { get; }
    }
}
