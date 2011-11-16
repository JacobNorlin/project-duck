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

        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;

        public Terrain(Engine _owner)
            : base(_owner)
        {
            Owner.addDraw3D(this);

            //Load heightmap
            Texture2D heightmap = Owner.Content.Load<Texture2D>("Terrain/heightmap");
            terrainWidth = heightmap.Width;
            terrainHeight = heightmap.Height;

            //Get colors (heights)
            heightData = new float[terrainWidth, terrainHeight];
            Color[] heightMapColors = new Color[terrainWidth * terrainHeight];
            heightmap.GetData<Color>(heightMapColors);

            //Create height data for physics engine
            for (int x = 0; x < terrainWidth; x++)
            {
                for (int y = 0; y < terrainHeight; y++)
                {
                    heightData[x, y] = (heightMapColors[x + y * terrainWidth].R - 128f) / 5.0f;
                }
            }

            //Create body & shape
            Shape terrainShape = new TerrainShape(heightData, 1f, 1f);
            RigidBody terrainBody = new RigidBody(terrainShape);
            terrainBody.IsStatic = true;
            Owner.World.AddBody(terrainBody);

            //Create vertices and indices for rendering
            SetUpVertices();
            SetUpIndices();
        }

        public void Draw3D(GameTime gameTime)
        {
            Owner.Helper3D.DrawVertices(vertexBuffer, indexBuffer);
        }

        private void SetUpVertices()
        {
            VertexPositionColor[] vertices = new VertexPositionColor[terrainWidth * terrainHeight];
            for (int x = 0; x < terrainWidth; x++)
            {
                for (int y = 0; y < terrainHeight; y++)
                {
                    vertices[x + y * terrainWidth].Position = new Vector3(x, heightData[x, y], y);
                    vertices[x + y * terrainWidth].Color = Color.White;
                }
            }

            vertexBuffer = new VertexBuffer(Owner.GraphicsDevice, VertexPositionColor.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionColor>(vertices);
        }

        private void SetUpIndices()
        {
            int[] indices = new int[(terrainWidth - 1) * (terrainHeight - 1) * 6];
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

            indexBuffer = new IndexBuffer(Owner.GraphicsDevice, typeof(int), indices.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData(indices);
        }
    }
}
