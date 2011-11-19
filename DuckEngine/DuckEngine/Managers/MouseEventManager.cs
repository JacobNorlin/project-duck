using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckEngine.Interfaces;
using Microsoft.Xna.Framework;
using Jitter.Dynamics;
using Jitter.LinearMath;
using DuckEngine.Input;
using DuckEngine.Helpers;

namespace DuckEngine.Managers
{
    public class MouseEventManager : Entity
    {
        private RigidBody mouseOver3D;
        public delegate void MouseOverHandler(GameTime gameTime, InputManager input,
            RigidBody hitBody, JVector hitNormal, float hitFraction);
        public MouseOverHandler OnMouseOver;

        public MouseEventManager(Engine _owner)
            : base(_owner)
        {
            OnMouseOver = DefaultOnMouseOver;
        }

        public void DefaultOnMouseOver(GameTime gameTime, InputManager input,
            RigidBody hitBody, JVector hitNormal, float hitFraction)
        {
            if (hitBody != mouseOver3D)
            {
                if (hitBody.Tag is IMouseEvent3D)
                {
                    ((IMouseEvent3D)hitBody.Tag).OnMouseOver();
                }
                if (mouseOver3D != null && mouseOver3D.Tag is IMouseEvent3D)
                {
                    ((IMouseEvent3D)mouseOver3D.Tag).OnMouseOut();
                }
            }
        }

        internal void ExecuteMouseEvents(GameTime gameTime, InputManager input)
        {
            //TODO
            //don't do anything if mouse is outside window bounds //Björn
            //TODO
            //check if the mouse is over any 2D (HUD) elements first
            d3(gameTime, input);
        }
        
        private void d3(GameTime gameTime, InputManager input)
        {
            int x = input.CurrentMouseState.X;
            int y = input.CurrentMouseState.Y;
            if (x >= 0 && y >= 0)
            {
                JVector rayOrigin = Conversion.ToJitterVector(Owner.Camera.Position);
                JVector rayDirection = Conversion.ToJitterVector(RayTo(x, y));
                RigidBody hitBody;
                JVector hitNormal;
                float hitFraction;

                bool result = Owner.Physics.CollisionSystem.Raycast(rayOrigin, rayDirection,
                    null, out hitBody, out hitNormal, out hitFraction);
                if (result && OnMouseOver != null)
                {
                    OnMouseOver(gameTime, input, hitBody, hitNormal, hitFraction);
                }
                mouseOver3D = hitBody;
            }
        }

        // Helper method to get the 3d ray
        private Vector3 RayTo(int x, int y)
        {
            Vector3 nearSource = new Vector3(x, y, 0);
            Vector3 farSource = new Vector3(x, y, 1);

            Matrix world = Matrix.CreateTranslation(0, 0, 0);
            Vector3 nearPoint = Owner.GraphicsDevice.Viewport.Unproject(nearSource,
                Owner.Camera.Projection, Owner.Camera.View, world);
            Vector3 farPoint = Owner.GraphicsDevice.Viewport.Unproject(farSource,
                Owner.Camera.Projection, Owner.Camera.View, world);

            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();

            return direction;
        }
    }
}
