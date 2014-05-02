using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Xml.Linq;
using IcicleFramework.Behaviors;
using IcicleFramework.Components.Behaviors;
using IcicleFramework.Pooling;
using Microsoft.Xna.Framework;

namespace IcicleFramework.GameServices.Factories
{
    public class BehaviorFactory : GameService, IBehaviorFactory
    {
        protected Dictionary<string, IBehavior> templateBehaviors;
 
        protected MultiTypePool<IBehavior> behaviorPools;

        public BehaviorFactory()
        {
            behaviorPools = new MultiTypePool<IBehavior>();
            templateBehaviors = new Dictionary<string, IBehavior>();
        }

        public bool PreloadBehavior<T>() where T : class, IBehavior
        {
            return PreloadBehavior(typeof (T));
        }

        public bool PreloadBehavior(Type type)
        {
            var loaded = true;

            if (!behaviorPools.HasPool(type))
            {
                behaviorPools.CreatePool(type);
            }

            return loaded;
        }

        public T GetBehavior<T>(string templateName = "") where T : class, IBehavior
        {
            return GetBehavior(typeof(T), templateName) as T;
        }

        public IBehavior GetBehavior(Type type, string templateName = "")
        {
            //var constructor = type.GetConstructor(
            //    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            //    null,
            //    new Type[] { },
            //    null
            //);

            //var newComponent = constructor.Invoke(null) as IBehavior;
            var newComponent = behaviorPools.New(type);

            //If we have a named template then try to find it and copy it into our new behavior.
            if (!string.IsNullOrWhiteSpace(templateName))
            {
                var key = Helper.GetLookupName(type, templateName);

                if (templateBehaviors.ContainsKey(key))
                {
                    var template = templateBehaviors[key];

                    template.CopyInto(newComponent);
                }
#if DEBUG
                else
                {
                    Trace.WriteLine("Could not find Behavior Template: " + key);
                }
#endif
            }

            return newComponent;
        }

        public override void Update(GameTime gameTime)
        {
           // behaviorPools.CleanUp();
            base.Update(gameTime);
        }

        public IBehavior LoadBehaviorFromXML(XElement element)
        {
            XAttribute classAttrib = element.Attribute("class"),
                       typeAttrib = element.Attribute("type"),
                       nameAttrib = element.Attribute("name") ;

            IBehavior behavior = null;

            if (classAttrib != null && typeAttrib != null)
            {
                string assembly;

                if (element.Attribute("assembly") != null)
                    assembly = element.Attribute("assembly").Value;
                else
                {
                    string[] temp = classAttrib.Value.Split('.');
                    assembly = temp[0];
                }

                var classType = Type.GetType(String.Format("{0},{1}", classAttrib.Value, assembly));
                var stringInterface = element.Attribute("type").Value;

                behavior = GetBehavior(classType);
                behavior.Deserialize(element);

                behavior.Name = nameAttrib != null ? nameAttrib.Value : Guid.NewGuid().ToString();
            }

            return behavior;
        }

        public bool SaveBehaviorAsTemplate(IBehavior behavior, string templateName, bool replaceExisting = false)
        {
            var key = Helper.GetLookupName(behavior.ConcreteType, templateName);
            var saved = false;

            if (!templateBehaviors.ContainsKey(key))
            {
                var template = GetBehavior(behavior.ConcreteType);

                behavior.CopyInto(template);
                
                templateBehaviors.Add(key, template);

                saved = true;

            }
            else if (replaceExisting)
            {
                var template = templateBehaviors[key];
                template.Reallocate();

                behavior.CopyInto(template);

                templateBehaviors[key] = template;

                saved = true;
            }

            return saved;
        }
    }
}
