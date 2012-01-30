using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jitter.Dynamics;

namespace DuckEngine
{
    abstract public class PhysicalEntity : Entity
    {
        private readonly RigidBody body;
        public RigidBody Body { get { return body; } }
        private bool inPhysicsEngine = false;
        protected bool InPhysicsEngine
        {
            set
            {
                if (value && !inPhysicsEngine)
                {
                    Owner.Physics.AddBody(Body);
                }
                else if (!value && inPhysicsEngine)
                {
                    Owner.Physics.RemoveBody(Body);
                }
                inPhysicsEngine = value;
            }
            get { return inPhysicsEngine; }
        }
        new public bool Active
        {
            get { return EnableInterfaceCalls && InPhysicsEngine; }
            set { EnableInterfaceCalls = InPhysicsEngine = value; }
        }

        public PhysicalEntity(Engine _owner, RigidBody _body)
            : base(_owner)
        {
            body = _body;
            Body.Tag = this;
            InPhysicsEngine = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>A new Entity, copy of this one</returns>
        abstract public PhysicalEntity Clone();
    }
}
