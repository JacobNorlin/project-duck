using DuckEngine;
using DuckEngine.Input;
using DuckEngine.Interfaces;
using DuckGame.Maps;
using Microsoft.Xna.Framework;
using DuckGame.Players;
using Microsoft.Xna.Framework.Input;
using System;

namespace DuckGame
{
    public class Game : StartupObject, IInput
    {
        private enum GameState { Playing, Editing };
        private GameState gameState = GameState.Playing;
        PlayerCamera playerCamera;
        DebugCamera debugCamera;
        private Engine Owner;

        public override void Initialize(Engine _owner)
        {
            Owner = _owner;
            playerCamera = new PlayerCamera(Owner);
            debugCamera = new DebugCamera(Owner, Vector3.Zero);
            Owner.Camera = playerCamera;
            Owner.Window.AllowUserResizing = true;
        }

        public override void LoadContent(Engine Owner)
        {
            new TestMap1(Owner);
            Player player = new LocalPlayer(Owner, new Vector3(3f, 5f, 3f));
            playerCamera.Player = player;
        }

        public void Input(GameTime gameTime, InputManager input)
        {
            if (input.Keyboard_IsKeyDown(Keys.D8)) {
                gameState = GameState.Playing;
                Owner.Camera = playerCamera;
            }
            if (input.Keyboard_IsKeyDown(Keys.D9)) {
                if (gameState != GameState.Editing)
                {
                    gameState = GameState.Editing;
                    debugCamera.Position = playerCamera.Position+Vector3.Up;
                    //+Vector3.Up only so you notice you've changed camera mode
                    debugCamera.Target = playerCamera.Player.Position;
                    Owner.Camera = debugCamera;
                }
            }
        }
    }
}
