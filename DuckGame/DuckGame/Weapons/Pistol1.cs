using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckEngine;
using DuckGame.Players;

namespace DuckGame.Weapons
{
    class Pistol1 : Weapon
    {
        public Pistol1(Engine _owner, Player _holder, string _description, float _reloadTime)
            : base(_owner)
        {
            holder = _holder;
            description = _description;
            reloadTime = _reloadTime;
        }

        public void fire(float x, float y, float z)
        {
            //Projectile p = new GayAssAmmo(owner.Position);

        }
     }
}
