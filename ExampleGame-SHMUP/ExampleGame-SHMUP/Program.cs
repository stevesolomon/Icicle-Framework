using System;

namespace ExampleGameSHMUP
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (ExampleSHMUP game = new ExampleSHMUP())
            {
                game.Run();
            }
        }
    }
#endif
}

