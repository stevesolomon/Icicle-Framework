using System.Diagnostics;
using System.Xml.Linq;
using ExampleGameSHMUP.Actions.Particles;
using IcicleFramework.Behaviors;
using IcicleFramework.Behaviors.Destruction;
using IcicleFramework.Components.Renderable;
using IcicleFramework.Entities;
using Microsoft.Xna.Framework;

namespace ExampleGameSHMUP.Behaviors
{
    public class ParticleEffectOnDestructionBehavior : BaseDestructionBehavior
    {
        protected string ParticleEffectName { get; set; }

        protected override void  OnParentDestroyed(IGameObject sender)
        {
            var renderComponent = ParentGameObject.GetComponent<IRenderComponent>();

            var particleAction = Parent.ActionFactory.GetAction<OneShotEmitParticleAction>();
            particleAction.ParticleName = ParticleEffectName;

            //Find the correct spot on the IRenderComponent if it's available.
            if (renderComponent != null)
            {
                float x = (ParentGameObject.Position.X + (renderComponent.Width / 2f));
                float y = (ParentGameObject.Position.Y + (renderComponent.Height / 2f));
                particleAction.TargetLocation = new Vector2(x, y);

                Parent.FireAction(particleAction, null);
            }
            else //No render component, just set the target to be this game object.
            {
                Parent.FireAction(particleAction, ParentGameObject);
            }

            base.OnParentDestroyed(sender);
        }

        public override void CopyInto(IBehavior newObject)
        {
            var behavior = newObject as ParticleEffectOnDestructionBehavior;

            Debug.Assert(behavior != null);

            behavior.ParticleEffectName = ParticleEffectName;

            base.CopyInto(newObject);
        }

        public override void Deserialize(XElement element)
        {
            var xElement = element.Element("particleEffectName");

            if (xElement != null)
                ParticleEffectName = xElement.Value;

            base.Deserialize(element);
        }

    }
}
