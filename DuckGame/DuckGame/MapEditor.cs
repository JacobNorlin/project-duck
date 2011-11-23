using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckEngine;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using DuckEngine.Input;
using Microsoft.Xna.Framework.Input;
using DuckEngine.Interfaces;
using DuckEngine.Helpers;

namespace DuckGame
{
    #region struct for Plane constructible from ray
    struct MyPlane
    {
        public readonly Plane Plane;
        public readonly Vector3 up;
        public readonly Vector3 right;

        public MyPlane(Ray normalRay)
        {
            if (Math.Abs(normalRay.Direction.X) < 0.1 &&
                Math.Abs(normalRay.Direction.Z) < 0.1)
            {
                right = Vector3.Cross(normalRay.Direction,
                            Vector3.Cross(normalRay.Direction, Vector3.Right));
            }
            else
            {
                right = Vector3.Cross(normalRay.Direction, Vector3.Up);
            }
            right.Normalize();
            up = Vector3.Cross(normalRay.Direction, right);
            up.Normalize();
            Plane = new Plane(
                normalRay.Position,
                normalRay.Position + up,
                normalRay.Position + right);
        }
    }
    #endregion

    class MapEditor : IInput, IDraw3D
    {
        private RigidBody hitBody;
        private Ray hitNormal = new Ray();
        private GameController game;
        private bool active;

        private enum MoveMode { None, CameraRelative, PlaneRelative, LineRelative };
        private MoveMode moveMode;
        private float hitDistance;
        private MyPlane plane;
        private Vector3 moveOffset;
        private float planeSize;
        private float lineLength;

        private bool physicsWereEnabledLastTime = false;
        private bool physicsEnabled
        {
            get { return physicsWereEnabledLastTime; }
            set
            {
                game.Owner.Physics.AllowDeactivation = value;
                physicsWereEnabledLastTime = value;
                game.Owner.PhysicsEnabled = value;
            }
        }

        public MapEditor(GameController _gameController)
        {
            game = _gameController;
            game.Owner.addAll(this);
        }

        internal void Activate()
        {
            active = true;
            physicsEnabled = physicsWereEnabledLastTime;
            game.Owner.Physics.AllowDeactivation = false;
            game.Owner.MouseEventManager.WhileMouseOver = whileMouseOver;
        }

        internal void Deactivate()
        {
            active = false;
            physicsEnabled = true;
            game.Owner.MouseEventManager.WhileMouseOver = game.Owner.MouseEventManager.DefaultWhileMouseOver;
        }

        public void Input(GameTime gameTime, InputManager input)
        {
            if (!active)
                return;
            if (input.Keyboard_WasKeyPressed(Keys.Space))
            {
                physicsEnabled = !physicsEnabled;
            }
            if (moveMode == MoveMode.None)
                return;

            //if mouse buttons were released
            if (input.Mouse_WasButtonReleased(InputManager.MouseButton.Left) &&
                !input.Mouse_IsButtonDown(InputManager.MouseButton.Right) ||
                input.Mouse_WasButtonReleased(InputManager.MouseButton.Right) &&
                !input.Mouse_IsButtonDown(InputManager.MouseButton.Left))
            {
                moveMode = MoveMode.None;
                hitBody.IsActive = true;
                hitBody = null;
                return;
            }
            
            hitBody.IsActive = false;
            switch (moveMode)
            {
                case MoveMode.CameraRelative:
                    hitDistance += input.MouseScroll * 0.01f;
                    hitBody.Position = Conversion.ToJitterVector(
                        input.MouseRay.Position -
                        moveOffset +
                        input.MouseRay.Direction * hitDistance
                        );
                    break;
                case MoveMode.LineRelative:
                    break;
                case MoveMode.PlaneRelative:
                    float? _mouseRayPlaneIntersectionDistance = input.MouseRay.Intersects(plane.Plane);
                    if (!_mouseRayPlaneIntersectionDistance.HasValue || _mouseRayPlaneIntersectionDistance.Value > 500)
                    {
                        _mouseRayPlaneIntersectionDistance = 500;
                    }
                    float mouseRayPlaneIntersectionDistance = (float)_mouseRayPlaneIntersectionDistance;
                    hitBody.Position = Conversion.ToJitterVector(
                        input.MouseRay.Position -
                        moveOffset +
                        input.MouseRay.Direction * mouseRayPlaneIntersectionDistance);
                    break;
            }
        }

