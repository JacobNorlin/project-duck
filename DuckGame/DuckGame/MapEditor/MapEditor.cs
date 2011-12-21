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
using DuckEngine.MapEditor;
using DuckEngine.MapEditor.SaveStates;

namespace DuckGame.MapEdit
{
    struct MyPlane //struct for Plane constructible from ray
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

    class MapEditor : IInput, IDraw3D
    {
        public IEnumerable<RigidBody> AllBodies { get { return game.Owner.Physics.RigidBodies; } }

        private Ray hitNormal = new Ray();
        private GameController game;
        private StateChange stateInChange;
        private SaveStateManager states = new SaveStateManager();
        private Selection selected = new Selection();
        private Selection copied = null;

        private enum MoveMode { None, CameraRelative, PlaneRelative, LineRelative };
        private MoveMode moveMode;
        private float hitDistance;
        private MyPlane plane;
        private Vector3 moveOffset;

        private bool paused = false;
        private bool Paused
        {
            get { return paused; }
            set
            {
                if (!value && paused) //on play
                {
                    if (moveMode != MoveMode.None)
                    {
                        finishStateChange(selected); //in case of pressing pause while dragging objects
                    }
                    startStateChange(AllBodies);
                }
                else if (value && !paused) //on pause
                {
                    finishStateChange(AllBodies);
                }
                game.Owner.PhysicsEnabled = !value;
                paused = value;
            }
        }

        private bool active = false;
        private bool mouseOverRanSinceLastLeftMousePress;
        private Selection selectedButNotMoving;
        public bool Active
        {
            get { return active; }
            set
            {
                if (value && !active)
                {
                    Paused = true;
                    game.Owner.Physics.AllowDeactivation = false; //maybe?
                    game.Owner.MouseEventManager.WhileMouseOver = whileMouseOver;
                }
                else if (!value && active)
                {
                    Paused = false;
                    game.Owner.Physics.AllowDeactivation = true; //maybe?
                    game.Owner.MouseEventManager.WhileMouseOver = game.Owner.MouseEventManager.DefaultWhileMouseOver;
                }
                active = value;
            }
        }

        public MapEditor(GameController _gameController)
        {
            game = _gameController;
            game.Owner.addAll(this);
        }

        public void Input(GameTime gameTime, InputManager input)
        {
            if (!Active)
                return;
            if (input.Keyboard_WasKeyPressed(Keys.Space))
            {
                Paused = !Paused;
            }
            if (input.Keyboard_WasKeyPressed(Keys.Delete))
            {
                startStateChange(selected);
                finishStateChange(null);
                selected.Clear();
                selected.Active = false;
            }

            if (input.Mouse_WasButtonPressed(MouseButton.Left))
            {
                mouseOverRanSinceLastLeftMousePress = false;
            }
            if (!mouseOverRanSinceLastLeftMousePress &&
                input.Mouse_WasButtonReleased(MouseButton.Left))
            {
                mouseOverRanSinceLastLeftMousePress = true;
                selected.Clear();
            }

            bool shift = input.Keyboard_IsKeyDown(Keys.LeftShift);
            bool ctrl = input.Keyboard_IsKeyDown(Keys.LeftControl);
            bool keyZ = input.Keyboard_WasKeyPressed(Keys.Z);
            bool keyY = input.Keyboard_WasKeyPressed(Keys.Y);
            if (ctrl)
            {
                if (keyY || keyZ && shift)  redo();
                else if (keyZ)              undo();
                if (input.Keyboard_WasKeyPressed(Keys.C))
                {
                    //copy, don't set :: moveMode = MoveMode.None;
                }
                if (input.Keyboard_WasKeyPressed(Keys.Delete))
                {
                    moveMode = MoveMode.None;
                }
                if (input.Keyboard_WasKeyPressed(Keys.V))
                {
                    //paste, don't set :: moveMode = MoveMode.None;
                }
            }

            if (moveMode == MoveMode.None)
                return;

            //if mouse buttons were released
            if (input.Mouse_WasButtonReleased(MouseButton.Left) &&
                !input.Mouse_IsButtonDown(MouseButton.Right) ||
                input.Mouse_WasButtonReleased(MouseButton.Right) &&
                !input.Mouse_IsButtonDown(MouseButton.Left))
            {
                finishStateChange(selected);
                return;
            }

            //Update the body's position
            switch (moveMode)
            {
                case MoveMode.CameraRelative:
                    hitDistance += input.MouseScroll * 0.02f;
                    selected.Position = input.MouseRay.Position -
                        moveOffset +
                        input.MouseRay.Direction * hitDistance;
                    break;
                case MoveMode.LineRelative: //The point on hitNormal closest to the mouseRay
                    //Guard against parallel or almost paralell lines.
                    if (Vector3.Dot(Vector3.Normalize(input.MouseRay.Direction),
                                    Vector3.Normalize(hitNormal.Direction)) > 0.99f)
                    {
                        break;
                    }
                    Vector3 between = input.MouseRay.Position - hitNormal.Position;
                    Vector3 normal = Vector3.Cross(input.MouseRay.Direction, hitNormal.Direction);
                    Vector3 R = Vector3.Divide(Vector3.Cross(between, normal), normal.LengthSquared());
                    Vector3 P = hitNormal.Position + hitNormal.Direction * Vector3.Dot(R, input.MouseRay.Direction);
                    selected.Position = P - moveOffset;
                    break;
                case MoveMode.PlaneRelative:
                    float? _mouseRayPlaneIntersectionDistance = input.MouseRay.Intersects(plane.Plane);
                    if (!_mouseRayPlaneIntersectionDistance.HasValue || _mouseRayPlaneIntersectionDistance.Value > 500)
                    {
                        _mouseRayPlaneIntersectionDistance = 500;
                    }
                    float mouseRayPlaneIntersectionDistance = (float)_mouseRayPlaneIntersectionDistance;
                    selected.Position = input.MouseRay.Position -
                        moveOffset +
                        input.MouseRay.Direction * mouseRayPlaneIntersectionDistance;
                    break;
            }
        }

