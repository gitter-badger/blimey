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
}
