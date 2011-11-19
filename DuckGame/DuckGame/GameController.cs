using DuckEngine;
using DuckEngine.Input;
using DuckEngine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using DuckGame.Players;

namespace DuckGame
{
    class GameController : Entity, IInput
    {
        private enum GameState { Playing, Editing };
        private GameState gameState = GameState.Playing;
        PlayerCamera playerCamera;
        DebugCamera debugCamera;

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
                }
            }
        }
    }
}
