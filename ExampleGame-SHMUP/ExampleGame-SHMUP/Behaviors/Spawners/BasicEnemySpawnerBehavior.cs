using System.Diagnostics;
using System.Globalization;
using System.Xml.Linq;
using ExampleGameSHMUP.Actions.Spawning;
using IcicleFramework.Behaviors;
using IcicleFramework.Components.Behaviors;
using IcicleFramework.GameServices;
using IcicleFramework.GameServices.HelperServices;
using Microsoft.Xna.Framework;

namespace ExampleGameSHMUP.Behaviors.Spawners
{
    public class BasicEnemySpawnerBehavior : BaseBehavior
    {
        public float SpawnTimeMinimum { get; set; }

        public float SpawnTimeMaximum { get; set; }

        public float MinXSpawnPosition { get; set; }

        public float MaxXSpawnPosition { get; set; }

        public float MinYSpawnPosition { get; set; }

        public float MaxYSpawnPosition { get; set; }

        public string EnemyName { get; set; }

        private float timeToNextSpawn;

        private IRandomGenerator randomGenerator;

        public BasicEnemySpawnerBehavior()
        {
            SpawnTimeMinimum = float.MaxValue;
            SpawnTimeMaximum = float.MaxValue;
        }

        public override void Initialize()
        {
            randomGenerator = GameServiceManager.GetService<IRandomGenerator>();
            ResetTimer();

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            timeToNextSpawn -= (float) gameTime.ElapsedGameTime.TotalSeconds;

            if (timeToNextSpawn <= 0f)
            {
                SpawnEnemy();
                ResetTimer();
            }

            base.Update(gameTime);
        }

        protected virtual void SpawnEnemy()
        {
            Vector2 spawnPosition;

            spawnPosition.X = randomGenerator.GenerateRandomFloat(MinXSpawnPosition, MaxXSpawnPosition);
            spawnPosition.Y = randomGenerator.GenerateRandomFloat(MinYSpawnPosition, MaxYSpawnPosition);

            var spawnAction = Parent.ActionFactory.GetAction<SpawnEnemyAction>();
            spawnAction.EnemyName = EnemyName;
            spawnAction.SpawnPosition = spawnPosition;

            Parent.FireAction(spawnAction, null);
        }

        private void ResetTimer()
        {
            timeToNextSpawn = randomGenerator.GenerateRandomFloat(SpawnTimeMinimum, SpawnTimeMaximum);
        }

        public override void CopyInto(IBehavior newObject)
        {
            var spawner = newObject as BasicEnemySpawnerBehavior;

            Debug.Assert(spawner != null);

            spawner.SpawnTimeMinimum = SpawnTimeMinimum;
            spawner.SpawnTimeMaximum = SpawnTimeMaximum;
            spawner.MinXSpawnPosition = MinXSpawnPosition;
            spawner.MaxXSpawnPosition = MaxXSpawnPosition;
            spawner.MinYSpawnPosition = MinYSpawnPosition;
            spawner.MaxYSpawnPosition = MaxYSpawnPosition;
            spawner.EnemyName = EnemyName;

            base.CopyInto(newObject);
        }

        public override void Deserialize(XElement element)
        {
            float floatValue;
            var floatElement = element.Element("spawnTimeMinimum");
            if (floatElement != null)
            {
                float.TryParse(floatElement.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out floatValue);
                SpawnTimeMinimum = floatValue;
            }

            floatElement = element.Element("spawnTimeMaximum");
            if (floatElement != null)
            {
                float.TryParse(floatElement.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out floatValue);
                SpawnTimeMaximum = floatValue;
            }

            floatElement = element.Element("minXSpawnPosition");
            if (floatElement != null)
            {
                float.TryParse(floatElement.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out floatValue);
                MinXSpawnPosition = floatValue;
            }

            floatElement = element.Element("maxXSpawnPosition");
            if (floatElement != null)
            {
                float.TryParse(floatElement.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out floatValue);
                MaxXSpawnPosition = floatValue;
            }

            floatElement = element.Element("minYSpawnPosition");
            if (floatElement != null)
            {
                float.TryParse(floatElement.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out floatValue);
                MinYSpawnPosition = floatValue;
            }

            floatElement = element.Element("maxYSpawnPosition");
            if (floatElement != null)
            {
                float.TryParse(floatElement.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out floatValue);
                MaxYSpawnPosition = floatValue;
            }

            var nameElement = element.Element("enemyName");
            if (nameElement != null)
            {
                EnemyName = nameElement.Value;
            }

            base.Deserialize(element);
        }
    }
}
