using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jitter.Dynamics;

namespace DuckEngine
{
    abstract class PhysicalEntity : Entity
    {
        protected RigidBody body;
        public RigidBody Body { get { return body; } }

        public PhysicalEntity(Engine _owner)
            : base(_owner) { }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>A new Entity, copy of this one</returns>
        abstract public PhysicalEntity Clone();
    }
}
