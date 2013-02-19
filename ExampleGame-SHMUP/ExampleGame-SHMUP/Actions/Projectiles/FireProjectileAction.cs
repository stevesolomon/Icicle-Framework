using System.Diagnostics;
using ExampleGameSHMUP.GameServices;
using IcicleFramework.Actions;
using IcicleFramework.GameServices;
using Microsoft.Xna.Framework;

namespace ExampleGameSHMUP.Actions.Projectiles
{
    public class FireProjectileAction : GameAction
    {
        public string ProjectileName { get; set; }

        public string ProjectileLayer { get; set; }

        public Vector2 ProjectilePosition { get; set; }

        public Vector2 Direction { get; set; }

        public IProjectileManager ProjectileManager { get; set; }

        public override void Initialize()
        {
            ProjectileManager = GameServiceManager.GetService<IProjectileManager>();

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            var proj = ProjectileManager.SpawnProjectile(ProjectileName, ProjectilePosition, Direction);
            proj.Active = true;
            proj.Layer = ProjectileLayer;

            Finished = true;

            base.Update(gameTime);
        }

        public override void Dispose()
        {
            ProjectileManager = null;

            base.Dispose();
        }

        public override void CopyInto(IGameAction newObject)
        {
            var action = newObject as FireProjectileAction;

            Debug.Assert(action != null);

            action.ProjectileName = ProjectileName;
            action.ProjectileLayer = ProjectileLayer;
            action.ProjectilePosition = ProjectilePosition;
            action.Direction = Direction;

            base.CopyInto(newObject);
        }
    }
}
