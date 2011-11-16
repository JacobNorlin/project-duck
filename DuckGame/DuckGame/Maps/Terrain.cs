using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckEngine.Interfaces;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace DuckEngine.Maps
{
    class Terrain : Entity, IDraw3D
    {
        int terrainWidth;
        int terrainHeight;
        float[,] heightData;
        BasicEffect basicEffect;

        VertexPositionColor[] vertices;
        int[] indices;

        public Terrain(Engine _owner)
            : base(_owner)
        {
            terrainWidth = 1;
            terrainHeight = 1;
            heightData = new float[,] { 
                {1f, 1f, 1f},
                {1f, 2f, 1f},
                {1f, 1f, 1f}
            };
            Shape terrainShape = new TerrainShape(heightData, terrainWidth, terrainHeight);
            RigidBody terrainBody = new RigidBody(terrainShape);
            basicEffect = new BasicEffect(Owner.GraphicsDevice);

        }

        public void Draw3D(GameTime gameTime)
        {
            Owner.Helper3D.DrawVertices(basicEffect, vertices, indices);
            //throw new NotImplementedException();
        }

        private void SetUpVertices()
        {
            vertices = new VertexPositionColor[terrainWidth * terrainHeight];
            for (int x = 0; x < terrainWidth; x++)
            {
                for (int y = 0; y < terrainHeight; y++)
                {
                    vertices[x + y * terrainWidth].Position = new Vector3(x, heightData[x, y], -y);
                    vertices[x + y * terrainWidth].Color = Color.White;
                }
            }
        }

        private void SetUpIndices()
        {
            indices = new int[(terrainWidth - 1) * (terrainHeight - 1) * 6];
            int counter = 0;
            for (int y = 0; y < terrainHeight - 1; y++)
            {
                for (int x = 0; x < terrainWidth - 1; x++)
                {
                    int lowerLeft = x + y * terrainWidth;
                    int lowerRight = (x + 1) + y * terrainWidth;
                    int topLeft = x + (y + 1) * terrainWidth;
                    int topRight = (x + 1) + (y + 1) * terrainWidth;

                    indices[counter++] = topLeft;
                    indices[counter++] = lowerRight;
                    indices[counter++] = lowerLeft;

                    indices[counter++] = topLeft;
                    indices[counter++] = topRight;
                    indices[counter++] = lowerRight;
                }
            }
        }
    }
}
