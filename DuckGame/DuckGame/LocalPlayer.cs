using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckEngine.Interfaces;
using Microsoft.Xna.Framework;
using DuckEngine.Input;
using Microsoft.Xna.Framework.Input;
using Jitter.LinearMath;
using DuckEngine.Helpers;

namespace DuckEngine
{
    class LocalPlayer : Player, IInput
    {
        const float MOVEMENT_SPEED = 5f;
        const float JUMP_SPEED = 15f;

        public LocalPlayer(Engine _owner)
            : base(_owner)
        {
            Owner.addInput(this);
        }

        public void Input(GameTime gameTime, InputManager input)
        {
            //Movement
            Vector3 movement = Vector3.Zero;
            if (input.Keyboard_IsKeyDown(Keys.Up)) { movement.Z -= 1; }
            if (input.Keyboard_IsKeyDown(Keys.Down)) { movement.Z += 1; }
            if (input.Keyboard_IsKeyDown(Keys.Left)) { movement.X -= 1; }
            if (input.Keyboard_IsKeyDown(Keys.Right)) { movement.X += 1; }

            //Apply movement
            if (movement.Length() > 0)
            {
                movement.Normalize();
                body.Position += Conversion.ToJitterVector(MOVEMENT_SPEED * movement * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            
            //Jump
            if (input.Keyboard_WasKeyPressed(Keys.Space))
            {
                body.ApplyImpulse(JVector.Up * JUMP_SPEED);
            }
        }

        ~LocalPlayer()
        {
            Owner.removeInput(this);
        }
    }
}
