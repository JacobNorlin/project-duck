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
    class Pickup : Entity, ILogic, IDraw3D, ICollide
    {
        private RigidBody body;
        public RigidBody Body { get { return body; } }

        public Pickup(Engine _owner, Vector3 position)
            : base(_owner)
        {
            Shape boxShape = new BoxShape(1f, .5f, .5f);
            body = new RigidBody(boxShape);
            body.AffectedByGravity = false;
            body.Position = Conversion.ToJitterVector(position);
            body.Tag = this;
            
            Owner.Physics.AddBody(body);
        }

        ~Pickup()
        {
            Owner.removeAll(this);
            Owner.Physics.RemoveBody(body);
        }

        public void Draw3D(GameTime gameTime)
        {
            Owner.Helper3D.DrawBoxBody(Body, Color.Red);
        }

        public void Collide(Entity other)
        {
            if (other is Player)
            {
                Owner.removeAll(this);
                Owner.Physics.RemoveBody(body);
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
