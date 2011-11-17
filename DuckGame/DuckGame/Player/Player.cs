using System.Collections.Generic;
using DuckEngine;
using DuckEngine.Helpers;
using DuckEngine.Interfaces;
using DuckGame.Weapons;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.Dynamics.Constraints;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using DuckEngine.Maps;

namespace DuckGame.Players
{
    class Player : Entity, ILogic, IDraw3D, ICollide
    {
        private static JVector size = new JVector(1, 2, 1);

        protected bool isJumping = false;
        public bool IsJumping { get { return isJumping; } }

        bool wasGrounded = true;
        bool isGrounded = true;

        private float hp;
        public float HP { get { return hp; } }

        protected RigidBody body;
        public RigidBody Body { get { return body; } }

        private List<Weapon> weapons = new List<Weapon>();
        public List<Weapon> Weapons { get { return weapons; } }

        public Vector3 Position { get { return Conversion.ToXNAVector(Body.Position); } }

        public Player(Engine _owner, Vector3 position)
            : base(_owner)
        {
            //Add to engine
            Owner.addDraw3D(this);
            Owner.addLogic(this);
            
            //Create body and add to physics engine
            Shape boxShape = new BoxShape(size);
            body = new RigidBody(boxShape);
            body.Mass = 2f;
            body.Position = Conversion.ToJitterVector(position);
            body.AllowDeactivation = false;
            body.Tag = this;
            Owner.Physics.AddBody(body);

            //Players can't fall
            Constraint upright = new Jitter.Dynamics.Constraints.SingleBody.FixedAngle(body);
            Owner.Physics.AddConstraint(upright);
        }

        ~Player()
        {
            //Remove from engine
            Owner.removeDraw3D(this);
            Owner.Physics.RemoveBody(body);
            Owner.removeLogic(this);
        }

        public void Update(GameTime gameTime)
        {
            //Push Grounded status back in queue.
            wasGrounded = isGrounded;
            isGrounded = false;
        }

        public void Draw3D(GameTime gameTime)
        {
            Owner.Helper3D.DrawBoxBody(Body, Color.Blue);
        }

        public void Collide(Entity other)
        {
            if (other is Box || other is Terrain)
            {
                //We are touching ground
                isGrounded = true;

                //If we were in the air, ...
                if (!wasGrounded)
                {
                    //... we have now landed
                    isJumping = false;
                }
            }
        }

        public bool BroadPhaseFilter(Entity other)
        {
            return true;
        }
    }
}
