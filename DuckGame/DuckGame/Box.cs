using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckEngine;
using Jitter.Dynamics;
using Jitter.Collision.Shapes;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using DuckEngine.Interfaces;

namespace DuckGame
{
    class Box : Entity, IDraw3D
    {
        private RigidBody boxBody;
        public RigidBody Body { get { return boxBody; } }

        public Box(Engine _owner, JVector size)
            : base(_owner)
        {
            Owner.addDraw3D(this);
            Shape boxShape = new BoxShape(size);
            boxBody = new RigidBody(boxShape);
            Owner.World.AddBody(boxBody);
        }
        ~Box()
        {
            Owner.removeDraw3D(this);
            Owner.World.RemoveBody(boxBody);
        }

        public void Draw3D(GameTime gameTime)
        {
            Owner.Helper3D.DrawBoxBody(boxBody);
        }
    }
}
