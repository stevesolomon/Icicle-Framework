using System;
using System.Collections.Generic;
using ExampleGame.Components.Score;
using IcicleFramework.Entities;
using IcicleFramework.GameServices;

namespace ExampleGame.GameSystems
{
    public delegate void ScoreChangedHandler(IScoreComponent source);

    public class ScoreManager : GameService, IScoreManager
    {
        protected Dictionary<Guid, IScoreComponent> scores;

        protected Dictionary<Guid, List<ScoreChangedHandler>> scoreChangedSubscribers;
        
        public ScoreManager()
        {
            scores = new Dictionary<Guid, IScoreComponent>();
            scoreChangedSubscribers = new Dictionary<Guid, List<ScoreChangedHandler>>();
        }

        public override void Initialize()
        {
            var gameObjectManager = GameServiceManager.GetService<IGameObjectManager>();

            if (gameObjectManager != null)
            {
                gameObjectManager.OnGameObjectAdded += OnGameObjectAdded;
                gameObjectManager.OnGameObjectRemoved += OnGameObjectRemoved;
            }
        }

        protected void OnGameObjectAdded(IGameObject newObject)
        {
            //Register the score component if the IGameObject has one.
            var component = newObject.GetComponent<IScoreComponent>();

            if (component != null)
            {
                scores.Add(newObject.GUID, component);
            }
        }

        private void OnGameObjectRemoved(IGameObject removedObject)
        {
            if (removedObject == null) 
                return;

            if (scores.ContainsKey(removedObject.GUID))
            {
                scores.Remove(removedObject.GUID);
            }

            if (scoreChangedSubscribers.ContainsKey(removedObject.GUID))
            {
                scoreChangedSubscribers.Remove(removedObject.GUID);
            }
        }

        protected void OnScoreChanged(IScoreComponent source, float newPoints)
        {
            //Notify anyone subscribed to this object's score!
            if (scoreChangedSubscribers.ContainsKey(source.Parent.GUID))
            {
                for (var i = 0; i < scoreChangedSubscribers[source.Parent.GUID].Count; i++)
                {
                    scoreChangedSubscribers[source.Parent.GUID][i](source);
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
            var score = -1.0f;

            if (scores.ContainsKey(gameObject.GUID))
                score = scores[gameObject.GUID].Score;

            return score;
        }
        
        public void SubscribeToScoreChanged(IGameObject objectOfInterest, ScoreChangedHandler scoreChangedDelegate)
        {
            if (!scoreChangedSubscribers.ContainsKey(objectOfInterest.GUID))
                scoreChangedSubscribers.Add(objectOfInterest.GUID, new List<ScoreChangedHandler>());

            scoreChangedSubscribers[objectOfInterest.GUID].Add(scoreChangedDelegate);
        }

    }
}
