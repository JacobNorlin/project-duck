using DuckEngine;
using DuckEngine.Input;
using DuckEngine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using DuckGame.Players;
using DuckEngine.Managers;
using Jitter.Dynamics;
using Jitter.LinearMath;
using DuckGame.MapEdit;

namespace DuckGame
{
    class GameController : Entity, IInput
    {
        private enum GameState { Playing, Editing };
        private GameState gameState = GameState.Playing;
        PlayerCamera playerCamera;
        DebugCamera debugCamera;
        MapEditor mapEditor;

        public GameController(Engine _owner, Player player)
            : base(_owner)
        {
            Owner.addInput(this);
            playerCamera = new PlayerCamera(Owner);
            debugCamera = new DebugCamera(Owner, Vector3.Zero);
            mapEditor = new MapEditor(this);
            Owner.Camera = playerCamera;
            playerCamera.Player = player;
           
        }

        public void Input(GameTime gameTime, InputManager input)
        {
            if (input.Keyboard_IsKeyDown(Keys.D9) && gameState != GameState.Editing)
            {
                gameState = GameState.Editing;
                debugCamera.Position = playerCamera.Position + Vector3.Up;
                //+Vector3.Up only so you notice you've changed camera mode
                debugCamera.Target = playerCamera.Player.Position;
                Owner.Camera = debugCamera;
                mapEditor.Activate();
            }
            if (input.Keyboard_IsKeyDown(Keys.D8) && gameState != GameState.Playing)
            {
                gameState = GameState.Playing;
                Owner.Camera = playerCamera;
                mapEditor.Deactivate();
            }
            
            if (input.Keyboard_WasKeyReleased(Keys.Escape))
            {
                Owner.Exit();
            }
        }
    }
}
