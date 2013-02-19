using System;

namespace IcicleFramework.GameServices.HelperServices
{
    public class RandomGenerator : GameService, IRandomGenerator
    {
        private Random random;

        public override void Initialize()
        {
            random = new Random();
        }

        public int GenerateRandomInt()
        {
            return random.Next();
        }

        public int GenerateRandomInt(int min, int max)
        {
            return random.Next(min, max);
        }

        public float GenerateRandomFloat()
        {
            return (float) random.NextDouble() * float.MaxValue;
        }

        public float GenerateRandomFloat(float min, float max)
        {
            return (float) (min + (random.NextDouble() * (max - min)));
        }
    }
}
