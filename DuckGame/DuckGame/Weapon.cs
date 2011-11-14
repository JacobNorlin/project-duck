using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckEngine
{
    /// <summary>
    /// Abstract for a weapon, contains all fields and methods each weapon must employ.
    /// </summary>
    abstract class Weapon : ILogic
    {
        protected Player owner;
        public Player Owner { get { return owner; } }

        protected String description;
        public String Description { get { return description; } }

        protected float reloadTime;
        public float ReloadTime { get { return reloadTime; } }

        /// <summary>
        /// Will fire the weapon towards the given coordinates x, y and z.
        /// </summary>
        /// <param name="x">x-coordinate</param>
        /// <param name="y">y-coordinate</param>
        /// <param name="z">z-coordinate</param>
        public void Fire(float x, float y, float z);

        /// <summary>
        /// Reloads the weapon.
        /// </summary>
        public void Reload();
        
    }
}
