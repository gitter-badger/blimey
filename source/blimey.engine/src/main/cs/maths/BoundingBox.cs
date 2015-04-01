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
}