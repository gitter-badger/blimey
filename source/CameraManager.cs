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
using System.Collections.Generic;
using Sungiant.Abacus;
using Sungiant.Abacus.Packed;
using Sungiant.Abacus.SinglePrecision;

namespace Sungiant.Blimey
{
	internal class CameraManager
	{
		internal Camera GetActiveCamera(String RenderPass)
		{ 
			return _activeCameras[RenderPass].GetTrait<Camera> (); 
		}

		Dictionary<String, SceneObject> _defaultCameras = new Dictionary<String,SceneObject>();
		Dictionary<String, SceneObject> _activeCameras = new Dictionary<String,SceneObject>();

		internal void SetDefaultCamera(String RenderPass)
		{
			_activeCameras[RenderPass] = _defaultCameras[RenderPass];
		}
		
		internal void SetMainCamera (String RenderPass, SceneObject go)
		{
			_activeCameras[RenderPass] = go;
		}

		internal CameraManager (Scene scene)
		{
			var settings = scene.Settings;

			foreach (String renderPass in settings.RenderPasses)
			{

				var renderPassSettings = settings.GetRenderPassSettings(renderPass);

				var go = scene.CreateSceneObject(renderPass + " Default Camera");

				

				var cam = go.AddTrait<Camera>();

				if (renderPassSettings.CameraProjectionType == CameraProjectionType.Perspective)
				{
					go.Transform.Position = new Vector3(2, 1, 5);

					var orbit = go.AddTrait<OrbitAroundSubject>();
					orbit.CameraSubject = Transform.Origin;

					var lookAtSub = go.AddTrait<LookAtSubject>();
					lookAtSub.Subject = Transform.Origin;
				}
				else
				{
					cam.Projection = CameraProjectionType.Orthographic;

					go.Transform.Position = new Vector3(0, 0, 0.5f);
					go.Transform.LookAt(Vector3.Zero);
				}

			
				_defaultCameras.Add(renderPass, go);
				_activeCameras.Add(renderPass, go);
			}
		}


	}
}