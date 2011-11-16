using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckEngine;
using DuckEngine.Interfaces;
using Microsoft.Xna.Framework;

namespace DuckGame.Pickups
{
    class Pickup : Entity, IDraw3D
    {
        public Pickup(Engine _owner)
            : base(_owner)
        {
        }

        public void Draw3D(GameTime gameTime)
        {
            //throw new NotImplementedException();
        }
    }
}
