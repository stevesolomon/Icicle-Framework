using System.Diagnostics;
using IcicleFramework.Actions;
using IcicleFramework.Components.Renderable;
using Microsoft.Xna.Framework;

namespace ExampleGame.Actions
{
    public class ColorChangeAction : GameAction
    {
        public Color Color { get; set; }

        public override void Update(GameTime gameTime)
        {
            //If we no longer have a target then die early.
            if (Target == null)
            {
                Finished = true;
                base.Update(gameTime);
                return;
            }

            var renderComp = Target != null ? Target.GetComponent<IRenderComponent>() : null;

            if (renderComp != null)
            {
                renderComp.SetShading(Color);
            }

            Finished = true;

            base.Update(gameTime);
        }

        public override void CopyInto(IGameAction newObject)
        {
            var action = newObject as ColorChangeAction;

            Debug.Assert(action != null);

            action.Color = Color;

            base.CopyInto(newObject);
        }
    }
}
