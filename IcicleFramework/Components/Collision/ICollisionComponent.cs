using IcicleFramework.Collision;
using IcicleFramework.Collision.QuadTree;

namespace IcicleFramework.Components.Collision
{
    public delegate void CollisionHandler(ICollisionComponent source, ICollisionComponent collided, CorrectionVector2 resolutionVector);

    public delegate void OnCollisionHandler(ICollisionComponent source, ICollisionComponent collided);

    public delegate void OnCollisionStoppedHandler(ICollisionComponent source, ICollisionComponent previousCollided);

    public interface ICollisionComponent : IBaseComponent, IBoundingBox
    {
        event OnCollisionHandler OnCollision;

        event OnCollisionStoppedHandler OnCollisionStopped;

        /// <summary>
        /// Gets or sets the Collision Priority of this ICollidable object.
        /// </summary>
        int CollisionPriority { get; }

        /// <summary>
        /// Gets or sets whether or not this ICollidableComponent is solid.
        /// </summary>
        bool Solid { get; set; }

        CorrectionVector2 GetCorrectionVector(ICollisionComponent B);
    }
}
