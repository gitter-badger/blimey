// ┌────────────────────────────────────────────────────────────────────────────────────────────────────────────────┐ \\
// │ Blimey - Fast, efficient, high level engine built upon Cor & Abacus                                            │ \\
// ├────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤ \\
// │                     Brought to you by:                                                                         │ \\
// │                              _________                    .__               __                                 │ \\
// │                             /   _____/__ __  ____    ____ |__|____    _____/  |_                               │ \\
// │                             \_____  \|  |  \/    \  / ___\|  \__  \  /    \   __\                              │ \\
// │                             /        \  |  /   |  \/ /_/  >  |/ __ \|   |  \  |                                │ \\
// │                            /_______  /____/|___|  /\___  /|__(____  /___|  /__|                                │ \\
// │                                    \/           \//_____/         \/     \/                                    │ \\
// │                                                                                                                │ \\
// ├────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤ \\
// │ Copyright © 2008-2014 A.J.Pook (http://ajpook.github.io)                                                       │ \\
// ├────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤ \\
// │ Authors: A.J.Pook                                                                                              │ \\
// ├────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤ \\
// │ Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated   │ \\
// │ documentation files (the "Software"), to deal in the Software without restriction, including without           │ \\
// │ limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sellcopies of the   │ \\
// │ Software, and to permit persons to whom the Software is furnished to do so, subject to the following           │ \\
// │ conditions:                                                                                                    │ \\
// │                                                                                                                │ \\
// │ The above copyright notice and this permission notice shall be included in all copies or substantial portions  │ \\
// │ of the Software.                                                                                               │ \\
// │                                                                                                                │ \\
// │ THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED  │ \\
// │ TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.  IN NO EVENT SHALL │ \\
// │ THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF  │ \\
// │ CONTRACT, TORT OR OTHERWISE, ARISING FROM,OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER        │ \\
// │ DEALINGS IN THE SOFTWARE.                                                                                      │ \\
// └────────────────────────────────────────────────────────────────────────────────────────────────────────────────┘ \\

namespace Blimey
{
    using System;
    using System.Runtime.InteropServices;
    using System.Globalization;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Abacus;
    using Fudge;
    using Abacus.SinglePrecision;
    using System.Linq;
    using Cor;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public enum Space
    {
        World,
        Self
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
                mat.R0C3, mat.R1C3, mat.R2C3, mat.R3C3) +

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
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public struct BoundingBox
        : IEquatable<BoundingBox>
    {
        public const int CornerCount = 8;

        public Vector3 Min;
        public Vector3 Max;

        public Vector3[] GetCorners ()
        {
            return new Vector3[]
            {
                new Vector3 (this.Min.X, this.Max.Y, this.Max.Z),
                new Vector3 (this.Max.X, this.Max.Y, this.Max.Z),
                new Vector3 (this.Max.X, this.Min.Y, this.Max.Z),
                new Vector3 (this.Min.X, this.Min.Y, this.Max.Z),
                new Vector3 (this.Min.X, this.Max.Y, this.Min.Z),
                new Vector3 (this.Max.X, this.Max.Y, this.Min.Z),
                new Vector3 (this.Max.X, this.Min.Y, this.Min.Z),
                new Vector3 (this.Min.X, this.Min.Y, this.Min.Z)
            };
        }

        public void GetCorners (Vector3[] corners)
        {
            if (corners == null) {
                throw new ArgumentNullException ("corners");
            }
            if (corners.Length < 8) {
                throw new ArgumentOutOfRangeException ("NotEnoughCorners");
            }
            corners [0].X = this.Min.X;
            corners [0].Y = this.Max.Y;
            corners [0].Z = this.Max.Z;

            corners [1].X = this.Max.X;
            corners [1].Y = this.Max.Y;
            corners [1].Z = this.Max.Z;

            corners [2].X = this.Max.X;
            corners [2].Y = this.Min.Y;
            corners [2].Z = this.Max.Z;

            corners [3].X = this.Min.X;
            corners [3].Y = this.Min.Y;
            corners [3].Z = this.Max.Z;

            corners [4].X = this.Min.X;
            corners [4].Y = this.Max.Y;
            corners [4].Z = this.Min.Z;

            corners [5].X = this.Max.X;
            corners [5].Y = this.Max.Y;
            corners [5].Z = this.Min.Z;

            corners [6].X = this.Max.X;
            corners [6].Y = this.Min.Y;
            corners [6].Z = this.Min.Z;

            corners [7].X = this.Min.X;
            corners [7].Y = this.Min.Y;
            corners [7].Z = this.Min.Z;
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

        public override Boolean Equals (object obj)
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
            return string.Format ("{{Min:{0} Max:{1}}}", new object[] { this.Min.ToString (), this.Max.ToString () });
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
}
