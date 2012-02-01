using DuckEngine;
using Jitter.LinearMath;
using DuckEngine.Helpers;
using Jitter.Dynamics;
using Microsoft.Xna.Framework;
using DuckEngine.Input;
using DuckEngine.Interfaces;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace DuckGame.Players
{
    class LocalPlayer : Player, IInput
    {
        const float MOVEMENT_SPEED = 500f;
        const float JUMP_SPEED = 8f;

        public LocalPlayer(Engine _engine, Tracker _tracker, Vector3 position, Model model)
            : base(_engine, _tracker, position, model)
        {
        }

        //Temporary raycastcallback for testing, might have to filter some things later on.
        private bool RaycastCallback(RigidBody body, JVector normal, float fraction)
        {
            return true;
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

            //Fire if button is down
            if (input.CurrentMouseState.LeftButton == ButtonState.Pressed)
            {
                //Convert the mouse coordinates to a place in the room
                int x = input.CurrentMouseState.X;
                int y = input.CurrentMouseState.Y;
                if (x >= 0 && y >= 0)
                {
                    JVector rayOrigin = Conversion.ToJitterVector(input.MouseRay.Position);
                    JVector rayDirection = Conversion.ToJitterVector(input.MouseRay.Direction);
                    RigidBody hitBody;
                    JVector hitNormal;
                    float hitFraction;

                    bool result = Engine.Physics.CollisionSystem.Raycast(rayOrigin, rayDirection,
                                              null, out hitBody, out hitNormal, out hitFraction);
                    if (result)
                    {
                        /*
                         * All i can say it works at the moment, if you move about and hold the cursor
                         * on a target you will continuosly hit the target. Correcting for movement
                         * would mess with that, but it should be easy to implement if necessary.
                        */
                        Vector3 targetPoint = Conversion.ToXNAVector((rayDirection * hitFraction) + rayOrigin); //Get the vector in relation to the origin and put it at the position of the camera
                        //Get the position at which have the mouse      
                        Vector3 target = (targetPoint - this.Position);
                        //ADJUST SO THAT BOOLITS DONT HIT GROUND CUZ OF TARGET VECTAR
                        currentWeapon.Fire(target);
                    }
                }
            }
        }
    }
}