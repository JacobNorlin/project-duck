using DuckEngine;
using DuckEngine.Helpers;
using DuckEngine.Interfaces;
using DuckGame.Players;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;

namespace DuckGame.Pickups
{
    class Pickup : Entity, IPhysical, ILogic, IDraw3D, ICollideEvent
    {
        private RigidBody body;
        public RigidBody Body { get { return body; } }

        public Pickup(Engine _engine, Tracker _tracker, Vector3 position)
            : base(_engine, _tracker, false)
        {
            Shape boxShape = new BoxShape(1f, .5f, .5f);
            body = new RigidBody(boxShape);
            body.AffectedByGravity = false;
            body.Position = Conversion.ToJitterVector(position);
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
                Tracker.Untrack(this);
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
            body.AngularVelocity = new JVector(0f, 5f, 0f);
        }
    }
}
