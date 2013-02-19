using IcicleFramework.Components.Physics;
using Microsoft.Xna.Framework;

namespace IcicleFramework
{
    public class FarseerUserData
    {
        /// <summary>
        /// The IPhysicsComponent that owns the associated Farseer Physics item.
        /// </summary>
        public IPhysicsComponent Owner { get; set; }

        /// <summary>
        /// Gets or sets the offset along the X and Y axes from the center of the Physics item to its
        /// top-left corner.
        /// </summary>
        public Vector2 TopLeftOffset { get; set; }

        /// <summary>
        /// Gets or sets the offset of this Physics object relative to the IGameObject's position.
        /// </summary>
        public Vector2 Offset { get; set; }

        /// <summary>
        /// Gets or sets whether this Physics object is the primary object for the owning IGameObject. A primary
        /// physics object changes the IGameObject's Position when it moves.
        /// </summary>
        public bool Primary { get; set; }

        /// <summary>
        /// Gets or sets the name of this Body.
        /// </summary>
        public string Name { get; set; }
    }

    public class FarseerJointUserData
    {
        public string BodyAName { get; set; }

        public string BodyBName { get; set; }
    }
}
