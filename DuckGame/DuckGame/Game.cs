using System;
using Jitter.LinearMath;
using Jitter.Dynamics;
using DuckEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using DuckEngine.Helpers;
using DuckEngine.Interfaces;
using DuckEngine.Input;
using DuckGame.Maps;

namespace DuckGame
{
    public class Game : IInput
    {
        Engine engine;
        Map map;
        RigidBody box1;

        public Game() {
            engine = new Engine();
            engine.Window.AllowUserResizing = true;

            map = new TestMap1(engine);

            engine.addInput(this);
        }

        public void Run()
        {
            engine.Run();
        }

        public void Input(GameTime gameTime, InputManager input)
        {
            
        }
    }
}
