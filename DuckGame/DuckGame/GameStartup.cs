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
    public class GameStartup : StartupObject
    {
        public override void Initialize(Engine _owner)
        {
            _owner.Window.AllowUserResizing = true;
        }

        public override void LoadContent(Engine _owner)
        {
            new TestMap1(_owner);
            Player player = new LocalPlayer(_owner, new Vector3(3f, 5f, 3f));
            GameController controller = new GameController(_owner, player);
        }
    }
}
