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
}