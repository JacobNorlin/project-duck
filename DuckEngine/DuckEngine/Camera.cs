using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using DuckEngine;
using DuckEngine.Interfaces;
using DuckEngine.Input;

namespace DuckEngine
{
    /// <summary>
    /// First person camera component for the demos, rotated by mouse.
    /// </summary>
    public class Camera : Entity, ILogic, IInput
    {
        private Matrix view;
        private Matrix projection;

        private Vector3 position = new Vector3(0, 0, 10);
        private Vector2 angles = Vector2.Zero;

        private int widthOver2;
        private int heightOver2;

        private float fieldOfView = Microsoft.Xna.Framework.MathHelper.PiOver4;
        private float aspectRatio;
        private float nearPlaneDistance = 0.01f;
        private float farPlaneDistance = 1000.0f;

        /// <summary>
        /// Initializes new camera component.
        /// </summary>
        /// <param name="owner">Game to which attach this camera.</param>
        public Camera(Engine _owner)
            : base(_owner)
        {
            Owner.addInput(this);
            Owner.addLogic(this);
            UpdateProjection();
            CenterMouse();
        }

        #region Properties
        /// <summary>
        /// Gets camera view matrix.
        /// </summary>
        public Matrix View { get { return view; } }

        /// <summary>
        /// Gets or sets camera projection matrix.
        /// </summary>
        public Matrix Projection { get { return projection; } set { projection = value; } }

        /// <summary>
        /// Gets camera view matrix multiplied by projection matrix.
        /// </summary>
        public Matrix ViewProjection { get { return view * projection; } }

        /// <summary>
        /// Gets or sets camera position.
        /// </summary>
        public Vector3 Position { get { return position; } set { position = value; } }

        /// <summary>
        /// Gets or sets camera field of view.
        /// </summary>
        public float FieldOfView { get { return fieldOfView; } set { fieldOfView = value; UpdateProjection(); } }

        /// <summary>
        /// Gets or sets camera aspect ratio.
        /// </summary>
        public float AspectRatio { get { return aspectRatio; } set { aspectRatio = value; UpdateProjection(); } }

        /// <summary>
        /// Gets or sets camera near plane distance.
        /// </summary>
        public float NearPlaneDistance { get { return nearPlaneDistance; } set { nearPlaneDistance = value; UpdateProjection(); } }

        /// <summary>
        /// Gets or sets camera far plane distance.
        /// </summary>
        public float FarPlaneDistance { get { return farPlaneDistance; } set { farPlaneDistance = value; UpdateProjection(); } }

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
                Vector3 forward = Vector3.Normalize(position - value);
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

        /// <summary>
        /// Updates camera
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime)
        {
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
            float movementFactor = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector3 moveVector = new Vector3();

            //Move camera
            if (input.CurrentKeyboardState.IsKeyDown(Keys.D) || input.CurrentGamePadStates[0].IsButtonDown(Buttons.DPadRight))
                moveVector.X += 5f * movementFactor;
            if (input.CurrentKeyboardState.IsKeyDown(Keys.A) || input.CurrentGamePadStates[0].IsButtonDown(Buttons.DPadLeft))
                moveVector.X -= 5f * movementFactor;
            if (input.CurrentKeyboardState.IsKeyDown(Keys.S) || input.CurrentGamePadStates[0].IsButtonDown(Buttons.DPadDown))
                moveVector.Z += 5f * movementFactor;
            if (input.CurrentKeyboardState.IsKeyDown(Keys.W) || input.CurrentGamePadStates[0].IsButtonDown(Buttons.DPadUp))
                moveVector.Z -= 5f * movementFactor;

            //Rotate camera
            angles.Y -= input.CurrentGamePadStates[0].ThumbSticks.Right.X * movementFactor * 1f;
            angles.X += input.CurrentGamePadStates[0].ThumbSticks.Right.Y * movementFactor * 1f;

            Matrix cameraRotation = Matrix.CreateRotationX(angles.X) * Matrix.CreateRotationY(angles.Y);
            position += Vector3.Transform(moveVector, cameraRotation);

            //Drag camera look at
            if (input.CurrentMouseState.RightButton == ButtonState.Pressed && input.LastMouseState.RightButton == ButtonState.Released)
            {
                Mouse.SetPosition(widthOver2, heightOver2);
            }
            else if (input.CurrentMouseState.RightButton == ButtonState.Pressed)
            {
                if (input.CurrentMouseState.X != widthOver2)
                    angles.Y -= movementFactor * 0.05f * (input.CurrentMouseState.X - widthOver2);
                if (input.CurrentMouseState.Y != heightOver2)
                    angles.X -= movementFactor * 0.05f * (input.CurrentMouseState.Y - heightOver2);

                Mouse.SetPosition(widthOver2, heightOver2);
            }

            //Constraints
            if (angles.X > 1.4) angles.X = 1.4f;
            if (angles.X < -1.4) angles.X = -1.4f;
            if (angles.Y > Math.PI) angles.Y -= 2 * (float)Math.PI;
            if (angles.Y < -Math.PI) angles.Y += 2 * (float)Math.PI;
        }

        private void UpdateProjection()
        {
            projection = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlaneDistance, farPlaneDistance);
        }

        internal void WindowSizeChanged()
        {
            widthOver2 = owner.Window.ClientBounds.Width / 2;
            heightOver2 = owner.Window.ClientBounds.Height / 2;
            Console.WriteLine(owner.Window.ClientBounds);
            aspectRatio = (float)owner.Window.ClientBounds.Width / (float)owner.Window.ClientBounds.Height;
            UpdateProjection();
        }

        internal void CenterMouse()
        {
            Mouse.SetPosition(widthOver2, heightOver2);
        }
    }
}
