using System.Collections.Generic;
using IcicleFramework.Components.Renderable;
using IcicleFramework.Entities;
using IcicleFramework.GameServices;
using IcicleFramework.GameServices.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IcicleFramework.Rendering
{
    public class Renderer : GameService, IRenderer
    {
        private ResolutionManager resolutionManager;

        private GraphicsDevice graphics;

        private SpriteBatch spriteBatch;

        private BlendState blendState;

        private SamplerState samplerState;

        private DepthStencilState depthStencilState;

        private RasterizerState rasterizerState;

        private Effect effect;

        private Matrix transformMatrix;

        private int screenWidth;

        private int screenHeight;

        protected IGameObjectFactory gameObjectFactory;

        protected List<IRenderComponent> renderComponents; 
        
        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }

        public bool Fullscreen { get; protected set; }

        public SpriteSortMode SpriteSortMode { get; set; }

        public Matrix ResolutionTransformMatrix
        {
            get { return resolutionManager.GetTransformationMatrix(); }
        }

        public Renderer(GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
        {
            this.graphics = graphics.GraphicsDevice;
            this.SpriteSortMode = SpriteSortMode.Deferred;
            this.spriteBatch = spriteBatch;
            this.transformMatrix = Matrix.Identity;

            this.resolutionManager = new ResolutionManager();
            this.resolutionManager.Initialize(ref graphics);

            RasterizerState state = new RasterizerState();
            state.FillMode = FillMode.WireFrame;
            spriteBatch.GraphicsDevice.RasterizerState = state;

            this.blendState = BlendState.AlphaBlend;
            this.samplerState = this.graphics.SamplerStates[0];
            this.depthStencilState = this.graphics.DepthStencilState;
           // this.rasterizerState = graphics.RasterizerState;
            this.effect = null;

            renderComponents = new List<IRenderComponent>();
        }

        public void SetInternalResolution(int width, int height)
        {
            resolutionManager.SetVirtualResolution(width, height);
        }
        
        public void SetScreenResolution(int width, int height, bool fullscreen)
        {
            screenWidth = width;
            screenHeight = height;
            Fullscreen = fullscreen;
            resolutionManager.SetResolution(width, height, Fullscreen);
        }

        public void Draw()
        {
            if (renderComponents.Count == 0) return;

            foreach (var renderComponent in renderComponents)
            {
                renderComponent.Draw(spriteBatch);
            }
        }

        public void ClearScreen()
        {
            graphics.Clear(Color.Black);
        }

        public void PrepareDraw(Matrix view)
        {
            resolutionManager.BeginDraw();
            spriteBatch.Begin(SpriteSortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, view * resolutionManager.GetTransformationMatrix());
        }

        public void EndDraw()
        {
            spriteBatch.End();
        }

        public override void Initialize()
        {
            //Subscribe to the game object created event on the GameObjectFactory.
            gameObjectFactory = GameServiceManager.GetService<IGameObjectFactory>();
            gameObjectFactory.OnGameObjectCreated += OnGameObjectCreated;
        }

        private void OnGameObjectCreated(IGameObject newObject)
        {
            IRenderComponent renderComponent = newObject.GetComponent<IRenderComponent>();

            if (renderComponent != null)
            {
                renderComponents.Add(renderComponent);

                newObject.OnDestroyed += OnGameObjectDestroyed;
            }
        }

        private void OnGameObjectDestroyed(IGameObject sender)
        {
            IRenderComponent renderComponent = sender.GetComponent<IRenderComponent>();

            if (renderComponent != null)
            {
                renderComponents.Remove(renderComponent);
            }
        }
    }
}
