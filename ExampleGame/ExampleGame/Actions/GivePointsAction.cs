using System.Diagnostics;
using ExampleGame.GameSystems;
using IcicleFramework.Actions;
using IcicleFramework.GameServices;
using Microsoft.Xna.Framework;

namespace ExampleGame.Actions
{
    public class GivePointsAction : GameAction 
    {
        public float Points { get; set; }

        public override void Update(GameTime gameTime)
        {
            var scoreManager = GameServiceManager.GetService<IScoreManager>();

            scoreManager.AddPoints(Target, Points);

            Finished = true;

            base.Update(gameTime);
        }

        public override void CopyInto(IGameAction newObject)
        {
            var action = newObject as GivePointsAction;

            Debug.Assert(action != null);

            action.Points = Points;

            base.CopyInto(newObject);
        }
    }
}
