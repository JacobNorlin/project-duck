using DuckEngine.Helpers;
using DuckEngine.Input;
using DuckEngine.Interfaces;
using DuckGame.Players;
using Jitter.Collision;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DuckEngine
{
    class PlayerCamera : Camera, IInput
    {
        Player player;
        internal Player Player
        {
            get { return player; }
            set { player = value; }
        }

        private Vector2 angles = new Vector2(MathHelper.PiOver4, 0);

        public PlayerCamera(Engine _engine, Tracker _tracker, Player _player = null)
            : base(_engine, _tracker)
        {
            player = _player;
            position = new Vector3();
        }

        public override void Update(GameTime gameTime)
        {
            if (!active) return;
            //Linear algebra, woho!
            Vector3 playerPosition = player.Position;
            Matrix cameraRotation = Matrix.CreateRotationX(angles.X) * Matrix.CreateRotationY(angles.Y);
            Vector3 cameraDirection = Vector3.Transform(Vector3.Forward, cameraRotation);
            Vector3 upVector = Vector3.Transform(Vector3.Up, cameraRotation);

            //Raycast to where camera should be
            RigidBody body;
            JVector normal;
            float fraction;
            Engine.Physics.CollisionSystem.Raycast(Conversion.ToJitterVector(playerPosition), Conversion.ToJitterVector(cameraDirection), new RaycastCallback(RaycastCallback), out body, out normal, out fraction);

            //TODO: Maybe add a smooth transition between distances.
            //If we didin't hit anything
            if (body == null)
            {
                //Set camera att default position
                position = playerPosition + 20f * cameraDirection;
            }
            else
            {
                //Else, set camera right before the object we hit
                position = playerPosition + fraction * cameraDirection;
            }

            //Create view
            view = Matrix.CreateLookAt(position, playerPosition, upVector);
        }

        private bool RaycastCallback(RigidBody body, JVector normal, float fraction)
        {
            //Don't hit the player, hit the game
            if (body != Player.Body)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Provides input to camera
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Input(GameTime gameTime, InputManager input)
        {
            if (!active) return;
            float movementFactor = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Rotate camera
            angles.Y -= input.CurrentGamePadStates[0].ThumbSticks.Right.X * movementFactor * 1f;
            angles.X += input.CurrentGamePadStates[0].ThumbSticks.Right.Y * movementFactor * 1f;

            Matrix cameraRotation = Matrix.CreateRotationX(angles.X) * Matrix.CreateRotationY(angles.Y);

            //Drag camera look at
            if (input.Mouse_WasButtonPressed(MouseButton.Right))
            {
                input.MouseFrozen = true;
                Engine.IsMouseVisible = false;
            }
            else if (input.Mouse_IsButtonDown(MouseButton.Right))
            {
                angles.X -= movementFactor * 0.05f * input.MouseMovement.Y;
            }
            else if (input.Mouse_WasButtonReleased(MouseButton.Right))
            {
                input.MouseFrozen = false;
                Engine.IsMouseVisible = true;
            }

            //Constraints
            angles.X = MathHelper.Clamp(angles.X, .4f, 1f);
            angles.Y = MathHelper.WrapAngle(angles.Y);
        }
    }
}
