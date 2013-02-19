using Microsoft.Xna.Framework;

namespace IcicleFramework.Collision
{
    public interface ICollision : IXmlBuildable
    {
        Vector2 Offset { get; set; }

        Vector2 Position { get; set; }

        float X { get; set; }

        float Y { get; set; }

        bool Active { get; set; }

        string Name { get; set; }

        bool Intersects(ICollision collisionObject);

        bool Contains(ICollision collisionObject);
        
        ICollision DeepCopy();
    }
}
