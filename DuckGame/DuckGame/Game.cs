using DuckEngine;
using DuckEngine.Input;
using DuckEngine.Interfaces;
using DuckGame.Maps;
using Microsoft.Xna.Framework;
using DuckGame.Players;

namespace DuckGame
{
    public class Game : StartupObject, IInput
    {
        PlayerCamera camera;

        public override void Initialize(Engine Owner)
        {
            //Owner.Camera = new DebugCamera(Owner, new Vector3(0, 3, 10));
            camera = new PlayerCamera(Owner);
            Owner.Camera = camera;
            Owner.Window.AllowUserResizing = true;
            Owner.addInput(this);
        }

        public override void LoadContent(Engine Owner)
        {
            new TestMap1(Owner);
            Player player = new LocalPlayer(Owner, new Vector3(3f, 5f, 3f));
            camera.Player = player;
        }

        public void Input(GameTime gameTime, InputManager input)
        {

        }
    }
}
