﻿using System;
using System.Diagnostics;
using Sungiant.Blimey.Internal;
using Sungiant.Abacus;

namespace Sungiant.Blimey.PsmRuntime
{
	
	public class Engine
		: EngineCommon
		, IDisposable
	    , IEngine
	{
		// PSS stuff
		Sce.Pss.Core.Graphics.GraphicsContext _graphicsContext;

		Stopwatch _timer = new Stopwatch();
        TimeSpan _previousTimeSpan;
		
		Single _elapsed;
		
		Int64 _frameCount;
		
		bool _running;
		
		EngineSettings settings;
		
		TouchScreen touchScreen;
		
		public EngineSettings Settings { get { return settings; } }
		
		public void Run()
		{
			_running = true;
			_timer.Start();
			
			while (_running) {
				Sce.Pss.Core.Environment.SystemEvents.CheckEvents ();
				this.Update ();
				this.Render ();
			}
			
			_timer.Stop();
		}

		public Engine(GameScene startScene, EngineSettings settings)
		{	
			this.settings = settings;
			
			// create new gfx device
			_graphicsContext = new Sce.Pss.Core.Graphics.GraphicsContext ();
			_graphicsContext.Enable(Sce.Pss.Core.Graphics.EnableMode.Blend);
			_graphicsContext.Enable(Sce.Pss.Core.Graphics.EnableMode.DepthTest);
			_graphicsContext.Enable(Sce.Pss.Core.Graphics.EnableMode.CullFace);
			
			_graphicsManager = new GraphicsManager(_graphicsContext);
			_resourceManager = new ResourceManager(_graphicsContext);
			touchScreen = new TouchScreen(this, _graphicsContext);
			_inputManager = new InputManager(this, touchScreen);
			_sceneManager = new SceneManager(this, startScene);
			
			
			_systemManager = new SystemManager(this, _graphicsContext, touchScreen);
		}
		

		void Update()
		{
            float dt = (float)(_timer.Elapsed.TotalSeconds - _previousTimeSpan.TotalSeconds);
            _previousTimeSpan = _timer.Elapsed;
			
			_elapsed += dt;
			
			var gt = new GameTime(dt, _elapsed, ++_frameCount);

			Boolean exit = Update(gt);
			
			if( exit )
			{
				_running = false;
			}
		}
		
		public override bool Update(GameTime time)
		{
			var gamePadData = Sce.Pss.Core.Input.GamePad.GetData (0);
			
			return base.Update(time);
		}

		public override void Render()
		{
			_graphicsContext.SetViewport(
				0, 
				0, 
				_graphicsContext.Screen.Width, 
				_graphicsContext.Screen.Height);
			
			base.Render();
			
			// Present the screen
			_graphicsContext.SwapBuffers ();

		}
		
		public void Dispose()
		{
			_graphicsContext.Dispose();
		}
	}
	
}


























