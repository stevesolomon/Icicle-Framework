using System;

namespace TestBed
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (TestBed game = new TestBed())
            {
                game.Run();
            }
        }
    }
#endif
}

