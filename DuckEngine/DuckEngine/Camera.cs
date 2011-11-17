using System;
using DuckEngine.Input;
using DuckEngine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DuckEngine
{
    public abstract class Camera : Entity, ILogic
    {
        protected Matrix view;
        protected Matrix projection;

        private int widthOver2;
        public int WidthOver2 { get { return widthOver2; } }
        private int heightOver2;
        public int HeightOver2 { get { return heightOver2; } }

        private float aspectRatio;
        private float fieldOfView = Microsoft.Xna.Framework.MathHelper.PiOver4;
        private float nearPlaneDistance = 0.01f;
        private float farPlaneDistance = 1000.0f;

        public Matrix View
        {
            get { return view; }
        }

        public Matrix Projection
        {
            get { return projection; }
        }

        public Camera(Engine _owner)
            : base(_owner)
        {
            Owner.addLogic(this);
        }

        /// <summary>
        /// Updates camera
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public abstract void Update(GameTime gameTime);

        internal void WindowSizeChanged()
        {
            widthOver2 = Owner.Window.ClientBounds.Width / 2;
            heightOver2 = Owner.Window.ClientBounds.Height / 2;
            aspectRatio = (float)Owner.Window.ClientBounds.Width / (float)Owner.Window.ClientBounds.Height;

            UpdateProjection();
        }

        protected void UpdateProjection()
        {
            projection = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlaneDistance, farPlaneDistance);
        }

        protected void CenterMouse()
        {
            Mouse.SetPosition(WidthOver2, HeightOver2);
        }
    }
}
