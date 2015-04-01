// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ __________.__  .__                                                     │ \\
// │ \______   \  | |__| _____   ____ ___.__.                               │ \\
// │  |    |  _/  | |  |/     \_/ __ <   |  |                               │ \\
// │  |    |   \  |_|  |  Y Y  \  ___/\___  |                               │ \\
// │  |______  /____/__|__|_|  /\___  > ____|                               │ \\
// │         \/              \/     \/\/                                    │ \\
// │                                                                        │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Copyright © 2012 - 2015 ~ Blimey Engine (http://www.blimey.io)         │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Authors:                                                               │ \\
// │ ~ Ash Pook (http://www.ajpook.com)                                     │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Permission is hereby granted, free of charge, to any person obtaining  │ \\
// │ a copy of this software and associated documentation files (the        │ \\
// │ "Software"), to deal in the Software without restriction, including    │ \\
// │ without limitation the rights to use, copy, modify, merge, publish,    │ \\
// │ distribute, sublicense, and/or sellcopies of the Software, and to      │ \\
// │ permit persons to whom the Software is furnished to do so, subject to  │ \\
// │ the following conditions:                                              │ \\
// │                                                                        │ \\
// │ The above copyright notice and this permission notice shall be         │ \\
// │ included in all copies or substantial portions of the Software.        │ \\
// │                                                                        │ \\
// │ THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,        │ \\
// │ EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF     │ \\
// │ MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. │ \\
// │ IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY   │ \\
// │ CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,   │ \\
// │ TORT OR OTHERWISE, ARISING FROM,OUT OF OR IN CONNECTION WITH THE       │ \\
// │ SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.                 │ \\
// └────────────────────────────────────────────────────────────────────────┘ \\

