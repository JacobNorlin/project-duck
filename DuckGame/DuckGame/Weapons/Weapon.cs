using System;
using DuckEngine;
using DuckEngine.Interfaces;
using DuckGame.Players;
using Microsoft.Xna.Framework;

namespace DuckGame.Weapons
{
    /// <summary>
    /// Abstract for a weapon, contains all fields and methods each weapon must employ.
    /// </summary>
    abstract class Weapon : Entity, ILogic
    {
        //Holder of the weapon
        protected Player holder;
        public Player Holder { get { return holder; } }

        //Weapon description
        protected String description;
        public String Description { get { return description; } }

        //Time needed to reload, in Ms
        protected float reloadTime;
        public float ReloadTime { get { return reloadTime; } }

        //Cooldown between each shot fired, in Ms
        protected float cooldown;
        public float Cooldown { get { return cooldown; } }
        
        //Variable to keep track of how much time has passed in between shots.
        protected float tempCooldown;
        public float TempCooldown { get { return tempCooldown; } }
        public Weapon(Engine _owner, Player _holder, float _reloadTime, String _description, float _cooldown)
            : base(_owner)
        {
            holder = _holder;
            reloadTime = _reloadTime;
            description = _description;
            cooldown = _cooldown;
        }

        /// <summary>
        /// Will fire the weapon towards the given coordinates Vector3.
        /// </summary>
        /// <param name="target">Target</param>
        public virtual void Fire(Vector3 target)
        {

        }

        /// <summary>
        /// Reloads the weapon.
        /// </summary>
        public void Reload() { }


        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            System.TimeSpan sysCooldown = gameTime.ElapsedGameTime;
            tempCooldown += sysCooldown.Milliseconds;
        }
    }
}
