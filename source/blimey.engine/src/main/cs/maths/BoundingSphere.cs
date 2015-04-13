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

namespace Blimey.Engine
{
    using System;
    using System.Runtime.InteropServices;
    using System.Globalization;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;

    using Fudge;
    using global::Blimey.Platform;
    using global::Blimey.Asset;
    using Abacus.SinglePrecision;

    using System.Linq;

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

            CreateFromPoints (frustum.platformnerArray, out sphere);
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
            foreach (Vector3 vector2 in frustum.platformnerArray) {
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
}
