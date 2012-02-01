using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jitter.Dynamics;
using DuckEngine.Interfaces;
using System.Xml;

namespace DuckEngine
{
    public abstract class Map : Entity //don't extend ISave
    {
        public readonly string LoadedFromFile;

        public IEnumerable<RigidBody> Bodies
        {
            get { return Tracker.Bodies; }
        }

        /// <summary>
        /// Creates a new map with an empty tracker.
        /// </summary>
        /// <param name="_engine"></param>
        /// <param name="_fromFile">File map was loaded from.</param>
        public Map(Engine _engine, string _fromFile)
            : base(_engine, new Tracker(_engine))
        {
            LoadedFromFile = _fromFile;
        }

        public abstract void Save(XmlDocument doc, XmlElement currNode);

        new public void Dispose()
        {
            foreach (RigidBody body in Tracker.Bodies)
            {
                Engine.Physics.RemoveBody(body);
            }
            base.Dispose();
        }
    }
}
