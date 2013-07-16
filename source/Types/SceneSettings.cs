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
	public struct RenderPassSettings
	{
		public static readonly RenderPassSettings Default;

		static RenderPassSettings()
		{
			Default.ClearDepthBuffer = false;
			Default.FogEnabled = false;
			Default.FogColour = Rgba32.CornflowerBlue;
			Default.FogStart = 300.0f;
			Default.FogEnd = 550.0f;
			Default.EnableDefaultLighting = true;
			Default.CameraProjectionType = CameraProjectionType.Perspective;
		}

		public Boolean ClearDepthBuffer;
		public Boolean FogEnabled;
		public Rgba32 FogColour;
		public Single FogStart;
		public Single FogEnd;
		public Boolean EnableDefaultLighting;
		public CameraProjectionType CameraProjectionType;
	}
	
	//
	// Game Scene Settings
	// -------------------
	// Game scene settings are used by the engine to detemine how
	// to manage an associated scene.  For example the game scene 
	// settings are used to define the render pass for the scene.
	public class SceneSettings
	{
        Dictionary<String, RenderPassSettings> renderPassSettings;
        List<String> renderPassOrder;
		Boolean startByClearingBackBuffer;
		Rgba32 clearBackBufferColour;

        readonly static SceneSettings defaultSettings = new SceneSettings();

        internal static SceneSettings Default
        {
            get
            {
                return defaultSettings;
            }
        }

        static SceneSettings()
        {
            defaultSettings.InitDefault();
        }

		public SceneSettings()
		{
            renderPassSettings = new Dictionary<String, RenderPassSettings>();
            renderPassOrder = new List<String>();
            startByClearingBackBuffer = false;
            clearBackBufferColour = Rgba32.CornflowerBlue;
        }

        void InitDefault()
        {
			/// BIG TODO, ADD AWAY TO MAKE RENDER PASSES SHARE THE SAME CAMERA!!!!!
			/// SO DEBUG AND DEFAULT CAN USER THE SAME
			AddRenderPass("Debug", RenderPassSettings.Default);

			var defaultPassSettings = RenderPassSettings.Default;
			defaultPassSettings.EnableDefaultLighting = true;
			defaultPassSettings.FogEnabled = true;
			AddRenderPass("Default", defaultPassSettings);

			var guiPassSettings = RenderPassSettings.Default;
			guiPassSettings.ClearDepthBuffer = true;
			guiPassSettings.CameraProjectionType = CameraProjectionType.Orthographic;
			AddRenderPass("Gui", guiPassSettings);
		}

		public void AddRenderPass(String passID, RenderPassSettings settings)
		{
			if (renderPassOrder.Contains(passID))
			{
				throw new Exception("Can't have render passes with the same name");
			}

			renderPassOrder.Add(passID);
			renderPassSettings.Add(passID, settings);
		}

		public RenderPassSettings GetRenderPassSettings(String passName)
		{
			return renderPassSettings[passName];
		}

		public List<String> RenderPasses
		{ 
			get 
			{ 
				return renderPassOrder; 
			} 
		}

		// Debug Background Rgba
		public bool StartByClearingBackBuffer
		{ 
			get 
			{ 
				return startByClearingBackBuffer; 
			}
            set
            {
                startByClearingBackBuffer = value;
            }
		}

		public Rgba32 BackgroundColour 
		{ 
			get 
			{ 
				return clearBackBufferColour; 
			} 
			set 
			{ 
				startByClearingBackBuffer = true; 
				clearBackBufferColour = value; 
			} 
		}
	
	}
}
