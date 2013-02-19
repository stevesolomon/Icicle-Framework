using System;
using System.Collections.Generic;
using ExampleGame.Components.Score;
using IcicleFramework.Entities;
using IcicleFramework.GameServices;
using IcicleFramework.GameServices.Factories;

namespace ExampleGame.GameSystems
{
    public delegate void ScoreChangedHandler(IScoreComponent source);

    public class ScoreManager : GameService, IScoreManager
    {
        protected Dictionary<Guid, IScoreComponent> scores;

        protected Dictionary<Guid, List<ScoreChangedHandler>> scoreChangedSubscribers;
        
        public ScoreManager()
        {
            this.scores = new Dictionary<Guid, IScoreComponent>();
            this.scoreChangedSubscribers = new Dictionary<Guid, List<ScoreChangedHandler>>();
        }

        public override void Initialize()
        {
            IGameObjectFactory factory = GameServiceManager.GetService<IGameObjectFactory>();

            if (factory != null)
                factory.OnGameObjectCreated += OnGameObjectCreated;
        }

        protected void OnGameObjectCreated(IGameObject newObject)
        {
            //Register the score component if the IGameObject has one.
            IScoreComponent component = newObject.GetComponent<IScoreComponent>();

            if (component != null)
            {
                this.scores.Add(newObject.GUID, component);
            }

            newObject.OnDestroyed += OnGameObjectDestroyed;
        }

        private void OnGameObjectDestroyed(object sender)
        {
            var gameObject = sender as IGameObject;

            if (gameObject == null) 
                return;

            if (scores.ContainsKey(gameObject.GUID))
            {
                scores.Remove(gameObject.GUID);
            }

            if (scoreChangedSubscribers.ContainsKey(gameObject.GUID))
            {
                scoreChangedSubscribers.Remove(gameObject.GUID);
            }
        }

        protected void OnScoreChanged(IScoreComponent source, float newPoints)
        {
            //Notify anyone subscribed to this object's score!
            if (this.scoreChangedSubscribers.ContainsKey(source.Parent.GUID))
            {
                for (int i = 0; i < this.scoreChangedSubscribers[source.Parent.GUID].Count; i++)
                {
                    this.scoreChangedSubscribers[source.Parent.GUID][i](source);
                }
            }
        }

        public void AddPoints(IGameObject target, float points)
        {
            if (target != null && scores.ContainsKey(target.GUID))
            {
                var scoreComp = scores[target.GUID];

                scoreComp.AddPoints(points);
                OnScoreChanged(scoreComp, scores[target.GUID].Score);
            }
        }

        public float GetScore(IGameObject gameObject)
        {
            float score = -1.0f;

            if (scores.ContainsKey(gameObject.GUID))
                score = scores[gameObject.GUID].Score;

            return score;
        }
        
        public void SubscribeToScoreChanged(IGameObject objectOfInterest, ScoreChangedHandler scoreChangedDelegate)
        {
            if (!this.scoreChangedSubscribers.ContainsKey(objectOfInterest.GUID))
                this.scoreChangedSubscribers.Add(objectOfInterest.GUID, new List<ScoreChangedHandler>());

            this.scoreChangedSubscribers[objectOfInterest.GUID].Add(scoreChangedDelegate);
        }

    }
}
