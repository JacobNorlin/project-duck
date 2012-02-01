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
using Microsoft.Xna.Framework.Graphics;
using System;

namespace DuckGame.Players
{
    class Player : Entity, IPhysical, ILogic, IDraw3D, ICollideEvent
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
        private Model model;

        public Player(Engine _engine, Tracker _tracker, Vector3 position, Model _model)
            : base(_engine, _tracker, false)
        {
            model = _model;
            //Create body and add to physics engine
            Shape capsuleShape = new CapsuleShape(1, 0.5f);
            body = new RigidBody(capsuleShape);
            body.Mass = 2f;
            body.Position = position.ToJitterVector();
            body.AllowDeactivation = false;
            body.Tag = this;
            EnableInterfaceCalls = true;

            //TODO: Fix so that players can rotate around Y-axis.
            //Players can't tip over
            Constraint upright = new Jitter.Dynamics.Constraints.SingleBody.FixedAngle(body);
            Engine.Physics.AddConstraint(upright);

            //TEMPORARY FOR VIDYAJUEGOS
            currentWeapon = new Pistol1(_engine, Tracker, this, "", 1, 1000);
        }

        public void Update(GameTime gameTime)
        {
            //Push Grounded status back in queue.
            wasGrounded = isGrounded;
            isGrounded = false;
        }

        public void Draw3D(GameTime gameTime)
        {
            Engine.Helper3D.DrawModel(model, body, Matrix.CreateScale(0.03f));
            Engine.Helper3D.BasicEffect.Alpha = 0.3f;
            Engine.Helper3D.DrawBody(Body, Color.White, false, false);
            Engine.Helper3D.BasicEffect.Alpha = 0.2f;
            Engine.Helper3D.DrawBody(Body, Color.White, true, false);
            Engine.Helper3D.BasicEffect.Alpha = 1;
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
