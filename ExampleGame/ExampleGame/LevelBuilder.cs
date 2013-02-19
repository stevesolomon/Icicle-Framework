using System;
using System.Collections.Generic;
using ExampleGame.GameSystems;
using IcicleFramework;
using IcicleFramework.GameServices;
using IcicleFramework.GameServices.Factories;
using IcicleFramework.Inputs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using IcicleFramework.Entities;
using TiledLib;

namespace ExampleGame
{
    /// <summary>
    /// Constructs a new Level based on a level definition file.
    /// </summary>
    public class LevelBuilder : GameService, IGameService
    {
        public  Vector2 ballStartLocation;
        private TiledGameObjectFactory factory;
        
        public override void Initialize()
        {
            factory = (TiledGameObjectFactory) GameServiceManager.GetService(typeof(IGameObjectFactory));
        }

        /// <summary>
        /// Builds a new level corresponding to the level name provided.
        /// </summary>
        /// <param name="content">The ContentManager used to load content for this game.</param>
        /// <param name="levelName">The name of the associated level file for the level to be constructed.</param>
        /// <returns>The constructed Level, or null if the Level could not be built.</returns>
        public Level BuildLevel(ContentManager content, string levelName)
        {
            Map map;
            TileLayer tileLayer;
            MapObjectLayer objectLayer;
            Level level = null;
            int i;

            List<IGameObject> gameObjects = new List<IGameObject>(64);
            
            map = content.Load<Map>(levelName);

            if (map != null && factory != null)
            {
                level = new Level();
                level.Initialize(new RectangleF(0,0,1280,720));
                
                //Build the interactive layer of the environment.
                tileLayer = map.GetLayer("interactive") as TileLayer;

                if (tileLayer != null)
                {
                    var tiles = factory.GenerateXMLEnvironmentObjectsFromTiled(map, tileLayer);
                    foreach (var tile in tiles)
                    {
                        tile.Active = true;
                        tile.PostInitialize();
                    }
                }

                //Then go and build up all of the objects in the map.
                objectLayer = map.GetLayer("objects") as MapObjectLayer;

                if (objectLayer != null)
                {
                    for (i = 0; i < objectLayer.Objects.Count; i++)
                    {
                        string name = objectLayer.Objects[i].Type;

                        //The ball start is special. We need to create the ball at  this location but also pass it to the game logic service.
                        if (objectLayer.Objects[i].Name.Equals("ballStart", StringComparison.InvariantCultureIgnoreCase))
                        {
                            name = objectLayer.Objects[i].Properties["objectName"].RawValue;
                            ballStartLocation = new Vector2(objectLayer.Objects[i].Bounds.Location.X, objectLayer.Objects[i].Bounds.Location.Y);
                        }

                        IGameObject gameObject = factory.GetGameObject(name, objectLayer.Objects[i].Bounds.X,
                                                          objectLayer.Objects[i].Bounds.Y);
                        gameObjects.Add(gameObject);

                        if (objectLayer.Objects[i].Name.Equals("gameover"))
                            gameObject.Active = false;
                        else if (objectLayer.Objects[i].Name.Equals("youwin"))
                            gameObject.Active = false;
                        else
                            gameObject.Active = true;

                        if (gameObject.HasMetadata("player"))
                        {
                            PlayerManager manager = (PlayerManager) GameServiceManager.GetService(typeof (PlayerManager));
                            Player player = manager.GetPlayer(LogicalPlayerIndex.One);

                            gameObject.UpdateMetadata("player", player);
                        }
                    }
                }

                GameServiceManager.PostInitialize();

                foreach (var gameObject in gameObjects)
                {
                    gameObject.PostInitialize();
                }
                
            }
            
            return level;
        }
       
    }
}
