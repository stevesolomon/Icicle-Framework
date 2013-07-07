using System;
using System.Reflection;
using System.Xml.Linq;
using IcicleFramework.Components;
using IcicleFramework.Pooling;
using Microsoft.Xna.Framework;

namespace IcicleFramework.GameServices
{
    /// <summary>
    /// A factory for generating generic components.
    /// It assumes that the components themselves can populate their fields
    /// based on some XML definition for the given component.
    /// </summary>
    public class ComponentFactory : GameService, IComponentFactory
    {
        protected MultiTypePool<IBaseComponent> componentPool; 

        public int PoolSize { get; set; }

        public ComponentFactory()
        {
            componentPool = new MultiTypePool<IBaseComponent>(128);

            PoolSize = 512;
        }

        public virtual bool PreLoadComponent<T>() where T : class, IBaseComponent
        {
            var type = typeof(T);

            return LoadDefaultComponent(type);
        }

        public virtual bool PreLoadComponent(Type type)
        {
            return LoadDefaultComponent(type);
        }

        protected virtual bool LoadDefaultComponent(Type type)
        {
            var loaded = false;

            var component = (IBaseComponent)Activator.CreateInstance(type);

            component.ConcreteType = type;
            component.InterfaceType = type.GetTopLevelInterfaces()[0];

            loaded = true;

            return loaded;
        }

        public virtual T GetComponent<T>() where T : class, IBaseComponent
        {
            var type = typeof(T);

            var newComponent = GetComponent(type);

            return newComponent as T;
        }

        public IBaseComponent GetComponent(Type type)
        {
            var constructor = type.GetConstructor(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new Type[] { },
                null
            );

            //var component = componentPool.New(type);

            var component = constructor.Invoke(null) as IBaseComponent;
            
            return component;
        }

        public virtual IBaseComponent LoadComponentFromXML(XElement element)
        {
            return  DeserializeComponent(element);;
        }

        public override void Update(GameTime gameTime)
        {
            //componentPool.CleanUp();
            base.Update(gameTime);
        }
 
        protected virtual IBaseComponent DeserializeComponent(XElement element)
        {
            IBaseComponent component = null;

            //As long as we have a defined class and type, try to instantiate the corresponding component.
            if (element.Attribute("class") != null && element.Attribute("type") != null)
            {
                string assembly;

                if (element.Attribute("assembly") != null)
                {
                    assembly = element.Attribute("assembly").Value;
                }
                else
                {
                    string[] temp = element.Attribute("class").Value.Split('.');
                    assembly = temp[0];
                }

                Type classType = Type.GetType(String.Format("{0},{1}", element.Attribute("class").Value, assembly));
                string stringInterface = element.Attribute("type").Value;

                //Find the proper interface designated by the entity definition so we can store it properly in the GameObject...
                Type interfaceType = classType.GetInterface(stringInterface);

                component = GetComponent(classType);

                component.InterfaceType = interfaceType;
                component.ConcreteType = classType;

                component.Deserialize(element);
            }

            return component;
        }
    }
}
