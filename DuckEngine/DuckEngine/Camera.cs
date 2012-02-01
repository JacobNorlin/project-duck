using System;
using DuckEngine.Input;
using DuckEngine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DuckEngine
{
    public abstract class Camera : Entity, ILogic
    {
        private int widthOver2;
        public int WidthOver2 { get { return widthOver2; } }
        private int heightOver2;
        public int HeightOver2 { get { return heightOver2; } }

        private float aspectRatio;
        private float fieldOfView = Microsoft.Xna.Framework.MathHelper.PiOver4;
        private float nearPlaneDistance = 0.01f;
        private float farPlaneDistance = 1000.0f;

        protected Matrix view = Matrix.Identity;
        public Matrix View { get { return view; } }

        protected Matrix projection;
        public Matrix Projection { get { return projection; } }

        protected Vector3 position;
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        protected bool active = false;
        public bool Active
        {
            get { return active; }
            set
            {
                if (!active && value)
                {
                    WindowSizeChanged();
                }
                active = value;
            }
        }

        public Camera(Engine _engine, Tracker _tracker)
            : base(_engine, _tracker)
        {
        }

        /// <summary>
        /// Updates camera
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public abstract void Update(GameTime gameTime);

        internal void WindowSizeChanged()
        {
            widthOver2 = Engine.Window.ClientBounds.Width / 2;
            heightOver2 = Engine.Window.ClientBounds.Height / 2;
            aspectRatio = (float)Engine.Window.ClientBounds.Width / (float)Engine.Window.ClientBounds.Height;

            UpdateProjection();
        }

        protected void UpdateProjection()
        {
            projection = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlaneDistance, farPlaneDistance);
        }
    }
}
