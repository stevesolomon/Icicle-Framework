using System.Globalization;
using System.Xml.Linq;
using IcicleFramework.Components;

namespace ExampleGame.Components.Score
{
    public class ScoreComponent : BaseComponent, IScoreComponent
    {
        public float Score { get; protected set; }

        public void AddPoints(float points)
        {
            Score += points;
        }

        public override void Deserialize(XElement element)
        {
            float score = 0.0f;
            if (element.Element("score") != null)
            {
                float.TryParse(element.Element("score").Value, NumberStyles.Float, CultureInfo.InvariantCulture,
                               out score);
            }
            this.Score = score;
        }
    }
}
