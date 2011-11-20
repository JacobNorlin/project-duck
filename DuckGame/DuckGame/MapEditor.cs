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

namespace DuckGame
{
    class MapEditor : IInput
    {
        private RigidBody moving;
        private JVector hitNormal;
        private GameController game;
        private bool active;

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

            if (moving != null)
            {
                moving.IsActive = false;
                if (input.Mouse_IsButtonDown(InputManager.MouseButton.Left))
                {
                    Point movement = input.MouseMovement;
                    moving.Position += hitNormal * movement.Y * 0.05f;
                }
                else
                {
                    moving.IsActive = true;
                    moving = null;
                }
            }
        }

        public void whileMouseOver(GameTime gameTime, InputManager input,
            RigidBody hitBody, JVector _hitNormal, float hitFraction)
        {
            if (input.Mouse_WasButtonPressed(InputManager.MouseButton.Left))
            {
                moving = hitBody;
                moving.IsActive = false;
                hitNormal = _hitNormal;
            }
        }
    }
}
