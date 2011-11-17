using DuckEngine;
using DuckEngine.Input;
using DuckEngine.Interfaces;
using DuckGame.Maps;
using Microsoft.Xna.Framework;

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
