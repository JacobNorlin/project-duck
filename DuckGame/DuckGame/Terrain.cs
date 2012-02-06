using DuckEngine.Helpers;
using DuckEngine.Interfaces;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using DuckEngine;
using System.Xml;

namespace DuckGame
{
    class Terrain : PhysicalEntity, IDraw3D, ISave
    {
        private static string terrainFolder = "Terrain/";
        short terrainWidth;
        short terrainHeight;
        float[,] heightData;

        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;
        private string terrainFile;

        public Terrain(Engine _engine, Tracker _tracker, string _terrainFile)
            : base(_engine, _tracker, null, false)
        {
            terrainFile = _terrainFile;
            //Load heightmap
            Texture2D heightmap = Engine.Content.Load<Texture2D>(terrainFolder + terrainFile);
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
            Body = new RigidBody(terrainShape);
            Body.IsStatic = true;
            Body.Position = new JVector(-terrainWidth / 2, 0, -terrainHeight / 2);

            //Create vertices and indices for rendering
            SetUpVerticesWithNormals();
            SetUpIndices();
            EnableInterfaceCalls = true;
        }

        public void Draw3D(GameTime gameTime)
        {
            //Body.IsStatic = false;
            //Body.IsActive = false;
            //Body.AffectedByGravity = false;
            //Body.AngularVelocity = new JVector(0,0.2f,0);
            Engine.Helper3D.BasicEffect.LightingEnabled = true;
            Engine.Helper3D.BasicEffect.DiffuseColor = Color.Khaki.ToVector3();
            Engine.Helper3D.DrawVertices(vertexBuffer, indexBuffer, Body.GetWorldMatrix());
            //foreach (VertexPositionNormalTexture n in vertices)
            //{
            //    Vector3 p = n.Position + Body.Position.ToXNAVector();
            //    Engine.DebugDrawer.DrawLine(p, p + n.Normal, Color.Black, Color.White);
            //}
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

            vertexBuffer = new VertexBuffer(Engine.GraphicsDevice, VertexPositionColor.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionColor>(vertices);
        }

        private void SetUpVerticesWithNormals()
        {
            VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[terrainWidth * terrainHeight];
            for (int x = 0; x < terrainWidth; x++)
            {
                for (int y = 0; y < terrainHeight; y++)
                {
                    vertices[x + y * terrainWidth].Position = new Vector3(x, heightData[x, y], y);
                }
            }
            for (int x = 0; x < terrainWidth; x++)
            {
                for (int y = 0; y < terrainHeight; y++)
                {
                    Vector3[] v = {
                        getPoint(x, y,  0,  1, vertices),
                        getPoint(x, y,  1,  1, vertices),
                        getPoint(x, y,  1,  0, vertices),
                        getPoint(x, y,  1, -1, vertices),
                        getPoint(x, y,  0, -1, vertices),
                        getPoint(x, y, -1, -1, vertices),
                        getPoint(x, y, -1,  0, vertices),
                        getPoint(x, y, -1,  1, vertices),
                    };

                    Vector3 normal = Vector3.Zero;
                    for (int i = 0; i < 8; i++)
                    {
                        Vector3 n = Vector3.Cross(v[i], v[(i + 1) % 8]);
                        n.Normalize();
                        if (i%2 == 0)
                        {
                            n *= (float)Math.Sqrt(2);
                        }
                        normal += n;
                    }
                    normal.Normalize();
                    vertices[x + y * terrainWidth].Normal = normal;
                }
            }

            vertexBuffer = new VertexBuffer(Engine.GraphicsDevice, VertexPositionNormalTexture.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionNormalTexture>(vertices);
        }

        private Vector3 getPoint(int x, int y, int xo, int yo, VertexPositionNormalTexture[] vertices)
        {
            float negate = 1;
            if (x == 0 && xo < 0)
            {
                xo = -xo;
                negate = -1;
            }
            else if (x == terrainWidth - 1 && xo > 0)
            {
                xo = -xo;
                negate = -1;
            }
            if (y == 0 && yo < 0)
            {
                yo = -yo;
                negate = -1;
            }
            else if (y == terrainHeight - 1 && yo > 0)
            {
                yo = -yo;
                negate = -1;
            }
            return negate * (vertices[x + xo + (y + yo) * terrainWidth].Position - vertices[x + y*terrainWidth].Position);
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

            indexBuffer = new IndexBuffer(Engine.GraphicsDevice, typeof(short), indices.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData(indices);
        }

        ~Terrain()
        {
            Dispose();
        }

        new public void Dispose()
        {
            vertexBuffer.Dispose();
            indexBuffer.Dispose();
            base.Dispose();
        }

        public override PhysicalEntity Clone(bool _)
        {
            return null;
        }

        public void Save(XmlDocument doc, XmlElement currNode)
        {
            currNode.SetAttribute("file", terrainFile);
        }

        public static Terrain Load(Engine _engine, Tracker _tracker, XmlNode node)
        {
            string terrainFile = node.Attributes.GetNamedItem("file").InnerText;
            return new Terrain(_engine, _tracker, terrainFile);
        }

        //public void Input(GameTime gameTime, DuckEngine.Input.InputManager input)
        //{
        //    if (input.Keyboard_WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.U))
        //    {
        //        a = (a + 1) % 3;
        //        SetUpVerticesWithNormals();
        //        Console.WriteLine("a=" + a);
        //    }
        //}
    }
}
