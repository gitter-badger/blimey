// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ Cor! - Low Level 3D App Engine                                         │ \\
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
using Sungiant.Abacus.Packed;
using Sungiant.Abacus.SinglePrecision;

namespace Sungiant.Cor.MonoTouchRuntime
{
	public class TouchScreen
		: MultiTouchController
		, IPanelSpecification
		, IScreenSpecification
	{
		Dictionary<Int32, iOSTouchState> touchData;
		MonoTouch.UIKit.UIView view;

		internal TouchScreen(
			ICor engine,
			MonoTouch.UIKit.UIView view,
			Dictionary<Int32, iOSTouchState> touches)
			: base(engine)
		{
			this.view = view;

			this.touchData = touches;

			Console.WriteLine(string.Format("Screen Specification - Width: {0}, Height: {1}", ScreenResolutionWidth, ScreenResolutionHeight));
		}

		public override IPanelSpecification PanelSpecification
		{
			get
			{
				return this;
			}
		}

		internal override void Update(AppTime time)
		{
			//Console.WriteLine(string.Format("MonoTouch.UIKit.UIScreen.MainScreen.Bounds - h: {0}, w: {1}", ScreenResolutionWidth, ScreenResolutionHeight));

			// seems to be a problem with mono touch reporting a new touch with
			// the same id across multiple frames.
			List<Int32> touchIDsLastFrame = new List<int>();

			foreach(var touch in this.collection)
			{
				touchIDsLastFrame.Add(touch.ID);
			}

			this.collection.ClearBuffer();


			foreach (var key in touchData.Keys)
			{
				var uiKitTouch = touchData[key];
				System.Drawing.PointF location = uiKitTouch.Location;

				Int32 id = uiKitTouch.Handle;

				Vector2 pos = new Vector2(location.X, location.Y);

				//Console.WriteLine(string.Format("UIKitTouch - id: {0}, pos: {1}", id, pos));

				// todo: this needs to be current display res, not just the screen specs


				pos.X = pos.X / engine.System.CurrentDisplaySize.X;
				pos.Y = pos.Y / engine.System.CurrentDisplaySize.Y;

				pos -= new Vector2(0.5f, 0.5f);

				pos.Y = -pos.Y;

				var state = EnumConverter.ToCorPrimitiveType(uiKitTouch.Phase);

				if( touchIDsLastFrame.Contains(id) )
				{
					if( state == TouchPhase.JustPressed )
					{
						//Sungiant.Core.Teletype.WriteLine("ignoring " + id);

						state = TouchPhase.Active;
					}
				}

				if( state == TouchPhase.JustPressed )
				{
					Console.WriteLine(string.Format("Touch - id: {0}, pos: {1}", id, pos));
				}

				this.collection.RegisterTouch(id, pos, state, time.FrameNumber, time.Elapsed);
			}
		}



		public Vector2 PanelPhysicalSize
		{
			get
			{
				// do lookup here into all device types
				//MonoTouch.ObjCRuntime.
				return new Vector2(0.0768f, 0.1024f);
			}
		}

		public float PanelPhysicalAspectRatio
		{
			get
			{
				return PanelPhysicalSize.X / PanelPhysicalSize.Y;
			}
		}
		public PanelType PanelType
		{
			get
			{
				return PanelType.TouchScreen;
			}
		}


		public float ScreenResolutionAspectRatio
		{ 
			get 
			{
				return this.ScreenResolutionWidth / this.ScreenResolutionHeight;
			} 
		}

		// need to think about
		public Single PixelDensity
		{
			get
			{
				return 1f;
			}
			set
			{
				;
			}
		}

		public Int32 ScreenResolutionHeight
		{
			get
			{
				return (Int32) (
					MonoTouch.UIKit.UIScreen.MainScreen.Bounds.Height *
					MonoTouch.UIKit.UIScreen.MainScreen.Scale);
			}
		}

		public Int32 ScreenResolutionWidth
		{
			get
			{
				return (Int32) (
					MonoTouch.UIKit.UIScreen.MainScreen.Bounds.Width *
					MonoTouch.UIKit.UIScreen.MainScreen.Scale);
			}
		}


	
	}
}

