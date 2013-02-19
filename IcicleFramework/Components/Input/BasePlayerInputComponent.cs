using System.Diagnostics;
using IcicleFramework.Inputs;

namespace IcicleFramework.Components.Input
{
    /// <summary>
    /// An initial abstract implementation of a Player-fed Input Component.
    /// Concrete implementations are left to the user, as this will be very game-specific.
    /// </summary>
    public abstract class BasePlayerInputComponent : BaseInputComponent, IPlayerInputComponent
    {
        public Player Player { get; set; }

        protected  BasePlayerInputComponent() {}

        protected BasePlayerInputComponent(BasePlayerInputComponent old)
        {
            this.Player = old.Player;
        }

        public override void CopyInto(IBaseComponent newObject)
        {
            var baseInputComponent = newObject as BasePlayerInputComponent;

            Debug.Assert(baseInputComponent != null, "baseInputComponent != null");

            baseInputComponent.Player = Player;

            base.CopyInto(newObject);
        }
    }
}