        public void whileMouseOver(GameTime gameTime, InputManager input,
            RigidBody _hitBody, Ray _hitNormal, float _hitDistance)
        {
            bool leftPressed =
                 input.Mouse_WasButtonPressed(InputManager.MouseButton.Left) &&
                !input.Mouse_IsButtonDown(InputManager.MouseButton.Right);
            bool rightPressed =
                 input.Mouse_WasButtonPressed(InputManager.MouseButton.Right) &&
                !input.Mouse_IsButtonDown(InputManager.MouseButton.Left);
            bool shiftDown = input.Keyboard_IsKeyDown(Keys.LeftShift);


            if      (leftPressed  && !shiftDown)    moveMode = MoveMode.CameraRelative;
            else if (rightPressed &&  shiftDown)    moveMode = MoveMode.LineRelative;
            else if (rightPressed && !shiftDown)    moveMode = MoveMode.PlaneRelative;
            else                                    return;

            if (input.Keyboard_IsKeyDown(Keys.LeftControl) &&
                _hitBody.Tag is PhysicalEntity)
            {
                _hitBody = ((PhysicalEntity)_hitBody.Tag).Clone().Body;
            }
            hitBody = _hitBody;
            hitBody.IsActive = false;
            moveOffset = _hitNormal.Position - Conversion.ToXNAVector(hitBody.Position);

            switch (moveMode)
            {
                case MoveMode.CameraRelative:
                    hitDistance = _hitDistance;
                    break;
                case MoveMode.LineRelative:
                    hitNormal = _hitNormal;
                    lineLength = (hitBody.BoundingBox.Max - hitBody.BoundingBox.Min).Length() / 2;
                    break;
                case MoveMode.PlaneRelative:
                    plane = new MyPlane(_hitNormal);
                    planeSize = (hitBody.BoundingBox.Max - hitBody.BoundingBox.Min).Length() / 2;
                    break;
            }
        }

        public void Draw3D(GameTime gameTime)
        {
            if (!active)
                return;
            if (moveMode == MoveMode.None)
                return;

            Vector3 center = Conversion.ToXNAVector(hitBody.Position);

            switch (moveMode)
            {
                case MoveMode.PlaneRelative:
                    drawPlane(center, plane, planeSize);
                    break;
            }
        }

        private void drawPlane(Vector3 center, MyPlane plane, float planeSize)
        {
            planeSize = planeSize*1.5f + 3;
            for (float i = -1; i <= 1; i += 0.25f)
            {
                Color middleColor = new Color(0, 0, 0, 1 - Math.Abs(i));
                game.Owner.DebugDrawer.DrawLine(
                    center + plane.up * planeSize * i - plane.right * planeSize,
                    center + plane.up * planeSize * i,
                    Color.Transparent,
                    middleColor);
                game.Owner.DebugDrawer.DrawLine(
                    center + plane.up * planeSize * i + plane.right * planeSize,
                    center + plane.up * planeSize * i,
                    Color.Transparent,
                    middleColor);
                game.Owner.DebugDrawer.DrawLine(
                    center + plane.right * planeSize * i - plane.up * planeSize,
                    center + plane.right * planeSize * i,
                    Color.Transparent,
                    middleColor);
                game.Owner.DebugDrawer.DrawLine(
                    center + plane.right * planeSize * i + plane.up * planeSize,
                    center + plane.right * planeSize * i,
                    Color.Transparent,
                    middleColor);
            }
        }
    }
}
