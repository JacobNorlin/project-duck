using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckEngine
{
    public class Entity
    {
        public Engine owner;
        public Engine Owner
        {
            get { return owner; }
        }
    }
}
