using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckEngine.Interfaces;
using Microsoft.Xna.Framework;
using DuckEngine.Input;
using Microsoft.Xna.Framework.Input;
using Jitter.LinearMath;

namespace DuckEngine
{
    class LocalPlayer : Player, IInput
    {
        public LocalPlayer(Engine _owner)
            : base(_owner)
        {
            Owner.addInput(this);
        }

        public void Input(GameTime gameTime, InputManager input)
        {
            //Movement
            Vector3 movement = Vector3.Zero;
            if (input.Keyboard_IsKeyDown(Keys.Up)) { movement.Z += 1; }
            if (input.Keyboard_IsKeyDown(Keys.Down)) { movement.Z -= 1; }
            if (input.Keyboard_IsKeyDown(Keys.Left)) { movement.X -= 1; }
            if (input.Keyboard_IsKeyDown(Keys.Right)) { movement.X += 1; }
            movement.Normalize();

            //Physics
            body.LinearVelocity = new JVector(movement.X * 5, body.LinearVelocity.Y, movement.Z * 5);
            
            //Jump
            if (input.Keyboard_WasKeyPressed(Keys.Space))
            {
                body.LinearVelocity += new JVector(0, 10, 0);
            }
        }

        ~LocalPlayer()
        {
            Owner.removeInput(this);
        }
    }
}
