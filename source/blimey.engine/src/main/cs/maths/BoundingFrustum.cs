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

    public class BoundingFrustum
        : IEquatable<BoundingFrustum>
    {
        const int BottomPlaneIndex = 5;

        internal Vector3[] cornerArray;

        public const int CornerCount = 8;

        const int FarPlaneIndex = 1;

        GJKDistance gjk;

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
                this.gjk = new GJKDistance ();
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
                this.gjk = new GJKDistance ();
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
                this.gjk = new GJKDistance ();
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
}