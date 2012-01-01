using System.Collections.Generic;
using DuckEngine;
using DuckEngine.Helpers;
using DuckEngine.Interfaces;
using DuckEngine.Maps;
using DuckGame.Weapons;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.Dynamics.Constraints;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;

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

        public Weapon currentWeapon;

        public Player(Engine _owner, Vector3 position)
            : base(_owner)
        {
            //Create body and add to physics engine
            Shape boxShape = new BoxShape(size);
            body = new RigidBody(boxShape);
            body.Mass = 2f;
            body.Position = Conversion.ToJitterVector(position);
            body.AllowDeactivation = false;
            body.Tag = this;
            Owner.Physics.AddBody(body);

            //TODO: Fix so that players can rotate around Y-axis.
            //Players can't fall
            Constraint upright = new Jitter.Dynamics.Constraints.SingleBody.FixedAngle(body);
            Owner.Physics.AddConstraint(upright);

            //TEMPORARY FOR VIDYAJUEGOS
            currentWeapon = new Pistol1(_owner, this, "", 1, 1000);
        }

        ~Player()
        {
            //Remove from engine
            Owner.removeAll(this);
            Owner.Physics.RemoveBody(body);
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
