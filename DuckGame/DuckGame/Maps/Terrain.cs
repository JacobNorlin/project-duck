using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckEngine.Interfaces;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using DuckEngine.Helpers;
using Jitter.LinearMath;

namespace DuckEngine.Maps
{
    class Terrain : Entity, IDraw3D
    {
        short terrainWidth;
        short terrainHeight;
        float[,] heightData;

        RigidBody body;
        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;

        public Terrain(Engine _owner, string terrainFile)
            : base(_owner)
        {
            Owner.addDraw3D(this);
            //Load heightmap
            Texture2D heightmap = Owner.Content.Load<Texture2D>("Terrain/" + terrainFile);
            terrainWidth = (short)heightmap.Width;
            terrainHeight = (short)heightmap.Height;

            //Get colors (heights)
            heightData = new float[terrainWidth, terrainHeight];
            Color[] heightMapColors = new Color[terrainWidth * terrainHeight];
            heightmap.GetData<Color>(heightMapColors);

            //Create height data
            for (int x = 0; x < terrainWidth; x++)
            {
                for (int y = 0; y < terrainHeight; y++)
                {
                    heightData[x, y] = (heightMapColors[x + y * terrainWidth].R - 128f) / 5.0f;
                }
            }

            //Create body & shape
            Shape terrainShape = new TerrainShape(heightData, 1f, 1f);
            body = new RigidBody(terrainShape);
            body.IsStatic = true;
            body.Position = new JVector(-terrainWidth / 2, 0f, -terrainHeight / 2);
            Owner.Physics.AddBody(body);

            //Create vertices and indices for rendering
            SetUpVertices();
            SetUpIndices();
        }

        public void Draw3D(GameTime gameTime)
        {
            Owner.Helper3D.DrawVertices(vertexBuffer, indexBuffer, Matrix.CreateTranslation(Conversion.ToXNAVector(body.Position)));
        }

        private void SetUpVertices()
        {
            VertexPositionColor[] vertices = new VertexPositionColor[terrainWidth * terrainHeight];
            for (int x = 0; x < terrainWidth; x++)
            {
                for (int y = 0; y < terrainHeight; y++)
                {
                    vertices[x + y * terrainWidth].Position = new Vector3(x, heightData[x, y], y);
                    int color = (int)(heightData[x, y] * 5f) + 128;
                    vertices[x + y * terrainWidth].Color = new Color(color, color, color);
                }
            }

            vertexBuffer = new VertexBuffer(Owner.GraphicsDevice, VertexPositionColor.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionColor>(vertices);
        }

        private void SetUpIndices()
        {
            short[] indices = new short[(terrainWidth - 1) * (terrainHeight - 1) * 6];
            int counter = 0;
            for (short y = 0; y < terrainHeight - 1; y++)
            {
                for (short x = 0; x < terrainWidth - 1; x++)
                {
                    short lowerLeft = (short)(x + y * terrainWidth);
                    short lowerRight = (short)((x + 1) + y * terrainWidth);
                    short topLeft = (short)(x + (y + 1) * terrainWidth);
                    short topRight = (short)((x + 1) + (y + 1) * terrainWidth);

                    indices[counter++] = topLeft;
                    indices[counter++] = lowerRight;
                    indices[counter++] = lowerLeft;

                    indices[counter++] = topLeft;
                    indices[counter++] = topRight;
                    indices[counter++] = lowerRight;
                }
            }

            indexBuffer = new IndexBuffer(Owner.GraphicsDevice, typeof(short), indices.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData(indices);
        }
    }
}
