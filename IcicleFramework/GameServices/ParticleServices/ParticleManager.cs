using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectMercury;

namespace IcicleFramework.GameServices.ParticleServices
{
    public class ParticleManager : GameService, IParticleManager
    {
        protected ContentManager Content { get; set; }

        protected string ContentPath { get; set; }

        protected string TexturePath { get; set; }

        protected Dictionary<string, ParticleEffect> ActiveParticleEffects { get; set; }

        public ParticleManager(ContentManager content, string contentPath, string texturePath)
        {
            this.Content = content;
            this.ContentPath = contentPath;
            this.TexturePath = texturePath;
            ActiveParticleEffects = new Dictionary<string, ParticleEffect>();
        }

        public IEnumerable<ParticleEffect> GetAllActiveParticleEffects()
        {
            return ActiveParticleEffects.Values;
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var effect in ActiveParticleEffects.Values)
                effect.Update((float) gameTime.ElapsedGameTime.TotalSeconds);
        }

        public void EmitParticle(string particleName, Vector2 position)
        {
            ParticleEffect effect;

            if (!ActiveParticleEffects.ContainsKey(particleName))
            {
                effect = Content.Load<ParticleEffect>(Path.Combine(ContentPath, particleName));
                ActiveParticleEffects.Add(particleName, effect);
                
                foreach (var emitter in effect.Emitters)
                {
                    emitter.ParticleTexture = Content.Load<Texture2D>(Path.Combine(TexturePath, emitter.ParticleTextureAssetPath));
                    emitter.Initialise();
                }
            }

           effect = ActiveParticleEffects[particleName];

           Vector3 pos = new Vector3(position.X, position.Y, 10f);

           effect.Trigger(ref pos);
        }
    }
}
