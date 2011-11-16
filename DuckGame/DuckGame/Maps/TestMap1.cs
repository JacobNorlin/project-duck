using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using DuckEngine.Helpers;
using DuckEngine;
using DuckEngine.Interfaces;
using Microsoft.Xna.Framework.Input;
using DuckGame.Players;
using DuckGame.Pickups;
using DuckEngine.Maps;

namespace DuckGame.Maps
{
    class TestMap1 : Map, IInput
    {
        RigidBody box1;

        public TestMap1(Engine _owner)
            : base(_owner)
        {
            //Ground
            RigidBody ground = new Box(Owner, new JVector(10, 1, 10)).Body;
            ground.IsStatic = true;

            //Falling box
            box1 = new Box(Owner, JVector.One).Body;
            box1.Position = new JVector(0, 10, 0);
            box1.Orientation = Conversion.ToJitterMatrix(
                Matrix.CreateFromYawPitchRoll(
                MathHelper.PiOver4,
                MathHelper.PiOver4,
                MathHelper.PiOver4));

            //Player*/
            Player player = new LocalPlayer(Owner, new Vector3(3f, 3f, 3f));

            //Pickup
            Pickup pickup1 = new Pickup(Owner, new Vector3(-3f, 1f, -3f));

            //Map
            Terrain terrain = new Terrain(Owner, "heightmap");
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
    }
}
