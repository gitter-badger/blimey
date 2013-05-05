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

namespace Sungiant.Cor.MonoTouchRuntime
{
    public class Engine
        : ICor
    {
        TouchScreen touchScreen;
        AppSettings settings;
        IApp app;
        IGraphicsManager graphicsManager;
        IResourceManager resourceManager;
        InputManager inputManager;
        SystemManager systemManager;
        AudioManager audioManager;

        internal Engine(
            AppSettings settings,
            IApp app,
            OpenTK.Platform.iPhoneOS.iPhoneOSGameView view,
            OpenTK.Graphics.IGraphicsContext gfxContext,
            Dictionary<Int32, iOSTouchState> touches)
        {   
            this.settings = settings;

            this.app = app;


            this.graphicsManager = new GraphicsManager(gfxContext);

            this.resourceManager = new ResourceManager();

            this.touchScreen = new TouchScreen(this, view, touches);

            this.systemManager = new SystemManager(touchScreen);

            this.inputManager = new InputManager(this, this.touchScreen);

            this.app.Initilise(this);

        }

        internal TouchScreen TouchScreenImplementation
        {
            get
            {
                return touchScreen;
            }
        }

        public IAudioManager Audio
        {
            get
            {
                return audioManager;
            }
        }

        public AppSettings Settings
        {
            get
            {
                return this.settings;
            }
        }

        public ISystemManager System
        {
            get
            {
                return systemManager;
            }
        }

        public IGraphicsManager Graphics
        { 
            get
            {
                return graphicsManager;
            }
        }

        public IResourceManager Resources
        { 
            get
            {
                return resourceManager;
            }
        }
        
        public IInputManager Input
        {
            get
            {
                return inputManager;
            }
        }

        internal Boolean Update(AppTime time)
        {
            inputManager.Update(time);
            return app.Update(time);
        }

        internal void Render()
        {
            app.Render();
        }

    }
    
}