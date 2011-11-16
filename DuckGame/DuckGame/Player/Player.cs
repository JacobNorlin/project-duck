using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckEngine.Interfaces;
using Microsoft.Xna.Framework;
using DuckEngine.Input;
using Microsoft.Xna.Framework.Input;
using Jitter.Dynamics;
using Jitter.Collision.Shapes;
using Jitter.LinearMath;
using Jitter.Dynamics.Constraints;
using DuckEngine;
using DuckGame.Weapons;
using DuckEngine.Helpers;

namespace DuckGame.Players
{
    class Player : Entity, ILogic, IDraw3D
    {
        private static JVector size = new JVector(1, 2, 1);

        private float hp;
        public float HP { get { return hp; } }

        protected RigidBody body;
        public RigidBody Body { get { return body; } }

        private List<Weapon> weapons = new List<Weapon>();
        public List<Weapon> Weapons { get { return weapons; } }

        public Player(Engine _owner, Vector3 position)
            : base(_owner)
        {
            //Add to engine
            Owner.addDraw3D(this);
            
            //Create body and add to physics engine
            Shape boxShape = new BoxShape(size);
            body = new RigidBody(boxShape);
            body.Position = Conversion.ToJitterVector(position);
            body.AllowDeactivation = false;
            body.Tag = this;
            Owner.World.AddBody(body);

            //Players can't fall
            Constraint upright = new Jitter.Dynamics.Constraints.SingleBody.FixedAngle(body);
            Owner.World.AddConstraint(upright);
        }

        ~Player()
        {
            //Remove from engine
            Owner.removeDraw3D(this);
            Owner.World.RemoveBody(body);
        }

        public void Update(GameTime gameTime)
        {
            //throw new NotImplementedException();
        }

        public void Draw3D(GameTime gameTime)
        {
            Owner.Helper3D.DrawBoxBody(Body, Color.Blue);
        }
    }
}
