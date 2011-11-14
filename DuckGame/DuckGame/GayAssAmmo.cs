using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckEngine
{
    class GayAssAmmo : Projectile
    {
        public GayAssAmmo(float[] _position)
        {
            position = _position;
        }

        public void Move()
        {
            position[0] = target[0] / speed;
            position[1] = target[1] / speed;
            position[2] = target[2] / speed;

        }

    }
}
