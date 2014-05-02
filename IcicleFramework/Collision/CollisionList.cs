using System;
using System.Collections.Generic;
using IcicleFramework.Components;
using IcicleFramework.Components.Collision;
using IcicleFramework.Entities;
using IcicleFramework.Pooling;

namespace IcicleFramework.Collision
{
    public class CollisionList : List<ICollisionComponent>, IPoolable<CollisionList>, IDisposable
    {
        private bool destroyed;

        public ICollisionComponent Source { get; set; }

        public bool Destroyed
        {
            get { return destroyed; }
            set
            {
                destroyed = value;

                if (destroyed)
                {
                    OnCollisionDictionaryDestroyed();
                }
            }
        }

        public event DestroyedHandler<CollisionList> OnDestroyed;

        public new void Add(ICollisionComponent value)
        {
            if (((IBaseComponent) value).Parent.GUID.Equals(((IBaseComponent) Source).Parent.GUID))
                return;

            base.Add(value);
        }

        public void Dispose()
        {
            Source = null;
            Clear();
        }

        protected void OnCollisionDictionaryDestroyed()
        {
            if (OnDestroyed != null)
            {
                OnDestroyed(this);
            }
        }


        public virtual void Reallocate()
        {
            Destroyed = false;
            Source = null;
        }

        public virtual void Destroy()
        {
            Destroyed = true;
        }
    }
}
