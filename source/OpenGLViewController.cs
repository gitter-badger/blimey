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



namespace Sungiant.Cor.MonoTouchRuntime
{
	[MonoTouch.Foundation.Register ("OpenGLViewController")]
	public partial class OpenGLViewController 
		: MonoTouch.UIKit.UIViewController
	{
		AppSettings _settings;
		IApp _game;
			
		public OpenGLViewController (
			AppSettings settings,
			IApp game)
			: base ()
		{
			MonoTouch.UIKit.UIApplication.SharedApplication.SetStatusBarHidden (true, MonoTouch.UIKit.UIStatusBarAnimation.None);
			_settings = settings;
			_game = game;
		}
		
		new EAGLView View
		{
			get
			{
				return (EAGLView) base.View;
			}
		}
		/*
		// stuff to expose specifically to the monotouch implementation
		public override MonoTouch.UIKit.UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return new MonoTouch.UIKit.UIInterfaceOrientationMask();
		}

		public override MonoTouch.UIKit.UIInterfaceOrientation PreferredInterfaceOrientationForPresentation ()
		{
			return base.PreferredInterfaceOrientationForPresentation ();
		}


		public override bool ShouldAutorotate ()
		{
			return base.ShouldAutorotate ();
		}
		*/

		public override void LoadView()
		{
			//var size = MonoTouch.UIKit.UIScreen.MainScreen.CurrentMode.Size;
			//var frame = new System.Drawing.RectangleF(0, 0, size.Width, size.Height);
			var frame = MonoTouch.UIKit.UIScreen.MainScreen.Bounds;
			base.View = new EAGLView(frame);
		}
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			MonoTouch.Foundation.NSNotificationCenter.DefaultCenter.AddObserver (
				MonoTouch.UIKit.UIApplication.WillResignActiveNotification, a => {
				if (IsViewLoaded && View.Window != null)
					View.StopAnimating ();
				},
				this
			);

			MonoTouch.Foundation.NSNotificationCenter.DefaultCenter.AddObserver (
				MonoTouch.UIKit.UIApplication.DidBecomeActiveNotification, a => {
				if (IsViewLoaded && View.Window != null)
					View.StartAnimating ();
				},
				this
			);

			MonoTouch.Foundation.NSNotificationCenter.DefaultCenter.AddObserver (
				MonoTouch.UIKit.UIApplication.WillTerminateNotification, a => {
				if (IsViewLoaded && View.Window != null)
					View.StopAnimating ();
				},
				this
			);
			
			View.SetEngineDetails(_settings, _game);
		}
		
		protected override void Dispose (System.Boolean disposing)
		{
			base.Dispose (disposing);
			
			MonoTouch.Foundation.NSNotificationCenter.DefaultCenter.RemoveObserver (this);
		}
		
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void DidRotate(MonoTouch.UIKit.UIInterfaceOrientation fromInterfaceOrientation)
		{
			base.DidRotate(fromInterfaceOrientation);

			//String previous = fromInterfaceOrientation


		}

		
		public override void ViewWillAppear (System.Boolean animated)
		{
			base.ViewWillAppear (animated);
			View.StartAnimating ();
		}
		
		public override void ViewWillDisappear (System.Boolean animated)
		{
			base.ViewWillDisappear (animated);
			View.StopAnimating ();
		}
	}
}
