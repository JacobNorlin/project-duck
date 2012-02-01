using DuckEngine;
using DuckEngine.Interfaces;
using DuckEngine.Helpers;
using DuckEngine.Storage;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using System.Xml;

namespace DuckGame
{
    class Box : PhysicalEntity, IDraw3D, ISave
    {
        public Box(Engine _engine, Tracker _tracker, JVector size)
            : this(_engine, _tracker, new RigidBody(new BoxShape(size))) {}

        public Box(Engine _engine, Tracker _tracker, RigidBody rigidBody)
            : base(_engine, _tracker, rigidBody) {}

        public Box(Engine _engine, Tracker _tracker, RigidBody rigidBody, bool _enableInterfaceCalls)
            : base(_engine, _tracker, rigidBody, _enableInterfaceCalls) {}

        public void Draw3D(GameTime gameTime)
        {
            Engine.Helper3D.DrawBody(Body, Color.Green, true, true);
        }

        public override PhysicalEntity Clone(bool _enableInterfaceCalls)
        {
            return new Box(Engine, Tracker, Body.Clone(), _enableInterfaceCalls);
        }

        public void Save(XmlDocument doc, XmlElement currNode)
        {
            XmlElement bodyNode = doc.CreateElement("body");
            Body.Save(doc, bodyNode);
            currNode.AppendChild(bodyNode);
        }

        public static Box Load(Engine _engine, Tracker _tracker, XmlNode node)
        {
            XmlNode bodyNode = node.SelectSingleNode("body");
            //RigidBody body = new RigidBody(new BoxShape());
            //body.Load(bodyNode);
            RigidBody body = DuckEngine.Storage.StorageExtensions.LoadRigidBody(bodyNode);
            return new Box(_engine, _tracker, body);
        }
    }
}
