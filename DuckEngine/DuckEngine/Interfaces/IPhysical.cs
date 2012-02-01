using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jitter.Dynamics;

namespace DuckEngine.Interfaces
{
    public interface IPhysical
    {
        RigidBody Body { get; }
    }
}
