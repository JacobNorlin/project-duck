using DuckEngine;
using DuckEngine.Interfaces;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;

namespace DuckGame
{
    class Box : PhysicalEntity, IDraw3D
    {
        public Box(Engine _owner, JVector size)
            : this(_owner, new BoxShape(size))
        {
        }

        public Box(Engine _owner, Shape boxShape)
            : base(_owner)
        {
            body = new RigidBody(boxShape);
            body.Tag = this;
            Owner.Physics.AddBody(body);
        }

        ~Box()
        {
            Owner.removeAll(this);
            Owner.Physics.RemoveBody(body);
        }

        public void Draw3D(GameTime gameTime)
        {
            Owner.Helper3D.DrawBoxBody(Body, Color.Green);
        }

        public override PhysicalEntity Clone()
        {
            Box newBox = new Box(Owner, body.Shape);
            newBox.body.Position = body.Position;
            newBox.body.Orientation = body.Orientation;
            return newBox;
        }
    }
}
