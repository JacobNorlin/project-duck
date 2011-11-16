using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckEngine.Interfaces;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;

namespace DuckEngine.Maps
{
    class Terrain : Entity, IDraw3D
    {
        public Terrain(Engine _owner)
            : base(_owner)
        {
            float[,] heights = new float[,] { 
                {1f, 1f, 1f},
                {1f, 2f, 1f},
                {1f, 1f, 1f}
            };
            Shape terrainShape = new TerrainShape(heights, 1f, 1f);
            RigidBody terrainBody = new RigidBody(terrainShape);

        }

        public void Draw3D(Microsoft.Xna.Framework.GameTime gameTime)
        {
            //throw new NotImplementedException();
        }
    }
}
