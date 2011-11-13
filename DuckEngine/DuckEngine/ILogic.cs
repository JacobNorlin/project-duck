using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DuckEngine
{
    public interface ILogic
    {
        void Update(GameTime gameTime);
    }
}
