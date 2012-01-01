using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckEngine
{
    /// <summary>
    /// A basic entity from which all entities inherit.
    /// Will automatically enable calls to the methods
    /// inherited from interfaces in the Interfaces folder.
    /// </summary>
    public abstract class Entity
    {
        readonly Engine owner;
        public Engine Owner { get { return owner; } }
        private bool enableInterfaceCalls = false;
        protected bool EnableInterfaceCalls
        {
            get { return enableInterfaceCalls; }
            set
            {
                if (value && !enableInterfaceCalls)
                {
                    Owner.addAll(this);
                }
                else if (!value && enableInterfaceCalls)
                {
                    Owner.removeAll(this);
                }
                enableInterfaceCalls = value;
            }
        }
        public bool Active
        {
            get { return EnableInterfaceCalls; }
            set { EnableInterfaceCalls = value; }
        }

        /// <summary>
        /// The base constructor for all entites.
        /// </summary>
        /// <param name="_owner">The Engine which owns the current entity.</param>
        public Entity(Engine _owner)
        {
            owner = _owner;
            Active = true;
        }
    }
}