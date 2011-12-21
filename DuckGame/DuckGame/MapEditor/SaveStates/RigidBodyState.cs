using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jitter.LinearMath;
using Jitter.Dynamics;

namespace DuckEngine.MapEditor.SaveStates
{
    public struct RigidBodyState
    {
        private readonly RigidBody body;
        private readonly JVector savedPosition;
        private readonly JMatrix savedOrientation;
        private readonly JVector savedLinearVelocity;
        private readonly JVector savedAngularVelocity;

        internal RigidBodyState(RigidBody _body)
        {
            body = _body;
            savedPosition = body.Position;
            savedOrientation = body.Orientation;
            savedLinearVelocity = body.LinearVelocity;
            savedAngularVelocity = body.AngularVelocity;
        }

        public void restore()
        {
            body.Position = savedPosition;
            body.Orientation = savedOrientation;
            if (!body.IsStatic)
            {
                body.LinearVelocity = savedLinearVelocity;
                body.AngularVelocity = savedAngularVelocity;
            }
        }
    }

    public static class Extension
    {
        public static RigidBodyState Save(this RigidBody body)
        {
            return new RigidBodyState(body);
        }
    }
}
