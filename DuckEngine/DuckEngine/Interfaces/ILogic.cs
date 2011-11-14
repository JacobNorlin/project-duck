using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DuckEngine.Interfaces
{
    /// <summary>
    /// An interface to update objects each fram
    /// </summary>
    public interface ILogic
    {
        /// <summary>
        /// Update the object each frame.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        void Update(GameTime gameTime);
    }
}
