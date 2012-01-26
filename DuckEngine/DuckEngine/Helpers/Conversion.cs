using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using Jitter.Dynamics;

namespace DuckEngine.Helpers
{
    /// <summary>
    /// A class holding various converter functions.
    /// </summary>
    public static class Conversion
    {
        /// <summary>
        /// Get the world matrix of the body, based on the
        /// orientation matrix and position vector
        /// </summary>
        /// <param name="body"></param>
        /// <returns>World matrix</returns>
        public static Matrix GetWorldMatrix(this RigidBody body)
        {
            //Maybe this should be moved to a new class, such as
            //"GeneralExtensions" or something... :)
            Matrix worldMatrix = body.Orientation.ToXNAMatrix();
            worldMatrix.Translation = body.Position.ToXNAVector();
            return worldMatrix;
        }

        public static Matrix GetBoundingBoxWorldMatrix(this RigidBody body)
        {
            Matrix worldMatrix = Matrix.CreateScale((body.Shape.BoundingBox.Max - body.Shape.BoundingBox.Min).ToXNAVector());
            worldMatrix *= GetWorldMatrix(body);
            return worldMatrix;
        }

        /// <summary>
        /// Converts a XNA Vector3 to a Jitter Jvector
        /// </summary>
        /// <param name="vector">The Vector3 object to convert</param>
        /// <returns>The converted JVector</returns>
        public static JVector ToJitterVector(this Vector3 vector)
        {
            return new JVector(vector.X, vector.Y, vector.Z);
        }

        /// <summary>
        /// Converts a Jitter JVector to a XNA Vector3
        /// </summary>
        /// <param name="vector">The JVector object to convert</param>
        /// <returns>The converted Vector3</returns>
        public static Vector3 ToXNAVector(this JVector vector)
        {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }


        /// <summary>
        /// Converts a Jitter JMatrix to a XNA Matrix
        /// </summary>
        /// <param name="matrix">The Jmatrix to convert</param>
        /// <returns>The converted XNA Matrix</returns>
        public static Matrix ToXNAMatrix(this JMatrix matrix)
        {
            return new Matrix(matrix.M11,
                            matrix.M12,
                            matrix.M13,
                            0.0f,
                            matrix.M21,
                            matrix.M22,
                            matrix.M23,
                            0.0f,
                            matrix.M31,
                            matrix.M32,
                            matrix.M33,
                            0.0f, 0.0f, 0.0f, 0.0f, 1.0f);
        }

        /// <summary>
        /// Converts a XNA Matrix to a Jitter JMatrix
        /// </summary>
        /// <param name="matrix">The XNA Matrix to convert</param>
        /// <returns>The converted Jmatrix</returns>
        public static JMatrix ToJitterMatrix(this Matrix matrix)
        {
            JMatrix result;
            result.M11 = matrix.M11;
            result.M12 = matrix.M12;
            result.M13 = matrix.M13;
            result.M21 = matrix.M21;
            result.M22 = matrix.M22;
            result.M23 = matrix.M23;
            result.M31 = matrix.M31;
            result.M32 = matrix.M32;
            result.M33 = matrix.M33;
            return result;
        }

    }
}
