﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DuckEngine
{
    class Ammo1 : Projectile
    {
        public Ammo1(Engine _owner, Vector3 _position)
            : base(_owner)
        {
            position = _position;
        }

        public override void Update(GameTime gameTime)
        {
            //throw new NotImplementedException();
        }

        public override void Draw3D(GameTime gameTime)
        {
            //throw new NotImplementedException();
        }
    }
}