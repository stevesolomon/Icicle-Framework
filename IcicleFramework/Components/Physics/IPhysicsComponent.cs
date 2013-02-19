using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace IcicleFramework.Components.Physics
{
    public interface IPhysicsComponent : IBaseComponent, IScalable
    {
        /// <summary>
        /// Gets or sets whether or not this IPhysicsComponent is solid.
        /// </summary>
        bool Solid { get; set; }

        /// <summary>
        /// Gets the number of Bodies in this IPhysicsComponent
        /// </summary>
        int NumBodies { get; }

        /// <summary>
        /// Gets or sets the maximum velocity of this IPhysicsComponent.
        /// </summary>
        float MaxVelocity { get; set; }

        /// <summary>
        /// Gets or sets the velocity of this IPhysicsComponent.
        /// Note: This may be unsafe for non-kinematic bodies!
        /// </summary>
        Vector2 Velocity { get; set; }
        
        /// <summary>
        /// Gets the Body attributed to the specific name.
        /// </summary>
        /// <param name="name">The name of the Body to be returned.</param>
        /// <returns>The Body with the given name, null if no matching Body was found.</returns>
        Body GetBody(string name);

        /// <summary>
        /// Gets the Body at the specific index.
        /// </summary>
        /// <param name="index">The index of the Body to be returned.</param>
        /// <returns>The Body at the given index, null if no Body exists at that index.</returns>
        Body GetBody(int index);

        /// <summary>
        /// Gets the first Body in this IPhysicsComponent.
        /// </summary>
        /// <returns>The first Body in this IPhysicsComponent, null if this IPhysicsComponent contains no Bodies.</returns>
        Body GetFirstBody();

        /// <summary>
        /// Gets the first Primary Body found in this IPhysicsComponent
        /// </summary>
        /// <returns>The first primary Body found in this IPhysicsComponent, null if no primary Body could be located.</returns>
        Body GetPrimaryBody();

        /// <summary>
        /// Adds the Body with the given name to this IPhysicsComponent.
        /// </summary>
        /// <param name="body">The body to add to this IPhysicsComponent.</param>
        /// <param name="name">The name of the body for lookup purposes.</param>
        /// <returns>True if the Body was added, false if otherwise (an IPhysicsComponent cannot contain Bodies with duplicate names).</returns>
        /// <remarks>It is assumed that the Body has already been registered in the World.</remarks>
        bool AddBody(Body body, string name);
        
        /// <summary>
        /// Applies a force impulse to the Body attributed to the specific name.
        /// </summary>
        /// <param name="impulse">The impulse to apply to the Body.</param>
        /// <param name="name">The name of the Body to apply the impulse to.</param>
        void ApplyImpulse(Vector2 impulse, string name);

        /// <summary>
        /// Applies a force impulse to all Bodies handled by the IPhysicsComponent.
        /// </summary>
        /// <param name="impulse">The impulse to apply to the Body.</param>
        void ApplyImpulse(Vector2 impulse);
    }
}
