using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckEngine
{
    public abstract class StartupObject
    {
        public abstract void Initialize(Engine Owner);
        public abstract void LoadContent(Engine Owner);
    }
}
