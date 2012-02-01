using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckEngine
{
    /// <summary>
    /// A basic entity from which all entities inherit.
    /// Will automatically enable calls to the methods
    /// inherited from interfaces in the Interfaces folder.
    /// </summary>
    public abstract class Entity : IDisposable
    {
        public readonly Engine Engine;
        public readonly Tracker Tracker; //TODO: should this be readonly?
        private bool enableInterfaceCalls = false;
        public bool EnableInterfaceCalls
        {
            get { return enableInterfaceCalls; }
            set
            {
                if (value && !enableInterfaceCalls)
                {
                    Tracker.Track(this);
                }
                else if (!value && enableInterfaceCalls)
                {
                    Tracker.Untrack(this);
                }
                enableInterfaceCalls = value;
            }
        }

        /// <summary>
        /// Same as Entity(_engine, _tracker, true)
        /// </summary>
        /// <param name="_engine">The Engine which owns the current entity.</param>
        /// <param name="_tracker">The Tracker this entity should belong to.</param>
        public Entity(Engine _engine, Tracker _tracker)
            : this(_engine, _tracker, true) { }

        /// <summary>
        /// Base constructor for all entities, 
        /// </summary>
        /// <param name="_engine">The Engine which owns the current entity.</param>
        /// <param name="_tracker">The Tracker this entity should belong to.</param>
        /// <param name="_enableInterfaceCalls">Whether to enable interface calls in the constructor</param>
        public Entity(Engine _engine, Tracker _tracker, bool _enableInterfaceCalls)
        {
            Engine = _engine;
            Tracker = _tracker;
            EnableInterfaceCalls = _enableInterfaceCalls;
        }

        public void Dispose()
        {
            EnableInterfaceCalls = false;
        }
    }
}