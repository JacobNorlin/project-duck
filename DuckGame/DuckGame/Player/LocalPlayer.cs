using DuckEngine;
using DuckEngine.Helpers;
using DuckEngine.Input;
using DuckEngine.Interfaces;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DuckGame.Players
{
    class LocalPlayer : Player, IInput
    {
        const float MOVEMENT_SPEED = 500f;
        const float JUMP_SPEED = 8f;

        public LocalPlayer(Engine _owner, Vector3 position)
            : base(_owner, position)
        {
            Owner.addInput(this);
        }

        public void Input(GameTime gameTime, InputManager input)
        {
            //Movement
            Vector3 movement = Vector3.Zero;
            if (input.Keyboard_IsKeyDown(Keys.Up)) { movement.Z += 1; }
            if (input.Keyboard_IsKeyDown(Keys.Down)) { movement.Z -= 1; }
            if (input.Keyboard_IsKeyDown(Keys.Left)) { movement.X += 1; }
            if (input.Keyboard_IsKeyDown(Keys.Right)) { movement.X -= 1; }

            //Apply movement
            if (movement.Length() > 0)
            {
                movement.Normalize();
                movement *= MOVEMENT_SPEED * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            //TODO: Fix the jumping code, you can get stuck on the ground.
            //You can't move if you are in the air
            if (!IsJumping)
            {
                JVector jmovement = Conversion.ToJitterVector(movement);
                body.LinearVelocity = new JVector(jmovement.X, body.LinearVelocity.Y, jmovement.Z);

                //Jump
                if (input.Keyboard_WasKeyPressed(Keys.Space))
                {
                    body.ApplyImpulse(JVector.Up * JUMP_SPEED);
                    isJumping = true;
                }
            }
        }

        ~LocalPlayer()
        {
            Owner.removeInput(this);
        }
    }
}
