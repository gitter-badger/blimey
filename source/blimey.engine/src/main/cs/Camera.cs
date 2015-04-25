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
    using System.Linq;
    using Fudge;
    using global::Blimey.Platform;
    using global::Blimey.Asset;
    using Abacus.SinglePrecision;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class OrthographicCamera: Camera
    {
        public Single Depth;
        public Single Width;
        public Single Height;
        public Single Zoom;

        public OrthographicCamera ()
            : base (Camera.ProjectionType.Orthographic)
        {
            this.Depth = 100f;
            this.Width = 1f;
            this.Height = 1f;
            this.Zoom = 1f;
        }

        public override Matrix44 Projection
        {
            get
            {
                bool TempWORKOUTANICERWAY = false;
                if(TempWORKOUTANICERWAY)
                {
                    throw new NotImplementedException ();
                    /*
                    Single width = (Single) this.Platform.Status.Width;
                    Single height = (Single)this.Platform.Status.Height;
                    _projection =
                        Matrix44.CreateOrthographic(
                            width / SpriteTrait.SpriteConfiguration.Default.SpriteSpaceScale,
                            height / SpriteTrait.SpriteConfiguration.Default.SpriteSpaceScale,
                            1, -1);
                    */
                }
                else
                {
                    return Matrix44.CreateOrthographicOffCenter(
                          -0.5f * Width * Zoom,  +0.5f * Width * Zoom,
                          -0.5f * Height * Zoom, +0.5f * Height * Zoom,
                          +0.5f * Depth,  -0.5f * Depth);
                }
            }
        }
    }

    public class PerspectiveCamera: Camera
    {
        public Single FieldOfView { get; private set; }
        public Single NearPlaneDistance { get; private set; }
        public Single FarPlaneDistance { get; private set; }

        public PerspectiveCamera ()
            : base (Camera.ProjectionType.Perspective)
        {
            this.FieldOfView = Maths.ToRadians(45.0f);
            this.NearPlaneDistance = NearPlaneDistance = 1.0f;
            this.FarPlaneDistance = FarPlaneDistance = 10000.0f;
        }

        public override Matrix44 Projection
        {
            get
            {
                var camUp = this.Parent.Transform.Up;

                var camLook = this.Parent.Transform.Forward;

                Vector3 pos = this.Parent.Transform.Position;
                Vector3 target = pos + (camLook * FarPlaneDistance);

                Matrix44.CreateLookAt(
                    ref pos,
                    ref target,
                    ref camUp,
                    out _view);

                return _view;
            }
        }
    }

    public abstract class Camera
    {
        public enum ProjectionType
        {
            Perspective,
            Orthographic,
        }

        public Vector3 Position { get; set; }
        public Quaternion Orientation { get; set; }

        public BoundingFrustum BoundingFrustum { get { throw new NotImplementedException (); } }

        readonly ProjectionType projectionType;

        public Camera (ProjectionType projectionType)
        {
            this.projectionType = projectionType;
        }

        public ProjectionType ProjType { get { return projectionType; } }

        // clipping planes

        public abstract Matrix44 Projection { get; }

        public Matrix44 View
        {
            get
            {
                Single width = (Single) this.Platform.Status.Width;
                Single height = (Single)this.Platform.Status.Height;

                _projection =
                    Matrix44.CreatePerspectiveFieldOfView (
                        FieldOfView,
                        width / height, // aspect ratio
                        NearPlaneDistance,
                        FarPlaneDistance);
            }
        }
    }
}
