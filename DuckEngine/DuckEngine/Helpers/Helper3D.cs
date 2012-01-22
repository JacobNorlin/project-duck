using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DuckEngine.Primitives3D;
using System;

namespace DuckEngine.Helpers
{
    public class GeometricPrimitives
    {
        public readonly BoxPrimitive Box;
        public readonly CapsulePrimitive Capsule;
        public readonly ConePrimitive Cone;
        public readonly CylinderPrimitive Cylinder;
        public readonly SpherePrimitive Sphere;

        public GeometricPrimitives(GraphicsDevice graphicsDevice)
        {
            Box = new BoxPrimitive(graphicsDevice);
            Capsule = new CapsulePrimitive(graphicsDevice);
            Cone = new ConePrimitive(graphicsDevice);
            Cylinder = new CylinderPrimitive(graphicsDevice);
            Sphere = new SpherePrimitive(graphicsDevice);
        }

    }
    public class Helper3D
    {
        public static readonly RasterizerState WireFrame;
        public static readonly RasterizerState CullClockwiseFace;
        public static readonly RasterizerState CullCounterClockwiseFace;
        public GeometricPrimitives Primitives;

        private Engine engine;
        public BasicEffect BasicEffect;

        /// <summary>
        /// static constructor for initializing static variables
        /// </summary>
        static Helper3D()
        {
            WireFrame = new RasterizerState();
            CullClockwiseFace = new RasterizerState();
            CullCounterClockwiseFace = new RasterizerState();

            WireFrame.FillMode = FillMode.WireFrame;
            WireFrame.CullMode = CullMode.None;
            CullClockwiseFace.CullMode = CullMode.CullClockwiseFace;
            CullCounterClockwiseFace.CullMode = CullMode.CullCounterClockwiseFace;
        }

        public Helper3D(Engine _engine)
        {
            engine = _engine;
        }

        public void LoadContent()
        {
            BasicEffect = new BasicEffect(engine.GraphicsDevice);
            BasicEffect.EnableDefaultLighting();
            BasicEffect.PreferPerPixelLighting = true;
            Primitives = new GeometricPrimitives(engine.GraphicsDevice);
        }

        /// <summary>
        /// Draw wireframe/solid of any of the RigidBody.Shape primitives, except
        /// TerrainShape... for now.
        /// </summary>
        /// <param name="body">body to draw</param>
        /// <param name="color">Color to bias wireframe/solid towards</param>
        /// <param name="drawSolid">True: draw solid, False: draw wireframe</param>
        /// <param name="enableLightning">enable the default lightning</param>
        public void DrawBody(RigidBody body, Color color, bool drawSolid, bool enableLightning)
        {
            GeometricPrimitive primitive = null;
            Matrix scaleMatrix;

            #region decide scale and shape
            Shape shape = body.Shape;
            if (shape is BoxShape)
            {
                primitive = Primitives.Box;
                scaleMatrix = Matrix.CreateScale(Conversion.ToXNAVector((shape as BoxShape).Size));
            }
            else if (shape is SphereShape)
            {
                primitive = Primitives.Sphere;
                scaleMatrix = Matrix.CreateScale((shape as SphereShape).Radius);
            }
            else if (shape is CylinderShape)
            {
                primitive = Primitives.Cylinder;
                CylinderShape cs = shape as CylinderShape;
                scaleMatrix = Matrix.CreateScale(cs.Radius, cs.Height, cs.Radius);
            }
            else if (shape is CapsuleShape)
            {
                CapsuleShape cs = shape as CapsuleShape;
                primitive = CapsulePrimitive.GetCapsulePrimitive(
                    BasicEffect.GraphicsDevice, cs.Radius * 2, cs.Length, 6);
                scaleMatrix = Matrix.Identity;

            }
            else if (shape is ConeShape)
            {
                primitive = Primitives.Cone;
                ConeShape cs = shape as ConeShape;
                scaleMatrix = Matrix.CreateScale(cs.Radius, cs.Height, cs.Radius);
            }
            else
            {
                throw new ArgumentException("Unable to draw given body (is it a TerrainShape?");
            }
            #endregion
            
            BasicEffect.World = scaleMatrix * body.GetWorldMatrix();
            BasicEffect.DiffuseColor = color.ToVector3();

            BasicEffect.LightingEnabled = enableLightning;
            if (drawSolid)
            {
                primitive.DrawSolid(BasicEffect);
            }
            else
            {
                primitive.DrawWireFrame(BasicEffect);
            }
        }

        public void DrawVertices(VertexBuffer vertexBuffer, IndexBuffer indexBuffer, Matrix worldMatrix)
        {
            engine.GraphicsDevice.RasterizerState = CullClockwiseFace;

            BasicEffect.World = worldMatrix;

            foreach (EffectPass pass in BasicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                engine.GraphicsDevice.Indices = indexBuffer;
                engine.GraphicsDevice.SetVertexBuffer(vertexBuffer);
                engine.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexBuffer.VertexCount, 0, indexBuffer.IndexCount / 3);
            }
        }

        public void DrawModel(Model model, RigidBody body)
        {
            DrawModel(model, body, Matrix.Identity);
        }
        public void DrawModel(Model model, RigidBody body, Matrix scale)
        {
            engine.GraphicsDevice.RasterizerState = CullCounterClockwiseFace;
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    //effect.DiffuseColor = color.ToVector3();
                    effect.EnableDefaultLighting();
                    effect.World = scale*body.GetWorldMatrix();

                    effect.View = engine.Camera.View;
                    effect.Projection = engine.Camera.Projection;
                }
                mesh.Draw();
            }
        }
    }
}
