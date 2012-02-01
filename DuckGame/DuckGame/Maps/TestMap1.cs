using DuckEngine;
using DuckEngine.Helpers;
using DuckEngine.Interfaces;
using DuckGame.Pickups;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Xml;
using System;

namespace DuckGame.Maps
{
    class TestMap1 : Map, IInput
    {
        RigidBody box1;

        public TestMap1(Engine _engine, String _fromFile)
            : base(_engine, _fromFile)
        {
            //Ground
            RigidBody ground = new Box(Engine, Tracker, new JVector(10, 4, 10)).Body;
            ground.IsStatic = true;

            //Falling box
            box1 = new Box(Engine, Tracker, JVector.One).Body;
            box1.Position = new JVector(0, 13, 0);
            box1.Orientation = Conversion.ToJitterMatrix(
                Matrix.CreateFromYawPitchRoll(
                    MathHelper.PiOver4,
                    MathHelper.PiOver4,
                    MathHelper.PiOver4
                )
            );

            //Pickup
            Pickup pickup1 = new Pickup(Engine, Tracker, new JVector(-3f, 3f, -3f));

            //Map
            Terrain terrain = new Terrain(Engine, Tracker, "heightmap");
        }

        public void Input(GameTime gameTime, DuckEngine.Input.InputManager input)
        {
            if (input.Keyboard_IsKeyDown(Keys.R))
            {
                //Reset box
                box1.LinearVelocity.Normalize();
                box1.Position = new JVector(0, 4, 0);
                box1.IsActive = true;
            }
        }

        public override void Save(XmlDocument doc, XmlElement currNode)
        {
        }

        public static TestMap1 Load(Engine engine, String fromFile, XmlNode node)
        {
            return new TestMap1(engine, fromFile);
        }
    }
}
