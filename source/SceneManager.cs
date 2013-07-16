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
using Sungiant.Cor;

namespace Sungiant.Blimey
{
	internal class SceneManager
	{
		Scene activeScene;
		ICor cor;

		SceneRenderManager renderManager;

		public event System.EventHandler SimulationStateChanged;

		public Scene ActiveState { get { return activeScene; } }

		public SceneManager (ICor cor, Scene startScene)
		{
			this.cor = cor;
			activeScene = startScene;
			activeScene.Initialize(cor);
			renderManager = new SceneRenderManager(cor);

		}

		public Boolean Update(AppTime time)
		{
			Scene a = activeScene.RunUpdate (time);

			// If the active state returns a game state other than itself then we need to shut
			// it down and start the returned state.  If a game state returns null then we need to
			// shut the engine down.

			//quitting the game
			if (a == null) 
			{
				activeScene.Uninitilise ();
				return true;
			} 
			else if (a != activeScene) 
			{
				activeScene.Uninitilise ();

				activeScene = a;

				this.cor.Graphics.Reset();

				GC.Collect();

				activeScene.Initialize (cor);

				if (SimulationStateChanged != null)
				{
					SimulationStateChanged(this, System.EventArgs.Empty);
				}

				this.Update(time);

			}

			return false;

		}

		public void Render()
		{
			if (activeScene != null && activeScene.Active)
			{
				renderManager.Render(activeScene);
			}
			else
			{
				Teletype.WriteLine("Beep");
			}
		}
	}


}