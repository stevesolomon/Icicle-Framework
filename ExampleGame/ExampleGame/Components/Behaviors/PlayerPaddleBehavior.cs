using System;
using System.Diagnostics;
using System.Xml.Linq;
using ExampleGame.GameSystems;
using IcicleFramework;
using IcicleFramework.Actions.Physics;
using IcicleFramework.Components.Behaviors;
using IcicleFramework.Components.Renderable;
using IcicleFramework.Entities;
using IcicleFramework.GameServices;
using IcicleFramework.Inputs;
using IcicleFramework.Renderables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ExampleGame.Components.Behaviors
{
    public class PlayerPaddleBehavior : BaseBehavior
    {
        protected Player Player { get; set; }

        protected Vector2 Velocity { get; set; }

        public override void PostInitialize()
        {
            //Pull player information from the metadata, if possible.
            Player player = ParentGameObject.GetMetadata("player") as Player;

            if (player != null)
                Player = player;

            //Hook up with the lives changed event on the level logic manager
            var levelLogic = GameServiceManager.GetService<ILevelLogicManager>();
            levelLogic.OnPlayerLivesChanged += LevelLogicOnOnPlayerLivesChanged;

            base.Initialize();
        }

        private void LevelLogicOnOnPlayerLivesChanged(IGameObject gameObject, int lives)
        {
            if (lives < 2)
            {
                //Find the renderable...
                var renderable = ParentGameObject.GetComponent<IRenderComponent>();
                var sprite = (AnimatedSprite) renderable.GetRenderable("paddle");
                sprite.StartAnimation("oneLifeLeft");
            }
        }

        protected void HandleInput(GameTime gameTime)
        {
            Vector2 impulse = Vector2.Zero;
            Vector2 actualSpeed = Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Player != null)
            {
                if (Player.InputHandler.IsDown(Buttons.DPadLeft, Player.Controller) ||
                    Player.InputHandler.IsDown(Keys.Left))
                {
                    impulse.X = -actualSpeed.X;
                }
                if (Player.InputHandler.IsDown(Buttons.DPadRight, Player.Controller) ||
                    Player.InputHandler.IsDown(Keys.Right))
                {
                    impulse.X = actualSpeed.X;
                }
            }

            if (impulse.X > 0f || impulse.X < 0f)
            {
                var impulseAction = Parent.ActionFactory.GetAction<PhysicsImpulseAction>();
                impulseAction.Impulse = new Vector2(impulse.X, 0f);

                Parent.FireAction(impulseAction, ParentGameObject, null, 0f);
            }
        }

        public override void Update(GameTime gameTime)
        {
            HandleInput(gameTime);

            base.Update(gameTime);
        }

        public override void Deserialize(XElement element)
        {
            Velocity = new Vector2(float.MaxValue);
            XElement maxVelElement = element.Element("velocity");
            if (maxVelElement != null)
            {
                Velocity = Velocity.DeserializeOffset(maxVelElement);
            }

            base.Deserialize(element);
        }

        public override void CopyInto(IBehavior newObject)
        {
            var behavior = newObject as PlayerPaddleBehavior;

            Debug.Assert(behavior != null);

            behavior.Velocity = Velocity;

            base.CopyInto(newObject);
        }
    }
}
