using System;
using Sungiant.Abacus.Packed;
using Sungiant.Abacus.SinglePrecision;
using Sungiant.Cor;

namespace Sungiant.Cor.Xna4Runtime
{

	public class Engine
		: ICor
	{
		public Engine(
			Microsoft.Xna.Framework.GraphicsDeviceManager gfxManager,
			Microsoft.Xna.Framework.Content.ContentManager content,
			AppSettings settings,
			IApp startGame
			)
		{
			this.Settings = settings;

			this.App = startGame;

			this.XnaGfxManager = gfxManager;

			this.GraphicsManager = new GraphicsManager(this, gfxManager);
			this.ResourceManager = new ResourceManager(this, gfxManager.GraphicsDevice, content);
			this.SystemManager = new SystemManager(this, gfxManager);
			this.InputManager = new InputManager(this);

			this.App.Initilise(this);
			
		}

		public IGraphicsManager GraphicsManager { get; private set; }

		public IResourceManager ResourceManager { get; private set; }

		public IInputManager InputManager { get; private set; }

		public ISystemManager SystemManager { get; private set; }

		public AppSettings Settings { get; private set; }

		private IApp App { get; set; }

		private Microsoft.Xna.Framework.GraphicsDeviceManager XnaGfxManager { get; set; }

		public Boolean Update(AppTime time)
		{
			this.InputManager.Update(time);
			var retVal = this.App.Update(time);

			return retVal;
		}

		public void Render()
		{
			this.App.Render();
		}

	}


}