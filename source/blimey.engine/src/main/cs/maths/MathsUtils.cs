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
    using System.Linq;
    using Fudge;
    using Abacus.SinglePrecision;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public static class MathsUtils
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

    }}
