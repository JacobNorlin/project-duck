using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jitter.Dynamics;
using Jitter.Collision.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DuckEngine.Helpers
{
    public class Helper3D
    {
        private Engine engine;
        Model boxModel;
        Effect effect;
        BasicEffect basicEffect;

        public Helper3D(Engine _engine)
        {
            engine = _engine;
        }

        public void LoadContent()
        {
            boxModel = engine.Content.Load<Model>("PrimitiveModels/box");
            effect = engine.Content.Load<Effect>("Effects/effects");
            basicEffect = new BasicEffect(engine.GraphicsDevice);
            basicEffect.VertexColorEnabled = true;
        }

        public void DrawBoxBody(RigidBody body, Color color)
        {
            // We know that the shape is a boxShape
            BoxShape shape = body.Shape as BoxShape;

            // Create the 4x4 xna matrix, containing the orientation
            // (represented in jitter by a 3x3 matrix) and the position.
            Matrix matrix = Conversion.ToXNAMatrix(body.Orientation);
            matrix.Translation = Conversion.ToXNAVector(body.Position);

            // We have a (1,1,1) box so packing the box size
            // information into the the "scale" part of the xna
            // matrix is a good idea.
            Matrix scaleMatrix = Matrix.CreateScale(shape.Size.X,
                shape.Size.Y, shape.Size.Z);

            // the next lines of code draw the boxModel using the matrix.
            ModelMesh mesh = boxModel.Meshes[0];

            foreach (BasicEffect effect in mesh.Effects)
            {
                effect.DiffuseColor = color.ToVector3();
                effect.EnableDefaultLighting();
                effect.World = scaleMatrix * matrix;

                effect.View = engine.Camera.View;
                effect.Projection = engine.Camera.Projection;
            }
            mesh.Draw();
        }

        public void DrawVertices(VertexBuffer vertices, IndexBuffer indices, Matrix worldMatrix)
        {
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.CullClockwiseFace;
            //rs.FillMode = FillMode.WireFrame;
            engine.GraphicsDevice.RasterizerState = rs;

            basicEffect.World = worldMatrix;
            basicEffect.View = engine.Camera.View;
            basicEffect.Projection = engine.Camera.Projection;

            /*effect.CurrentTechnique = effect.Techniques["ColoredNoShading"];
            effect.Parameters["xView"].SetValue(engine.Camera.View);
            effect.Parameters["xProjection"].SetValue(engine.Camera.Projection);
            effect.Parameters["xWorld"].SetValue(worldMatrix);*/

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                engine.GraphicsDevice.Indices = indices;
                engine.GraphicsDevice.SetVertexBuffer(vertices);
                engine.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertices.VertexCount, 0, indices.IndexCount / 3);
            }
        }
    }
}
