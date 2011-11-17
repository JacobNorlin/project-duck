using DuckEngine;
using DuckEngine.Interfaces;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;

namespace DuckGame
{
    class Box : Entity, IDraw3D
    {
        private RigidBody body;
        public RigidBody Body { get { return body; } }

        public Box(Engine _owner, JVector size)
            : base(_owner)
        {
            Owner.addDraw3D(this);
            Shape boxShape = new BoxShape(size);
            body = new RigidBody(boxShape);
            body.Tag = this;
            Owner.Physics.AddBody(body);
        }

        ~Box()
        {
            Owner.removeDraw3D(this);
            Owner.Physics.RemoveBody(body);
        }

        public void Draw3D(GameTime gameTime)
        {
            Owner.Helper3D.DrawBoxBody(Body, Color.Green);
        }
    }
}