namespace Blimey
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Abacus.SinglePrecision;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public abstract class BezierPrimitive
        : GeometricPrimitive
    {
        /// <summary>
        /// Creates indices for a patch that is tessellated at the specified level.
        /// </summary>
        protected void CreatePatchIndices(int tessellation, Boolean isMirrored)
        {
            int stride = tessellation + 1;

            for (int i = 0; i < tessellation; i++)
            {
                for (int j = 0; j < tessellation; j++)
                {
                    // Make a list of six index values (two triangles).
                    int[] indices =
                    {
                        i * stride + j,
                        (i + 1) * stride + j,
                        (i + 1) * stride + j + 1,

                        i * stride + j,
                        (i + 1) * stride + j + 1,
                        i * stride + j + 1,
                    };

                    // If this patch is mirrored, reverse the
                    // indices to keep the correct winding order.
                    if (isMirrored)
                    {
                        Array.Reverse(indices);
                    }

                    // Create the indices.
                    foreach (int index in indices)
                    {
                        AddIndex(CurrentVertex + index);
                    }
                }
            }
        }

        /// <summary>
        /// Creates vertices for a patch that is tessellated at the specified level.
        /// </summary>
        protected void CreatePatchVertices(Vector3[] patch, int tessellation, Boolean isMirrored)
        {
            Debug.Assert(patch.Length == 16);

            for (int i = 0; i <= tessellation; i++)
            {
                float ti = (float)i / tessellation;

                for (int j = 0; j <= tessellation; j++)
                {
                    float tj = (float)j / tessellation;

                    // Perform four horizontal bezier interpolations
                    // between the control points of this patch.
                    Vector3 p1 = Bezier(patch[0], patch[1], patch[2], patch[3], ti);
                    Vector3 p2 = Bezier(patch[4], patch[5], patch[6], patch[7], ti);
                    Vector3 p3 = Bezier(patch[8], patch[9], patch[10], patch[11], ti);
                    Vector3 p4 = Bezier(patch[12], patch[13], patch[14], patch[15], ti);

                    // Perform a vertical interpolation between the results of the
                    // previous horizontal interpolations, to compute the position.
                    Vector3 position = Bezier(p1, p2, p3, p4, tj);

                    // Perform another four bezier interpolations between the control
                    // points, but this time vertically rather than horizontally.
                    Vector3 q1 = Bezier(patch[0], patch[4], patch[8], patch[12], tj);
                    Vector3 q2 = Bezier(patch[1], patch[5], patch[9], patch[13], tj);
                    Vector3 q3 = Bezier(patch[2], patch[6], patch[10], patch[14], tj);
                    Vector3 q4 = Bezier(patch[3], patch[7], patch[11], patch[15], tj);

                    // Compute vertical and horizontal tangent vectors.
                    Vector3 tangentA = BezierTangent(p1, p2, p3, p4, tj);
                    Vector3 tangentB = BezierTangent(q1, q2, q3, q4, ti);

                    // Cross the two tangent vectors to compute the normal.
                    Vector3 normal;
                    Vector3.Cross(ref tangentA, ref tangentB, out normal);

                    if (normal.Length() > 0.0001f)
                    {
                        Vector3.Normalise(ref normal, out normal);

                        // If this patch is mirrored, we must invert the normal.
                        if (isMirrored)
                            normal = -normal;
                    }
                    else
                    {
                        // In a tidy and well constructed bezier patch, the preceding
                        // normal computation will always work. But the classic teapot
                        // model is not tidy or well constructed! At the top and bottom
                        // of the teapot, it contains degenerate geometry where a patch
                        // has several control points in the same place, which causes
                        // the tangent computation to fail and produce a zero normal.
                        // We 'fix' these cases by just hard-coding a normal that points
                        // either straight up or straight down, depending on whether we
                        // are on the top or bottom of the teapot. This is not a robust
                        // solution for all possible degenerate bezier patches, but hey,
                        // it's good enough to make the teapot work correctly!

                        if (position.Y > 0)
                            normal = Vector3.Up;
                        else
                            normal = Vector3.Down;
                    }

                    // Create the vertex.
                    AddVertex(position, normal);
                }
            }
        }


        /// <summary>
        /// Performs a cubic bezier interpolation between four scalar control
        /// points, returning the value at the specified time (t ranges 0 to 1).
        /// </summary>
        static float Bezier(float p1, float p2, float p3, float p4, float t)
        {
            return p1 * (1 - t) * (1 - t) * (1 - t) +
                   p2 * 3 * t * (1 - t) * (1 - t) +
                   p3 * 3 * t * t * (1 - t) +
                   p4 * t * t * t;
        }


        /// <summary>
        /// Performs a cubic bezier interpolation between four Vector3 control
        /// points, returning the value at the specified time (t ranges 0 to 1).
        /// </summary>
        static Vector3 Bezier(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float t)
        {
            Vector3 result = new Vector3();

            result.X = Bezier(p1.X, p2.X, p3.X, p4.X, t);
            result.Y = Bezier(p1.Y, p2.Y, p3.Y, p4.Y, t);
            result.Z = Bezier(p1.Z, p2.Z, p3.Z, p4.Z, t);

            return result;
        }


        /// <summary>
        /// Computes the tangent of a cubic bezier curve at the specified time,
        /// when given four scalar control points.
        /// </summary>
        static float BezierTangent(float p1, float p2, float p3, float p4, float t)
        {
            return p1 * (-1 + 2 * t - t * t) +
                   p2 * (1 - 4 * t + 3 * t * t) +
                   p3 * (2 * t - 3 * t * t) +
                   p4 * (t * t);
        }


        /// <summary>
        /// Computes the tangent of a cubic bezier curve at the specified time,
        /// when given four Vector3 control points. This is used for calculating
        /// normals (by crossing the horizontal and vertical tangent vectors).
        /// </summary>
        static Vector3 BezierTangent(Vector3 p1, Vector3 p2,
                                     Vector3 p3, Vector3 p4, float t)
        {
            Vector3 result = new Vector3();

            result.X = BezierTangent(p1.X, p2.X, p3.X, p4.X, t);
            result.Y = BezierTangent(p1.Y, p2.Y, p3.Y, p4.Y, t);
            result.Z = BezierTangent(p1.Z, p2.Z, p3.Z, p4.Z, t);

            try
            {
                Vector3.Normalise(ref result, out result);
            }
            catch
            {
                result = Vector3.Zero;
            }

            return result;
        }
    }
}
