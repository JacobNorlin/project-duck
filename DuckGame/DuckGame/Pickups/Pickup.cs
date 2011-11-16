﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckEngine;
using DuckEngine.Interfaces;
using Microsoft.Xna.Framework;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Jitter.Collision.Shapes;
using DuckEngine.Helpers;
using DuckGame.Players;

namespace DuckGame.Pickups
{
    class Pickup : Entity, ILogic, IDraw3D, ICollide
    {
        private RigidBody body;
        public RigidBody Body { get { return body; } }

        public Pickup(Engine _owner, Vector3 position)
            : base(_owner)
        {
            Owner.addDraw3D(this);
            Owner.addLogic(this);
            Shape boxShape = new BoxShape(1f, .5f, .5f);
            body = new RigidBody(boxShape);
            body.AffectedByGravity = false;
            body.Position = Conversion.ToJitterVector(position);
            body.Tag = this;
            
            Owner.World.AddBody(body);
        }

        ~Pickup()
        {
            Owner.removeDraw3D(this);
            Owner.World.RemoveBody(body);
        }

        public void Draw3D(GameTime gameTime)
        {
            Owner.Helper3D.DrawBoxBody(Body, Color.Red);
        }

        public void Collide(Entity other)
        {
            if (other is Player)
            {
                Owner.removeDraw3D(this);
                Owner.World.RemoveBody(body);
            }
        }

        public bool BroadPhaseFilter(Entity other)
        {
            if (other is Player)
            {
                return true;
            }

            return false;
        }

        public void Update(GameTime gameTime)
        {
            body.AngularVelocity = new JVector(0f, 5f, 0f);
        }
    }
}
