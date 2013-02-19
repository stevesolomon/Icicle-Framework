using Microsoft.Xna.Framework;

namespace IcicleFramework.Components.Movement
{
    public interface IMovementComponent : IBaseComponent
    {
        Vector2 VelocityDampingPercent { get; set; }

        Vector2 Velocity { get; set; }
        
        Vector2 MoveVelocity { get; set; }

        Vector2 MaxVelocity { get; set; }

        void MoveInDirection(Vector2 direction);
    }
}
