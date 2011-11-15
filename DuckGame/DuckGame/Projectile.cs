using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna;
using DuckEngine.Interfaces;

namespace DuckEngine
{
    abstract class Projectile : ILogic
    {
        protected float[] position = new float[3];
        public float[] Position { get { return position; } }

        protected float damage;
        public float Damage { get { return damage; } }

        protected float speed;
        public float Speed { get { return speed; } }

        protected float[] target = new float[3];
        public float[] Target { get { return target; } }

        protected float collisionSize;
        public float CollisionSize { get { return collisionSize; } }

        public void Move() { }

        public void onHit() { }

        public void Update(Microsoft.Xna.Framework.GameTime a)
        {
            Move();
        }
    }
}
