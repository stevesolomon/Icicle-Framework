using IcicleFramework.Components;

namespace ExampleGame.Components.Score
{
    //public delegate void ScoreChangedHandler(IScoreComponent source, float change);

    public interface IScoreComponent : IBaseComponent
    {
        //event ScoreChangedHandler ScoreChanged;

        /// <summary>
        /// Gets or sets the current score.
        /// </summary>
        float Score { get; }

        void AddPoints(float points);
    }
}
