using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DuckEngine.Input;

namespace DuckEngine.Interfaces
{
    public interface IInput
    {
        /// <summary>
        /// Provides input to the game object
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="input">Handles input</param>
        void Input(GameTime gameTime, InputManager input);
    }
}
