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
	//
	// MESH RENDERER
	//
	// This behaviour takes a Sungiant.Blimey.Model and a Material, it then renders the models
	// at location, scale and orientaion of the parent SceneObject's Transform.
	//
	public sealed class MeshRenderer
		: Trait
	{

		public Mesh Mesh;
		public Material Material;

		internal override void Render (IGraphicsManager zGfx, Matrix44 zView, Matrix44 zProjection)
		{
			if (!Active)
				return;

			zGfx.GpuUtils.BeginEvent(Rgba32.Red, "MeshRenderer.Render");

			// Set our vertex declaration, vertex buffer, and index buffer.
			zGfx.SetActiveGeometryBuffer(Mesh.GeomBuffer);


			// Get the material's shader and apply all of the settings
			// it needs.
			Material.CalibrateShader (
				this.Parent.Transform.Location,
				zView,
				zProjection
				);

			Material.CalibrateGpu (zGfx);

			var shader = Material.GetShader ();

			if( shader != null)
			{
				foreach (var effectPass in shader.Passes)
				{
					effectPass.Activate (Mesh.GeomBuffer.VertexBuffer.VertexDeclaration);

					zGfx.DrawIndexedPrimitives (
						PrimitiveType.TriangleList, 0, 0,
						Mesh.VertexCount, 0, Mesh.TriangleCount);
				}
			}

			zGfx.GpuUtils.EndEvent();

		}
	}
}
