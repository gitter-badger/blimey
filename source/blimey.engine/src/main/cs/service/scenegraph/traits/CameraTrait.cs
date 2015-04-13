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
    using Fudge;
    using global::Blimey.Platform;
    using global::Blimey.Asset;
    using Abacus.SinglePrecision;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class CameraTrait
        : Trait
    {
        public CameraProjectionType Projection = CameraProjectionType.Perspective;

        // perspective settings
        public Single FieldOfView = Maths.ToRadians(45.0f);

        // orthographic settings
        public Single ortho_depth = 100f;
        public Single ortho_width = 1f;
        public Single ortho_height = 1f;
        public Single ortho_zoom = 1f;

        // clipping planes
        public Single NearPlaneDistance = 1.0f;
        public Single FarPlaneDistance = 10000.0f;

        public Matrix44 ViewMatrix44 { get { return _view; } }

        public Matrix44 ProjectionMatrix44 { get { return _projection; } }

        Matrix44 _projection;
        Matrix44 _view;

        // return this cameras bounding frustum
        //public BoundingFrustum BoundingFrustum { get { return new BoundingFrustum (ViewMatrix44 * ProjectionMatrix44); } }

        public bool TempWORKOUTANICERWAY = false;

        // Allows the game component to update itself.
        public override void OnUpdate (AppTime time)
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

            Single width = (Single) this.Platform.Status.Width;
            Single height = (Single)this.Platform.Status.Height;

            if (Projection == CameraProjectionType.Orthographic)
            {
                if(TempWORKOUTANICERWAY)
                {
                    _projection =
                        Matrix44.CreateOrthographic(
                            width / SpriteTrait.SpriteConfiguration.Default.SpriteSpaceScale,
                            height / SpriteTrait.SpriteConfiguration.Default.SpriteSpaceScale,
                            1, -1);
                }
                else
                {
                    _projection =
                        Matrix44.CreateOrthographicOffCenter(
              -0.5f * ortho_width * ortho_zoom,  +0.5f * ortho_width * ortho_zoom,
              -0.5f * ortho_height * ortho_zoom, +0.5f * ortho_height * ortho_zoom,
              +0.5f * ortho_depth,  -0.5f * ortho_depth);
                }
            }
            else
            {
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
