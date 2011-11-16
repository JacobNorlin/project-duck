using System;
using Jitter.LinearMath;
using Jitter.Dynamics;
using DuckEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using DuckEngine.Helpers;
using DuckEngine.Interfaces;
using DuckEngine.Input;
using DuckGame.Maps;
using DuckEngine;

namespace DuckGame
{
    public class Game : StartupObject, IInput
    {
        public override void Initialize(Engine Owner)
        {
            Owner.Window.AllowUserResizing = true;
            Owner.addInput(this);
        }

        public override void LoadContent(Engine Owner)
        {
            new TestMap1(Owner);
        }

        public void Input(GameTime gameTime, InputManager input)
        {

        }
    }
}
