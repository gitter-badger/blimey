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

using System.Collections.Generic;
using Sungiant.Cor;

namespace Sungiant.Blimey
{
	internal class SceneRenderManager
	{
		ICor Castle { get; set; }

		internal SceneRenderManager(ICor cor)
		{
			this.Castle = cor;
		}

		internal void Render(Scene scene)
		{
			var sceneSettings = scene.Settings;

			// Clear the background colour if the scene settings want us to.
			if (sceneSettings.StartByClearingBackBuffer)
			{
				this.Castle.Graphics.ClearColourBuffer(sceneSettings.BackgroundColour);
			}

			foreach (string renderPass in sceneSettings.RenderPasses)
			{
				this.RenderPass(scene, renderPass);
			}
		}

		List<MeshRenderer> GetMeshRenderersWithMaterials(Scene scene, string pass)
		{
			var list = new List<MeshRenderer>();

			foreach (var go in scene.SceneObjects)
			{
				var mr = go.GetTrait<MeshRenderer>();

				if (mr == null)
				{
					continue;
				}

                if (mr.Material == null)
                {
                    continue;
                }

				// if the material is for this pass
				if (mr.Material.RenderPass == pass)
				{
					list.Add(mr);
				}
			}

			return list;
		}

		void RenderPass(Scene scene, string pass)
		{
			// init pass
			var passSettings = scene.Settings.GetRenderPassSettings(pass);

			var gfxManager = this.Castle.Graphics;

			if (passSettings.ClearDepthBuffer)
			{
				gfxManager.ClearDepthBuffer();
			}

			var cam = scene.CameraManager.GetActiveCamera(pass);

			var meshRenderers = this.GetMeshRenderersWithMaterials(scene, pass);

			// TODO: big one
			// we really need to group the mesh renderers by material
			// and only make a new draw call when there are changes.

			foreach (var mr in meshRenderers)
			{
				mr.Render(gfxManager, cam.ViewMatrix44, cam.ProjectionMatrix44);
			}

			scene.Blimey.DebugShapeRenderer.Render(
				gfxManager, pass, cam.ViewMatrix44, cam.ProjectionMatrix44);

		}
	}
}
