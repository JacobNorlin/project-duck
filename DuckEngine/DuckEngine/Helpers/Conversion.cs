﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;

namespace DuckEngine.Helpers
{
    /// <summary>
    /// A class holding various converter functions.
    /// </summary>
    public class Conversion
    {
        /// <summary>
        /// Converts a XNA Vector3 to a Jitter Jvector
        /// </summary>
        /// <param name="vector">The Vector3 object to convert</param>
        /// <returns>The converted JVector</returns>
        public static JVector ToJitterVector(Vector3 vector)
        {
            return new JVector(vector.X, vector.Y, vector.Z);
        }

        /// <summary>
        /// Converts a Jitter JVector to a XNA Vector3
        /// </summary>
        /// <param name="vector">The JVector object to convert</param>
        /// <returns>The converted Vector3</returns>
        public static Vector3 ToXNAVector(JVector vector)
        {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }


        /// <summary>
        /// Converts a Jitter JMatrix to a XNA Matrix
        /// </summary>
        /// <param name="matrix">The Jmatrix to convert</param>
        /// <returns>The converted XNA Matrix</returns>
        public static Matrix ToXNAMatrix(JMatrix matrix)
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
        public static JMatrix ToJitterMatrix(Matrix matrix)
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
