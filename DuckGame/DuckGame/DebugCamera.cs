using System;
using DuckEngine;
using DuckEngine.Input;
using DuckEngine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DuckGame
{
    class DebugCamera : Camera, IInput
    {
        const float MOVE_SPEED = 20f;

        private Vector2 angles = Vector2.Zero;

        private bool moveEnabled = true;

        /// <summary>
        /// Initializes new camera component.
        /// </summary>
        /// <param name="owner">Game to which attach this camera.</param>
        public DebugCamera(Engine _owner, Vector3 _position)
            : base(_owner)
        {
            position = _position;
            UpdateProjection();
            CenterMouse();
        }

        #region Properties
        /// <summary>
        /// Gets or sets camera's target.
        /// </summary>
        public Vector3 Target
        {
            get
            {
                Matrix cameraRotation = Matrix.CreateRotationX(angles.X) * Matrix.CreateRotationY(angles.Y);
                return position + Vector3.Transform(Vector3.Forward, cameraRotation);
            }
            set
            {
                Vector3 forward = Vector3.Normalize(value - position);
                Vector3 right = Vector3.Normalize(Vector3.Cross(forward, Vector3.Up));
                Vector3 up = Vector3.Normalize(Vector3.Cross(right, forward));

                Matrix test = Matrix.Identity;
                test.Forward = forward;
                test.Right = right;
                test.Up = up;
                angles.X = -(float)Math.Asin(test.M32);
                angles.Y = -(float)Math.Asin(test.M13);
            }
        }
        #endregion

        public override void Update(GameTime gameTime)
        {
            if (!active) return;
            //Update view
            Matrix cameraRotation = Matrix.CreateRotationX(angles.X) * Matrix.CreateRotationY(angles.Y);
            Vector3 targetPos = position + Vector3.Transform(Vector3.Forward, cameraRotation);

            Vector3 upVector = Vector3.Transform(Vector3.Up, cameraRotation);

            view = Matrix.CreateLookAt(position, targetPos, upVector);
        }

        /// <summary>
        /// Provides input to camera
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Input(GameTime gameTime, InputManager input)
        {
            if (!active) return;
            float movementFactor = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector3 moveVector = new Vector3();

            moveEnabled ^= input.Keyboard_WasKeyPressed(Keys.Q);
            if (moveEnabled) {
                //Move camera
                if (input.Keyboard_IsKeyDown(Keys.D) || input.CurrentGamePadStates[0].IsButtonDown(Buttons.DPadRight)) moveVector.X += 1f;
                if (input.Keyboard_IsKeyDown(Keys.A) || input.CurrentGamePadStates[0].IsButtonDown(Buttons.DPadLeft)) moveVector.X -= 1f;
                if (input.Keyboard_IsKeyDown(Keys.S) || input.CurrentGamePadStates[0].IsButtonDown(Buttons.DPadDown)) moveVector.Z += 1f;
                if (input.Keyboard_IsKeyDown(Keys.W) || input.CurrentGamePadStates[0].IsButtonDown(Buttons.DPadUp)) moveVector.Z -= 1f;

                if (moveVector.Length() > 0)
                {
                    moveVector.Normalize();
                    moveVector *= MOVE_SPEED * movementFactor;
                }
            }
            //Rotate camera
            angles.Y -= input.CurrentGamePadStates[0].ThumbSticks.Right.X * movementFactor * 1f;
            angles.X += input.CurrentGamePadStates[0].ThumbSticks.Right.Y * movementFactor * 1f;

            Matrix cameraRotation = Matrix.CreateRotationX(angles.X) * Matrix.CreateRotationY(angles.Y);
            position += Vector3.Transform(moveVector, cameraRotation);

            //Drag camera look at
            if (input.CurrentMouseState.RightButton == ButtonState.Pressed && input.LastMouseState.RightButton == ButtonState.Released)
            {
                CenterMouse();
            }
            else if (input.CurrentMouseState.RightButton == ButtonState.Pressed)
            {
                if (input.CurrentMouseState.X != WidthOver2)
                    angles.Y -= movementFactor * 0.05f * (input.CurrentMouseState.X - WidthOver2);
                if (input.CurrentMouseState.Y != HeightOver2)
                    angles.X -= movementFactor * 0.05f * (input.CurrentMouseState.Y - HeightOver2);

                CenterMouse();
            }

            //Constraints
            angles.X = MathHelper.Clamp(angles.X, -1.4f, 1.4f);
            angles.Y = MathHelper.WrapAngle(angles.Y);
        }
    }
}
