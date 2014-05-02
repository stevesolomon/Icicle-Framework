using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;
using IcicleFramework.Actions;
using IcicleFramework.Behaviors;
using IcicleFramework.Components.EntityState;
using IcicleFramework.Entities;
using IcicleFramework.GameServices;
using IcicleFramework.GameServices.Factories;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace IcicleFramework.Components.Behaviors
{
    public class BehaviorComponent : BaseComponent, IBehaviorComponent
    {
        [JsonPropertyAttribute]
        private Dictionary<string, IBehavior> behaviors;

        protected List<IBehavior> behaviorsToRemove; 

        protected IEntityStateComponent stateComponent;

        protected IBehaviorFactory behaviorFactory;

        protected IActionManager actionManager;

        [JsonIgnore]
        public IEnumerator<IBehavior> Behaviors
        {
            get { return behaviors.Values.GetEnumerator(); }
        }

        public IActionFactory ActionFactory { get; protected set; }

        public BehaviorComponent()
        {
            behaviors = new Dictionary<string, IBehavior>(16);
            behaviorsToRemove = new List<IBehavior>(16);
        }

        public override void Initialize()
        {
            foreach (var behavior in behaviors)
                behavior.Value.Initialize();

            actionManager = GameServiceManager.GetService<IActionManager>();
            ActionFactory = GameServiceManager.GetService<IActionFactory>();
            behaviorFactory = GameServiceManager.GetService<IBehaviorFactory>();

            base.Initialize();
        }

        public override void PostInitialize()
        {
            //Track down the IEntityStateComponent for this IGameObject, if they exists.
            stateComponent = this.Parent.GetComponent<IEntityStateComponent>();

            foreach (var behavior in behaviors)
                behavior.Value.PostInitialize();

            base.PostInitialize();
        }

        public override void Update(GameTime gameTime)
        {
            //Update our Behavior component.
            foreach (var behavior in behaviors)
                behavior.Value.Update(gameTime);

            if (behaviorsToRemove.Count > 0)
            {
                foreach (var behavior in behaviorsToRemove)
                    behaviors.Remove(behavior.Name);

                behaviorsToRemove.Clear();
            }


            base.Update(gameTime);
        }

        public void FireAction(IGameAction action, IGameObject target, ActionCompletedCallback callback = null, float delay = 0f)
        {
            action.Parent = Parent;
            action.Target = target;

            if (delay > 0f)
            {
                actionManager.RegisterDelayedAction(action, delay, callback);    
            }
            else
            {
                actionManager.RegisterAction(action, callback);
            }
        }

        public bool RequestEntityStateChanged(IEntityState newState)
        {
            bool changed = false;

            if (stateComponent != null)
            {
                changed = stateComponent.ChangeState(newState);
            }

            return changed;
        }

        public IBehavior GetBehavior(string name)
        {
            if (behaviors.ContainsKey(name))
            {
                return behaviors[name];
            }

            return null;
        }

        public void AddBehavior(IBehavior behavior)
        {
            behavior.Parent = this;
            behaviors.Add(behavior.Name, behavior);
        }

        public void RemoveBehavior(IBehavior behavior)
        {
            behaviorsToRemove.Add(behavior);
        }

        public override void Deserialize(XElement element)
        {
            IEnumerable<XElement> behaviorElements = element.Element("behaviors").Elements("behavior");
            behaviorFactory = GameServiceManager.GetService<IBehaviorFactory>();

            foreach (var behaviorElement in behaviorElements)
            {
                var behavior = behaviorFactory.LoadBehaviorFromXML(behaviorElement);
                AddBehavior(behavior);
            }
        }

        public override void Reallocate()
        {
            //foreach (var behavior in behaviors)
            //{
            //    behavior.Value.Reallocate();
            //}

            //foreach (var behavior in behaviorsToRemove)
            //{
            //    behavior.Reallocate();
            //}

            //behaviors.Clear();
            //behaviorsToRemove.Clear();
            stateComponent = null;
            behaviorFactory = null;
            actionManager = null;

            base.Reallocate();
        }

        public override void Destroy()
        {
            foreach (var behavior in behaviors)
            {
                behavior.Value.Destroy();
            }

            foreach (var behavior in behaviorsToRemove)
            {
                behavior.Destroy();
            }

            behaviors.Clear();
            behaviorsToRemove.Clear();

            base.Destroy();
        }

        public override void CopyInto(IBaseComponent newObject)
        {
            var behaviorComp = newObject as BehaviorComponent;

            Debug.Assert(behaviorComp != null, "behaviorComp != null");

            foreach (var behavior in behaviors.Values)
            {
                var newBehavior = behaviorFactory.GetBehavior(behavior.GetType());
                behavior.CopyInto(newBehavior);
                newBehavior.Parent = behaviorComp;

                behaviorComp.behaviors.Add(newBehavior.Name, newBehavior);
            }

            base.CopyInto(newObject);
        }
    }
}
