using System;
using Jitter.LinearMath;
using Jitter.Dynamics;
using DuckEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using DuckEngine.Helpers;
using DuckEngine.Interfaces;
using DuckEngine.Input;

namespace DuckGame
{
    public class Game : IInput
    {
        private Engine engine;
        private RigidBody box1;

        public Game() {
            engine = new Engine();
            setupEngine();
            setupWorld();
            engine.addInput(this);
        }

        public void Run()
        {
            engine.Run();
        }

        private void setupEngine()
        {
            engine.Window.AllowUserResizing = true;
        }

        private void setupWorld()
        {
            RigidBody ground = new Box(engine, new JVector(10,1,10)).Body;
            ground.IsStatic = true;
            ground.Tag = Color.LightGreen;
            box1 = new Box(engine, JVector.One).Body;
            box1.Position = new JVector(0, 10, 0);
            box1.Orientation = Conversion.ToJitterMatrix(
                Matrix.CreateFromYawPitchRoll(
                MathHelper.PiOver4,
                MathHelper.PiOver4,
                MathHelper.PiOver4));
            box1.Tag = Color.Green;

            Player player = new Player(engine);
            player.Body.Position = new JVector(3, 3, 3);
            player.Body.Tag = Color.Bisque;
        }

        public void Input(GameTime gameTime, InputManager input)
        {
            if (input.Keyboard_IsKeyDown(Keys.R))
            {
                box1.LinearVelocity.Normalize();
                box1.Position = new JVector(0, 4, 0);
                box1.IsActive = true;
            }
        }
    }
}
