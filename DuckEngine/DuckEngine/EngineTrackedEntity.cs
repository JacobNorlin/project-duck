using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckEngine
{
    /// <summary>
    /// A class for objects which should be directly tracked by the
    /// engine, and not by a map or something else.
    /// </summary>
    public class EngineTrackedEntity : Entity
    {
        /// <summary>
        /// Create this entity, and make the engine track it.
        /// </summary>
        /// <param name="_engine"></param>
        public EngineTrackedEntity(Engine _engine)
            : base(_engine, new Tracker(_engine))
        {
            Engine.Tracker.Track(Tracker);
        }
    }
}
