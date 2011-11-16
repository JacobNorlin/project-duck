using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckEngine.Interfaces
{
    public interface ICollide
    {
        void Collide(Entity other);
        bool BroadPhaseFilter(Entity other);
    }
}
