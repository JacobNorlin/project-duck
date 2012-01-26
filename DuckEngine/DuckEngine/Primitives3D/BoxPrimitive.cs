#region File Description
//-----------------------------------------------------------------------------
// BoxPrimitive.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace DuckEngine.Primitives3D
{
    /// <summary>
    /// Geometric primitive class for drawing cubes.
    /// </summary>
    public class BoxPrimitive : GeometricPrimitive
    {
        /// <summary>
        /// Constructs a new cube primitive, using default settings.
        /// </summary>
        public BoxPrimitive(GraphicsDevice graphicsDevice)
            : this(graphicsDevice, 1)
        {
        }


        /// <summary>
        /// Constructs a new cube primitive, with the specified size.
        /// </summary>
        public BoxPrimitive(GraphicsDevice graphicsDevice, float size)
        {
            Vector3[] normals =
            {
                new Vector3( 0, 1, 0),
                new Vector3(-1, 0, 0),
                new Vector3( 0, 0,-1),
                new Vector3( 1, 0, 0),
                new Vector3( 0,-1, 0),
                new Vector3( 0, 0, 1),
            };

            // Create each face in turn.
            for (int i=0; i<normals.Length; i++)
            {
                Vector3 normal = normals[i];
                // Get two vectors perpendicular to the face normal and to each other.
                Vector3 side1 = new Vector3(normal.Y, normal.Z, normal.X);
                Vector3 side2 = Vector3.Cross(normal, side1);

                // Six indices (two triangles) per face.
                if (i == 2 || i == 4)
                {
                    AddIndex(CurrentVertex + 1);
                    AddIndex(CurrentVertex + 2);
                    AddIndex(CurrentVertex + 3);

                    AddIndex(CurrentVertex + 3);
                    AddIndex(CurrentVertex + 0);
                    AddIndex(CurrentVertex + 1);
                }
                else
                {
                    AddIndex(CurrentVertex + 0);
                    AddIndex(CurrentVertex + 1);
                    AddIndex(CurrentVertex + 2);

                    AddIndex(CurrentVertex + 2);
                    AddIndex(CurrentVertex + 3);
                    AddIndex(CurrentVertex + 0);
                }

                // Four vertices per face.
                AddVertex((normal - side1 - side2) * size / 2, normal);
                AddVertex((normal - side1 + side2) * size / 2, normal);
                AddVertex((normal + side1 + side2) * size / 2, normal);
                AddVertex((normal + side1 - side2) * size / 2, normal);
            }

            InitializePrimitive(graphicsDevice);
        }

        /// <summary>
        /// Draws a wire frame of primitive model, using the specified effect.
        /// </summary>
        new public void DrawWireFrame(BasicEffect effect)
        {
            GraphicsDevice graphicsDevice = effect.GraphicsDevice;

            graphicsDevice.SetVertexBuffer(vertexBuffer);
            graphicsDevice.Indices = indexBuffer;
            
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineStrip,
                    0, 0, vertices.Count, 0, indices.Count + 1);
            }
        }
    }
}
