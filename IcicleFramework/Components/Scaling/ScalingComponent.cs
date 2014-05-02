using System;
using System.Xml.Linq;
using IcicleFramework.Components.Physics;
using IcicleFramework.Components.Renderable;

namespace IcicleFramework.Components.Scaling
{
    /// <summary>
    /// We'll just use a single, basic scaling component right now, which will
    /// be aware of anything that needs to be scaled. Rendering, Physics, etc.
    /// </summary>
    public class ScalingComponent : BaseComponent, IScalingComponent
    {
        protected float scale;

        protected IRenderComponent renderComponent;

        protected IPhysicsComponent physicsComponent;

        public float Scale
        {
            get { return scale; }
        }

        public float DefaultScale { get; protected set; }

        public ScalingComponent()
        {
            scale = 1.0f;
            DefaultScale = 1.0f;
        }

        public override void Initialize()
        {
            renderComponent = Parent.GetComponent<IRenderComponent>();
            physicsComponent = Parent.GetComponent<IPhysicsComponent>();

            base.Initialize();
        }

        public void ApplyScaling(float scalingValue)
        {
            //Make sure we're not going to waste time scaling to the same scale level!
            if (Math.Abs(scalingValue - scale) > 0.0001f)
            {
                //Dispose the default scaling value so we can get back later!
                DefaultScale = DefaultScale / scalingValue;

                if (renderComponent != null)
                    ScaleRendering(scalingValue);

                if (physicsComponent != null)
                    ScalePhysics(scalingValue);
            }
        }

        public void ResetDefaultScaling()
        {
            ApplyScaling(DefaultScale);
        }

        protected void ScaleRendering(float scalingValue)
        {
            renderComponent.SetScale(scalingValue);
        }

        protected void ScalePhysics(float scalingValue)
        {
            

        }

        public override void Deserialize(XElement element)
        { }

        public override void Reallocate()
        {
            renderComponent = null;
            physicsComponent = null;

            base.Reallocate();
        }
    }
}
