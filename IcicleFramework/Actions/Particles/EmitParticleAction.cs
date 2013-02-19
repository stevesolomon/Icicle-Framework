using System.Diagnostics;
using IcicleFramework.GameServices;
using IcicleFramework.GameServices.ParticleServices;
using Microsoft.Xna.Framework;

namespace IcicleFramework.Actions.Particles
{
    public class EmitParticleAction : GameAction
    {
        public string ParticleName { get; set; }

        protected bool targetSet;

        protected Vector2 targetPosition;

        protected IParticleManager particleManager;

        public Vector2 TargetPosition
        {
            get { return targetPosition; }
            set
            {
                targetSet = true;
                targetPosition = value;
            }
        }

        public override void Update(GameTime gameTime)
        {
            particleManager.EmitParticle(ParticleName, targetSet ? targetPosition : Target.Position);
            Finished = true;

            base.Update(gameTime);
        }

        public override void Initialize()
        {
            particleManager = GameServiceManager.GetService<IParticleManager>();
            base.Initialize();
        }

        public override void Dispose()
        {
            particleManager = null;
            targetSet = false;

            base.Dispose();
        }

        public override void CopyInto(IGameAction newObject)
        {
            var action = newObject as EmitParticleAction;

            Debug.Assert(action != null);

            action.TargetPosition = TargetPosition;
            action.ParticleName = ParticleName;

            base.CopyInto(newObject);
        }
    }
}
