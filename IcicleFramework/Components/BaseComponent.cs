using System;
using System.Xml.Linq;
using IcicleFramework.Entities;
using Microsoft.Xna.Framework;

namespace IcicleFramework.Components
{
    public abstract class BaseComponent : IBaseComponent
    {
        private bool destroyed;

        public event DestroyedHandler<IBaseComponent> OnDestroyed;

        public bool Destroyed
        {
            get { return destroyed; }
            set
            {
                destroyed = value;

                if (destroyed)
                {
                    OnComponentDestroyed();
                }
            }
        }

        public bool Unallocated { get { return Destroyed; } }

        public virtual bool Active { get; set; }

        public virtual IGameObject Parent { get; set; }

        public Type InterfaceType { get; set; }

        public Type ConcreteType { get; set; }

        public bool Paused { get; protected set; }
        
        public virtual void Initialize() {}

        public virtual void PostInitialize() {}

        public virtual void Update(GameTime gameTime)
        {}

        public virtual void Pause()
        {
            Paused = true;
        }

        public virtual void Resume()
        {
            Paused = false;
        }

        public abstract void Deserialize(XElement element);

        public virtual void Dispose()
        {
            Parent = null;
            Paused = false;
            Destroyed = false;
        }

        public virtual void CopyInto(IBaseComponent newObject)
        {
            newObject.InterfaceType = InterfaceType;
            newObject.ConcreteType = ConcreteType;
        }

        protected virtual void OnComponentDestroyed()
        {
            if (OnDestroyed != null)
            {
                OnDestroyed(this);
            }
        }
    }
}
