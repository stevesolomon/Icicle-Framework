using IcicleFramework.Entities;
using Microsoft.Xna.Framework;

namespace IcicleFramework.Actions
{
    public abstract class GameAction : IGameAction
    {
        public event OnGameActionFinishedHandler OnGameActionFinished;

        protected bool finished;

        protected bool destroyed;

        protected IGameObject target;

        public event DestroyedHandler<IGameAction> OnDestroyed;

        public virtual bool Finished
        {
            get { return finished; }
            protected set
            {
                finished = value;

                if (finished && OnGameActionFinished != null)
                    OnGameActionFinished(this);
            }
        }

        public virtual IGameObject Parent { get; set; }

        public virtual bool Destroyed
        {
            get { return destroyed; }
            set
            {
                destroyed = value;

                if (destroyed)
                {
                    GameActionDestroyed();
                }
            }
        }

        public virtual bool Paused { get; protected set; }

        public virtual IGameObject Target { get { return target; } set { target = value; } }

        public virtual void Initialize()
        {
            Finished = false;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (Finished)
                Destroyed = true;
        }

        public virtual void CopyInto(IGameAction newObject) { }

        public virtual void Dispose()
        {
            Destroyed = false;
            Finished = false;
        }

        protected virtual void GameActionDestroyed()
        {
            Finished = true;

            if (OnDestroyed != null)
            {
                OnDestroyed(this);
            }
        }

        #region IPauseable Methods

        public void Pause()
        {
            if (!Paused)
            {
                Paused = true;
            }
        }

        public void Resume()
        {
            if (Paused)
            {
                Paused = false;
            }
        }

        #endregion


        #region IPoolable Methods

        public virtual void Reallocate()
        {
            target = null;
            Parent = null;
            Finished = true;
            Destroyed = false;
        }

        public virtual void Destroy()
        {
            Destroyed = true;
        }

        #endregion
    }
}
