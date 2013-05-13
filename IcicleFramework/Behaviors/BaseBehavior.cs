using System;
using System.Xml.Linq;
using IcicleFramework.Components.Behaviors;
using IcicleFramework.Entities;
using Microsoft.Xna.Framework;

namespace IcicleFramework.Behaviors
{
    public abstract class BaseBehavior : IBehavior
    {
        private bool destroyed;

        public IBehaviorComponent Parent { get; set; }
        
        public IGameObject ParentGameObject
        {
            get { return Parent.Parent; }
        }

        public event DestroyedHandler<IBehavior> OnDestroyed;

        public bool Destroyed
        {
            get { return destroyed;  }
            set
            {
                destroyed = value;

                if (destroyed)
                {
                    OnBehaviorDestroyed();
                }
            }
        }

        public bool Unallocated { get { return Destroyed; } }

        public Type ConcreteType { get; set; }

        public bool Active { get; set; }

        public string Name { get; set; }

        public bool Paused { get; protected set; }

        public virtual void Deserialize(XElement element) { }

        public void Pause()
        {
            Paused = true;
        }

        public void Resume()
        {
            Paused = false;
        }

        public virtual void Initialize() { }

        public virtual void PostInitialize() { }

        public virtual void Update(GameTime gameTime) { }

        public virtual void Dispose()
        {
            Parent = null;
            Destroyed = false;
        }

        public virtual void CopyInto(IBehavior newObject)
        {
            newObject.Name = Name;
        }

        protected virtual void OnBehaviorDestroyed()
        {
            if (OnDestroyed != null)
            {
                OnDestroyed(this);
            }
        }
    }
}
