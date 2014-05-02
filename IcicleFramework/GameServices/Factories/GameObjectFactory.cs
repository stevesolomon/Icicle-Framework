using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using IcicleFramework.Components;
using IcicleFramework.Entities;
using IcicleFramework.Pooling;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace IcicleFramework.GameServices.Factories
{
    public class GameObjectFactory : GameService, IGameObjectFactory
    {
        public event GameObjectCreatedHandler OnGameObjectCreated;

        protected PoolNew<IGameObject> rawPool;

        protected Dictionary<string, IGameObject> gameObjectTemplates; 

        protected ContentManager content;

        protected XDocument xmlDoc;

        protected string pathToXML;

        protected string fullXMLPath;

        protected IComponentFactory ComponentFactory { get; set; }
        
        public string PathToXML
        {
            get { return pathToXML; }
            set
            {
                pathToXML = value;

                fullXMLPath = Path.Combine(content.RootDirectory, pathToXML);
                xmlDoc = XDocument.Load(fullXMLPath);
            }
        }

        public GameObjectFactory(ContentManager content)
        {
            this.content = content;
        }

        public override void Initialize()
        {
            ComponentFactory = (IComponentFactory) GameServiceManager.GetService(typeof (IComponentFactory));
            
            rawPool = new PoolNew<IGameObject>(typeof(GameObject), 1024);

            gameObjectTemplates = new Dictionary<string, IGameObject>();
        }

        protected virtual void FireOnGameObjectCreated(IGameObject newGameObject)
        {
            if (OnGameObjectCreated != null)
                OnGameObjectCreated(newGameObject);
        }

        protected void GenerateComponents(IGameObject gameObject, XElement element)
        {
            if (element.Element("components") != null && ComponentFactory != null)
            {
                List<XElement> componentElements = element.Element("components").Elements("component").ToList();

                //Run through all the components defined in the XElement and create them.
                for (int i = 0; i < componentElements.Count; i++)
                {
                    //Instantiate the component itself.
                    IBaseComponent component = ComponentFactory.LoadComponentFromXML(componentElements[i]);

                    //Finally, add the component to the provided IGameObject.
                    if (component != null && component.InterfaceType != null)
                        gameObject.AddComponent(component.InterfaceType, component);
                }
            }
        }

        private void OnGameObjectInitialize(IGameObject sender)
        {
            //Unsubscribe from the event...
            sender.OnInitialize -= OnGameObjectInitialize;

            //Fire our own event!
            FireOnGameObjectCreated(sender);
        }

        public IGameObject CopyExistingGameObject(IGameObject toCopy)
        {
            var gameObject = GetRawGameObject();
            gameObject.Active = false;
            toCopy.CopyInto(gameObject);
            gameObject.Initialize();

            return gameObject;
        }

        public IGameObject GetRawGameObject()
        {
            var gameObject = new GameObject();//rawPool.New();

            //Subscribe to the OnInitialize event for this game object so we know when it's been "created" and ready to go.
            gameObject.OnInitialize += OnGameObjectInitialize;

            return gameObject;
        }

        public IGameObject GetGameObject(string name, float x = 0f, float y = 0f, bool usedAsTemplate = false)
        {
            //If we don't already have a template then generate one first
            if (!gameObjectTemplates.ContainsKey(name))
            {
                var generated = GenerateTemplate(name);

                if (!generated)
                {
                    throw new Exception(string.Format("Could not find a definition for the named game object: {0}", name));
                }
            }

            var template = gameObjectTemplates[name];
            var newObject = new GameObject();//rawPool.New();

            template.CopyInto(newObject);
            newObject.Position = new Vector2(x, y);

            newObject.Initialize();

            if (!usedAsTemplate)
                FireOnGameObjectCreated(newObject);

            return newObject;
        }

        protected bool GenerateTemplate(string name)
        {
            IGameObject template = null;
            bool generated = false;

            if (xmlDoc != null)
            {
                template = new GameObject();//rawPool.New();

                 //Find the element corresponding to the object name, if it exists...
                if (xmlDoc.Root != null)
                {
                    List<XElement> elementList = (xmlDoc.Root.Elements("entity")
                        .Where(el => (string)el.Attribute("name") == name)).ToList();

                    //Just grab the first element if there was one.
                    if (elementList.Count > 0)
                    {
                        template.Deserialize(elementList[0]);
                        GenerateComponents(template, elementList[0]);
                    }
                }
            }

            if (template != null)
            {
                template.Active = false;

                //Now that the object and all of its components have been created, and the subsystems have been notified,
                //run the 'burn in' phase by 'Initializing' them.
                template.Initialize();

                gameObjectTemplates.Add(name, template);

                generated = true;
            }

            return generated;
        }

        public override void Update(GameTime gameTime)
        {
            //rawPool.CleanUp();

            base.Update(gameTime);
        }
    }
}
