using DuckEngine;
using DuckEngine.Interfaces;
using Microsoft.Xna.Framework;

namespace DuckGame.Weapons
{
    abstract class Projectile : Entity, ILogic, IDraw3D
    {
        protected Vector3 position;
        public Vector3 Position { get { return position; } }

        protected float damage;
        public float Damage { get { return damage; } }

        protected float speed;
        public float Speed { get { return speed; } }

        protected Vector3 target;
        public Vector3 Target { get { return target; } }

        protected float collisionSize;
        public float CollisionSize { get { return collisionSize; } }

        public Projectile(Engine _owner)
            : base(_owner)
        {
        }

        //public void Move() { } //Unecessary, use physics engine instead.

        public virtual void OnHit() { }

        public abstract void Update(GameTime gameTime);

        public abstract void Draw3D(GameTime gameTime);
    }
}
