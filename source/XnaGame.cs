using System;
using Sungiant.Cor;

namespace Sungiant.Cor.Xna4Runtime
{
#if !WP7
	public class Xna4App
		: IDisposable // http://msdn.microsoft.com/en-us/library/system.idisposable.aspx
	{
		XnaGame game;

		// Track whether Dispose has been called. 
		private bool disposed = false;


		public Xna4App(AppSettings startSettings, IApp startGame)
		{
			game = new XnaGame(startSettings, startGame);
		}

		public void Run()
		{
			game.Run();
		}

		// Implement IDisposable. 
		// Do not make this method virtual. 
		// A derived class should not be able to override this method. 
		public void Dispose()
		{
			Dispose(true);
			// This object will be cleaned up by the Dispose method. 
			// Therefore, you should call GC.SupressFinalize to 
			// take this object off the finalization queue 
			// and prevent finalization code for this object 
			// from executing a second time.
			GC.SuppressFinalize(this);
		}

		// Dispose(bool disposing) executes in two distinct scenarios. 
		// If disposing equals true, the method has been called directly 
		// or indirectly by a user's code. Managed and unmanaged resources 
		// can be disposed. 
		// If disposing equals false, the method has been called by the 
		// runtime from inside the finalizer and you should not reference 
		// other objects. Only unmanaged resources can be disposed. 
		protected virtual void Dispose(bool disposing)
		{
			// Check to see if Dispose has already been called. 
			if (!this.disposed)
			{
				// If disposing equals true, dispose all managed 
				// and unmanaged resources. 
				if (disposing)
				{
					// Dispose managed resources.
					game.Dispose();
				}

				// Call the appropriate methods to clean up 
				// unmanaged resources here. 
				// If disposing is false, 
				// only the following code is executed.
				// ..

				// Note disposing has been done.
				disposed = true;

			}
		}
	}

	internal 
#else
	public
#endif
		
	class XnaGame
		: Microsoft.Xna.Framework.Game
	{
		Engine engine;
		Microsoft.Xna.Framework.GraphicsDeviceManager graphics;
		IApp startGame;
		Single elapsed;
		Int64 frameNumber = -1;
		AppSettings startSettings;

		public XnaGame(AppSettings startSettings, IApp startGame)
		{
			this.graphics = new Microsoft.Xna.Framework.GraphicsDeviceManager(this);
			this.startGame = startGame;

			this.startSettings = startSettings;
			this.Content.RootDirectory = "Content";
			this.IsFixedTimeStep = false;
			this.InactiveSleepTime = TimeSpan.FromSeconds(1);

			this.IsMouseVisible = true;
			this.Window.AllowUserResizing = true;
		}

		protected override void Initialize()
		{
			base.Initialize();

			graphics.PreferredDepthStencilFormat = Microsoft.Xna.Framework.Graphics.DepthFormat.Depth24;
#if XBOX
			
			graphics.PreferMultiSampling = true;
			graphics.PreferredBackBufferWidth = 1920;
			graphics.PreferredBackBufferHeight = 1200;
			graphics.ApplyChanges();

#elif WINDOWS
			graphics.PreferMultiSampling = true;
			graphics.PreferredBackBufferWidth = 800;
			graphics.PreferredBackBufferHeight = 480;
			graphics.ApplyChanges();

#elif WP7

			//graphics.SupportedOrientations = Microsoft.Xna.Framework.DisplayOrientation.Portrait;

			//graphics.PreferredBackBufferWidth = 480;
			//graphics.PreferredBackBufferHeight = 800;
			//graphics.ApplyChanges();
#endif
			engine = new Engine(graphics, Content, startSettings, startGame);
		}


		protected override void LoadContent()
		{

		}


		protected override void UnloadContent()
		{

		}

		protected override void Update(Microsoft.Xna.Framework.GameTime xnaGameTime)
		{

#if WP7
			(engine.SystemManager as SystemManager).SetDeviceOrientation(EnumConverter.ToBlimey(this.Window.CurrentOrientation));
#endif

			Single dt = (float)xnaGameTime.ElapsedGameTime.TotalSeconds;
			elapsed += dt;
			var appTime = new AppTime(dt, elapsed, ++frameNumber);
			engine.Update(appTime);

			base.Update(xnaGameTime);
		}


		protected override void Draw(Microsoft.Xna.Framework.GameTime xnaGameTime)
		{
			engine.Render();

			base.Draw(xnaGameTime);
		}
	}
}
