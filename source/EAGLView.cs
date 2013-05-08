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
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.CoreText;
using System.Drawing;

namespace Sungiant.Cor.MonoTouchRuntime
{

	[MonoTouch.Foundation.Register ("EAGLView")]
	public class EAGLView 
		: OpenTK.Platform.iPhoneOS.iPhoneOSGameView
	{

		AppSettings settings;
		IApp game;

		Engine gameEngine;
		Stopwatch timer = new Stopwatch();
		Single elapsedTime;
		Int64 frameCounter = -1;
		TimeSpan previousTimeSpan;
		Int32 frameInterval;

		MonoTouch.CoreAnimation.CADisplayLink displayLink;

		Dictionary<Int32, iOSTouchState> touchState = new Dictionary<int, iOSTouchState>();

		public System.Boolean IsAnimating 
		{ 
			get; 
			private set; 
		}
		
		// How many display frames must pass between each time the display link fires.
		public Int32 FrameInterval
		{
			get
			{
				return frameInterval;
			}
			set
			{
				if (value <= 0)
				{
					throw new ArgumentException ();
				}

				frameInterval = value;

				if (IsAnimating)
				{
					StopAnimating ();
					StartAnimating ();
				}
			}
		}


		/*
		[MonoTouch.Foundation.Export("initWithCoder:")]
		public EAGLView (MonoTouch.Foundation.NSCoder coder) 
			: base (coder)
		{
			LayerRetainsBacking = true;
			LayerColorFormat = MonoTouch.OpenGLES.EAGLColorFormat.RGBA8;


		}

*/
		public EAGLView (System.Drawing.RectangleF frame)
			: base (frame)
		{
			LayerRetainsBacking = true;
			LayerColorFormat = MonoTouch.OpenGLES.EAGLColorFormat.RGBA8;
			ContextRenderingApi = MonoTouch.OpenGLES.EAGLRenderingAPI.OpenGLES2;

		}

		[MonoTouch.Foundation.Export ("layerClass")]
		public static new MonoTouch.ObjCRuntime.Class GetLayerClass ()
		{
			return OpenTK.Platform.iPhoneOS.iPhoneOSGameView.GetLayerClass ();
		}

		
		protected override void ConfigureLayer (MonoTouch.CoreAnimation.CAEAGLLayer eaglLayer)
		{
			eaglLayer.Opaque = true;
		}

		uint _depthRenderbuffer;

		protected override void CreateFrameBuffer()
		{
			base.CreateFrameBuffer();

			//
			// Enable the depth buffer
			//
			OpenTK.Graphics.ES20.GL.GenRenderbuffers(1, out _depthRenderbuffer);
			OpenTKHelper.CheckError();

			OpenTK.Graphics.ES20.GL.BindRenderbuffer(OpenTK.Graphics.ES20.RenderbufferTarget.Renderbuffer, _depthRenderbuffer);
			OpenTKHelper.CheckError();

			OpenTK.Graphics.ES20.GL.RenderbufferStorage(OpenTK.Graphics.ES20.RenderbufferTarget.Renderbuffer, OpenTK.Graphics.ES20.RenderbufferInternalFormat.DepthComponent16, Size.Width, Size.Height);
			OpenTKHelper.CheckError();

			OpenTK.Graphics.ES20.GL.FramebufferRenderbuffer(
				OpenTK.Graphics.ES20.FramebufferTarget.Framebuffer,
				OpenTK.Graphics.ES20.FramebufferSlot.DepthAttachment,
				OpenTK.Graphics.ES20.RenderbufferTarget.Renderbuffer,
				_depthRenderbuffer);
			OpenTKHelper.CheckError();

		}
		
		public void SetEngineDetails(AppSettings settings, IApp game)
		{
			this.settings = settings;
			this.game = game;
		}

		void CreateEngine()
		{
			gameEngine = new Engine(
				this.settings,
				this.game,
				this, 
				this.GraphicsContext, 
				this.touchState);
			timer.Start();
		}
		
		protected override void DestroyFrameBuffer ()
		{
			base.DestroyFrameBuffer ();
		}

		public void StartAnimating ()
		{
			if (IsAnimating)
				return;
			
			CreateFrameBuffer ();

			CreateEngine();

			displayLink = 
				MonoTouch.UIKit.UIScreen.MainScreen.CreateDisplayLink (
					this, 
					new MonoTouch.ObjCRuntime.Selector ("drawFrame")
					);

			displayLink.FrameInterval = frameInterval;
			displayLink.AddToRunLoop (MonoTouch.Foundation.NSRunLoop.Current, MonoTouch.Foundation.NSRunLoop.NSDefaultRunLoopMode);
			
			IsAnimating = true;
		}
		
		public void StopAnimating ()
		{
			if (!IsAnimating)
				return;

			displayLink.Invalidate ();
			displayLink = null;

			DestroyFrameBuffer ();

			IsAnimating = false;
		}
		
		

		[MonoTouch.Foundation.Export ("drawFrame")]
		void DrawFrame ()
		{
			var e = new OpenTK.FrameEventArgs ();
			OnUpdateFrame(e);
			OnRenderFrame(e);

		}



		protected override void OnUpdateFrame(OpenTK.FrameEventArgs e)
		{
			base.OnUpdateFrame(e);

			this.ClearOldTouches();

			Single dt = (Single)(timer.Elapsed.TotalSeconds - previousTimeSpan.TotalSeconds);
			previousTimeSpan = timer.Elapsed;
			
			if (dt > 0.5f)
			{
				dt = 0.0f;
			}

			elapsedTime += dt;

			var appTime = new AppTime(dt, elapsedTime, ++frameCounter);


			gameEngine.Update(appTime);
		
		}

		void ClearOldTouches()
		{
			var keysToDitch = new List<Int32>();

			//remove stuff
			var keys = touchState.Keys;

			foreach(var key in keys)
			{
				var ts = touchState[key];

				if( ts.Phase == MonoTouch.UIKit.UITouchPhase.Cancelled ||
					ts.Phase == MonoTouch.UIKit.UITouchPhase.Ended )
				{
					if( ts.LastUpdated < this.frameCounter )
					{
						keysToDitch.Add(key);
					}
				}
			}

			foreach(var key in keysToDitch)
			{
				touchState.Remove(key);
				
				//Console.WriteLine("remove "+key);
			}
		}

		protected override void OnRenderFrame (OpenTK.FrameEventArgs e)
		{
			base.OnRenderFrame (e);

			base.MakeCurrent();
			
			gameEngine.Render();

			this.SwapBuffers ();
        }

		/*
        public override void Draw(RectangleF rect)
        {
            var gctx = UIGraphics.GetCurrentContext ();
            
            gctx.TranslateCTM (10, 0.5f * Bounds.Height);
            gctx.ScaleCTM (1, -1);
            gctx.RotateCTM ((float)Math.PI * 315 / 180);
            
            gctx.SetFillColor (UIColor.Green.CGColor);
            
            string someText = "ä½ å¥½ä¸ç";

            var attributedString = new NSAttributedString (someText,
                                                           new CTStringAttributes{
                ForegroundColorFromContext =  true,
                Font = new CTFont ("Arial", 24)
            });

            using (var textLine = new CTLine (attributedString)) {
                textLine.Draw (gctx);
            }
            
            base.Draw(rect);

		}*/

		public override void TouchesBegan(MonoTouch.Foundation.NSSet touches, MonoTouch.UIKit.UIEvent evt)
		{
			ProcessTouchChange(touches);

			base.TouchesBegan(touches, evt);
		}

		public override void TouchesMoved(MonoTouch.Foundation.NSSet touches, MonoTouch.UIKit.UIEvent evt)
		{
			ProcessTouchChange(touches);

			base.TouchesMoved(touches, evt);
		}

		public override void TouchesCancelled(MonoTouch.Foundation.NSSet touches, MonoTouch.UIKit.UIEvent evt)
		{
			ProcessTouchChange(touches);

			base.TouchesCancelled(touches, evt);
		}

		public override void TouchesEnded(MonoTouch.Foundation.NSSet touches, MonoTouch.UIKit.UIEvent evt)
		{
			ProcessTouchChange(touches);

			base.TouchesEnded(touches, evt);
		}


		void ProcessTouchChange(MonoTouch.Foundation.NSSet touches)
		{
			var touchesArray = touches.ToArray<MonoTouch.UIKit.UITouch> ();

			for (int i = 0; i < touchesArray.Length; ++i) 
			{
				var touch = touchesArray [i];

				//Get position touch
				var location = touch.LocationInView (this);
				var id = touch.Handle.ToInt32 ();
				var phase = touch.Phase;

				var ts = new iOSTouchState();
				ts.Handle = id;
				ts.LastUpdated = this.frameCounter;
				ts.Location = location;
				ts.Phase = phase;

				if( phase == MonoTouch.UIKit.UITouchPhase.Began )
				{
					//Console.WriteLine("add "+id);
					touchState.Add(id, ts);
				}
				else
				{
					if( touchState.ContainsKey(id) )
					{
						touchState[id] = ts;

						if(ts.Phase == MonoTouch.UIKit.UITouchPhase.Began)
						{
							ts.Phase = MonoTouch.UIKit.UITouchPhase.Stationary;
						}

					}
					else
					{
						throw new Exception("eerrr???");
					}
				}
			}
		}
	}
}
