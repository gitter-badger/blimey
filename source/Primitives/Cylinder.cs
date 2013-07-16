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
using Debug = System.Diagnostics.Debug;

namespace Sungiant.Blimey
{
	public class CylinderPrimitive
		: GeometricPrimitive
	{
		const int _tessellation = 32;
		const float _height = 0.5f;
		const float _radius = 0.5f;

		public CylinderPrimitive (IGraphicsManager graphicsDevice)
		{
			Debug.Assert (_tessellation >= 3);

			// Create a ring of triangles around the outside of the cylinder.
			for (int i = 0; i <= _tessellation; i++) {
				Vector3 normal = GetCircleVector (i, _tessellation);

				Vector3 topPos = normal * _radius + Vector3.Up * _height;
				Vector3 botPos = normal * _radius + Vector3.Down * _height;

				AddVertex (topPos, normal);
				AddVertex (botPos, normal);
			}

			for (int i = 0; i < _tessellation; i++) {
				AddIndex (i * 2);
				AddIndex (i * 2 + 1);
				AddIndex ((i * 2 + 2));

				AddIndex (i * 2 + 1);
				AddIndex (i * 2 + 3);
				AddIndex (i * 2 + 2);
			}


			// Create flat triangle fan caps to seal the top and bottom.
			CreateCap (_tessellation, _height, _radius, Vector3.Up);
			CreateCap (_tessellation, _height, _radius, Vector3.Down);

			InitializePrimitive (graphicsDevice);
		}

		/// <summary>
		/// Helper method creates a triangle fan to close the ends of the cylinder.
		/// </summary>
		void CreateCap (int tessellation, float height, float radius, Vector3 normal)
		{
			// Create cap indices.
			for (int i = 0; i < tessellation - 2; i++) {
				if (normal.Y > 0) {
					AddIndex (CurrentVertex);
					AddIndex (CurrentVertex + (i + 1) % tessellation);
					AddIndex (CurrentVertex + (i + 2) % tessellation);
				} else {
					AddIndex (CurrentVertex);
					AddIndex (CurrentVertex + (i + 2) % tessellation);
					AddIndex (CurrentVertex + (i + 1) % tessellation);
				}
			}

			// Create cap vertices.
			for (int i = 0; i < tessellation; i++) {
				Vector3 circleVec = GetCircleVector (i, tessellation);
				Vector3 position = circleVec * radius +
								   normal * height;

				AddVertex (position, normal);
			}
		}


		/// <summary>
		/// Helper method computes a point on a circle.
		/// </summary>
		static Vector3 GetCircleVector (int i, int tessellation)
		{
			float tau; RealMaths.Tau(out tau);
			float angle = i * tau / tessellation;

			float dx = (float)Math.Cos (angle);
			float dz = (float)Math.Sin (angle);

			return new Vector3 (dx, 0, dz);
		}

	}
}
