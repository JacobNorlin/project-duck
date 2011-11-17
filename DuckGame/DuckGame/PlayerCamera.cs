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

        public PlayerCamera(Engine _owner, Player _player = null)
            : base(_owner)
        {
            Owner.addInput(this);
            player = _player;
        }

        public override void Update(GameTime gameTime)
        {
            //Linear algebra, woho!
            Vector3 playerPosition = player.Position;
            Matrix cameraRotation = Matrix.CreateRotationX(angles.X) * Matrix.CreateRotationY(angles.Y);
            Vector3 cameraDirection = Vector3.Transform(Vector3.Forward, cameraRotation);
            Vector3 upVector = Vector3.Transform(Vector3.Up, cameraRotation);

            //Raycast to where camera should be
            RigidBody body;
            JVector normal;
            float fraction;
            Owner.Physics.CollisionSystem.Raycast(Conversion.ToJitterVector(playerPosition), Conversion.ToJitterVector(cameraDirection), new RaycastCallback(RaycastCallback), out body, out normal, out fraction);

            //TODO: Maybe add a smooth transition between distances.
            //If we didin't hit anything
            Vector3 cameraPosition;
            if (body == null)
            {
                //Set camera att default position
                cameraPosition = playerPosition + 20f * cameraDirection;
            }
            else
            {
                //Else, set camera right before the object we hit
                cameraPosition = playerPosition + fraction * cameraDirection;
            }

            //Create view
            view = Matrix.CreateLookAt(cameraPosition, playerPosition, upVector);
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
            float movementFactor = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Rotate camera
            angles.Y -= input.CurrentGamePadStates[0].ThumbSticks.Right.X * movementFactor * 1f;
            angles.X += input.CurrentGamePadStates[0].ThumbSticks.Right.Y * movementFactor * 1f;

            Matrix cameraRotation = Matrix.CreateRotationX(angles.X) * Matrix.CreateRotationY(angles.Y);

            //Drag camera look at
            if (input.CurrentMouseState.RightButton == ButtonState.Pressed && input.LastMouseState.RightButton == ButtonState.Released)
            {
                CenterMouse();
            }
            else if (input.CurrentMouseState.RightButton == ButtonState.Pressed)
            {
                /*if (input.CurrentMouseState.X != WidthOver2)
                    angles.Y -= movementFactor * 0.05f * (input.CurrentMouseState.X - WidthOver2);*/
                if (input.CurrentMouseState.Y != HeightOver2)
                    angles.X -= movementFactor * 0.05f * (input.CurrentMouseState.Y - HeightOver2);

                CenterMouse();
            }

            //Constraints
            angles.X = MathHelper.Clamp(angles.X, .4f, 1f);
            angles.Y = MathHelper.WrapAngle(angles.Y);
        }
    }
}
