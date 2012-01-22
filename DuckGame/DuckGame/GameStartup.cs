using DuckEngine;
using DuckEngine.Input;
using DuckEngine.Interfaces;
using DuckGame.Maps;
using Microsoft.Xna.Framework;
using DuckGame.Players;
using Microsoft.Xna.Framework.Input;
using System;
using Microsoft.Xna.Framework.Graphics;

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
            Model playerModel = _owner.Content.Load<Model>("Models/milk");
            Player player = new LocalPlayer(_owner, new Vector3(3f, 5f, 3f), playerModel);
            GameController controller = new GameController(_owner, player);
        }
    }
}
