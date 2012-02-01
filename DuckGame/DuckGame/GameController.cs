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
using DuckGame.Maps;
using Microsoft.Xna.Framework.Graphics;
using Jitter.Collision.Shapes;
using DuckEngine.Storage;

namespace DuckGame
{
    class GameController : EngineTrackedEntity, IInput
    {
        private enum GameState { Playing, Editing };
        private GameState gameState = GameState.Playing;
        PlayerCamera playerCamera;
        DebugCamera debugCamera;
        MapEditor mapEditor;

        public GameController(Engine _engine)
            : base(_engine)
        {
            Map map = StorageManager.Load(Engine);
            //Map map = new TestMap1(Engine, null);
            //Map map = new LoadedMap(Engine, null);
            Model playerModel = Engine.Content.Load<Model>("Models/tire");
            Player player = new LocalPlayer(Engine, Tracker, new Vector3(3f, 5f, 3f), playerModel);
            Engine.Map = map;
            
            playerCamera = new PlayerCamera(Engine, Tracker);
            debugCamera = new DebugCamera(Engine, Tracker, Vector3.Zero);
            mapEditor = new MapEditor(this, Tracker);
            playerCamera.Player = player;
            Engine.Camera = playerCamera;
        }

        public void Input(GameTime gameTime, InputManager input)
        {
            if (input.Keyboard_IsKeyDown(Keys.D9) && gameState != GameState.Editing)
            {
                gameState = GameState.Editing;
                debugCamera.Position = playerCamera.Position + Vector3.Up;
                //+Vector3.Up only so you notice you've changed camera mode
                debugCamera.Target = playerCamera.Player.Position;
                Engine.Camera = debugCamera;
                mapEditor.Active = true;
            }
            if (input.Keyboard_IsKeyDown(Keys.D8) && gameState != GameState.Playing)
            {
                gameState = GameState.Playing;
                Engine.Camera = playerCamera;
                mapEditor.Active = false;
            }
            
            if (input.Keyboard_WasKeyReleased(Keys.Escape))
            {
                Engine.Exit();
            }
        }
    }
}
