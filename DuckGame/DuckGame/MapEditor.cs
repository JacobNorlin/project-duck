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
        private float hitFraction;
        private MyPlane plane;
        private Vector3 moveOffset;
        private float planeSize;

        public MapEditor(GameController _gameController)
        {
            game = _gameController;
            game.Owner.addAll(this);
        }

        internal void Activate()
        {
            active = true;
            game.Owner.Physics.AllowDeactivation = false;
            game.Owner.MouseEventManager.WhileMouseOver = whileMouseOver;
        }

        internal void Deactivate()
        {
            active = false;
            game.Owner.Physics.AllowDeactivation = true;
            game.Owner.MouseEventManager.WhileMouseOver = game.Owner.MouseEventManager.DefaultWhileMouseOver;
        }

        public void Input(GameTime gameTime, InputManager input)
        {
            if (!active)
                return;

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
                    hitBody.Position = Conversion.ToJitterVector(
                        input.MouseRay.Position -
                        moveOffset +
                        input.MouseRay.Direction * hitFraction
                        
                        );
                    break;
                case MoveMode.LineRelative:
                    break;
                case MoveMode.PlaneRelative:
                    float? _f = input.MouseRay.Intersects(plane.Plane);
                    if (!_f.HasValue || _f.Value > 500)
                    {
                        _f = 500;
                    }
                    float f = (float)_f;
                    hitBody.Position = Conversion.ToJitterVector(
                        input.MouseRay.Position -
                        moveOffset +
                        input.MouseRay.Direction * f);
                    break;
            }
        }

        public void whileMouseOver(GameTime gameTime, InputManager input,
            RigidBody _hitBody, Ray _hitNormal, float _hitFraction)
        {
            bool leftPressed =
                 input.Mouse_WasButtonPressed(InputManager.MouseButton.Left) &&
                !input.Mouse_IsButtonDown(InputManager.MouseButton.Right);
            bool rightPressed =
                 input.Mouse_WasButtonPressed(InputManager.MouseButton.Right) &&
                !input.Mouse_IsButtonDown(InputManager.MouseButton.Left);

            if (leftPressed || rightPressed) {
                hitBody = _hitBody;
                hitBody.IsActive = false;

                if (leftPressed)
                {
                    moveMode = MoveMode.CameraRelative;
                    hitFraction = _hitFraction;
                }
                else
                {
                    if (input.Keyboard_IsKeyDown(Keys.LeftShift))
                    {
                        moveMode = MoveMode.LineRelative;
                        hitNormal = _hitNormal;
                    }
                    else
                    {
                        Console.WriteLine(_hitNormal);
                        moveMode = MoveMode.PlaneRelative;
                        planeSize = (hitBody.BoundingBox.Max - hitBody.BoundingBox.Min).Length() / 2;
                        plane = new MyPlane(_hitNormal);
                        moveOffset = _hitNormal.Position - Conversion.ToXNAVector(hitBody.Position);
                    }
                }
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
