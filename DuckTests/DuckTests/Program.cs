using System;
using DuckGame;
using DuckEngine;

namespace DuckMain
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            //This is how everything used to be started
            Game game = new Game();
            game.Run();
        }
    }
#endif
}

