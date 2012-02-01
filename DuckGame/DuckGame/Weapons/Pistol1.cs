﻿using DuckEngine;
using DuckGame.Players;
using Microsoft.Xna.Framework;
using DuckEngine.Helpers;

namespace DuckGame.Weapons
{

    class Pistol1 : Weapon
    {
        

        public Pistol1(Engine _engine, Tracker _tracker, Player _holder, string _description, float _reloadTime, float _cooldown)
            : base(_engine, _tracker, _holder, _reloadTime, _description, _cooldown)
        {
        }



        public override void Fire(Vector3 target)
        {
            //Check if the counter time since lasted fire shot is at the cooldown time.
            if (tempCooldown >= cooldown)
            {
                Vector3 origin = Conversion.ToXNAVector(holder.Body.Position);
                origin.Z += 1;
                //Projectile(Engine, Holder, Damage, Speed, Target, CollisionSize)
                Projectile p = new Ammo1(Engine, Tracker, origin, 1, 25f, target, 1);
                //reset the cooldown.
                tempCooldown = 0;
            }
        }
    }
}
