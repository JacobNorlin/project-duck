using DuckEngine;
using DuckGame;

namespace DuckTests
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Engine engine = new Engine(new GameStartup()))
            {
                engine.Run();
            }
        }
    }
#endif
}