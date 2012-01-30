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
        public Box(Engine _owner, JVector size)
            : this(_owner, new RigidBody(new BoxShape(size)))
        {
        }

        public Box(Engine owner, RigidBody rigidBody)
            : base(owner, rigidBody)
        {
        }

        public void Draw3D(GameTime gameTime)
        {
            Owner.Helper3D.DrawBody(Body, Color.Green, true, true);
        }

        public override PhysicalEntity Clone()
        {
            return new Box(Owner, Body.Clone());
        }

        public void Save(XmlDocument doc, XmlElement currNode)
        {
            XmlElement bodyNode = doc.CreateElement("body");
            Body.Save(doc, bodyNode);
            currNode.AppendChild(bodyNode);
        }

        public static Box Load(Engine engine, XmlNode node)
        {
            XmlNode bodyNode = node.SelectSingleNode("body");
            //RigidBody body = new RigidBody(new BoxShape());
            //body.Load(bodyNode);
            RigidBody body = DuckEngine.Storage.StorageExtensions.LoadRigidBody(bodyNode);
            return new Box(engine, body);
        }
    }
}
