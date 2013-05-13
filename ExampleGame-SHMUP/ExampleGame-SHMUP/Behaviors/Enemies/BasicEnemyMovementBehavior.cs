using IcicleFramework.Behaviors;
using IcicleFramework.Components.Behaviors;
using IcicleFramework.Components.Movement;
using Microsoft.Xna.Framework;

namespace ExampleGameSHMUP.Behaviors.Enemies
{
    public class BasicEnemyMovementBehavior : BaseBehavior
    {
        public override void Initialize()
        {
            var movementComponent = ParentGameObject.GetComponent<IMovementComponent>(); 
            movementComponent.MoveInDirection(new Vector2(0, 1));

            base.Initialize();
        }
    }
}
