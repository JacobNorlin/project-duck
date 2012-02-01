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
        public override void Initialize(Engine Engine)
        {
            Engine.Window.AllowUserResizing = true;
        }

        public override void LoadContent(Engine Engine)
        {
            new GameController(Engine);
        }
    }
}
