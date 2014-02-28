using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using IcicleFramework.Components.Renderable;
using IcicleFramework.Entities;
using IcicleFramework.GameServices;
using IcicleFramework.GameServices.Factories;
using IcicleFramework.Renderables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using TiledLib;

namespace ExampleGame.GameSystems
{
    public class TiledGameObjectFactory : GameObjectFactory
    {
        public TiledGameObjectFactory(ContentManager content)
            : base (content) { }

        /// <summary>
        /// Just a quick and dirty method for generating any object in our Tiled map that has a corresponding entity definition
        /// in an XML file.
        /// </summary>
        public List<IGameObject> GenerateXMLEnvironmentObjectsFromTiled(Map map, TileLayer layer)
        {
            List<IGameObject> gameObjects = new List<IGameObject>();

            for (int i = 0; i < layer.Width; i++)
            {
                for (int j = 0; j < layer.Height; j++)
                {
                    if (layer.Tiles[i, j] != null)
                    {
                        IGameObject gameObject = rawPool.New();
                        gameObject.Position = new Vector2(map.TileWidth * i, map.TileHeight * j);

                        //Load in the correct element from the entity definitions file.
                        string objectName = layer.Tiles[i, j].Properties["name"].RawValue;

                        if (xmlDoc.Root != null)
                        {
                            List<XElement> elements = (xmlDoc.Root.Elements("entity")
                                .Where(el => (string)el.Attribute("name") == objectName)).ToList();

                            //We'll let the IRenderableComponent build itself but before finishing it off we'll swap the texture, so just
                            //generate all the components via the Tiled file...
                            if (elements.Count > 0)
                                GenerateComponents(gameObject, elements[0]);
                        }

                        //And then swap the texture with the one from Tiled...
                        IRenderComponent renderComponent = gameObject.GetComponent<IRenderComponent>();

                        if (renderComponent != null)
                        {
                            ITexturedRenderable renderable = (ITexturedRenderable)renderComponent.GetRenderable(0);

                            if (renderable != null)
                            {
                                renderable.AssetName = layer.Tiles[i, j].Properties["texture"].RawValue;
                                renderComponent.Load(content);
                                renderComponent.SetLayer(1f);
                            }
                        }

                        FireOnGameObjectCreated(gameObject);
                        gameObjects.Add(gameObject);

                        gameObject.Initialize();
                    }
                }
            }

            return gameObjects;
        }
    }
}
