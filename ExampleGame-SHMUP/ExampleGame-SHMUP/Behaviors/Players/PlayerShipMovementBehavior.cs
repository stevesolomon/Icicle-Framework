using IcicleFramework.Actions.Movement;
using IcicleFramework.Behaviors;
using IcicleFramework.Components.Behaviors;
using IcicleFramework.Inputs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ExampleGameSHMUP.Behaviors.Players
{
    public class PlayerShipMovementBehavior : BaseBehavior
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
                var moveAction = Parent.ActionFactory.GetAction<DirectionBasedMovementAction>();
                moveAction.Direction = direction;

                Parent.FireAction(moveAction, ParentGameObject);
            }

            base.Update(gameTime);
        }

        public override void Reallocate()
        {
            player = null;

            base.Reallocate();
        }
    }
}
