using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckEngine
{
    class Player
    {
        private float hp;
        public float HP { get { return hp; } }

        private float[] position = new float[3];
        public float[] Position { get { return position; } }

        private List<Weapon> weapons = new List<Weapon>();
        public List<Weapon> Weapons { get { return weapons; } }

    }
}
