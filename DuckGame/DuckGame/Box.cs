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
            : base(_owner, new RigidBody(boxShape))
        {
        }

        public void Draw3D(GameTime gameTime)
        {
            Owner.Helper3D.DrawBody(Body, Color.Green, true, true);
        }

        public override PhysicalEntity Clone()
        {
            Box newBox = new Box(Owner, Body.Shape);
            newBox.Body.Position = Body.Position;
            newBox.Body.Orientation = Body.Orientation;
            newBox.Body.IsStatic = Body.IsStatic;
            return newBox;
        }
    }
}
