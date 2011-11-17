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
        protected Player holder;
        public Player Holder { get { return holder; } }

        protected String description;
        public String Description { get { return description; } }

        protected float reloadTime;
        public float ReloadTime { get { return reloadTime; } }

        public Weapon(Engine _owner)
            : base(_owner)
        {

        }

        /// <summary>
        /// Will fire the weapon towards the given coordinates Vector3.
        /// </summary>
        /// <param name="target">Target</param>
        public virtual void Fire(Vector3 target) { }

        /// <summary>
        /// Reloads the weapon.
        /// </summary>
        public void Reload() { }


        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}
