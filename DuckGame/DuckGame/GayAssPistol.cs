using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckEngine
{
    class GayAssPistol : Weapon
    {
        public GayAssPistol(Player _owner, string _description, float _reloadTime)
        {
            owner = _owner;
            description = _description;
            reloadTime = _reloadTime;
        }

        public void fire(float x, float y, float z)
        {
            Projectile p = new GayAssAmmo(owner.Position);

        }
     }
}
