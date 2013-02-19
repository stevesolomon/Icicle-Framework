using System.Diagnostics;
using IcicleFramework.Actions;
using IcicleFramework.GameServices;
using IcicleFramework.GameServices.ParticleServices;
using Microsoft.Xna.Framework;

namespace ExampleGameSHMUP.Actions.Particles
{
    public class OneShotEmitParticleAction : GameAction
    {
        protected IParticleManager particleManager;

        public string ParticleName { get; set; }

        public Vector2 TargetLocation { get; set; }

        public override void Initialize()
        {
            particleManager = GameServiceManager.GetService<IParticleManager>();
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            particleManager.EmitParticle(ParticleName, Target == null ? TargetLocation : Target.Position);

            Finished = true;

            base.Update(gameTime);
        }

        public override void Dispose()
        {
            particleManager = null;

            base.Dispose();
        }

        public override void CopyInto(IGameAction newObject)
        {
            var action = newObject as OneShotEmitParticleAction;

            Debug.Assert(action != null);

            action.ParticleName = ParticleName;

            base.CopyInto(newObject);
        }
    }
}
