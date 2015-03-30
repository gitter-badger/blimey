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
    using System.Runtime.InteropServices;
    using System.Globalization;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;

    using Fudge;
    using Abacus.SinglePrecision;

    using System.Linq;


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public enum Space
    {
        World,
        Self
    }

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //


    public enum ContainmentType
    {
        Disjoint,
        Contains,
        Intersects
    }

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //


    public enum PlaneIntersectionType
    {
        Front,
        Back,
        Intersecting
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public static class BlimeyMathsHelper
    {
        public static Single Distance(Single value1, Single value2)
        {
            return Math.Abs((Single)(value1 - value2));
        }

        public static T Clamp<T>(T value, T min, T max)
            where T : System.IComparable<T>
        {
            T result = value;

            if (value.CompareTo(max) > 0)
                result = max;

            if (value.CompareTo(min) < 0)
                result = min;

            return result;
        }

        public static Single Limit(ref Single zItem, Single zLower, Single zUpper)
        {
            if (zItem < zLower)
            {
                zItem = zLower;
            }

            else if (zItem > zUpper)
            {
                zItem = zUpper;
            }

            return zItem;
        }

        public static Single Wrap(ref Single zItem, Single zLower, Single zUpper)
        {
            while (zItem < zLower)
            {
                zItem += (zUpper - zLower);
            }

            while (zItem >= zUpper)
            {
                zItem -= (zUpper - zLower);
            }

            return zItem;
        }

        public static Quaternion EulerToQuaternion(Vector3 e)
        {

            Single x = Maths.ToRadians(e.X);
            Single y = Maths.ToRadians(e.Y);
            Single z = Maths.ToRadians(e.Z);

            Quaternion result;
            Quaternion.CreateFromYawPitchRoll(ref x, ref y, ref z, out result);
            return result;
        }

        public static Vector3 QuaternionToEuler(Quaternion rotation)
        {
            // This bad boy works, taken from:

            Single q2 = rotation.I;
            Single q1 = rotation.J;
            Single q3 = rotation.K;
            Single q0 = rotation.U;

            Vector3 angles = Vector3.Zero;

            // METHOD 1: http://forums.create.msdn.com/forums/p/28687/159870.aspx

            angles.X = (Single)Math.Atan2(2 * (q0 * q1 + q2 * q3), 1 - 2 * (Math.Pow(q1, 2) + Math.Pow(q2, 2)));
            angles.Y = (Single)Math.Asin(2 * (q0 * q2 - q3 * q1));
            angles.Z = (Single)Math.Atan2(2 * (q0 * q3 + q1 * q2), 1 - 2 * (Math.Pow(q2, 2) + Math.Pow(q3, 2)));


            // METHOD 2: http://forums.create.msdn.com/forums/p/4574/23763.aspx
            //angles.X = (Single)Math.Atan2(2 * q1 * q0 - 2 * q2 * q3, 1 - 2 * Math.Pow(q1, 2) - 2 * Math.Pow(q3, 2));
            //angles.Z = (Single)Math.Asin(2 * q2 * q1 + 2 * q3 * q0);
            //angles.Y = (Single)Math.Atan2(2 * q2 * q0 - 2 * q1 * q3, 1 - 2 * Math.Pow(q2, 2) - 2 * Math.Pow(q3, 2));
            //if (q2 * q1 + q3 * q0 == 0.5)
            //{
            //    angles.X = (Single)(2 * Math.Atan2(q2, q0));
            //    angles.Y = 0;
            //}
            //else if (q2 * q1 + q3 * q0 == -0.5)
            //{
            //    angles.X = (Single)(-2 * Math.Atan2(q2, q0));
            //    angles.Y = 0;
            //}

            // METHOD 3: http://forums.create.msdn.com/forums/p/4574/23763.aspx
            //const Single Epsilon = 0.0009765625f;
            //const Single Threshold = 0.5f - Epsilon;
            //Single XY = q2 * q1;
            //Single ZW = q3 * q0;
            //Single TEST = XY + ZW;
            //if (TEST < -Threshold || TEST > Threshold)
            //{
            //    int sign = Math.Sign(TEST);
            //    angles.X = sign * 2 * (Single)Math.Atan2(q2, q0);
            //    angles.Y = sign * MathHelper.PiOver2;
            //    angles.Z = 0;
            //}
            //else
            //{
            //    Single XX = q2 * q2;
            //    Single XZ = q2 * q3;
            //    Single XW = q2 * q0;
            //    Single YY = q1 * q1;
            //    Single YW = q1 * q0;
            //    Single YZ = q1 * q3;
            //    Single ZZ = q3 * q3;
            //    angles.X = (Single)Math.Atan2(2 * YW - 2 * XZ, 1 - 2 * YY - 2 * ZZ);
            //    angles.Y = (Single)Math.Atan2(2 * XW - 2 * YZ, 1 - 2 * XX - 2 * ZZ);
            //    angles.Z = (Single)Math.Asin(2 * TEST);
            //}


            angles.X = Maths.ToDegrees(angles.X);
            angles.Y = Maths.ToDegrees(angles.Y);
            angles.Z = Maths.ToDegrees(angles.Z);


            return angles;
        }

        public static Vector3 QuaternionToYawPitchRoll(Quaternion q)
        {
            const Single Epsilon = 0.0009765625f;
            const Single Threshold = 0.5f - Epsilon;

            Single yaw;
            Single pitch;
            Single roll;

            Single XY = q.I * q.J;
            Single ZW = q.K * q.U;

            Single TEST = XY + ZW;

            if (TEST < -Threshold || TEST > Threshold)
            {

                int sign = Math.Sign(TEST);

                yaw = sign * 2 * (Single)Math.Atan2(q.I, q.U);

                Single piOver2;
                Maths.Pi(out piOver2);
                piOver2 /= 2;

                pitch = sign * piOver2;

                roll = 0;

            }
            else
            {

                Single XX = q.I * q.I;
                Single XZ = q.I * q.K;
                Single XW = q.I * q.U;

                Single YY = q.J * q.J;
                Single YW = q.J * q.U;
                Single YZ = q.J * q.K;

                Single ZZ = q.K * q.K;

                yaw = (Single)Math.Atan2(2 * YW - 2 * XZ, 1 - 2 * YY - 2 * ZZ);

                pitch = (Single)Math.Atan2(2 * XW - 2 * YZ, 1 - 2 * XX - 2 * ZZ);

                roll = (Single)Math.Asin(2 * TEST);

            }

            return new Vector3(yaw, pitch, roll);

        }

        public static Boolean CheckThatAllComponentsAreValidNumbers(Vector3 zVec)
        {
            if (Single.IsNaN(zVec.X) || Single.IsNaN(zVec.Y) || Single.IsNaN(zVec.Z))
            {
                return false;
            }
            return true;
        }

        /// Return angle between two vectors. Used for visbility testing and
        /// for checking angles between vectors for the road sign generation.
        public static Single GetAngleBetweenVectors(Vector3 vec1, Vector3 vec2)
        {
            // See http://en.wikipedia.org/wiki/Vector_(spatial)
            // for help and check out the Dot Product section ^^
            // Both vectors are normalized so we can save deviding through the
            // lengths.

            Boolean isVec1Ok = CheckThatAllComponentsAreValidNumbers(vec1);
            Boolean isVec2Ok = CheckThatAllComponentsAreValidNumbers(vec2);
            System.Diagnostics.Debug.Assert(isVec1Ok && isVec2Ok);

            Vector3.Normalise(ref vec1, out vec1);
            Vector3.Normalise(ref vec2, out vec2);
            Single dot;
            Vector3.Dot(ref vec1, ref vec2, out dot);
            dot = Clamp(dot, -1.0f, 1.0f);
            Single result = (Single)System.Math.Acos(dot);
            System.Diagnostics.Debug.Assert(!Single.IsNaN(result));
            return result;
        }

        public static Single GetSignedAngleBetweenVectors(Vector3 vec1, Vector3 vec2)
        {
            // See http://en.wikipedia.org/wiki/Vector_(spatial)
            // for help and check out the Dot Product section ^^
            // Both vectors are normalized so we can save deviding through the
            // lengths.

            Single dot;
            Vector3.Dot(ref vec1, ref vec2, out dot);
            Single angle = Maths.ArcCos(dot);

            //check to see if the car->camera vector is to the left or right of the
            //inverse car.look vector using the cross product
            //to do this we can just check the sign of the y as we set the y
            //of the two input vector to zero
            Vector3 cross;
            Vector3.Cross(ref vec1, ref vec2, out cross);
            Single sign = 1.0f;

            if (cross.Y < 0.0f)
            {
                sign = -1.0f;
            }

            //check to see if the angle between the car->camera vector and the inverse car.look
            //vector is greater than our limiting angle
            angle *= sign;

            Single pi;
            Maths.Pi(out pi);

            Single tau;
            Maths.Tau(out tau);

            while (angle < -pi)
                angle += tau;
            while (angle >= pi)
                angle -= tau;

            return angle;
        }

        /// Distance from our point to the line described by linePos1 and linePos2.
        public static Single DistanceToLine(Vector3 point, Vector3 linePos1, Vector3 linePos2)
        {
            // For help check out this article:
            // http://mathworld.wolfram.com/Point-LineDistance3-Dimensional.html
            Vector3 lineVec = linePos2 - linePos1;
            Vector3 pointVec = linePos1 - point;

            Vector3 cross;
            Vector3.Cross(ref lineVec, ref pointVec, out cross);

            return cross.Length() / lineVec.Length();
        }

        /// Signed distance to plane
        public static Single SignedDistanceToPlane(Vector3 point, Vector3 planePosition, Vector3 planeNormal)
        {
            Vector3 pointVec = planePosition - point;

            Single dot;

            Vector3.Dot(ref planeNormal, ref pointVec, out dot);

            return dot;
        }


        public static string NiceMatrixString(Matrix44 mat)
        {
            return string.Format(
                "| {0:+00000.00;-00000.00;} {1:+00000.00;-00000.00;} {2:+00000.00;-00000.00;} {3:+00000.00;-00000.00;} |\n" +
                "| {4:+00000.00;-00000.00;} {5:+00000.00;-00000.00;} {6:+00000.00;-00000.00;} {7:+00000.00;-00000.00;} |\n" +
                "| {8:+00000.00;-00000.00;} {9:+00000.00;-00000.00;} {10:+00000.00;-00000.00;} {11:+00000.00;-00000.00;} |\n" +
                "| {12:+00000.00;-00000.00;} {13:+00000.00;-00000.00;} {14:+00000.00;-00000.00;} {15:+00000.00;-00000.00;} |\n",
                mat.R0C0, mat.R1C0, mat.R2C0, mat.R3C0,
                mat.R0C1, mat.R1C1, mat.R2C1, mat.R3C1,
                mat.R0C2, mat.R1C2, mat.R2C2, mat.R3C2,
                mat.R0C3, mat.R1C3, mat.R2C3, mat.R3C0) +

            "Translation: " + mat.Translation + "\n";
        }

        public static Single FastInverseSquareRoot(Single val)
        {
            if (Single.IsNaN(val))
                throw new Exception("FastInverseSquareRoot only works on numbers!");

            if (Single.IsInfinity(val))
                return 0f;

            if (val == 0f)
                return val;

            unsafe
            {
                Single halfVal = 0.5f * val;
                Int32 i = *(Int32*)&val;    // evil floating point bit level hacking
                i = 0x5f3759df - (i >> 1);  // what the fuck?
                val = *(Single*)&i;
                val = val * (1.5f - (halfVal * val * val));
                //val = val * (1.5f - (halfVal * val * val));
                return val;
            }
        }

        #region Create

        public static void CreateTranslation (ref Vector3 position, out Matrix44 result)
        {
            result.R0C0 = 1;
            result.R0C1 = 0;
            result.R0C2 = 0;
            result.R0C3 = 0;
            result.R1C0 = 0;
            result.R1C1 = 1;
            result.R1C2 = 0;
            result.R1C3 = 0;
            result.R2C0 = 0;
            result.R2C1 = 0;
            result.R2C2 = 1;
            result.R2C3 = 0;
            result.R3C0 = position.X;
            result.R3C1 = position.Y;
            result.R3C2 = position.Z;
            result.R3C3 = 1;
        }

        public static void CreateTranslation (Single xPosition, Single yPosition, Single zPosition, out Matrix44 result)
        {
            result.R0C0 = 1;
            result.R0C1 = 0;
            result.R0C2 = 0;
            result.R0C3 = 0;
            result.R1C0 = 0;
            result.R1C1 = 1;
            result.R1C2 = 0;
            result.R1C3 = 0;
            result.R2C0 = 0;
            result.R2C1 = 0;
            result.R2C2 = 1;
            result.R2C3 = 0;
            result.R3C0 = xPosition;
            result.R3C1 = yPosition;
            result.R3C2 = zPosition;
            result.R3C3 = 1;
        }

        // Creates a scaling matrix based on x, y, z.
        public static void CreateScale (Single xScale, Single yScale, Single zScale, out Matrix44 result)
        {
            result.R0C0 = xScale;
            result.R0C1 = 0;
            result.R0C2 = 0;
            result.R0C3 = 0;
            result.R1C0 = 0;
            result.R1C1 = yScale;
            result.R1C2 = 0;
            result.R1C3 = 0;
            result.R2C0 = 0;
            result.R2C1 = 0;
            result.R2C2 = zScale;
            result.R2C3 = 0;
            result.R3C0 = 0;
            result.R3C1 = 0;
            result.R3C2 = 0;
            result.R3C3 = 1;
        }

        // Creates a scaling matrix based on a vector.
        public static void CreateScale (ref Vector3 scales, out Matrix44 result)
        {
            result.R0C0 = scales.X;
            result.R0C1 = 0;
            result.R0C2 = 0;
            result.R0C3 = 0;
            result.R1C0 = 0;
            result.R1C1 = scales.Y;
            result.R1C2 = 0;
            result.R1C3 = 0;
            result.R2C0 = 0;
            result.R2C1 = 0;
            result.R2C2 = scales.Z;
            result.R2C3 = 0;
            result.R3C0 = 0;
            result.R3C1 = 0;
            result.R3C2 = 0;
            result.R3C3 = 1;
        }

        // Create a scaling matrix consistant along each axis
        public static void CreateScale (Single scale, out Matrix44 result)
        {
            result.R0C0 = scale;
            result.R0C1 = 0;
            result.R0C2 = 0;
            result.R0C3 = 0;
            result.R1C0 = 0;
            result.R1C1 = scale;
            result.R1C2 = 0;
            result.R1C3 = 0;
            result.R2C0 = 0;
            result.R2C1 = 0;
            result.R2C2 = scale;
            result.R2C3 = 0;
            result.R3C0 = 0;
            result.R3C1 = 0;
            result.R3C2 = 0;
            result.R3C3 = 1;
        }

        public static void CreateRotationX (Single radians, out Matrix44 result)
        {
            // http://en.wikipedia.org/wiki/Rotation_matrix

            Single cos = Maths.Cos (radians);
            Single sin = Maths.Sin (radians);

            result.R0C0 = 1;
            result.R0C1 = 0;
            result.R0C2 = 0;
            result.R0C3 = 0;
            result.R1C0 = 0;
            result.R1C1 = cos;
            result.R1C2 = sin;
            result.R1C3 = 0;
            result.R2C0 = 0;
            result.R2C1 = -sin;
            result.R2C2 = cos;
            result.R2C3 = 0;
            result.R3C0 = 0;
            result.R3C1 = 0;
            result.R3C2 = 0;
            result.R3C3 = 1;
        }

        public static void CreateRotationY (Single radians, out Matrix44 result)
        {
            // http://en.wikipedia.org/wiki/Rotation_matrix

            Single cos = Maths.Cos (radians);
            Single sin = Maths.Sin (radians);

            result.R0C0 = cos;
            result.R0C1 = 0;
            result.R0C2 = -sin;
            result.R0C3 = 0;
            result.R1C0 = 0;
            result.R1C1 = 1;
            result.R1C2 = 0;
            result.R1C3 = 0;
            result.R2C0 = sin;
            result.R2C1 = 0;
            result.R2C2 = cos;
            result.R2C3 = 0;
            result.R3C0 = 0;
            result.R3C1 = 0;
            result.R3C2 = 0;
            result.R3C3 = 1;
        }

        public static void CreateRotationZ (Single radians, out Matrix44 result)
        {
            // http://en.wikipedia.org/wiki/Rotation_matrix

            Single cos = Maths.Cos (radians);
            Single sin = Maths.Sin (radians);

            result.R0C0 = cos;
            result.R0C1 = sin;
            result.R0C2 = 0;
            result.R0C3 = 0;
            result.R1C0 = -sin;
            result.R1C1 = cos;
            result.R1C2 = 0;
            result.R1C3 = 0;
            result.R2C0 = 0;
            result.R2C1 = 0;
            result.R2C2 = 1;
            result.R2C3 = 0;
            result.R3C0 = 0;
            result.R3C1 = 0;
            result.R3C2 = 0;
            result.R3C3 = 1;
        }

        public static void CreateFromAxisAngle (ref Vector3 axis, Single angle, out Matrix44 result)
        {
            Single one = 1;

            Single x = axis.X;
            Single y = axis.Y;
            Single z = axis.Z;

            Single sin = Maths.Sin (angle);
            Single cos = Maths.Cos (angle);

            Single xx = x * x;
            Single yy = y * y;
            Single zz = z * z;

            Single xy = x * y;
            Single xz = x * z;
            Single yz = y * z;

            result.R0C0 = xx + (cos * (one - xx));
            result.R0C1 = (xy - (cos * xy)) + (sin * z);
            result.R0C2 = (xz - (cos * xz)) - (sin * y);
            result.R0C3 = 0;

            result.R1C0 = (xy - (cos * xy)) - (sin * z);
            result.R1C1 = yy + (cos * (one - yy));
            result.R1C2 = (yz - (cos * yz)) + (sin * x);
            result.R1C3 = 0;

            result.R2C0 = (xz - (cos * xz)) + (sin * y);
            result.R2C1 = (yz - (cos * yz)) - (sin * x);
            result.R2C2 = zz + (cos * (one - zz));
            result.R2C3 = 0;

            result.R3C0 = 0;
            result.R3C1 = 0;
            result.R3C2 = 0;
            result.R3C3 = one;
        }

        public static void CreateFromAllAxis (ref Vector3 right, ref Vector3 up, ref Vector3 backward, out Matrix44 result)
        {
            if(!right.IsUnit() || !up.IsUnit() || !backward.IsUnit() )
            {
                throw new ArgumentException("The input vertors must be normalised.");
            }

            result.R0C0 = right.X;
            result.R0C1 = right.Y;
            result.R0C2 = right.Z;
            result.R0C3 = 0;
            result.R1C0 = up.X;
            result.R1C1 = up.Y;
            result.R1C2 = up.Z;
            result.R1C3 = 0;
            result.R2C0 = backward.X;
            result.R2C1 = backward.Y;
            result.R2C2 = backward.Z;
            result.R2C3 = 0;
            result.R3C0 = 0;
            result.R3C1 = 0;
            result.R3C2 = 0;
            result.R3C3 = 1;
        }

        public static void CreateWorldNew (ref Vector3 position, ref Vector3 forward, ref Vector3 up, out Matrix44 result)
        {
            Vector3 backward = -forward;

            Vector3 right;

            Vector3.Cross (ref up, ref backward, out right);

            right.Normalise();

            Matrix44.CreateFromCartesianAxes (ref right, ref up, ref backward, out result);

            result.R3C0 = position.X;
            result.R3C1 = position.Y;
            result.R3C2 = position.Z;
        }

        public static void CreateWorld (ref Vector3 position, ref Vector3 forward, ref Vector3 up, out Matrix44 result)
        {
            if(!forward.IsUnit() || !up.IsUnit() )
            {
                throw new ArgumentException("The input vertors must be normalised.");
            }

            Vector3 backward = -forward;

            Vector3 vector; Vector3.Normalise (ref backward, out vector);

            Vector3 cross; Vector3.Cross (ref up, ref vector, out cross);

            Vector3 vector2; Vector3.Normalise (ref cross, out vector2);

            Vector3 vector3; Vector3.Cross (ref vector, ref vector2, out vector3);

            result.R0C0 = vector2.X;
            result.R0C1 = vector2.Y;
            result.R0C2 = vector2.Z;
            result.R0C3 = 0;
            result.R1C0 = vector3.X;
            result.R1C1 = vector3.Y;
            result.R1C2 = vector3.Z;
            result.R1C3 = 0;
            result.R2C0 = vector.X;
            result.R2C1 = vector.Y;
            result.R2C2 = vector.Z;
            result.R2C3 = 0;
            result.R3C0 = position.X;
            result.R3C1 = position.Y;
            result.R3C2 = position.Z;
            result.R3C3 = 1;
        }

        public static void CreateFromQuaternion (ref Quaternion quaternion, out Matrix44 result)
        {
            if(!quaternion.IsUnit())
            {
                throw new ArgumentException("Input quaternion must be normalised.");
            }

            Single zero = 0;
            Single one = 1;

            Single xs = quaternion.I + quaternion.I;
            Single ys = quaternion.J + quaternion.J;
            Single zs = quaternion.K + quaternion.K;
            Single wx = quaternion.U * xs;
            Single wy = quaternion.U * ys;
            Single wz = quaternion.U * zs;
            Single xx = quaternion.I * xs;
            Single xy = quaternion.I * ys;
            Single xz = quaternion.I * zs;
            Single yy = quaternion.J * ys;
            Single yz = quaternion.J * zs;
            Single zz = quaternion.K * zs;

            result.R0C0 = one - (yy + zz);
            result.R1C0 = xy - wz;
            result.R2C0 = xz + wy;
            result.R3C0 = zero;

            result.R0C1 = xy + wz;
            result.R1C1 = one - (xx + zz);
            result.R2C1 = yz - wx;
            result.R3C1 = zero;

            result.R0C2 = xz - wy;
            result.R1C2 = yz + wx;
            result.R2C2 = one - (xx + yy);
            result.R3C2 = zero;

            result.R0C3 = zero;
            result.R1C3 = zero;
            result.R2C3 = zero;
            result.R3C3 = one;
        }



        // todo: remove when we dont need this for the tests
        internal static void CreateFromQuaternionOld (ref Quaternion quaternion, out Matrix44 result)
        {
            Single zero = 0;
            Single one; Maths.One(out one);
            Single two = 2;

            Single num9 = quaternion.I * quaternion.I;
            Single num8 = quaternion.J * quaternion.J;
            Single num7 = quaternion.K * quaternion.K;
            Single num6 = quaternion.I * quaternion.J;
            Single num5 = quaternion.K * quaternion.U;
            Single num4 = quaternion.K * quaternion.I;
            Single num3 = quaternion.J * quaternion.U;
            Single num2 = quaternion.J * quaternion.K;
            Single num = quaternion.I * quaternion.U;
            result.R0C0 = one - (two * (num8 + num7));
            result.R0C1 = two * (num6 + num5);
            result.R0C2 = two * (num4 - num3);
            result.R0C3 = zero;
            result.R1C0 = two * (num6 - num5);
            result.R1C1 = one - (two * (num7 + num9));
            result.R1C2 = two * (num2 + num);
            result.R1C3 = zero;
            result.R2C0 = two * (num4 + num3);
            result.R2C1 = two * (num2 - num);
            result.R2C2 = one - (two * (num8 + num9));
            result.R2C3 = zero;
            result.R3C0 = zero;
            result.R3C1 = zero;
            result.R3C2 = zero;
            result.R3C3 = one;
        }

        public static void CreateFromYawPitchRoll (Single yaw, Single pitch, Single roll, out Matrix44 result)
        {
            Quaternion quaternion;

            Quaternion.CreateFromYawPitchRoll (ref yaw, ref pitch, ref roll, out quaternion);

            CreateFromQuaternion (ref quaternion, out result);
        }










        /////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////
        // TODO: REVIEW FROM HERE ONWARDS
        /////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////


        // FROM XNA
        // --------
        // Creates a cylindrical billboard that rotates around a specified axis.
        // This method computes the facing direction of the billboard from the object position and camera position.
        // When the object and camera positions are too close, the matrix will not be accurate.
        // To avoid this problem, the method uses the optional camera forward vector if the positions are too close.
        public static void CreateBillboard (ref Vector3 ObjectPosition, ref Vector3 cameraPosition, ref Vector3 cameraUpVector, Vector3? cameraForwardVector, out Matrix44 result)
        {
            Single zero = 0;
            Single one; Maths.One(out one);

            Vector3 vector;
            Vector3 vector2;
            Vector3 vector3;
            vector.X = ObjectPosition.X - cameraPosition.X;
            vector.Y = ObjectPosition.Y - cameraPosition.Y;
            vector.Z = ObjectPosition.Z - cameraPosition.Z;
            Single num = vector.LengthSquared ();
            Single limit; Maths.FromString("0.0001", out limit);

            if (num < limit)
			{
                vector = cameraForwardVector.HasValue ? -cameraForwardVector.Value : Vector3.Forward;
            }
			else
			{
				Single t = (Single)(one / (Maths.Sqrt (num)));
				Vector3.Multiply (ref vector, ref t, out vector);
            }

            Vector3.Cross (ref cameraUpVector, ref vector, out vector3);
            vector3.Normalise ();
            Vector3.Cross (ref vector, ref vector3, out vector2);
            result.R0C0 = vector3.X;
            result.R0C1 = vector3.Y;
            result.R0C2 = vector3.Z;
            result.R0C3 = zero;
            result.R1C0 = vector2.X;
            result.R1C1 = vector2.Y;
            result.R1C2 = vector2.Z;
            result.R1C3 = zero;
            result.R2C0 = vector.X;
            result.R2C1 = vector.Y;
            result.R2C2 = vector.Z;
            result.R2C3 = zero;
            result.R3C0 = ObjectPosition.X;
            result.R3C1 = ObjectPosition.Y;
            result.R3C2 = ObjectPosition.Z;
            result.R3C3 = one;
        }

        public static void CreateConstrainedBillboard (ref Vector3 objectPosition, ref Vector3 cameraPosition, ref Vector3 rotateAxis, Vector3? cameraForwardVector, Vector3? objectForwardVector, out Matrix44 result)
        {
            Single zero = 0;
            Single one; Maths.One(out one);

            Single num;
            Vector3 vector;
            Vector3 vector2;
            Vector3 vector3;
            vector2.X = objectPosition.X - cameraPosition.X;
            vector2.Y = objectPosition.Y - cameraPosition.Y;
            vector2.Z = objectPosition.Z - cameraPosition.Z;
            Single num2 = vector2.LengthSquared ();
            Single limit; Maths.FromString("0.0001", out limit);

            if (num2 < limit)
			{
                vector2 = cameraForwardVector.HasValue ? -cameraForwardVector.Value : Vector3.Forward;
            }
			else
			{
				Single t = (Single)(one / (Maths.Sqrt (num2)));
				Vector3.Multiply (ref vector2, ref t, out vector2);
            }
            Vector3 vector4 = rotateAxis;
            Vector3.Dot (ref rotateAxis, ref vector2, out num);

            Single realHorrid; Maths.FromString("0.9982547", out realHorrid);

            if (Maths.Abs (num) > realHorrid) {
                if (objectForwardVector.HasValue) {
                    vector = objectForwardVector.Value;
                    Vector3.Dot (ref rotateAxis, ref vector, out num);
                    if (Maths.Abs (num) > realHorrid) {
                        num = ((rotateAxis.X * Vector3.Forward.X) + (rotateAxis.Y * Vector3.Forward.Y)) + (rotateAxis.Z * Vector3.Forward.Z);
                        vector = (Maths.Abs (num) > realHorrid) ? Vector3.Right : Vector3.Forward;
                    }
                } else {
                    num = ((rotateAxis.X * Vector3.Forward.X) + (rotateAxis.Y * Vector3.Forward.Y)) + (rotateAxis.Z * Vector3.Forward.Z);
                    vector = (Maths.Abs (num) > realHorrid) ? Vector3.Right : Vector3.Forward;
                }
                Vector3.Cross (ref rotateAxis, ref vector, out vector3);
                vector3.Normalise ();
                Vector3.Cross (ref vector3, ref rotateAxis, out vector);
                vector.Normalise ();
            } else {
                Vector3.Cross (ref rotateAxis, ref vector2, out vector3);
                vector3.Normalise ();
                Vector3.Cross (ref vector3, ref vector4, out vector);
                vector.Normalise ();
            }
            result.R0C0 = vector3.X;
            result.R0C1 = vector3.Y;
            result.R0C2 = vector3.Z;
            result.R0C3 = zero;
            result.R1C0 = vector4.X;
            result.R1C1 = vector4.Y;
            result.R1C2 = vector4.Z;
            result.R1C3 = zero;
            result.R2C0 = vector.X;
            result.R2C1 = vector.Y;
            result.R2C2 = vector.Z;
            result.R2C3 = zero;
            result.R3C0 = objectPosition.X;
            result.R3C1 = objectPosition.Y;
            result.R3C2 = objectPosition.Z;
            result.R3C3 = one;
        }

        // ref: http://msdn.microsoft.com/en-us/library/bb205351(v=vs.85).aspx
        public static void CreatePerspectiveFieldOfView (Single fieldOfView, Single aspectRatio, Single nearPlaneDistance, Single farPlaneDistance, out Matrix44 result)
        {
            Single zero = 0;
            Single half; Maths.Half(out half);
            Single one; Maths.One(out one);
            Single pi; Maths.Pi(out pi);

            if ((fieldOfView <= zero) || (fieldOfView >= pi)) {
                throw new ArgumentOutOfRangeException ("fieldOfView");
            }
            if (nearPlaneDistance <= zero) {
                throw new ArgumentOutOfRangeException ("nearPlaneDistance");
            }
            if (farPlaneDistance <= zero) {
                throw new ArgumentOutOfRangeException ("farPlaneDistance");
            }
            if (nearPlaneDistance >= farPlaneDistance) {
                throw new ArgumentOutOfRangeException ("nearPlaneDistance");
            }
            Single num = one / (Maths.Tan ((fieldOfView * half)));
            Single num9 = num / aspectRatio;
            result.R0C0 = num9;
            result.R0C1 = result.R0C2 = result.R0C3 = zero;
            result.R1C1 = num;
            result.R1C0 = result.R1C2 = result.R1C3 = zero;
            result.R2C0 = result.R2C1 = zero;
            result.R2C2 = farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
            result.R2C3 = -one;
            result.R3C0 = result.R3C1 = result.R3C3 = zero;
            result.R3C2 = (nearPlaneDistance * farPlaneDistance) / (nearPlaneDistance - farPlaneDistance);
        }

        // ref: http://msdn.microsoft.com/en-us/library/bb205355(v=vs.85).aspx
        public static void CreatePerspective (Single width, Single height, Single nearPlaneDistance, Single farPlaneDistance, out Matrix44 result)
        {
            Single zero = 0;
            Single one; Maths.One(out one);
            Single two = 2;

            if (nearPlaneDistance <= zero) {
                throw new ArgumentOutOfRangeException ("nearPlaneDistance");
            }
            if (farPlaneDistance <= zero) {
                throw new ArgumentOutOfRangeException ("farPlaneDistance");
            }
            if (nearPlaneDistance >= farPlaneDistance) {
                throw new ArgumentOutOfRangeException ("nearPlaneDistance");
            }
            result.R0C0 = (two * nearPlaneDistance) / width;
            result.R0C1 = result.R0C2 = result.R0C3 = zero;
            result.R1C1 = (two * nearPlaneDistance) / height;
            result.R1C0 = result.R1C2 = result.R1C3 = zero;
            result.R2C2 = farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
            result.R2C0 = result.R2C1 = zero;
            result.R2C3 = -one;
            result.R3C0 = result.R3C1 = result.R3C3 = zero;
            result.R3C2 = (nearPlaneDistance * farPlaneDistance) / (nearPlaneDistance - farPlaneDistance);
        }


        // ref: http://msdn.microsoft.com/en-us/library/bb205354(v=vs.85).aspx
        public static void CreatePerspectiveOffCenter (Single left, Single right, Single bottom, Single top, Single nearPlaneDistance, Single farPlaneDistance, out Matrix44 result)
        {
            Single zero = 0;
            Single one; Maths.One(out one);
            Single two = 2;

            if (nearPlaneDistance <= zero) {
                throw new ArgumentOutOfRangeException ("nearPlaneDistance");
            }
            if (farPlaneDistance <= zero) {
                throw new ArgumentOutOfRangeException ("farPlaneDistance");
            }
            if (nearPlaneDistance >= farPlaneDistance) {
                throw new ArgumentOutOfRangeException ("nearPlaneDistance");
            }
            result.R0C0 = (two * nearPlaneDistance) / (right - left);
            result.R0C1 = result.R0C2 = result.R0C3 = zero;
            result.R1C1 = (two * nearPlaneDistance) / (top - bottom);
            result.R1C0 = result.R1C2 = result.R1C3 = zero;
            result.R2C0 = (left + right) / (right - left);
            result.R2C1 = (top + bottom) / (top - bottom);
            result.R2C2 = farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
            result.R2C3 = -one;
            result.R3C2 = (nearPlaneDistance * farPlaneDistance) / (nearPlaneDistance - farPlaneDistance);
            result.R3C0 = result.R3C1 = result.R3C3 = zero;
        }

        // ref: http://msdn.microsoft.com/en-us/library/bb205349(v=vs.85).aspx
        public static void CreateOrthographic (Single width, Single height, Single zNearPlane, Single zFarPlane, out Matrix44 result)
        {
            Single zero = 0;
            Single one; Maths.One(out one);
            Single two = 2;

            result.R0C0 = two / width;
            result.R0C1 = result.R0C2 = result.R0C3 = zero;
            result.R1C1 = two / height;
            result.R1C0 = result.R1C2 = result.R1C3 = zero;
            result.R2C2 = one / (zNearPlane - zFarPlane);
            result.R2C0 = result.R2C1 = result.R2C3 = zero;
            result.R3C0 = result.R3C1 = zero;
            result.R3C2 = zNearPlane / (zNearPlane - zFarPlane);
            result.R3C3 = one;
        }

        // ref: http://msdn.microsoft.com/en-us/library/bb205348(v=vs.85).aspx
        public static void CreateOrthographicOffCenter (Single left, Single right, Single bottom, Single top, Single zNearPlane, Single zFarPlane, out Matrix44 result)
        {
            Single zero = 0;
            Single one; Maths.One(out one);
            Single two = 2;

            result.R0C0 = two / (right - left);
            result.R0C1 = result.R0C2 = result.R0C3 = zero;
            result.R1C1 = two / (top - bottom);
            result.R1C0 = result.R1C2 = result.R1C3 = zero;
            result.R2C2 = one / (zNearPlane - zFarPlane);
            result.R2C0 = result.R2C1 = result.R2C3 = zero;
            result.R3C0 = (left + right) / (left - right);
            result.R3C1 = (top + bottom) / (bottom - top);
            result.R3C2 = zNearPlane / (zNearPlane - zFarPlane);
            result.R3C3 = one;
        }

        // ref: http://msdn.microsoft.com/en-us/library/bb205343(v=VS.85).aspx
        public static void CreateLookAt (ref Vector3 cameraPosition, ref Vector3 cameraTarget, ref Vector3 cameraUpVector, out Matrix44 result)
        {
            Single zero = 0;
            Single one; Maths.One(out one);

            Vector3 targetToPosition = cameraPosition - cameraTarget;

            Vector3 vector; Vector3.Normalise (ref targetToPosition, out vector);

            Vector3 cross; Vector3.Cross (ref cameraUpVector, ref vector, out cross);

            Vector3 vector2; Vector3.Normalise (ref cross, out vector2);
            Vector3 vector3; Vector3.Cross (ref vector, ref vector2, out vector3);
            result.R0C0 = vector2.X;
            result.R0C1 = vector3.X;
            result.R0C2 = vector.X;
            result.R0C3 = zero;
            result.R1C0 = vector2.Y;
            result.R1C1 = vector3.Y;
            result.R1C2 = vector.Y;
            result.R1C3 = zero;
            result.R2C0 = vector2.Z;
            result.R2C1 = vector3.Z;
            result.R2C2 = vector.Z;
            result.R2C3 = zero;

            Vector3.Dot (ref vector2, ref cameraPosition, out result.R3C0);
            Vector3.Dot (ref vector3, ref cameraPosition, out result.R3C1);
            Vector3.Dot (ref vector, ref cameraPosition, out result.R3C2);

            result.R3C0 *= -one;
            result.R3C1 *= -one;
            result.R3C2 *= -one;

            result.R3C3 = one;
        }




        // ref: http://msdn.microsoft.com/en-us/library/bb205364(v=VS.85).aspx
        public static void CreateShadow (ref Vector3 lightDirection, ref Plane plane, out Matrix44 result)
        {
            Single zero = 0;

            Plane plane2;
            Plane.Normalise (ref plane, out plane2);
            Single num = ((plane2.Normal.X * lightDirection.X) + (plane2.Normal.Y * lightDirection.Y)) + (plane2.Normal.Z * lightDirection.Z);
            Single num5 = -plane2.Normal.X;
            Single num4 = -plane2.Normal.Y;
            Single num3 = -plane2.Normal.Z;
            Single num2 = -plane2.D;
            result.R0C0 = (num5 * lightDirection.X) + num;
            result.R1C0 = num4 * lightDirection.X;
            result.R2C0 = num3 * lightDirection.X;
            result.R3C0 = num2 * lightDirection.X;
            result.R0C1 = num5 * lightDirection.Y;
            result.R1C1 = (num4 * lightDirection.Y) + num;
            result.R2C1 = num3 * lightDirection.Y;
            result.R3C1 = num2 * lightDirection.Y;
            result.R0C2 = num5 * lightDirection.Z;
            result.R1C2 = num4 * lightDirection.Z;
            result.R2C2 = (num3 * lightDirection.Z) + num;
            result.R3C2 = num2 * lightDirection.Z;
            result.R0C3 = zero;
            result.R1C3 = zero;
            result.R2C3 = zero;
            result.R3C3 = num;
        }

        // ref: http://msdn.microsoft.com/en-us/library/bb205356(v=VS.85).aspx
        public static void CreateReflection (ref Plane value, out Matrix44 result)
        {
            Single zero = 0;
            Single one; Maths.One(out one);
            Single two = 2;

            Plane plane;

            Plane.Normalise (ref value, out plane);

            value.Normalise ();

            Single x = plane.Normal.X;
            Single y = plane.Normal.Y;
            Single z = plane.Normal.Z;

            Single num3 = -two * x;
            Single num2 = -two * y;
            Single num = -two * z;

            result.R0C0 = (num3 * x) + one;
            result.R0C1 = num2 * x;
            result.R0C2 = num * x;
            result.R0C3 = zero;
            result.R1C0 = num3 * y;
            result.R1C1 = (num2 * y) + one;
            result.R1C2 = num * y;
            result.R1C3 = zero;
            result.R2C0 = num3 * z;
            result.R2C1 = num2 * z;
            result.R2C2 = (num * z) + one;
            result.R2C3 = zero;
            result.R3C0 = num3 * plane.D;
            result.R3C1 = num2 * plane.D;
            result.R3C2 = num * plane.D;
            result.R3C3 = one;
        }

        #endregion

    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Every object in a scene has a Transform. It's used to store and manipulate
    /// the position, rotation and scale of the object. Every Transforms can have a
    /// parent, which allows you to apply position, rotation and scale hierarchically.
    /// This is the hierarchy seen in the Hierarchy pane. They also support
    /// enumerators so you can loop through children using:
    /// </summary>
    public sealed class Transform
        : IEnumerable
    {

        public static Transform Origin = new Transform ();

        // DATA --------------------------------------------------
        Vector3 _localPosition = Vector3.Zero;
        Vector3 _localScale = new Vector3 (1, 1, 1);
        Quaternion _localRotation = Quaternion.Identity;
        List<Transform> _children = new List<Transform> ();
        List<Transform> _cachedHierarchyToRootParent = new List<Transform> (0);
        Transform _parent = null;
        //--------------------------------------------------------



        // The parent of the transform.
        public Transform Parent {
            get {
                return _parent;
            }
            set {
                _parent = value;
                _cachedHierarchyToRootParent.Clear ();
                Transform temp = this.Parent;
                while (temp != null) {
                    _cachedHierarchyToRootParent.Add (temp);
                    temp = temp.Parent;
                }
            }
        }

        // Returns the topmost transform in the hierarchy.
        public Transform Root {
            get {
                Transform temp = this;
                while (temp.Parent != null) {
                    temp = temp.Parent;
                }

                return temp;
            }
        }

        // How many child transforms?
        public int ChildCount {
            get {
                return _children.Count;
            }
        }

        // Matrix44 that transforms a point from local space into world space.
        internal Matrix44 LocalToWorld {
            get {
                Matrix44 trans = Matrix44.Identity;
                Transform temp = this.Parent;
                while (temp != null) {
                    trans = trans * temp.LocalLocation;
                    temp = temp.Parent;
                }
                return trans;
            }
        }

        // Matrix44 that transforms a point from world space into local space.
        internal Matrix44 WorldToLocal {
            get {
                // why doesn't this work
                //Matrix44 trans = Matrix44.Identity;
                //for (int i = _cachedHierarchyToRootParent.Count - 1; i > -1; --i)
                //{
                //    trans = _cachedHierarchyToRootParent[i].LocalLocation * trans;
                //}

                //use this for now
                Matrix44 loc2World = LocalToWorld;

                Matrix44 trans; Matrix44.Invert(ref loc2World, out trans);
                return trans;
            }
        }

        // In world space.
        public Vector3 Forward { get { return Location.Forward; } }

        public Vector3 Up { get { return Location.Up; } }

        public Vector3 Right { get { return Location.Right; } }

        public Vector3 Position {
            get
            {
                Vector3 localPos = LocalPosition;
                Matrix44 location; Matrix44.CreateTranslation(ref localPos, out location);
                Transform temp = this.Parent;
                while (temp != null)
                {
                    Matrix44 rotMat;
                    Quaternion lr = temp.LocalRotation;
                    Matrix44.CreateFromQuaternion(ref lr, out rotMat);
                    Matrix44.Transform(ref location, ref lr, out location);
                    //rotMat * location;


                    location.Translation += temp.LocalPosition;
                    temp = temp.Parent;
                }
                return location.Translation;
            }

            set
            {
                Matrix44 trans;
                Matrix44.CreateTranslation(ref value, out trans);

                Matrix44 newMat;
                Matrix44 w2l = WorldToLocal;
                Matrix44.Multiply(ref trans, ref w2l, out newMat);

                LocalPosition = newMat.Translation;
            }
        }

        public Quaternion Rotation {
            get {
                Quaternion rotation = LocalRotation;
                Transform temp = this.Parent;
                while (temp != null) {
                    rotation = rotation * temp.LocalRotation;
                    temp = temp.Parent;
                }
                return rotation;
            }
            set
            {
                Quaternion q = value;
                q.Normalise ();

                if (WorldToLocal != Matrix44.Identity)
                {
                    Matrix44 mat;
                    Matrix44.CreateFromQuaternion (ref q, out mat);

                    Matrix44 r = WorldToLocal * mat;

                    Quaternion newRot;
                    Quaternion.CreateFromRotationMatrix (ref r, out newRot);
                    LocalRotation = newRot;
                }
                else
                {
                    LocalRotation = q;
                }
            }
        }

        public Vector3 Scale {
            get {
                Vector3 scale = this.LocalScale;
                Transform temp = this.Parent;
                while (temp != null) {
                    scale = scale * temp.LocalScale;
                    temp = temp.Parent;
                }
                return scale;
            }
        }

        public Vector3 EulerAngles { get { return BlimeyMathsHelper.QuaternionToEuler (Rotation); } }

        public Matrix44 Location {
            get {
                return LocalLocation * LocalToWorld;
            }
        }


        // Relative to the parent transform.
        public Vector3 LocalPosition { get { return _localPosition; } set { _localPosition = value; } }

        public Quaternion LocalRotation { get { return _localRotation; } set { _localRotation = value; _localRotation.Normalise(); } }

        public Vector3 LocalScale { get { return _localScale; } set { _localScale = value; } }

        public Vector3 LocalEulerAngles
        {
            get { return BlimeyMathsHelper.QuaternionToEuler(_localRotation); }
            set { _localRotation = BlimeyMathsHelper.EulerToQuaternion(value); }
        }

        public Matrix44 LocalLocation
        {
            get
            {
                Matrix44 scale;
                Matrix44.CreateScale(ref _localScale, out scale);

                Matrix44 rotation;
                Matrix44.CreateFromQuaternion(ref _localRotation, out rotation);

                Matrix44 translation;
                Matrix44.CreateTranslation(ref _localPosition, out translation);

                Matrix44 result = scale * rotation * translation;
                return result;
            }
        }


        // Moves the transform in the direction and distance of translation.
        //
        // If relativeTo is left out or set to Space.Self the movement is applied
        // relative to the transform's local axes. (the x, y and z axes shown when
        // selecting the object inside the Scene View.) If relativeTo is Space.World
        // the movement is applied relative to the world coordinate system.
        public void Translate (Vector3 translation)
        {
            Position += translation;
        }

        public void Translate (Vector3 translation, Space relativeTo)
        {
            if (relativeTo == Space.World)
                Position += translation;
            else
                LocalPosition += translation;
        }

        public void Translate (Vector3 translation, Transform relativeTo)
        {
            Vector3 pointInWorld = relativeTo.TransformPoint (translation);
            Vector3 worldTrans = pointInWorld - relativeTo.Position;
            this.Position += worldTrans;
        }

        public void Translate (Single x, Single y, Single z)
        {
            this.Translate (new Vector3 (x, y, z));
        }

        public void Translate (Single x, Single y, Single z, Space relativeTo)
        {
            this.Translate (new Vector3 (x, y, z), relativeTo);
        }

        public void Translate (Single x, Single y, Single z, Transform relativeTo)
        {
            this.Translate (new Vector3 (x, y, z), relativeTo);
        }

        // Applies a rotation of eulerAngles.z degrees around the z axis,
        // eulerAngles.x degrees around the x axis, and eulerAngles.y
        // degrees around the y axis (in that order).
        //
        // If relativeTo is left out or set to Space.Self the rotation is applied
        // around the transform's local axes. (The x, y and z axes shown when
        // selecting the object inside the Scene View.) If relativeTo is
        // Space.World the rotation is applied around the world x, y, z axes.
        public void Rotate (Vector3 eulerAngles)
        {
            throw new System.NotImplementedException();
        }

        public void Rotate (Vector3 axis, Single angle)
        {
            throw new System.NotImplementedException();
        }

        public void Rotate (Vector3 eulerAngles, Space relativeTo)
        {
            throw new System.NotImplementedException();
        }

        public void Rotate (Single xAngle, Single yAngle, Single zAngle)
        {
            throw new System.NotImplementedException();
        }

        public void Rotate (Vector3 axis, Single angle, Space relativeTo)
        {
            throw new System.NotImplementedException();
        }

        public void Rotate (Single xAngle, Single yAngle, Single zAngle, Space relativeTo)
        {
            throw new System.NotImplementedException();
        }

        // Rotates the transform about axis passing through point
        // in world coordinates by angle degrees.
        // This modifies both the position and the rotation of the transform.
        public void RotateAround (Vector3 axis, Single angle)
        {
            throw new System.NotImplementedException();
        }

        public void RotateAround (Vector3 point, Vector3 axis, Single angle)
        {
            throw new System.NotImplementedException();
        }

        public void RotateAroundLocal (Vector3 axis, Single angle)
        {
            throw new System.NotImplementedException();
        }

        //Rotates the transform so the forward vector points at /target/'s current position.
        //
        // Then it rotates the transform to point its up direction vector in the direction
        // hinted at by the worldUp vector. If you leave out the worldUp parameter, the
        // function will use the world y axis. worldUp is only a hint vector. The up
        // vector of the rotation will only match the worldUp vector if the forward
        // direction is perpendicular to worldUp
        public void LookAt (Transform target)
        {
            LookAt (target, Vector3.Up);
        }

        public void LookAt (Vector3 worldPosition)
        {
            LookAt (worldPosition, Vector3.Up);
        }

        public void LookAt (Transform target, Vector3 worldUp)
        {
            LookAt (target.Position, worldUp);
        }

        public void LookAt (Vector3 worldPosition, Vector3 worldUp)
        {
            Vector3 lookAtVector = worldPosition - this.Position;
            Vector3.Normalise(ref lookAtVector, out lookAtVector);

            Matrix44 newOrientation = Matrix44.Identity;

            newOrientation.Forward = lookAtVector;

            Vector3 newRight;
            Vector3.Cross(ref lookAtVector, ref worldUp, out newRight);

            Single epsilon; Maths.Epsilon(out epsilon);

            Single newRightLengthSquared =
                (newRight.X * newRight.X) +
                (newRight.Y * newRight.Y) +
                (newRight.Z * newRight.Z);

            if (newRightLengthSquared <= epsilon ||
                Single.IsInfinity (newRightLengthSquared)) {
                newRight = Vector3.Zero;
            } else {
                Vector3.Normalise(ref newRight, out newRight);

            }

            newOrientation.Right = newRight;

            Vector3 newUp;
            Vector3.Cross(ref newRight, ref lookAtVector, out newUp);

            Single newUpLengthSquared =
                (newUp.X * newUp.X) +
                (newUp.Y * newUp.Y) +
                (newUp.Z * newUp.Z);


            if (newUpLengthSquared <= epsilon ||
                Single.IsInfinity (newUpLengthSquared)) {
                newUp = Vector3.Zero;
            } else {
                Vector3.Normalise(ref newUp, out newUp);

            }

            newOrientation.Up = newUp;

            Quaternion rotation;
            Quaternion.CreateFromRotationMatrix(ref newOrientation, out rotation);

            this.Rotation = rotation;

            /*

            // A vector going from our parent game object to our Subject
            Vector3 lookAtVector = Subject.Position - this.Parent.Transform.Position;

            // A direction from our parent game object to our Subject
            Vector3.Normalise(ref lookAtVector, out lookAtVector);

            // Build a new orientation matrix
            Matrix44 newOrientation = Matrix44.Identity;

            Vector3 t1;
            Vector3.Normalise(ref lookAtVector, out t1);
            newOrientation.Forward = t1;

            if (LockToY)
            {
                Vector3 t2 = Vector3.Up;
                Vector3.Normalise(ref t2, out t2);
                newOrientation.Up = t2;

                Vector3 b = newOrientation.Backward;
                Vector3 u = newOrientation.Up;

                Vector3 r;
                Vector3.Cross(ref b, ref u, out r);
                Vector3.Normalise(ref r, out r);
                newOrientation.Right = r;
            }
            else
            {
                Vector3 f = newOrientation.Forward;
                Vector3 u = Vector3.Up;
                Vector3 r;
                Vector3.Cross(ref f, ref u, out r);
                Vector3.Normalise(ref r, out r);
                newOrientation.Right = r;

                Vector3.Cross(ref r, ref f, out u);
                Vector3.Normalise(ref u, out u);
                newOrientation.Up = u;
            }

            Quaternion rotation;
            Quaternion.CreateFromRotationMatrix(ref newOrientation, out rotation);
            this.Parent.Transform.Rotation = rotation;
        }

            */

        }


        // Transforms direction from local space to world space.
        // This operation is not affected by scale or position of the transform.
        // The returned vector has the same length as direction.
        public Vector3 TransformDirection (Vector3 direction)
        {
            Single length = direction.Length ();
            Vector3.Normalise(ref direction, out direction);
            var t = TransformPoint (direction);
            Vector3.Normalise(ref t, out t);

            return t * length;
        }

        public Vector3 TransformDirection (Single x, Single y, Single z)
        {
            return TransformDirection (new Vector3 (x, y, z));
        }

        // Transforms a direction from world space to local space.
        // The opposite of Transform.TransformDirection.
        // This operation is unaffected by scale.
        public Vector3 InverseTransformDirection (Vector3 direction)
        {
            Single length = direction.Length ();
            Vector3.Normalise(ref direction, out direction);
            var t = InverseTransformPoint(direction);
            Vector3.Normalise(ref t, out t);
            return t * length;
        }

        public Vector3 InverseTransformDirection (Single x, Single y, Single z)
        {
            return InverseTransformDirection (new Vector3 (x, y, z));
        }

        // Transforms position from local space to world space.
        // Note that the returned position is affected by scale.
        // Use Transform.TransformDirection if you are dealing with directions.
        public Vector3 TransformPoint (Vector3 position)
        {
            Matrix44 trans;
            Matrix44.CreateTranslation(ref position, out trans);

            return (trans * LocalToWorld).Translation;
        }

        public Vector3 TransformPoint (Single x, Single y, Single z)
        {
            return TransformPoint (new Vector3 (x, y, z));
        }

        // Transforms position from world space to local space. The
        // opposite of Transform.TransformPoint.
        // Note that the returned position is affected by scale. Use
        // Transform.InverseTransformDirection if you are dealing with directions.
        public Vector3 InverseTransformPoint (Vector3 position)
        {
            Matrix44 trans;
            Matrix44.CreateTranslation(ref position, out trans);

            return (trans * WorldToLocal).Translation;
        }

        public Vector3 InverseTransformPoint (Single x, Single y, Single z)
        {
            return InverseTransformPoint (new Vector3 (x, y, z));
        }

        // Unparents all children.
        // Useful if you want to destroy the root of a hierarchy without destroying the children.
        public void DetachChildren ()
        {
            while (_children.Count > 0) {
                _children [0].Parent = null;
                _children.RemoveAt (0);
            }
        }

        // Not sure if we want this?
        public Transform GetChild (int index)
        {
            if (_children.Count > index)
                return _children [index];

            return null;
        }



        // Returns a Booleanean value that indicates whether the transform
        // is a child of a given transform. true if this transform is a child,
        // deep child (child of a child...) or identical to this transform, otherwise false.
        public Boolean IsChildOf (Transform parent)
        {
            Transform temp = this;
            while (temp != null) {
                if (temp == parent)
                    return true;

                temp = temp.Parent;
            }

            return false;
        }

        public override String ToString ()
        {
            return
                "LOCAL \n" +
                string.Format (" - Position: |{0} {1} {2}|\n", LocalPosition.X, LocalPosition.Y, LocalPosition.Z) +
                string.Format (" - Rotation: |{0} {1} {2}|\n", LocalEulerAngles.X, LocalEulerAngles.Y, LocalEulerAngles.Z) +
                string.Format (" - Scale:    |{0} {1} {2}|\n", LocalScale.X, LocalScale.Y, LocalScale.Z) +

                "WORLD \n" +
                string.Format (" - Position: |{0} {1} {2}|\n", Position.X, Position.Y, Position.Z) +
                string.Format (" - Rotation: |{0} {1} {2}|\n", EulerAngles.X, EulerAngles.Y, EulerAngles.Z) +
                string.Format (" - Scale:    |{0} {1} {2}|\n", Scale.X, Scale.Y, Scale.Z);

        }
        //--------------------------------------------------------------------------

        // IEnumerable implementation
        public IEnumerator GetEnumerator ()
        {
            return new Enumerator (this);
        }

        // Nested Types
        sealed class Enumerator
            : IEnumerator
        {
            // Fields
            int currentIndex = -1;
            Transform outer;

            // Methods
            internal Enumerator (Transform outer)
            {
                this.outer = outer;
            }

            public Boolean MoveNext ()
            {
                int childCount = this.outer.ChildCount;
                return (++this.currentIndex < childCount);
            }

            public void Reset ()
            {
                this.currentIndex = -1;
            }

            // Properties
            public object Current {
                get {
                    return this.outer.GetChild (this.currentIndex);
                }
            }
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    [StructLayout (LayoutKind.Sequential)]
    public struct BoundingBox
        : IEquatable<BoundingBox>
    {
        public const int CornerCount = 8;
        public Vector3 Min;
        public Vector3 Max;

        public Vector3[] GetCorners ()
        {
            return new Vector3[] { new Vector3 (this.Min.X, this.Max.Y, this.Max.Z), new Vector3 (this.Max.X, this.Max.Y, this.Max.Z), new Vector3 (this.Max.X, this.Min.Y, this.Max.Z), new Vector3 (this.Min.X, this.Min.Y, this.Max.Z), new Vector3 (this.Min.X, this.Max.Y, this.Min.Z), new Vector3 (this.Max.X, this.Max.Y, this.Min.Z), new Vector3 (this.Max.X, this.Min.Y, this.Min.Z), new Vector3 (this.Min.X, this.Min.Y, this.Min.Z) };
        }

        public BoundingBox (Vector3 min, Vector3 max)
        {
            this.Min = min;
            this.Max = max;
        }

        public Boolean Equals (BoundingBox other)
        {
            return ((this.Min == other.Min) && (this.Max == other.Max));
        }

        public override Boolean Equals (Object obj)
        {
            Boolean flag = false;
            if (obj is BoundingBox) {
                flag = this.Equals ((BoundingBox)obj);
            }
            return flag;
        }

        public override Int32 GetHashCode ()
        {
            return (this.Min.GetHashCode () + this.Max.GetHashCode ());
        }

        public override String ToString ()
        {
            return string.Format ("{{Min:{0} Max:{1}}}", new Object[] { this.Min.ToString (), this.Max.ToString () });
        }

        public static void CreateMerged (ref BoundingBox original, ref BoundingBox additional, out BoundingBox result)
        {
            Vector3 vector;
            Vector3 vector2;
            Vector3.Min (ref original.Min, ref additional.Min, out vector2);
            Vector3.Max (ref original.Max, ref additional.Max, out vector);
            result.Min = vector2;
            result.Max = vector;
        }

        public static void CreateFromSphere (ref BoundingSphere sphere, out BoundingBox result)
        {
            result.Min.X = sphere.Center.X - sphere.Radius;
            result.Min.Y = sphere.Center.Y - sphere.Radius;
            result.Min.Z = sphere.Center.Z - sphere.Radius;
            result.Max.X = sphere.Center.X + sphere.Radius;
            result.Max.Y = sphere.Center.Y + sphere.Radius;
            result.Max.Z = sphere.Center.Z + sphere.Radius;
        }

        public static BoundingBox CreateFromPoints (IEnumerable<Vector3> points)
        {
            if (points == null) {
                throw new ArgumentNullException ();
            }
            Boolean flag = false;
            Vector3 vector3 = new Vector3 (Single.MaxValue, Single.MaxValue, Single.MaxValue);
            Vector3 vector2 = new Vector3 (Single.MinValue, Single.MinValue, Single.MinValue);
            foreach (Vector3 vector in points) {
                Vector3 vector4 = vector;
                Vector3.Min (ref vector3, ref vector4, out vector3);
                Vector3.Max (ref vector2, ref vector4, out vector2);
                flag = true;
            }
            if (!flag) {
                throw new ArgumentException ("BoundingBoxZeroPoints");
            }
            return new BoundingBox (vector3, vector2);
        }

        public Boolean Intersects (ref BoundingBox box)
        {
            if ((this.Max.X < box.Min.X) || (this.Min.X > box.Max.X)) {
                return false;
            }
            if ((this.Max.Y < box.Min.Y) || (this.Min.Y > box.Max.Y)) {
                return false;
            }
            return ((this.Max.Z >= box.Min.Z) && (this.Min.Z <= box.Max.Z));
        }

        public Boolean Intersects (ref BoundingFrustum frustum)
        {
            if (null == frustum) {
                throw new ArgumentNullException ("frustum - NullNotAllowed");
            }
            return frustum.Intersects (ref this);
        }

        public PlaneIntersectionType Intersects (ref Plane plane)
        {
            Single zero = 0;

            Vector3 vector;
            Vector3 vector2;
            vector2.X = (plane.Normal.X >= zero) ? this.Min.X : this.Max.X;
            vector2.Y = (plane.Normal.Y >= zero) ? this.Min.Y : this.Max.Y;
            vector2.Z = (plane.Normal.Z >= zero) ? this.Min.Z : this.Max.Z;
            vector.X = (plane.Normal.X >= zero) ? this.Max.X : this.Min.X;
            vector.Y = (plane.Normal.Y >= zero) ? this.Max.Y : this.Min.Y;
            vector.Z = (plane.Normal.Z >= zero) ? this.Max.Z : this.Min.Z;
            Single num = ((plane.Normal.X * vector2.X) + (plane.Normal.Y * vector2.Y)) + (plane.Normal.Z * vector2.Z);
            if ((num + plane.D) > zero) {
                return PlaneIntersectionType.Front;
            }
            num = ((plane.Normal.X * vector.X) + (plane.Normal.Y * vector.Y)) + (plane.Normal.Z * vector.Z);
            if ((num + plane.D) < zero) {
                return PlaneIntersectionType.Back;
            }
            return PlaneIntersectionType.Intersecting;
        }

        public Single? Intersects (ref Ray ray)
        {
            Single epsilon; Maths.Epsilon(out epsilon);

            Single zero = 0;
            Single one = 1;

            Single num = zero;
            Single maxValue = Single.MaxValue;
            if (Maths.Abs (ray.Direction.X) < epsilon) {
                if ((ray.Position.X < this.Min.X) || (ray.Position.X > this.Max.X)) {
                    return null;
                }
            } else {
                Single num11 = one / ray.Direction.X;
                Single num8 = (this.Min.X - ray.Position.X) * num11;
                Single num7 = (this.Max.X - ray.Position.X) * num11;
                if (num8 > num7) {
                    Single num14 = num8;
                    num8 = num7;
                    num7 = num14;
                }
                num = Maths.Max (num8, num);
                maxValue = Maths.Min (num7, maxValue);
                if (num > maxValue) {
                    return null;
                }
            }
            if (Maths.Abs (ray.Direction.Y) < epsilon) {
                if ((ray.Position.Y < this.Min.Y) || (ray.Position.Y > this.Max.Y)) {
                    return null;
                }
            } else {
                Single num10 = one / ray.Direction.Y;
                Single num6 = (this.Min.Y - ray.Position.Y) * num10;
                Single num5 = (this.Max.Y - ray.Position.Y) * num10;
                if (num6 > num5) {
                    Single num13 = num6;
                    num6 = num5;
                    num5 = num13;
                }
                num = Maths.Max (num6, num);
                maxValue = Maths.Min (num5, maxValue);
                if (num > maxValue) {
                    return null;
                }
            }


            if (Maths.Abs (ray.Direction.Z) < epsilon) {
                if ((ray.Position.Z < this.Min.Z) || (ray.Position.Z > this.Max.Z)) {
                    return null;
                }
            } else {
                Single num9 = one / ray.Direction.Z;
                Single num4 = (this.Min.Z - ray.Position.Z) * num9;
                Single num3 = (this.Max.Z - ray.Position.Z) * num9;
                if (num4 > num3) {
                    Single num12 = num4;
                    num4 = num3;
                    num3 = num12;
                }
                num = Maths.Max (num4, num);
                maxValue = Maths.Min (num3, maxValue);
                if (num > maxValue) {
                    return null;
                }
            }
            return new Single? (num);
        }

        public Boolean Intersects (ref BoundingSphere sphere)
        {
            Single num;
            Vector3 vector;
            Vector3.Clamp (ref sphere.Center, ref this.Min, ref this.Max, out vector);
            Vector3.DistanceSquared (ref sphere.Center, ref vector, out num);
            return (num <= (sphere.Radius * sphere.Radius));
        }

        public ContainmentType Contains (ref BoundingBox box)
        {
            if ((this.Max.X < box.Min.X) || (this.Min.X > box.Max.X)) {
                return ContainmentType.Disjoint;
            }
            if ((this.Max.Y < box.Min.Y) || (this.Min.Y > box.Max.Y)) {
                return ContainmentType.Disjoint;
            }
            if ((this.Max.Z < box.Min.Z) || (this.Min.Z > box.Max.Z)) {
                return ContainmentType.Disjoint;
            }
            if ((((this.Min.X <= box.Min.X) && (box.Max.X <= this.Max.X)) && ((this.Min.Y <= box.Min.Y) && (box.Max.Y <= this.Max.Y))) && ((this.Min.Z <= box.Min.Z) && (box.Max.Z <= this.Max.Z))) {
                return ContainmentType.Contains;
            }
            return ContainmentType.Intersects;
        }

        public ContainmentType Contains (ref BoundingFrustum frustum)
        {
            if (null == frustum) {
                throw new ArgumentNullException ("frustum - NullNotAllowed");
            }
            if (!frustum.Intersects (ref this)) {
                return ContainmentType.Disjoint;
            }

            for (Int32 i = 0; i < frustum.cornerArray.Length; ++i) {
                Vector3 vector = frustum.cornerArray[i];
                if (this.Contains (ref vector) == ContainmentType.Disjoint) {
                    return ContainmentType.Intersects;
                }
            }
            return ContainmentType.Contains;
        }

        public ContainmentType Contains (ref Vector3 point)
        {
            if ((((this.Min.X <= point.X) && (point.X <= this.Max.X)) && ((this.Min.Y <= point.Y) && (point.Y <= this.Max.Y))) && ((this.Min.Z <= point.Z) && (point.Z <= this.Max.Z))) {
                return ContainmentType.Contains;
            }
            return ContainmentType.Disjoint;
        }

        public ContainmentType Contains (ref BoundingSphere sphere)
        {
            Single num2;
            Vector3 vector;
            Vector3.Clamp (ref sphere.Center, ref this.Min, ref this.Max, out vector);
            Vector3.DistanceSquared (ref sphere.Center, ref vector, out num2);
            Single radius = sphere.Radius;
            if (num2 > (radius * radius)) {
                return ContainmentType.Disjoint;
            }
            if (((((this.Min.X + radius) <= sphere.Center.X) && (sphere.Center.X <= (this.Max.X - radius))) && (((this.Max.X - this.Min.X) > radius) && ((this.Min.Y + radius) <= sphere.Center.Y))) && (((sphere.Center.Y <= (this.Max.Y - radius)) && ((this.Max.Y - this.Min.Y) > radius)) && ((((this.Min.Z + radius) <= sphere.Center.Z) && (sphere.Center.Z <= (this.Max.Z - radius))) && ((this.Max.X - this.Min.X) > radius)))) {
                return ContainmentType.Contains;
            }
            return ContainmentType.Intersects;
        }

        internal void SupportMapping (ref Vector3 v, out Vector3 result)
        {
            Single zero = 0;

            result.X = (v.X >= zero) ? this.Max.X : this.Min.X;
            result.Y = (v.Y >= zero) ? this.Max.Y : this.Min.Y;
            result.Z = (v.Z >= zero) ? this.Max.Z : this.Min.Z;
        }

        public static Boolean operator == (BoundingBox a, BoundingBox b)
        {
            return a.Equals (b);
        }

        public static Boolean operator != (BoundingBox a, BoundingBox b)
        {
            if (!(a.Min != b.Min)) {
                return (a.Max != b.Max);
            }
            return true;
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class BoundingFrustum
        : IEquatable<BoundingFrustum>
    {
        const int BottomPlaneIndex = 5;

        internal Vector3[] cornerArray;

        public const int CornerCount = 8;

        const int FarPlaneIndex = 1;

        GjkDistance gjk;

        const int LeftPlaneIndex = 2;

        Matrix44 matrix;

        const int NearPlaneIndex = 0;

        const int NumPlanes = 6;

        Plane[] planes;

        const int RightPlaneIndex = 3;

        const int TopPlaneIndex = 4;

        BoundingFrustum ()
        {
            this.planes = new Plane[6];
            this.cornerArray = new Vector3[8];
        }

        public BoundingFrustum (Matrix44 value)
        {
            this.planes = new Plane[6];
            this.cornerArray = new Vector3[8];
            this.SetMatrix (ref value);
        }

        static Vector3 ComputeIntersection (ref Plane plane, ref Ray ray)
        {
            Single planeNormDotRayPos;
            Single planeNormDotRayDir;

            Vector3.Dot (ref plane.Normal, ref ray.Position, out planeNormDotRayPos);
            Vector3.Dot (ref plane.Normal, ref ray.Direction, out planeNormDotRayDir);

            Single num = (-plane.D - planeNormDotRayPos) / planeNormDotRayDir;

            return (ray.Position + (ray.Direction * num));
        }

        static Ray ComputeIntersectionLine (ref Plane p1, ref Plane p2)
        {
            Ray ray = new Ray ();

            Vector3.Cross (ref p1.Normal, ref p2.Normal, out ray.Direction);

            Single num = ray.Direction.LengthSquared ();

            Vector3 a = (-p1.D * p2.Normal) + (p2.D * p1.Normal);

            Vector3 cross;

            Vector3.Cross (ref a, ref ray.Direction, out cross);

            ray.Position =  cross / num;

            return ray;
        }

        public ContainmentType Contains (ref BoundingBox box)
        {
            Boolean flag = false;
            for(Int32 i = 0; i < this.planes.Length; ++i)
            {
                Plane plane = this.planes[i];
                switch (box.Intersects (ref plane)) {
                case PlaneIntersectionType.Front:
                    return ContainmentType.Disjoint;

                case PlaneIntersectionType.Intersecting:
                    flag = true;
                    break;
                }
            }
            if (!flag) {
                return ContainmentType.Contains;
            }
            return ContainmentType.Intersects;
        }

        public ContainmentType Contains (ref BoundingFrustum frustum)
        {
            if (frustum == null) {
                throw new ArgumentNullException ("frustum");
            }
            ContainmentType disjoint = ContainmentType.Disjoint;
            if (this.Intersects (ref frustum)) {
                disjoint = ContainmentType.Contains;
                for (int i = 0; i < this.cornerArray.Length; i++) {
                    if (this.Contains (ref frustum.cornerArray [i]) == ContainmentType.Disjoint) {
                        return ContainmentType.Intersects;
                    }
                }
            }
            return disjoint;
        }

        public ContainmentType Contains (ref BoundingSphere sphere)
        {
            Vector3 center = sphere.Center;
            Single radius = sphere.Radius;
            int num2 = 0;
            foreach (Plane plane in this.planes) {
                Single num5 = ((plane.Normal.X * center.X) + (plane.Normal.Y * center.Y)) + (plane.Normal.Z * center.Z);
                Single num3 = num5 + plane.D;
                if (num3 > radius) {
                    return ContainmentType.Disjoint;
                }
                if (num3 < -radius) {
                    num2++;
                }
            }
            if (num2 != 6) {
                return ContainmentType.Intersects;
            }
            return ContainmentType.Contains;
        }

        public ContainmentType Contains (ref Vector3 point)
        {
            Single epsilon; Maths.FromString("0.00001", out epsilon);

            foreach (Plane plane in this.planes) {
                Single num2 = (((plane.Normal.X * point.X) + (plane.Normal.Y * point.Y)) + (plane.Normal.Z * point.Z)) + plane.D;
                if (num2 > epsilon) {
                    return ContainmentType.Disjoint;
                }
            }
            return ContainmentType.Contains;
        }

        public Boolean Equals (BoundingFrustum other)
        {
            if (other == null) {
                return false;
            }
            return (this.matrix == other.matrix);
        }

        public override Boolean Equals (Object obj)
        {
            Boolean flag = false;
            BoundingFrustum frustum = obj as BoundingFrustum;
            if (frustum != null) {
                flag = this.matrix == frustum.matrix;
            }
            return flag;
        }

        public Vector3[] GetCorners ()
        {
            return (Vector3[])this.cornerArray.Clone ();
        }

        public override Int32 GetHashCode ()
        {
            return this.matrix.GetHashCode ();
        }

        public Boolean Intersects (ref BoundingBox box)
        {
            Boolean flag;
            this.Intersects (ref box, out flag);
            return flag;
        }

        void Intersects (ref BoundingBox box, out Boolean result)
        {
            Single epsilon; Maths.FromString("0.00001", out epsilon);
            Single zero = 0;
            Single four = 4;

            Vector3 closestPoint;
            Vector3 vector2;
            Vector3 vector3;
            Vector3 vector4;
            Vector3 vector5;
            if (this.gjk == null) {
                this.gjk = new GjkDistance ();
            }
            this.gjk.Reset ();
            Vector3.Subtract (ref this.cornerArray [0], ref box.Min, out closestPoint);
            if (closestPoint.LengthSquared () < epsilon) {
                Vector3.Subtract (ref this.cornerArray [0], ref box.Max, out closestPoint);
            }
            Single maxValue = Single.MaxValue;
            Single num3 = zero;
            result = false;
        Label_006D:
            vector5.X = -closestPoint.X;
            vector5.Y = -closestPoint.Y;
            vector5.Z = -closestPoint.Z;
            this.SupportMapping (ref vector5, out vector4);
            box.SupportMapping (ref closestPoint, out vector3);
            Vector3.Subtract (ref vector4, ref vector3, out vector2);
            Single num4 = ((closestPoint.X * vector2.X) + (closestPoint.Y * vector2.Y)) + (closestPoint.Z * vector2.Z);
            if (num4 <= zero) {
                this.gjk.AddSupportPoint (ref vector2);
                closestPoint = this.gjk.ClosestPoint;
                Single num2 = maxValue;
                maxValue = closestPoint.LengthSquared ();
                if ((num2 - maxValue) > (epsilon * num2)) {
                    num3 = four * epsilon * this.gjk.MaxLengthSquared;
                    if (!this.gjk.FullSimplex && (maxValue >= num3)) {
                        goto Label_006D;
                    }
                    result = true;
                }
            }
        }

        public Boolean Intersects (ref BoundingFrustum frustum)
        {
            Single epsilon; Maths.FromString("0.00001", out epsilon);
            Single zero = 0;
            Single four = 4;

            Vector3 closestPoint;
            if (frustum == null) {
                throw new ArgumentNullException ("frustum");
            }
            if (this.gjk == null) {
                this.gjk = new GjkDistance ();
            }
            this.gjk.Reset ();
            Vector3.Subtract (ref this.cornerArray [0], ref frustum.cornerArray [0], out closestPoint);
            if (closestPoint.LengthSquared () < epsilon) {
                Vector3.Subtract (ref this.cornerArray [0], ref frustum.cornerArray [1], out closestPoint);
            }
            Single maxValue = Single.MaxValue;
            Single num3 = zero;
            do {
                Vector3 vector2;
                Vector3 vector3;
                Vector3 vector4;
                Vector3 vector5;
                vector5.X = -closestPoint.X;
                vector5.Y = -closestPoint.Y;
                vector5.Z = -closestPoint.Z;
                this.SupportMapping (ref vector5, out vector4);
                frustum.SupportMapping (ref closestPoint, out vector3);
                Vector3.Subtract (ref vector4, ref vector3, out vector2);
                Single num4 = ((closestPoint.X * vector2.X) + (closestPoint.Y * vector2.Y)) + (closestPoint.Z * vector2.Z);
                if (num4 > zero) {
                    return false;
                }
                this.gjk.AddSupportPoint (ref vector2);
                closestPoint = this.gjk.ClosestPoint;
                Single num2 = maxValue;
                maxValue = closestPoint.LengthSquared ();
                num3 = four * epsilon * this.gjk.MaxLengthSquared;
                if ((num2 - maxValue) <= (epsilon * num2)) {
                    return false;
                }
            } while (!this.gjk.FullSimplex && (maxValue >= num3));
            return true;
        }

        public Boolean Intersects (ref BoundingSphere sphere)
        {
            Boolean flag;
            this.Intersects (ref sphere, out flag);
            return flag;
        }

        void Intersects (ref BoundingSphere sphere, out Boolean result)
        {
            Single zero = 0;
            Single epsilon; Maths.FromString("0.00001", out epsilon);
            Single four = 4;

            Vector3 unitX;
            Vector3 vector2;
            Vector3 vector3;
            Vector3 vector4;
            Vector3 vector5;
            if (this.gjk == null) {
                this.gjk = new GjkDistance ();
            }
            this.gjk.Reset ();
            Vector3.Subtract (ref this.cornerArray [0], ref sphere.Center, out unitX);
            if (unitX.LengthSquared () < epsilon) {
                unitX = Vector3.UnitX;
            }
            Single maxValue = Single.MaxValue;
            Single num3 = zero;
            result = false;
        Label_005A:
            vector5.X = -unitX.X;
            vector5.Y = -unitX.Y;
            vector5.Z = -unitX.Z;
            this.SupportMapping (ref vector5, out vector4);
            sphere.SupportMapping (ref unitX, out vector3);
            Vector3.Subtract (ref vector4, ref vector3, out vector2);
            Single num4 = ((unitX.X * vector2.X) + (unitX.Y * vector2.Y)) + (unitX.Z * vector2.Z);
            if (num4 <= zero) {
                this.gjk.AddSupportPoint (ref vector2);
                unitX = this.gjk.ClosestPoint;
                Single num2 = maxValue;
                maxValue = unitX.LengthSquared ();
                if ((num2 - maxValue) > (epsilon * num2)) {
                    num3 = four * epsilon * this.gjk.MaxLengthSquared;
                    if (!this.gjk.FullSimplex && (maxValue >= num3)) {
                        goto Label_005A;
                    }
                    result = true;
                }
            }
        }

        public PlaneIntersectionType Intersects (ref Plane plane)
        {
            Single zero = 0;

            int num = 0;
            for (int i = 0; i < 8; i++) {
                Single num3;
                Vector3.Dot (ref this.cornerArray [i], ref plane.Normal, out num3);
                if ((num3 + plane.D) > zero) {
                    num |= 1;
                } else {
                    num |= 2;
                }
                if (num == 3) {
                    return PlaneIntersectionType.Intersecting;
                }
            }
            if (num != 1) {
                return PlaneIntersectionType.Back;
            }
            return PlaneIntersectionType.Front;
        }

        public Single? Intersects (ref Ray ray)
        {
            Single? nullable;
            this.Intersects (ref ray, out nullable);
            return nullable;
        }

        void Intersects (ref Ray ray, out Single? result)
        {
            Single epsilon; Maths.FromString("0.00001", out epsilon);
            Single zero = 0;

            ContainmentType type = this.Contains (ref ray.Position);
            if (type == ContainmentType.Contains) {
                result = zero;
            } else {
                Single minValue = Single.MinValue;
                Single maxValue = Single.MaxValue;
                result = zero;
                foreach (Plane plane in this.planes) {
                    Single num3;
                    Single num6;
                    Vector3 normal = plane.Normal;
                    Vector3.Dot (ref ray.Direction, ref normal, out num6);
                    Vector3.Dot (ref ray.Position, ref normal, out num3);
                    num3 += plane.D;
                    if (Maths.Abs (num6) < epsilon) {
                        if (num3 > zero) {
                            return;
                        }
                    } else {
                        Single num = -num3 / num6;
                        if (num6 < zero) {
                            if (num > maxValue) {
                                return;
                            }
                            if (num > minValue) {
                                minValue = num;
                            }
                        } else {
                            if (num < minValue) {
                                return;
                            }
                            if (num < maxValue) {
                                maxValue = num;
                            }
                        }
                    }
                }
                Single num7 = (minValue >= zero) ? minValue : maxValue;
                if (num7 >= zero) {
                    result = new Single? (num7);
                }
            }
        }

        public static Boolean operator == (BoundingFrustum a, BoundingFrustum b)
        {
            return Object.Equals (a, b);
        }

        public static Boolean operator != (BoundingFrustum a, BoundingFrustum b)
        {
            return !Object.Equals (a, b);
        }

        void SetMatrix (ref Matrix44 value)
        {
            this.matrix = value;

            this.planes [2].Normal.X = -value.R0C3 - value.R0C0;
            this.planes [2].Normal.Y = -value.R1C3 - value.R1C0;
            this.planes [2].Normal.Z = -value.R2C3 - value.R2C0;
            this.planes [2].D = -value.R3C3 - value.R3C0;

            this.planes [3].Normal.X = -value.R0C3 + value.R0C0;
            this.planes [3].Normal.Y = -value.R1C3 + value.R1C0;
            this.planes [3].Normal.Z = -value.R2C3 + value.R2C0;
            this.planes [3].D = -value.R3C3 + value.R3C0;

            this.planes [4].Normal.X = -value.R0C3 + value.R0C1;
            this.planes [4].Normal.Y = -value.R1C3 + value.R1C1;
            this.planes [4].Normal.Z = -value.R2C3 + value.R2C1;
            this.planes [4].D = -value.R3C3 + value.R3C1;

            this.planes [5].Normal.X = -value.R0C3 - value.R0C1;
            this.planes [5].Normal.Y = -value.R1C3 - value.R1C1;
            this.planes [5].Normal.Z = -value.R2C3 - value.R2C1;
            this.planes [5].D = -value.R3C3 - value.R3C1;

            this.planes [0].Normal.X = -value.R0C2;
            this.planes [0].Normal.Y = -value.R1C2;
            this.planes [0].Normal.Z = -value.R2C2;
            this.planes [0].D = -value.R3C2;

            this.planes [1].Normal.X = -value.R0C3 + value.R0C2;
            this.planes [1].Normal.Y = -value.R1C3 + value.R1C2;
            this.planes [1].Normal.Z = -value.R2C3 + value.R2C2;
            this.planes [1].D = -value.R3C3 + value.R3C2;

            for (int i = 0; i < 6; i++) {
                Single num2 = this.planes [i].Normal.Length ();
                this.planes [i].Normal = (Vector3)(this.planes [i].Normal / num2);
                this.planes [i].D /= num2;
            }

            Ray ray = ComputeIntersectionLine (ref this.planes [0], ref this.planes [2]);

            this.cornerArray [0] = ComputeIntersection (ref this.planes [4], ref ray);
            this.cornerArray [3] = ComputeIntersection (ref this.planes [5], ref ray);

            ray = ComputeIntersectionLine (ref this.planes [3], ref this.planes [0]);

            this.cornerArray [1] = ComputeIntersection (ref this.planes [4], ref ray);
            this.cornerArray [2] = ComputeIntersection (ref this.planes [5], ref ray);

            ray = ComputeIntersectionLine (ref this.planes [2], ref this.planes [1]);

            this.cornerArray [4] = ComputeIntersection (ref this.planes [4], ref ray);
            this.cornerArray [7] = ComputeIntersection (ref this.planes [5], ref ray);

            ray = ComputeIntersectionLine (ref this.planes [1], ref this.planes [3]);

            this.cornerArray [5] = ComputeIntersection (ref this.planes [4], ref ray);
            this.cornerArray [6] = ComputeIntersection (ref this.planes [5], ref ray);
        }

        internal void SupportMapping (ref Vector3 v, out Vector3 result)
        {
            Single num3;

            int index = 0;

            Vector3.Dot (ref this.cornerArray [0], ref v, out num3);

            for (int i = 1; i < this.cornerArray.Length; i++)
            {
                Single num2;

                Vector3.Dot (ref this.cornerArray [i], ref v, out num2);

                if (num2 > num3)
                {
                    index = i;
                    num3 = num2;
                }
            }

            result = this.cornerArray [index];
        }

        public override String ToString ()
        {
            return string.Format ("{{Near:{0} Far:{1} Left:{2} Right:{3} Top:{4} Bottom:{5}}}", new Object[] { this.Near.ToString (), this.Far.ToString (), this.Left.ToString (), this.Right.ToString (), this.Top.ToString (), this.Bottom.ToString () });
        }

        // Properties
        public Plane Bottom
        {
            get
            {
                return this.planes [5];
            }
        }

        public Plane Far {
            get {
                return this.planes [1];
            }
        }

        public Plane Left {
            get {
                return this.planes [2];
            }
        }

        public Matrix44 Matrix {
            get {
                return this.matrix;
            }
            set {
                this.SetMatrix (ref value);
            }
        }

        public Plane Near {
            get {
                return this.planes [0];
            }
        }

        public Plane Right {
            get {
                return this.planes [3];
            }
        }

        public Plane Top {
            get {
                return this.planes [4];
            }
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    [StructLayout (LayoutKind.Sequential)]
    public struct BoundingSphere
        : IEquatable<BoundingSphere>
    {
        public Vector3 Center;
        public Single Radius;

        public BoundingSphere (Vector3 center, Single radius)
        {
            Single zero = 0;

            if (radius < zero) {
                throw new ArgumentException ("NegativeRadius");
            }
            this.Center = center;
            this.Radius = radius;
        }

        public Boolean Equals (BoundingSphere other)
        {
            return ((this.Center == other.Center) && (this.Radius == other.Radius));
        }

        public override Boolean Equals (Object obj)
        {
            Boolean flag = false;
            if (obj is BoundingSphere) {
                flag = this.Equals ((BoundingSphere)obj);
            }
            return flag;
        }

        public override Int32 GetHashCode ()
        {
            return (this.Center.GetHashCode () + this.Radius.GetHashCode ());
        }

        public override String ToString ()
        {
            return string.Format ("{{Center:{0} Radius:{1}}}", new Object[] { this.Center.ToString (), this.Radius.ToString () });
        }

        public static void CreateMerged (ref BoundingSphere original, ref BoundingSphere additional, out BoundingSphere result)
        {
            Single half; Maths.Half(out half);
            Single one = 1;
            Vector3 vector2;
            Vector3.Subtract (ref additional.Center, ref original.Center, out vector2);
            Single num = vector2.Length ();
            Single radius = original.Radius;
            Single num2 = additional.Radius;
            if ((radius + num2) >= num) {
                if ((radius - num2) >= num) {
                    result = original;
                    return;
                }
                if ((num2 - radius) >= num) {
                    result = additional;
                    return;
                }
            }
            Vector3 vector = (Vector3)(vector2 * (one / num));
            Single num5 = Maths.Min (-radius, num - num2);
            Single num4 = (Maths.Max (radius, num + num2) - num5) * half;
            result.Center = original.Center + ((Vector3)(vector * (num4 + num5)));
            result.Radius = num4;
        }

        public static void CreateFromBoundingBox (ref BoundingBox box, out BoundingSphere result)
        {
            Single half; Maths.Half(out half);
            Single num;
            Vector3.Lerp (ref box.Min, ref box.Max, ref half, out result.Center);
            Vector3.Distance (ref box.Min, ref box.Max, out num);
            result.Radius = num * half;
        }

        public static void CreateFromPoints (IEnumerable<Vector3> points, out BoundingSphere sphere)
        {
            Single half; Maths.Half(out half);
            Single one = 1;

            Single num;
            Single num2;
            Vector3 vector2;
            Single num4;
            Single num5;

            Vector3 vector5;
            Vector3 vector6;
            Vector3 vector7;
            Vector3 vector8;
            Vector3 vector9;
            if (points == null) {
                throw new ArgumentNullException ("points");
            }
            IEnumerator<Vector3> enumerator = points.GetEnumerator ();
            if (!enumerator.MoveNext ()) {
                throw new ArgumentException ("BoundingSphereZeroPoints");
            }
            Vector3 vector4 = vector5 = vector6 = vector7 = vector8 = vector9 = enumerator.Current;
            foreach (Vector3 vector in points) {
                if (vector.X < vector4.X) {
                    vector4 = vector;
                }
                if (vector.X > vector5.X) {
                    vector5 = vector;
                }
                if (vector.Y < vector6.Y) {
                    vector6 = vector;
                }
                if (vector.Y > vector7.Y) {
                    vector7 = vector;
                }
                if (vector.Z < vector8.Z) {
                    vector8 = vector;
                }
                if (vector.Z > vector9.Z) {
                    vector9 = vector;
                }
            }
            Vector3.Distance (ref vector5, ref vector4, out num5);
            Vector3.Distance (ref vector7, ref vector6, out num4);
            Vector3.Distance (ref vector9, ref vector8, out num2);
            if (num5 > num4) {
                if (num5 > num2) {
                    Vector3.Lerp (ref vector5, ref vector4, ref half, out vector2);
                    num = num5 * half;
                } else {
                    Vector3.Lerp (ref vector9, ref vector8, ref half, out vector2);
                    num = num2 * half;
                }
            } else if (num4 > num2) {
                Vector3.Lerp (ref vector7, ref vector6, ref half, out vector2);
                num = num4 * half;
            } else {
                Vector3.Lerp (ref vector9, ref vector8, ref half, out vector2);
                num = num2 * half;
            }
            foreach (Vector3 vector10 in points) {
                Vector3 vector3;
                vector3.X = vector10.X - vector2.X;
                vector3.Y = vector10.Y - vector2.Y;
                vector3.Z = vector10.Z - vector2.Z;
                Single num3 = vector3.Length ();
                if (num3 > num) {
                    num = (num + num3) * half;
                    vector2 += (Vector3)((one - (num / num3)) * vector3);
                }
            }
            sphere.Center = vector2;
            sphere.Radius = num;
        }

        public static void CreateFromFrustum (ref BoundingFrustum frustum, out BoundingSphere sphere)
        {
            if (frustum == null) {
                throw new ArgumentNullException ("frustum");
            }

            CreateFromPoints (frustum.cornerArray, out sphere);
        }

        public Boolean Intersects (ref BoundingBox box)
        {
            Single num;
            Vector3 vector;
            Vector3.Clamp (ref this.Center, ref box.Min, ref box.Max, out vector);
            Vector3.DistanceSquared (ref this.Center, ref vector, out num);
            return (num <= (this.Radius * this.Radius));
        }

        public Boolean Intersects (ref BoundingFrustum frustum)
        {
            if (null == frustum) {
                throw new ArgumentNullException ("frustum - NullNotAllowed");
            }
            return frustum.Intersects (ref this);
        }

        public PlaneIntersectionType Intersects (ref Plane plane)
        {
            return plane.Intersects (ref this);
        }

        public Single? Intersects (ref Ray ray)
        {
            return ray.Intersects (ref this);
        }

        public Boolean Intersects (ref BoundingSphere sphere)
        {
            Single two = 2;

            Single num3;
            Vector3.DistanceSquared (ref this.Center, ref sphere.Center, out num3);
            Single radius = this.Radius;
            Single num = sphere.Radius;
            if ((((radius * radius) + ((two * radius) * num)) + (num * num)) <= num3) {
                return false;
            }
            return true;
        }

        public ContainmentType Contains (ref BoundingBox box)
        {
            Vector3 vector;
            if (!box.Intersects (ref this)) {
                return ContainmentType.Disjoint;
            }
            Single num = this.Radius * this.Radius;
            vector.X = this.Center.X - box.Min.X;
            vector.Y = this.Center.Y - box.Max.Y;
            vector.Z = this.Center.Z - box.Max.Z;
            if (vector.LengthSquared () > num) {
                return ContainmentType.Intersects;
            }
            vector.X = this.Center.X - box.Max.X;
            vector.Y = this.Center.Y - box.Max.Y;
            vector.Z = this.Center.Z - box.Max.Z;
            if (vector.LengthSquared () > num) {
                return ContainmentType.Intersects;
            }
            vector.X = this.Center.X - box.Max.X;
            vector.Y = this.Center.Y - box.Min.Y;
            vector.Z = this.Center.Z - box.Max.Z;
            if (vector.LengthSquared () > num) {
                return ContainmentType.Intersects;
            }
            vector.X = this.Center.X - box.Min.X;
            vector.Y = this.Center.Y - box.Min.Y;
            vector.Z = this.Center.Z - box.Max.Z;
            if (vector.LengthSquared () > num) {
                return ContainmentType.Intersects;
            }
            vector.X = this.Center.X - box.Min.X;
            vector.Y = this.Center.Y - box.Max.Y;
            vector.Z = this.Center.Z - box.Min.Z;
            if (vector.LengthSquared () > num) {
                return ContainmentType.Intersects;
            }
            vector.X = this.Center.X - box.Max.X;
            vector.Y = this.Center.Y - box.Max.Y;
            vector.Z = this.Center.Z - box.Min.Z;
            if (vector.LengthSquared () > num) {
                return ContainmentType.Intersects;
            }
            vector.X = this.Center.X - box.Max.X;
            vector.Y = this.Center.Y - box.Min.Y;
            vector.Z = this.Center.Z - box.Min.Z;
            if (vector.LengthSquared () > num) {
                return ContainmentType.Intersects;
            }
            vector.X = this.Center.X - box.Min.X;
            vector.Y = this.Center.Y - box.Min.Y;
            vector.Z = this.Center.Z - box.Min.Z;
            if (vector.LengthSquared () > num) {
                return ContainmentType.Intersects;
            }
            return ContainmentType.Contains;
        }

        public ContainmentType Contains (ref BoundingFrustum frustum)
        {
            if (null == frustum) {
                throw new ArgumentNullException ("frustum - NullNotAllowed");
            }
            if (!frustum.Intersects (ref this)) {
                return ContainmentType.Disjoint;
            }
            Single num2 = this.Radius * this.Radius;
            foreach (Vector3 vector2 in frustum.cornerArray) {
                Vector3 vector;
                vector.X = vector2.X - this.Center.X;
                vector.Y = vector2.Y - this.Center.Y;
                vector.Z = vector2.Z - this.Center.Z;
                if (vector.LengthSquared () > num2) {
                    return ContainmentType.Intersects;
                }
            }
            return ContainmentType.Contains;
        }

        public ContainmentType Contains (ref Vector3 point)
        {
            Single temp;
            Vector3.DistanceSquared (ref point, ref this.Center, out temp);

            if (temp >= (this.Radius * this.Radius))
            {
                return ContainmentType.Disjoint;
            }
            return ContainmentType.Contains;
        }

        public ContainmentType Contains (ref BoundingSphere sphere)
        {
            Single num3;
            Vector3.Distance (ref this.Center, ref sphere.Center, out num3);
            Single radius = this.Radius;
            Single num = sphere.Radius;
            if ((radius + num) < num3) {
                return ContainmentType.Disjoint;
            }
            if ((radius - num) < num3) {
                return ContainmentType.Intersects;
            }
            return ContainmentType.Contains;
        }

        internal void SupportMapping (ref Vector3 v, out Vector3 result)
        {
            Single num2 = v.Length ();
            Single num = this.Radius / num2;
            result.X = this.Center.X + (v.X * num);
            result.Y = this.Center.Y + (v.Y * num);
            result.Z = this.Center.Z + (v.Z * num);
        }

        public BoundingSphere Transform (Matrix44 matrix)
        {
            BoundingSphere sphere = new BoundingSphere ();
            Vector3.Transform (ref this.Center, ref matrix, out sphere.Center);
            Single num4 = ((matrix.R0C0 * matrix.R0C0) + (matrix.R0C1 * matrix.R0C1)) + (matrix.R0C2 * matrix.R0C2);
            Single num3 = ((matrix.R1C0 * matrix.R1C0) + (matrix.R1C1 * matrix.R1C1)) + (matrix.R1C2 * matrix.R1C2);
            Single num2 = ((matrix.R2C0 * matrix.R2C0) + (matrix.R2C1 * matrix.R2C1)) + (matrix.R2C2 * matrix.R2C2);
            Single num = Maths.Max (num4, Maths.Max (num3, num2));
            sphere.Radius = this.Radius * (Maths.Sqrt (num));
            return sphere;
        }

        public void Transform (ref Matrix44 matrix, out BoundingSphere result)
        {
            Vector3.Transform (ref this.Center, ref matrix, out result.Center);
            Single num4 = ((matrix.R0C0 * matrix.R0C0) + (matrix.R0C1 * matrix.R0C1)) + (matrix.R0C2 * matrix.R0C2);
            Single num3 = ((matrix.R1C0 * matrix.R1C0) + (matrix.R1C1 * matrix.R1C1)) + (matrix.R1C2 * matrix.R1C2);
            Single num2 = ((matrix.R2C0 * matrix.R2C0) + (matrix.R2C1 * matrix.R2C1)) + (matrix.R2C2 * matrix.R2C2);
            Single num = Maths.Max (num4, Maths.Max (num3, num2));
            result.Radius = this.Radius * (Maths.Sqrt (num));
        }

        public static Boolean operator == (BoundingSphere a, BoundingSphere b)
        {
            return a.Equals (b);
        }

        public static Boolean operator != (BoundingSphere a, BoundingSphere b)
        {
            if (!(a.Center != b.Center)) {
                return !(a.Radius == b.Radius);
            }
            return true;
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    [StructLayout (LayoutKind.Sequential)]
    public struct Plane
        : IEquatable<Plane>
    {
        public Vector3 Normal;
        public Single D;

        public Plane (Single a, Single b, Single c, Single d)
        {
            this.Normal.X = a;
            this.Normal.Y = b;
            this.Normal.Z = c;
            this.D = d;
        }

        public Plane (Vector3 normal, Single d)
        {
            this.Normal = normal;
            this.D = d;
        }

        public Plane (Vector4 value)
        {
            this.Normal.X = value.X;
            this.Normal.Y = value.Y;
            this.Normal.Z = value.Z;
            this.D = value.W;
        }

        public Plane (Vector3 point1, Vector3 point2, Vector3 point3)
        {
            Single one = 1;

            Single num10 = point2.X - point1.X;
            Single num9 = point2.Y - point1.Y;
            Single num8 = point2.Z - point1.Z;
            Single num7 = point3.X - point1.X;
            Single num6 = point3.Y - point1.Y;
            Single num5 = point3.Z - point1.Z;
            Single num4 = (num9 * num5) - (num8 * num6);
            Single num3 = (num8 * num7) - (num10 * num5);
            Single num2 = (num10 * num6) - (num9 * num7);
            Single num11 = ((num4 * num4) + (num3 * num3)) + (num2 * num2);
            Single num = one / Maths.Sqrt (num11);
            this.Normal.X = num4 * num;
            this.Normal.Y = num3 * num;
            this.Normal.Z = num2 * num;
            this.D = -(((this.Normal.X * point1.X) + (this.Normal.Y * point1.Y)) + (this.Normal.Z * point1.Z));
        }

        public Boolean Equals (Plane other)
        {
            return ((((this.Normal.X == other.Normal.X) && (this.Normal.Y == other.Normal.Y)) && (this.Normal.Z == other.Normal.Z)) && (this.D == other.D));
        }

        public override Boolean Equals (Object obj)
        {
            Boolean flag = false;
            if (obj is Plane) {
                flag = this.Equals ((Plane)obj);
            }
            return flag;
        }

        public override Int32 GetHashCode ()
        {
            return (this.Normal.GetHashCode () + this.D.GetHashCode ());
        }

        public override String ToString ()
        {
            return string.Format ("{{Normal:{0} D:{1}}}", new Object[] { this.Normal.ToString (), this.D.ToString () });
        }

        public void Normalise ()
        {
            Single one = 1;
            Single somethingWicked; Maths.FromString("0.0000001192093", out somethingWicked);

            Single num2 = ((this.Normal.X * this.Normal.X) + (this.Normal.Y * this.Normal.Y)) + (this.Normal.Z * this.Normal.Z);
            if (Maths.Abs (num2 - one) >= somethingWicked) {
                Single num = one / Maths.Sqrt (num2);
                this.Normal.X *= num;
                this.Normal.Y *= num;
                this.Normal.Z *= num;
                this.D *= num;
            }
        }

        public static void Normalise (ref Plane value, out Plane result)
        {
            Single one = 1;
            Single somethingWicked; Maths.FromString("0.0000001192093", out somethingWicked);

            Single num2 = ((value.Normal.X * value.Normal.X) + (value.Normal.Y * value.Normal.Y)) + (value.Normal.Z * value.Normal.Z);
            if (Maths.Abs (num2 - one) < somethingWicked) {
                result.Normal = value.Normal;
                result.D = value.D;
            } else {
                Single num = one / Maths.Sqrt (num2);
                result.Normal.X = value.Normal.X * num;
                result.Normal.Y = value.Normal.Y * num;
                result.Normal.Z = value.Normal.Z * num;
                result.D = value.D * num;
            }
        }

        public static void Transform (ref Plane plane, ref Matrix44 matrix, out Plane result)
        {
            Matrix44 matrix2;
            Matrix44.Invert (ref matrix, out matrix2);
            Single x = plane.Normal.X;
            Single y = plane.Normal.Y;
            Single z = plane.Normal.Z;
            Single d = plane.D;
            result.Normal.X = (((x * matrix2.R0C0) + (y * matrix2.R0C1)) + (z * matrix2.R0C2)) + (d * matrix2.R0C3);
            result.Normal.Y = (((x * matrix2.R1C0) + (y * matrix2.R1C1)) + (z * matrix2.R1C2)) + (d * matrix2.R1C3);
            result.Normal.Z = (((x * matrix2.R2C0) + (y * matrix2.R2C1)) + (z * matrix2.R2C2)) + (d * matrix2.R2C3);
            result.D = (((x * matrix2.R3C0) + (y * matrix2.R3C1)) + (z * matrix2.R3C2)) + (d * matrix2.R3C3);
        }


        public static void Transform (ref Plane plane, ref Quaternion rotation, out Plane result)
        {
            Single one = 1;

            Single num15 = rotation.I + rotation.I;
            Single num5 = rotation.J + rotation.J;
            Single num = rotation.K + rotation.K;
            Single num14 = rotation.U * num15;
            Single num13 = rotation.U * num5;
            Single num12 = rotation.U * num;
            Single num11 = rotation.I * num15;
            Single num10 = rotation.I * num5;
            Single num9 = rotation.I * num;
            Single num8 = rotation.J * num5;
            Single num7 = rotation.J * num;
            Single num6 = rotation.K * num;
            Single num24 = (one - num8) - num6;
            Single num23 = num10 - num12;
            Single num22 = num9 + num13;
            Single num21 = num10 + num12;
            Single num20 = (one - num11) - num6;
            Single num19 = num7 - num14;
            Single num18 = num9 - num13;
            Single num17 = num7 + num14;
            Single num16 = (one - num11) - num8;
            Single x = plane.Normal.X;
            Single y = plane.Normal.Y;
            Single z = plane.Normal.Z;
            result.Normal.X = ((x * num24) + (y * num23)) + (z * num22);
            result.Normal.Y = ((x * num21) + (y * num20)) + (z * num19);
            result.Normal.Z = ((x * num18) + (y * num17)) + (z * num16);
            result.D = plane.D;
        }



        public Single Dot(ref Vector4 value)
        {
            return (((this.Normal.X * value.X) + (this.Normal.Y * value.Y)) + (this.Normal.Z * value.Z)) + (this.D * value.W);
        }

        public Single DotCoordinate (ref Vector3 value)
        {
            return (((this.Normal.X * value.X) + (this.Normal.Y * value.Y)) + (this.Normal.Z * value.Z)) + this.D;
        }

        public Single DotNormal (ref Vector3 value)
        {
            return ((this.Normal.X * value.X) + (this.Normal.Y * value.Y)) + (this.Normal.Z * value.Z);
        }

        public PlaneIntersectionType Intersects (ref BoundingBox box)
        {
            Single zero = 0;

            Vector3 vector;
            Vector3 vector2;
            vector2.X = (this.Normal.X >= zero) ? box.Min.X : box.Max.X;
            vector2.Y = (this.Normal.Y >= zero) ? box.Min.Y : box.Max.Y;
            vector2.Z = (this.Normal.Z >= zero) ? box.Min.Z : box.Max.Z;
            vector.X = (this.Normal.X >= zero) ? box.Max.X : box.Min.X;
            vector.Y = (this.Normal.Y >= zero) ? box.Max.Y : box.Min.Y;
            vector.Z = (this.Normal.Z >= zero) ? box.Max.Z : box.Min.Z;
            Single num = ((this.Normal.X * vector2.X) + (this.Normal.Y * vector2.Y)) + (this.Normal.Z * vector2.Z);
            if ((num + this.D) > zero) {
                return PlaneIntersectionType.Front;
            } else {
                num = ((this.Normal.X * vector.X) + (this.Normal.Y * vector.Y)) + (this.Normal.Z * vector.Z);
                if ((num + this.D) < zero) {
                    return PlaneIntersectionType.Back;
                } else {
                    return PlaneIntersectionType.Intersecting;
                }
            }
        }

        public PlaneIntersectionType Intersects (ref BoundingFrustum frustum)
        {
            if (null == frustum) {
                throw new ArgumentNullException ("frustum - NullNotAllowed");
            }
            return frustum.Intersects (ref this);
        }

        public PlaneIntersectionType Intersects (ref BoundingSphere sphere)
        {
            Single num2 = ((sphere.Center.X * this.Normal.X) + (sphere.Center.Y * this.Normal.Y)) + (sphere.Center.Z * this.Normal.Z);
            Single num = num2 + this.D;
            if (num > sphere.Radius) {
                return PlaneIntersectionType.Front;
            } else if (num < -sphere.Radius) {
                return PlaneIntersectionType.Back;
            } else {
                return PlaneIntersectionType.Intersecting;
            }
        }

        public static Boolean operator == (Plane lhs, Plane rhs)
        {
            return lhs.Equals (rhs);
        }

        public static Boolean operator != (Plane lhs, Plane rhs)
        {
            if (((lhs.Normal.X == rhs.Normal.X) && (lhs.Normal.Y == rhs.Normal.Y)) && (lhs.Normal.Z == rhs.Normal.Z)) {
                return !(lhs.D == rhs.D);
            }
            return true;
        }
    }



    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    [StructLayout (LayoutKind.Sequential)]
    public struct Ray
        : IEquatable<Ray>
    {
        // The starting position of this ray
        public Vector3 Position;

        // Normalised vector that defines the direction of this ray
        public Vector3 Direction;

        public Ray (Vector3 position, Vector3 direction)
        {
            this.Position = position;
            this.Direction = direction;
        }

        // Determines whether or not this ray is equal in value to another ray
        public Boolean Equals (Ray other)
        {
            // Check position
            if (this.Position.X != other.Position.X) return false;
            if (this.Position.Y != other.Position.Y) return false;
            if (this.Position.Z != other.Position.Z) return false;

            // Check direction
            if (this.Direction.X != other.Direction.X) return false;
            if (this.Direction.Y != other.Direction.Y) return false;
            if (this.Direction.Z != other.Direction.Z) return false;

            // They match!
            return true;
        }

        // Determines whether or not this ray is equal in value to another System.Object
        public override Boolean Equals (Object obj)
        {
            if (obj == null) return false;

            if (obj is Ray)
            {
                // Ok, it is a Ray, so just use the method above to compare.
                return this.Equals ((Ray) obj);
            }

            return false;
        }

        public override Int32 GetHashCode ()
        {
            return (this.Position.GetHashCode () + this.Direction.GetHashCode ());
        }

        public override String ToString ()
        {
            return string.Format ("{{Position:{0} Direction:{1}}}", this.Position, this.Direction);
        }

        // At what distance from it's starting position does this ray
        // intersect the given box.  Returns null if there is no
        // intersection.
        public Single? Intersects (ref BoundingBox box)
        {
            return box.Intersects (ref this);
        }

        // At what distance from it's starting position does this ray
        // intersect the given frustum.  Returns null if there is no
        // intersection.
        public Single? Intersects (ref BoundingFrustum frustum)
        {
            if (frustum == null)
            {
                throw new ArgumentNullException ();
            }

            return frustum.Intersects (ref this);
        }

        // At what distance from it's starting position does this ray
        // intersect the given plane.  Returns null if there is no
        // intersection.
        public Single? Intersects (ref Plane plane)
        {
            Single zero = 0;

            Single nearZero; Maths.FromString("0.00001", out nearZero);

            Single num2 = ((plane.Normal.X * this.Direction.X) + (plane.Normal.Y * this.Direction.Y)) + (plane.Normal.Z * this.Direction.Z);

            if (Maths.Abs (num2) < nearZero)
            {
                return null;
            }

            Single num3 = ((plane.Normal.X * this.Position.X) + (plane.Normal.Y * this.Position.Y)) + (plane.Normal.Z * this.Position.Z);

            Single num = (-plane.D - num3) / num2;

            if (num < zero)
            {
                if (num < -nearZero)
                {
                    return null;
                }

                num = zero;
            }

            return new Single? (num);
        }

        // At what distance from it's starting position does this ray
        // intersect the given sphere.  Returns null if there is no
        // intersection.
        public Single? Intersects (ref BoundingSphere sphere)
        {
            Single zero = 0;

            Single initialXOffset = sphere.Center.X - this.Position.X;

            Single initialYOffset = sphere.Center.Y - this.Position.Y;

            Single initialZOffset = sphere.Center.Z - this.Position.Z;

            Single num7 = ((initialXOffset * initialXOffset) + (initialYOffset * initialYOffset)) + (initialZOffset * initialZOffset);

            Single num2 = sphere.Radius * sphere.Radius;

            if (num7 <= num2)
            {
                return zero;
            }

            Single num = ((initialXOffset * this.Direction.X) + (initialYOffset * this.Direction.Y)) + (initialZOffset * this.Direction.Z);
            if (num < zero)
            {
                return null;
            }

            Single num6 = num7 - (num * num);
            if (num6 > num2)
            {
                return null;
            }

            Single num8 = Maths.Sqrt ((num2 - num6));

            return new Single? (num - num8);
        }

        public static Boolean operator == (Ray a, Ray b)
        {
            return a.Equals(b);
        }

        public static Boolean operator != (Ray a, Ray b)
        {
            return !a.Equals(b);
        }
    }



    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// todo
    /// </summary>
    internal class GjkDistance
    {
        /// <summary>
        /// todo
        /// </summary>
        internal GjkDistance ()
        {
            for (Int32 i = 0; i < 0x10; i++)
            {
                this.det [i] = new Single[4];
            }
        }

        /// <summary>
        /// todo
        /// </summary>
        internal Boolean AddSupportPoint (ref Vector3 newPoint)
        {
            Int32 index = (BitsToIndices [this.simplexBits ^ 15] & 7) - 1;

            this.y [index] = newPoint;
            Single newPointLs;
            Vector3.LengthSquared(ref newPoint, out newPointLs);
            this.yLengthSq [index] = newPointLs;

            for (Int32 i = BitsToIndices[this.simplexBits]; i != 0; i = i >> 3)
            {
                Int32 idx = (i & 7) - 1;

                Vector3 vector;
                vector.X = this.y [idx].X - newPoint.X;
                vector.Y = this.y [idx].Y - newPoint.Y;
                vector.Z = this.y [idx].Z - newPoint.Z;

                this.edges [idx] [index] = vector;
                this.edges [index] [idx].X = -vector.X;
                this.edges [index] [idx].Y = -vector.Y;
                this.edges [index] [idx].Z = -vector.Z;

                Single vectorLs;
                Vector3.LengthSquared (ref vector, out vectorLs);

                this.edgeLengthSq [index] [idx] = vectorLs;
                this.edgeLengthSq [idx] [index] = vectorLs;
            }

            this.UpdateDeterminant (index);

            return this.UpdateSimplex (index);
        }

        /// <summary>
        /// todo
        /// </summary>
        internal void Reset ()
        {
            Single zero = 0;

            this.simplexBits = 0;
            this.maxLengthSq = zero;
        }

        /// <summary>
        /// todo
        /// </summary>
        internal Vector3 ClosestPoint
        {
            get { return this.closestPoint; }
        }

        /// <summary>
        /// todo
        /// </summary>
        internal Boolean FullSimplex
        {
            get { return (this.simplexBits == 15); }
        }

        /// <summary>
        /// todo
        /// </summary>
        internal Single MaxLengthSquared
        {
            get { return this.maxLengthSq; }
        }

        /// <summary>
        /// todo
        /// </summary>
        Vector3 closestPoint;

        /// <summary>
        /// todo
        /// </summary>
        Single[][] det = new Single[0x10][];

        /// <summary>
        /// todo
        /// </summary>
        Single[][] edgeLengthSq =
            new Single[][]
            {
                new Single[4],
                new Single[4],
                new Single[4],
                new Single[4]
            };

        /// <summary>
        /// todo
        /// </summary>
        Vector3[][] edges =
            new Vector3[][]
            {
                new Vector3[4],
                new Vector3[4],
                new Vector3[4],
                new Vector3[4]
            };

        /// <summary>
        /// todo
        /// </summary>
        Single maxLengthSq;

        /// <summary>
        /// todo
        /// </summary>
        Int32 simplexBits;

        /// <summary>
        /// todo
        /// </summary>
        Vector3[] y = new Vector3[4];

        /// <summary>
        /// todo
        /// </summary>
        Single[] yLengthSq = new Single[4];

        /// <summary>
        /// todo
        /// </summary>
        static Int32[] BitsToIndices =
            new Int32[]
            {
                0, 1, 2, 0x11, 3, 0x19, 0x1a, 0xd1,
                4, 0x21, 0x22, 0x111, 0x23, 0x119, 0x11a, 0x8d1
            };

        /// <summary>
        /// todo
        /// </summary>
        Vector3 ComputeClosestPoint ()
        {
            Single fzero; Maths.Zero(out fzero);

            Single num3 = fzero;
            Vector3 zero = Vector3.Zero;

            this.maxLengthSq = fzero;

            for (Int32 i = BitsToIndices[this.simplexBits]; i != 0; i = i >> 3)
            {
                Int32 index = (i & 7) - 1;
                Single num4 = this.det [this.simplexBits] [index];

                num3 += num4;
                zero += (Vector3)(this.y [index] * num4);

                this.maxLengthSq =
                Maths.Max (this.maxLengthSq, this.yLengthSq [index]);
            }

            return (Vector3)(zero / num3);
        }

        /// <summary>
        /// todo
        /// </summary>
        Boolean IsSatisfiesRule (Int32 xBits, Int32 yBits)
        {
            Single fzero; Maths.Zero(out fzero);

            for (Int32 i = BitsToIndices[yBits]; i != 0; i = i >> 3)
            {
                Int32 index = (i & 7) - 1;
                Int32 num3 = ((Int32)1) << index;

                if ((num3 & xBits) != 0)
                {
                    if (this.det [xBits] [index] <= fzero)
                    {
                        return false;
                    }
                }
                else if (this.det [xBits | num3] [index] > fzero)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// todo
        /// </summary>
        void UpdateDeterminant (Int32 xmIdx)
        {
            Single fone; Maths.One(out fone);
            Int32 index = ((Int32)1) << xmIdx;

            this.det [index] [xmIdx] = fone;

            Int32 num14 = BitsToIndices [this.simplexBits];
            Int32 num8 = num14;

            for (Int32 i = 0; num8 != 0; i++)
            {
                Int32 num = (num8 & 7) - 1;
                Int32 num12 = ((int)1) << num;
                Int32 num6 = num12 | index;

                this.det [num6] [num] =
                    Dot (ref this.edges [xmIdx] [num], ref this.y [xmIdx]);

                this.det [num6] [xmIdx] =
                    Dot (ref this.edges [num] [xmIdx], ref this.y [num]);

                Int32 num11 = num14;

                for (Int32 j = 0; j < i; j++)
                {
                    int num3 = (num11 & 7) - 1;
                    int num5 = ((int)1) << num3;
                    int num9 = num6 | num5;
                    int num4 = (this.edgeLengthSq [num] [num3] < this.edgeLengthSq [xmIdx] [num3]) ? num : xmIdx;

                    this.det [num9] [num3] =
                        (this.det [num6] [num] * Dot (ref this.edges [num4] [num3], ref this.y [num])) +
                        (this.det [num6] [xmIdx] * Dot (ref this.edges [num4] [num3], ref this.y [xmIdx]));

                    num4 = (this.edgeLengthSq [num3] [num] < this.edgeLengthSq [xmIdx] [num]) ? num3 : xmIdx;

                    this.det [num9] [num] =
                        (this.det [num5 | index] [num3] * Dot (ref this.edges [num4] [num], ref this.y [num3])) +
                        (this.det [num5 | index] [xmIdx] * Dot (ref this.edges [num4] [num], ref this.y [xmIdx]));

                    num4 = (this.edgeLengthSq [num] [xmIdx] < this.edgeLengthSq [num3] [xmIdx]) ? num : num3;

                    this.det [num9] [xmIdx] =
                        (this.det [num12 | num5] [num3] * Dot (ref this.edges [num4] [xmIdx], ref this.y [num3])) +
                        (this.det [num12 | num5] [num] * Dot (ref this.edges [num4] [xmIdx], ref this.y [num]));

                    num11 = num11 >> 3;
                }

                num8 = num8 >> 3;
            }

            if ((this.simplexBits | index) == 15)
            {
                int num2 =
                    (this.edgeLengthSq [1] [0] < this.edgeLengthSq [2] [0]) ?
                    ((this.edgeLengthSq [1] [0] < this.edgeLengthSq [3] [0]) ? 1 : 3) :
                    ((this.edgeLengthSq [2] [0] < this.edgeLengthSq [3] [0]) ? 2 : 3);

                this.det [15] [0] =
                    ((this.det [14] [1] * Dot (ref this.edges [num2] [0], ref this.y [1])) +
                    (this.det [14] [2] * Dot (ref this.edges [num2] [0], ref this.y [2]))) +
                    (this.det [14] [3] * Dot (ref this.edges [num2] [0], ref this.y [3]));

                num2 =
                    (this.edgeLengthSq [0] [1] < this.edgeLengthSq [2] [1]) ?
                    ((this.edgeLengthSq [0] [1] < this.edgeLengthSq [3] [1]) ? 0 : 3) :
                    ((this.edgeLengthSq [2] [1] < this.edgeLengthSq [3] [1]) ? 2 : 3);

                this.det [15] [1] =
                    ((this.det [13] [0] * Dot (ref this.edges [num2] [1], ref this.y [0])) +
                    (this.det [13] [2] * Dot (ref this.edges [num2] [1], ref this.y [2]))) +
                    (this.det [13] [3] * Dot (ref this.edges [num2] [1], ref this.y [3]));

                num2 =
                    (this.edgeLengthSq [0] [2] < this.edgeLengthSq [1] [2]) ?
                    ((this.edgeLengthSq [0] [2] < this.edgeLengthSq [3] [2]) ? 0 : 3) :
                    ((this.edgeLengthSq [1] [2] < this.edgeLengthSq [3] [2]) ? 1 : 3);

                this.det [15] [2] =
                    ((this.det [11] [0] * Dot (ref this.edges [num2] [2], ref this.y [0])) +
                    (this.det [11] [1] * Dot (ref this.edges [num2] [2], ref this.y [1]))) +
                    (this.det [11] [3] * Dot (ref this.edges [num2] [2], ref this.y [3]));

                num2 =
                    (this.edgeLengthSq [0] [3] < this.edgeLengthSq [1] [3]) ?
                    ((this.edgeLengthSq [0] [3] < this.edgeLengthSq [2] [3]) ? 0 : 2) :
                    ((this.edgeLengthSq [1] [3] < this.edgeLengthSq [2] [3]) ? 1 : 2);

                this.det [15] [3] =
                    ((this.det [7] [0] * Dot (ref this.edges [num2] [3], ref this.y [0])) +
                    (this.det [7] [1] * Dot (ref this.edges [num2] [3], ref this.y [1]))) +
                    (this.det [7] [2] * Dot (ref this.edges [num2] [3], ref this.y [2]));
            }
        }

        /// <summary>
        /// todo
        /// </summary>
        Boolean UpdateSimplex (Int32 newIndex)
        {
            Int32 yBits = this.simplexBits | (((Int32)1) << newIndex);

            Int32 xBits = ((Int32)1) << newIndex;

            for (Int32 i = this.simplexBits; i != 0; i--)
            {
                if (((i & yBits) == i) && this.IsSatisfiesRule (i | xBits, yBits))
                {
                    this.simplexBits = i | xBits;
                    this.closestPoint = this.ComputeClosestPoint ();

                    return true;
                }
            }

            Boolean flag = false;

            if (this.IsSatisfiesRule (xBits, yBits))
            {
                this.simplexBits = xBits;
                this.closestPoint = this.y [newIndex];
                this.maxLengthSq = this.yLengthSq [newIndex];

                flag = true;
            }

            return flag;
        }

        /// <summary>
        /// todo
        /// </summary>
        static Single Dot (ref Vector3 a, ref Vector3 b)
        {
            return (((a.X * b.X) + (a.Y * b.Y)) + (a.Z * b.Z));
        }
    }
}
