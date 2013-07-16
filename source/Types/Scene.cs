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
using Sungiant.Cor;

namespace Sungiant.Blimey
{
	// THe user creates game states and uses them to interact with the engine
	public abstract class Scene
	{
        public virtual SceneSettings Settings
        { 
            get
            {
                return SceneSettings.Default;
            }
        }

		internal void RegisterDrawCall() { drawCalls++; }
		int drawCalls = 0;

		internal void RegisterTriangles(int count) { triCount += count; }
		int triCount = 0;


		BlimeyContext blimey;

		public ICor Cor { get; set;}

		public IBlimey Blimey { get { return blimey; } }

		public Boolean Active { get { return _active; } }

		Boolean _active;
		List<SceneObject> _gameObjects = new List<SceneObject> ();

		public List<SceneObject> SceneObjects { get { return _gameObjects; } }

		CameraManager cameraManager;

		public SceneObject CreateSceneObject (string zName)
		{
			var go = new SceneObject (this, zName);
			_gameObjects.Add (go);
			return go;
		}

		public void DestroySceneObject (SceneObject zGo)
		{
			zGo.Shutdown ();
			foreach (SceneObject go in zGo.Children) {
				this.DestroySceneObject (go);
				_gameObjects.Remove (go);
			}
			_gameObjects.Remove (zGo);

			zGo = null;
		}

		public abstract void Start ();

		public abstract Scene Update(AppTime time);

		public abstract void Shutdown ();

		public void Initialize(ICor cor)
		{
			this.Cor = cor;
			this.blimey = new BlimeyContext(this.Cor, this.Settings); ;

			cameraManager = new CameraManager(this);
			_active = true;

			this.Start();
		}

		internal Scene RunUpdate(AppTime time)
		{
			drawCalls = 0;
			triCount = 0;

			this.blimey.PreUpdate(time);
			

			foreach (SceneObject go in _gameObjects) {
				go.Update(time);
			}

			

			var ret =  this.Update(time);

			this.blimey.PostUpdate(time);


			return ret;
		}


		
		internal CameraManager CameraManager { get { return cameraManager; } }

		public void SetRenderPassCameraToDefault(string renderPass)
		{
			cameraManager.SetDefaultCamera (renderPass);
		}
		
		public void SetRenderPassCameraTo (string renderPass, SceneObject go)
		{
			cameraManager.SetMainCamera(renderPass, go);
		}

		public SceneObject GetRenderPassCamera(string renderPass)
		{
			return cameraManager.GetActiveCamera(renderPass).Parent;
		}
		public virtual void Uninitilise ()
		{
			this.Shutdown ();

			_active = false;

			List<SceneObject> onesToDestroy = new List<SceneObject> ();

			foreach (SceneObject go in _gameObjects) {
				if (go.Transform.Parent == null)
					onesToDestroy.Add (go);
			}

			foreach (SceneObject go in onesToDestroy) {
				DestroySceneObject (go);
			}

			System.Diagnostics.Debug.Assert(_gameObjects.Count == 0);

			this.blimey = null;
			this.cameraManager = null;
		}
	}



}
