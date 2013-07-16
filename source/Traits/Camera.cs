// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ Blimey - Fast, efficient, high level engine built upon Cor & Abacus    │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Brought to you by:                                                     │ \\
// │          _________                    .__               __             │ \\
// │         /   _____/__ __  ____    ____ |__|____    _____/  |_           │ \\
// │         \_____  \|  |  \/    \  / ___\|  \__  \  /    \   __\          │ \\
// │         /        \  |  /   |  \/ /_/  >  |/ __ \|   |  \  |            │ \\
// │        /_______  /____/|___|  /\___  /|__(____  /___|  /__|            │ \\
// │                \/           \//_____/         \/     \/                │ \\
// │                                                                        │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Copyright © 2013 A.J.Pook (http://sungiant.github.com)                 │ \\
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

using System;
using Sungiant.Abacus;
using Sungiant.Abacus.Packed;
using Sungiant.Abacus.SinglePrecision;
using Sungiant.Cor;

namespace Sungiant.Blimey
{

	public sealed class Camera
		: Trait
	{
		public CameraProjectionType Projection = CameraProjectionType.Perspective;

		// perspective settings
		public Single FieldOfView = RealMaths.ToRadians(45.0f);

		// orthographic settings
		public Single size = 100f;

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

			var camLook = this.Parent.Transform.Position + (100f * this.Parent.Transform.Forward);
			Vector3.Normalise(ref camLook, out camLook);

			if (this.Parent.Owner.Cor.System.CurrentOrientation == DeviceOrientation.Rightside)
			{
				Vector3.Cross(ref camLook, ref camUp, out camUp);
			}

			if (this.Parent.Owner.Cor.System.CurrentOrientation == DeviceOrientation.Leftside)
			{
				Vector3.Cross(ref camUp, ref camLook, out camUp);
			}

			if (this.Parent.Owner.Cor.System.CurrentOrientation == DeviceOrientation.Upsidedown)
			{
				camUp = -camUp;
			}

			Vector3 pos = this.Parent.Transform.Position;
			Matrix44.CreateLookAt(
				ref pos,
				ref camLook,
				ref camUp,
				out _view);

			Single width = (Single) this.Cor.System.CurrentDisplaySize.X;
			Single height = (Single) this.Cor.System.CurrentDisplaySize.Y;

			if (Projection == CameraProjectionType.Orthographic)
			{
                if(TempWORKOUTANICERWAY)
                {
                    Matrix44.CreateOrthographic(
                        width / Sprite.cSpriteSpaceScale, 
                        height / Sprite.cSpriteSpaceScale, 
                        1, -1, out _projection);
                }
                else
                {
				    Matrix44.CreateOrthographicOffCenter(
                        -0.5f, 0.5f, -0.5f, 0.5f, 0.5f * size, -0.5f * size, out _projection);
                }
			} 
			else
			{
				Matrix44.CreatePerspectiveFieldOfView (
					FieldOfView,
					width / height, // aspect ratio
					NearPlaneDistance,
					FarPlaneDistance,
					out _projection);
			}
		}
	}
}

