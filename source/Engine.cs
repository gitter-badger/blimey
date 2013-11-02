using System;
using Sungiant.Abacus.Packed;
using Sungiant.Abacus.SinglePrecision;
using Sungiant.Cor;

namespace Sungiant.Cor.Platform.Managed.Xna4
{

	public class Engine
		: ICor
	{

        readonly GraphicsManager graphics;
		readonly ResourceManager resources;
		readonly SystemManager system;
        readonly InputManager input;
        readonly AppSettings settings;

		public Engine(
			Microsoft.Xna.Framework.GraphicsDeviceManager gfxManager,
			Microsoft.Xna.Framework.Content.ContentManager content,
			AppSettings settings,
			IApp startGame
			)
		{
			this.settings = settings;

			this.App = startGame;

			this.XnaGfxManager = gfxManager;

            this.graphics = new GraphicsManager(this, gfxManager);
            this.resources = new ResourceManager(this, gfxManager.GraphicsDevice, content);
            this.system = new SystemManager(this, gfxManager);
            this.input = new InputManager(this);

			this.App.Initilise(this);
			
		}

        public IGraphicsManager Graphics { get { return graphics;  } }

        public IResourceManager Resources { get { return resources; } }

        public IInputManager Input { get { return input; } }

        public ISystemManager System { get { return system; } }

        public AppSettings Settings { get { return settings; } }

        public IAudioManager Audio { get { return null; } }

		private IApp App { get; set; }

		private Microsoft.Xna.Framework.GraphicsDeviceManager XnaGfxManager { get; set; }

		public Boolean Update(AppTime time)
		{
			this.input.Update(time);
			var retVal = this.App.Update(time);

			return retVal;
		}

		public void Render()
		{
			this.App.Render();
		}

	}


}