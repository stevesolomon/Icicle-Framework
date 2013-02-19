using IcicleFramework.Actions.Movement;
using IcicleFramework.Components.Behaviors;
using IcicleFramework.Inputs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TestBed.Components.Behaviors
{
    public class PlayerInputBehavior : BaseBehavior
    {
        protected Player player;

        public override void PostInitialize()
        {
            //Try to find the Metadata component and pull player information from it.
            player = ParentGameObject.GetMetadata("player") as Player;

            base.PostInitialize();
        }

        public override void Update(GameTime gameTime)
        {
            Vector2 direction = Vector2.Zero;

            if (player != null)
            {
                if (player.InputHandler.IsDown(Buttons.DPadLeft, player.Controller) ||
                    player.InputHandler.IsDown(Keys.Left))
                {
                    direction.X = -1;
                }
                if (player.InputHandler.IsDown(Buttons.DPadRight, player.Controller) ||
                    player.InputHandler.IsDown(Keys.Right))
                {
                    direction.X = 1;
                }

                if (player.InputHandler.IsDown(Buttons.DPadUp, player.Controller) ||
                    player.InputHandler.IsDown(Keys.Up))
                {
                    direction.Y = -1;
                }
                if (player.InputHandler.IsDown(Buttons.DPadDown, player.Controller) ||
                    player.InputHandler.IsDown(Keys.Down))
                {
                    direction.Y = 1;
                }
            }

            if (direction != Vector2.Zero)
            {
                Vector2 movement = new Vector2(direction.X * 100f * (float) gameTime.ElapsedGameTime.TotalSeconds, direction.Y * 100f * (float) gameTime.ElapsedGameTime.TotalSeconds);
                DirectMovementAction action = new DirectMovementAction() { MovementAmount = movement };
                Parent.FireAction(action, ParentGameObject);
            }

            base.Update(gameTime);
        }
    }
}
