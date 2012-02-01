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

namespace DuckGame.Weapons
{
    abstract class Projectile : Entity, IPhysical, ILogic, IDraw3D, ICollideEvent
    {

        private static JVector size = new JVector(0.5f, 0.5f, 0.5f);

        protected float damage;
        public float Damage { get { return damage; } }

        protected float speed;
        public float Speed { get { return speed; } }

        protected Vector3 target;
        public Vector3 Target { get { return target; } }

        protected float collisionSize;
        public float CollisionSize { get { return collisionSize; } }

        protected RigidBody body;
        public RigidBody Body { get { return body; } }

        public Projectile(Engine _engine, Tracker _tracker, Vector3 _position, float _damage, float _speed, Vector3 _target, float _collisionSize)
            : base(_engine, _tracker)
        {
            damage = _damage;
            speed = _speed;
            target = _target;
            collisionSize = _collisionSize;

            //Create body and add to physics engine
            Shape boxShape = new BoxShape(size);
            body = new RigidBody(boxShape);
            body.Mass = 1f;
            body.Position = Conversion.ToJitterVector(_position);
            body.AllowDeactivation = false;
            body.AffectedByGravity = false;
            body.Tag = this;
            Engine.Physics.AddBody(body);
        }

        //public void Move() { } //Unecessary, use physics engine instead.

        public virtual void OnHit() { }

        public abstract void Update(GameTime gameTime);

        public abstract void Draw3D(GameTime gameTime);

        public void Collide(Entity other)
        {
            OnHit();
        }

        public bool BroadPhaseFilter(Entity other)
        {
            if (other is Projectile)
            {
                return false;
            }

            return false;
        }
    }
}
