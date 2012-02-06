using DuckEngine;
using DuckEngine.Helpers;
using DuckEngine.Interfaces;
using DuckEngine.Storage;
using DuckGame.Players;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using System.Xml;

namespace DuckGame.Pickups
{
    class Pickup : Entity, IPhysical, ILogic, IDraw3D, ICollideEvent, ISave
    {
        private RigidBody body;
        private JVector rotationVector = new JVector(0f, 3f, 0f);
        public RigidBody Body { get { return body; } }

        public Pickup(Engine _engine, Tracker _tracker, JVector position)
            : base(_engine, _tracker, false)
        {
            Shape boxShape = new BoxShape(1f, .5f, .5f);
            body = new RigidBody(boxShape);
            body.AffectedByGravity = false;
            body.Position = position;
            body.Tag = this;
            EnableInterfaceCalls = true;
        }

        public void Draw3D(GameTime gameTime)
        {
            Engine.Helper3D.DrawBody(Body, Color.Red, true, true);
        }

        public void Collide(Entity other)
        {
            if (other is Player)
            {
                Dispose();
            }
        }

        public bool BroadPhaseFilter(Entity other)
        {
            if (other is Player)
            {
                return true;
            }

            return false;
        }

        public void Update(GameTime gameTime)
        {
            body.AngularVelocity = rotationVector;
        }

        public void Save(XmlDocument doc, XmlElement currNode)
        {
            XmlElement posNode = doc.CreateElement("pos");
            Body.Position.Save(doc, posNode);
            currNode.AppendChild(posNode);

        }

        public static Pickup Load(Engine _engine, Tracker _tracker, XmlNode node)
        {
            XmlNode posNode = node.SelectSingleNode("pos");
            JVector pos = StorageExtensions.LoadJVector(posNode);
            return new Pickup(_engine, _tracker, pos);
        }
    }
}