        private void startStateChange(IEnumerable<RigidBody> bodies)
        {
            //if (bodies == null)
            //    Console.WriteLine("start with body count:    null");
            //else
            //    Console.WriteLine("start with body count:    " + bodies.Count());
            
            stateInChange = new StateChange(bodies);
        }

        private void finishStateChange(IEnumerable<RigidBody> bodies)
        {
            moveMode = MoveMode.None;
            if (stateInChange != null)
            {
                stateInChange.After = bodies;
                states.saveState(stateInChange);
                stateInChange = null;
            }
            if (selectedButNotMoving != null)
            {
                selected.Add(selectedButNotMoving);
                selectedButNotMoving = null;
            }
        }

        private void undo()
        {
            if (moveMode != MoveMode.None)
            {
                finishStateChange(selected); //in case of dragging
            }
            Paused = true;
            selected.Clear();
            stateInChange = states.undo();
        }

        private void redo()
        {
            if (moveMode != MoveMode.None)
            {
                finishStateChange(selected); //in case of dragging
            }
            Paused = true;
            selected.Clear();
            stateInChange = states.redo();
        }

        public void whileMouseOver(GameTime gameTime, InputManager input,
            RigidBody _hitBody, Ray _hitNormal, float _hitDistance)
        {
            mouseOverRanSinceLastLeftMousePress = true;
            bool leftPressed =
                 input.Mouse_WasButtonPressed(MouseButton.Left) &&
                !input.Mouse_IsButtonDown(MouseButton.Right);
            bool rightPressed =
                 input.Mouse_WasButtonPressed(MouseButton.Right) &&
                !input.Mouse_IsButtonDown(MouseButton.Left);

            if (leftPressed || rightPressed)
            {
                bool shiftDown = input.Keyboard_IsKeyDown(Keys.LeftShift);
                bool ctrlDown = input.Keyboard_IsKeyDown(Keys.LeftControl);
                bool altDown = input.Keyboard_IsKeyDown(Keys.LeftAlt);

                Paused = true; //must be done before much, if not everything, else

                if (leftPressed)
                {
                    if (ctrlDown) //expand (or reduce) selection
                    {
                        moveMode = MoveMode.None;
                        selected.toggleSelection(_hitBody);
                        return;
                    }
                    else
                    {
                        moveMode = MoveMode.CameraRelative;
                    }
                }
                else //rightPressed
                {
                    if (shiftDown)
                    {
                        moveMode = MoveMode.LineRelative;
                    }
                    else
                    {
                        moveMode = MoveMode.PlaneRelative;
                    }
                }

                if (!selected.Contains(_hitBody))
                {
                    selected.Clear();
                }
                selected.Add(_hitBody);

                if (altDown) //copy and move
                {
                    if (ctrlDown)
                    {
                        selectedButNotMoving = selected;
                    }
                    selected = selected.copy(true);
                    startStateChange(null);
                }
                else //just move
                {
                    startStateChange(selected);
                }

                moveOffset = _hitNormal.Position - selected.Position;

                switch (moveMode)
                {
                    case MoveMode.CameraRelative:
                        hitDistance = _hitDistance;
                        break;
                    case MoveMode.LineRelative:
                        hitNormal = _hitNormal;
                        break;
                    case MoveMode.PlaneRelative:
                        plane = new MyPlane(_hitNormal);
                        break;
                }
            }
        }

        public void Draw3D(GameTime gameTime)
        {
            if (!active)
                return;
            if (moveMode == MoveMode.None)
                return;
            if (selected.Highlighted == null)
                return;

            Vector3 center = selected.Highlighted.Position.ToXNAVector();

            switch (moveMode)
            {
                case MoveMode.PlaneRelative:
                    drawPlane(center, plane, selected.BoundingBoxSize);
                    break;
                case MoveMode.LineRelative:
                    drawLine(center, hitNormal.Direction, selected.BoundingBoxSize);
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

        private void drawLine(Vector3 center, Vector3 direction, float lineLength)
        {
            lineLength = lineLength * 1.5f + 3;
            game.Owner.DebugDrawer.DrawLine(
                center + direction * lineLength,
                center,
                Color.Transparent,
                Color.Black);
            game.Owner.DebugDrawer.DrawLine(
                center - direction * lineLength,
                center,
                Color.Transparent,
                Color.Black);
        }
    }
}
