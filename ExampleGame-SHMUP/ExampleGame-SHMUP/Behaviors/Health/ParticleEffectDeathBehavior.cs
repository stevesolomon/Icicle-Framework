using System.Diagnostics;
using System.Xml.Linq;
using ExampleGameSHMUP.Actions.Particles;
using IcicleFramework.Behaviors;
using IcicleFramework.Components.Behaviors;
using IcicleFramework.Components.Health;
using IcicleFramework.Components.Renderable;
using IcicleFramework.Entities;
using Microsoft.Xna.Framework;

namespace ExampleGameSHMUP.Behaviors.Health
{
    public class ParticleEffectDeathBehavior : BasicDeathBehavior
    {
        protected string ParticleEffectName { get; set; }

        protected override void OnHealthDepleted(IHealthComponent sender, IGameObject damageInitator)
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
                Parent.FireAction(particleAction, sender.Parent);
            }

            base.OnHealthDepleted(sender, damageInitator);
        }

        public override void CopyInto(IBehavior newObject)
        {
            var behavior = newObject as ParticleEffectDeathBehavior;

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
