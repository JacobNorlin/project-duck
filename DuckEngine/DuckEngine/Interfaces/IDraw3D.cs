using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DuckEngine.Interfaces
{
    /// <summary>
    /// An interface for drawing 3D to the screen
    /// </summary>
    public interface IDraw3D
    {
        /// <summary>
        /// A function responsible for drawing 3D to the screen.
        /// </summary>
        /// <param name="gameTime">Provides timing snapshots.</param>
        void Draw3D(GameTime gameTime);
    }
}
