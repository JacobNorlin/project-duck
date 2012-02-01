using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jitter.Dynamics;
using DuckEngine.Helpers;
using DuckEngine.Interfaces;

namespace DuckEngine
{
    public abstract class PhysicalEntity : Entity, IPhysical
    {
        private RigidBody body;
        public RigidBody Body
        {
            get { return body; }
            protected set
            {
                RigidBody oldBody = body;
                //oldBody.Tag = null;
                body = value;
                if (body != null)
                {
                    body.Tag = this;
                }
                Tracker.UpdatePhysicalTrack(this, oldBody);
            }
        }

        public PhysicalEntity(Engine _engine, Tracker _tracker, RigidBody _body)
            : this(_engine, _tracker, _body, true) {}

        public PhysicalEntity(Engine _engine, Tracker _tracker, RigidBody _body, bool _enableInterfaceCalls)
            : base(_engine, _tracker, false)
        {
            Body = _body;
            EnableInterfaceCalls = _enableInterfaceCalls;
        }

        /// <summary>
        /// Create a clone of this PhysicalEntity
        /// </summary>
        /// <param name="_enableInterfaceCalls">Enable interface calls on the new object upon creation</param>
        /// <returns>The copy, or null if </returns>
        public virtual PhysicalEntity Clone(bool _enableInterfaceCalls)
        {
            return null;
        }
    }
}
