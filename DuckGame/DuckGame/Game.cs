using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jitter.LinearMath;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using DuckEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using DuckEngine.Helpers;

namespace DuckGame
{
    public class Game : ILogic, IInput
    {
        private DuckEngine engine;
        private RigidBody box1;

        public Game() {
            engine = new DuckEngine();
            setupEngine();
            engine.addLogic(this);
            engine.addInput(this);
            setupWorld();
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
            Console.WriteLine(Conversion.ToXNAMatrix(box1.Orientation));
            box1.Tag = Color.Green;
        }

        public void Update(GameTime gameTime)
        {
            Console.WriteLine(box1.Position);
            //if (gameTime.TotalGameTime.Seconds > 5) engine.Exit();
        }

        public void Input(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Console.WriteLine("ESC down");
        }
    }
}
