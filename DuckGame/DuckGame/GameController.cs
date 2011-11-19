using DuckEngine;
using DuckEngine.Input;
using DuckEngine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using DuckGame.Players;
using DuckEngine.Managers;
using Jitter.Dynamics;
using Jitter.LinearMath;

namespace DuckGame
{
    class GameController : Entity, IInput
    {
        private enum GameState { Playing, Editing };
        private GameState gameState = GameState.Playing;
        PlayerCamera playerCamera;
        DebugCamera debugCamera;
        private  RigidBody moving;
        private JVector along;

        public GameController(Engine _owner, Player player)
            : base(_owner)
        {
            Owner.addInput(this);
            playerCamera = new PlayerCamera(Owner);
            debugCamera = new DebugCamera(Owner, Vector3.Zero);
            _owner.Camera = playerCamera;
            playerCamera.Player = player;
           
        }

        public void Input(GameTime gameTime, InputManager input)
        {
            if (input.Keyboard_IsKeyDown(Keys.D8))
            {
                gameState = GameState.Playing;
                Owner.Camera = playerCamera;
                Owner.MouseEventManager.OnMouseOver = Owner.MouseEventManager.DefaultOnMouseOver;
            }
            if (input.Keyboard_IsKeyDown(Keys.D9))
            {
                if (gameState != GameState.Editing)
                {
                    gameState = GameState.Editing;
                    debugCamera.Position = playerCamera.Position + Vector3.Up;
                    //+Vector3.Up only so you notice you've changed camera mode
                    debugCamera.Target = playerCamera.Player.Position;
                    Owner.Camera = debugCamera;
                    Owner.MouseEventManager.OnMouseOver = lol;
                }
            }
            editorMouseHandling(gameTime, input);
        }

        public void lol(GameTime gameTime, InputManager input,
            RigidBody hitBody, JVector hitNormal, float hitFraction)
        {
            if (input.Mouse_WasButtonPressed(InputManager.MouseButton.Left))
            {
                moving = hitBody;
                moving.IsActive = false;
                along = hitNormal;
            }
        }

        private void editorMouseHandling(GameTime gameTime, InputManager input)
        {
            if (moving != null) {
                moving.IsActive = false;
                if (input.Mouse_IsButtonDown(InputManager.MouseButton.Left))
                {
                    Point movement = input.Mouse_Movement();
                    moving.Position += along * movement.Y * 0.05f;
                }
                else
                {
                    moving.IsActive = true;
                    moving = null;
                }
            }
        }
    }
}
