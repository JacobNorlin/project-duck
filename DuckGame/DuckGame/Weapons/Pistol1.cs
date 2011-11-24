﻿using DuckEngine;
using DuckGame.Players;
using Microsoft.Xna.Framework;
using DuckEngine.Helpers;

namespace DuckGame.Weapons
{
    class Pistol1 : Weapon
    {
        public Pistol1(Engine _owner, Player _holder, string _description, float _reloadTime)
            : base(_owner, _holder)
        {
        }

        public override void Fire(Vector3 target)
        {
            Vector3 origin = Conversion.ToXNAVector(holder.Body.Position);
            origin.Z += 1;
            //Projectile(Owner, Holder, Damage, Speed, Target, CollisionSize)
            Projectile p = new Ammo1(Owner,origin, 1, 25f, target, 1);
        }
    }
}
