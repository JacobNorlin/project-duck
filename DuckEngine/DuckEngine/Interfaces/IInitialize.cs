using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckEngine
{
    /// <summary>
    /// An interface for everything that needs to initialize
    /// something _after_ the engine is fully initialized.
    /// </summary>
    public interface IInitialize
    {
        /// <summary>
        /// The function to be run after the engine has been initialized
        /// </summary>
        void Initialize();
    }
}
