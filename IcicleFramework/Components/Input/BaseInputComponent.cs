using System.Diagnostics;
using IcicleFramework.GameServices;
using IcicleFramework.Inputs;

namespace IcicleFramework.Components.Input
{
    public abstract class BaseInputComponent : BaseComponent, IInputComponent
    {
        protected IInputHandler InputHandler { get; set; }

        public override void Initialize()
        {
            //Grab a reference to the IInputHandler service.
            InputHandler = GameServiceManager.GetService<IInputHandler>();

            base.Initialize();
        }

        public override void CopyInto(IBaseComponent newObject)
        {
            var baseInputComponent = newObject as BaseInputComponent;

            Debug.Assert(baseInputComponent != null, "baseInputComponent != null");

            baseInputComponent.InputHandler = InputHandler;

            base.CopyInto(newObject);
        }
    }
}
