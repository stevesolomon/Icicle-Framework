using System.Diagnostics;
using System.Globalization;
using System.Xml.Linq;
using IcicleFramework.Behaviors;
using IcicleFramework.Components.Behaviors;
using IcicleFramework.Components.Movement;
using Microsoft.Xna.Framework;

namespace ExampleGameSHMUP.Behaviors.Projectiles
{
    public abstract class BaseProjectileMovementBehavior : BaseBehavior
    {
        public float MoveSpeed { get; set; }

        public Vector2 InitialDirection { get; set; }

        protected IMovementComponent MovementComponent { get; set; }

        public override void Initialize()
        {
            MovementComponent = ParentGameObject.GetComponent<IMovementComponent>();

            base.Initialize();
        }

        public override void Deserialize(XElement element)
        {
            float moveSpeed = 0.0f;
            var moveSpeedElem = element.Element("moveSpeed");

            if (moveSpeedElem != null)
            {
                float.TryParse(moveSpeedElem.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out moveSpeed);
            }

            MoveSpeed = moveSpeed;

            base.Deserialize(element);
        }

        public override void Reallocate()
        {
            MoveSpeed = 0.0f;
            InitialDirection = Vector2.Zero;

            base.Reallocate();
        }

        public override void CopyInto(IBehavior newObject)
        {
            var behavior = newObject as BaseProjectileMovementBehavior;

            Debug.Assert(behavior != null, "BaseProjectileMovementBehavior was null in CopyInto");

            behavior.MoveSpeed = MoveSpeed;
            behavior.InitialDirection = InitialDirection;

            base.CopyInto(newObject);
        }
    }
}
