// ┌────────────────────────────────────────────────────────────────────────────────────────────────────────────────┐ \\
// │ Blimey - Fast, efficient, high level engine built upon Cor & Abacus                                            │ \\
// ├────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤ \\
// │                     Brought to you by:                                                                         │ \\
// │                              _________                    .__               __                                 │ \\
// │                             /   _____/__ __  ____    ____ |__|____    _____/  |_                               │ \\
// │                             \_____  \|  |  \/    \  / ___\|  \__  \  /    \   __\                              │ \\
// │                             /        \  |  /   |  \/ /_/  >  |/ __ \|   |  \  |                                │ \\
// │                            /_______  /____/|___|  /\___  /|__(____  /___|  /__|                                │ \\
// │                                    \/           \//_____/         \/     \/                                    │ \\
// │                                                                                                                │ \\
// ├────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤ \\
// │ Copyright © 2008-2014 A.J.Pook (http://ajpook.github.io)                                                       │ \\
// ├────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤ \\
// │ Authors: A.J.Pook                                                                                              │ \\
// ├────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤ \\
// │ Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated   │ \\
// │ documentation files (the "Software"), to deal in the Software without restriction, including without           │ \\
// │ limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sellcopies of the   │ \\
// │ Software, and to permit persons to whom the Software is furnished to do so, subject to the following           │ \\
// │ conditions:                                                                                                    │ \\
// │                                                                                                                │ \\
// │ The above copyright notice and this permission notice shall be included in all copies or substantial portions  │ \\
// │ of the Software.                                                                                               │ \\
// │                                                                                                                │ \\
// │ THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED  │ \\
// │ TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.  IN NO EVENT SHALL │ \\
// │ THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF  │ \\
// │ CONTRACT, TORT OR OTHERWISE, ARISING FROM,OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER        │ \\
// │ DEALINGS IN THE SOFTWARE.                                                                                      │ \\
// └────────────────────────────────────────────────────────────────────────────────────────────────────────────────┘ \\

namespace Blimey.Demo
{
    using System;
    using Fudge;
    using Abacus.SinglePrecision;
    using Cor;
    using System.Collections.Generic;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class RandomLocalRotate
        : Trait
    {
        Single _speed;
        Single _timer;
        Vector3 _vec;

        public RandomLocalRotate()
        {
            _speed = RandomGenerator.Default.GetRandomSingle(-1f, 1.0f);

            _vec = new Vector3(
                RandomGenerator.Default.GetRandomSingle(0.25f, 2f),
                RandomGenerator.Default.GetRandomSingle(0.25f, 2f),
                RandomGenerator.Default.GetRandomSingle(0.25f, 2f)
                );

            Vector3.Normalise(ref _vec, out _vec);
        }
        
        public override void OnUpdate(AppTime time)
        {
            _timer += time.Delta;

            Single displacement = _speed * _timer;
            Quaternion rot;
            Quaternion.CreateFromAxisAngle(ref _vec, ref displacement, out rot);

            this.Parent.Transform.LocalRotation = rot;
        }
    }
    
    public class ColourChanger
        : Trait
    {
        Rgba32 _current;
        Rgba32 _target;

        Single _colourChangeTime;

        Single _timer;

        MeshRenderer _renderer;

        public ColourChanger()
        {
            _current = RandomGenerator.Default.GetRandomColour();
            _target = RandomGenerator.Default.GetRandomColour();
            _colourChangeTime = RandomGenerator.Default.GetRandomSingle(3f, 10f);
            _timer = _colourChangeTime;
        }

        public override void OnAwake()
        {
            _renderer = this.Parent.GetTrait<MeshRenderer>();
        }

        public override void OnUpdate(AppTime time)
        {
            _timer -= time.Delta;

            if( _timer < 0f )
            {
                _timer = _colourChangeTime - _timer;
                _current = _target;
                _target = RandomGenerator.Default.GetRandomColour();
            }

            Single lerpVal = 1f - _timer;
            if(lerpVal < 0f) lerpVal = 0f;

            Rgba32 colToSet = Rgba32.Lerp(_current, _target, lerpVal);

            _renderer.Material.SetColour("MaterialColour", colToSet);
        }
    }

    public static class RandomObjectHelper
    {
        public static List<Entity> Generate(Scene scene)
        {
            var objects = new List<Entity>();
            var cubeModel = new CubePrimitive(scene.Cor.Graphics);
            var billboardModel = new BillboardPrimitive(scene.Cor.Graphics);
            var teapotModel = new TeapotPrimitive(scene.Cor.Graphics);
            var cylinderModel = new CylinderPrimitive(scene.Cor.Graphics);
            var sphereModel = new SpherePrimitive(scene.Cor.Graphics);
            var torusModel = new TorusPrimitive(scene.Cor.Graphics);

            objects.Add(CreateShapeGO(scene, "Default", cubeModel, 2));
            objects.Add(CreateShapeGO(scene, "Default", billboardModel));
            objects.Add(CreateShapeGO(scene, "Default", teapotModel, 1));
            objects.Add(CreateShapeGO(scene, "Default", cylinderModel));
            objects.Add(CreateShapeGO(scene, "Default", sphereModel));
            objects.Add(CreateShapeGO(scene, "Default", torusModel, 1));
            objects.Add(CreateShapeGO(scene, "Default", torusModel, 2));
            objects.Add(CreateShapeGO(scene, "Default", torusModel, 0));
            return objects;
        }


        public static Entity CreateShapeGO(Scene scene, string renderPass,  Mesh modelpart, int shaderIndex = 0)
        {
            // create a game object
            Entity testGO = scene.SceneGraph.CreateSceneObject ("test");


            Single scale = RandomGenerator.Default.GetRandomSingle(0.25f, 0.5f);


            // size it
            testGO.Transform.LocalPosition = new Vector3(
                RandomGenerator.Default.GetRandomSingle(-1.28f, 1.28f),
                RandomGenerator.Default.GetRandomSingle(-1.28f, 1.28f),
                RandomGenerator.Default.GetRandomSingle(-1.28f, 1.28f));

            testGO.Transform.LocalScale = new Vector3(scale, scale, scale);
            testGO.AddTrait<RandomLocalRotate>();

            // load a texture
            //Texture tex = null;//scene.Engine.Resources.Load<Texture> (new Uri("\\Textures\\recycle"));

            ShaderAsset shaderAsset = null;

            if (shaderIndex == 0)
                shaderAsset = scene.Cor.Assets.Load<ShaderAsset>("vertex_lit.cba");
            else if (shaderIndex == 1)
                shaderAsset = scene.Cor.Assets.Load<ShaderAsset>("pixel_lit.cba");
            else
                shaderAsset = scene.Cor.Assets.Load<ShaderAsset>("unlit.cba");;

            IShader shader = scene.Cor.Graphics.CreateShader (shaderAsset);

            // create a material on the fly
            var mat = new Material(renderPass, shader);
            //mat.SetTexture("_texture", tex);




            // add a mesh renderer
            MeshRenderer meshRendererTrait = testGO.AddTrait<MeshRenderer> ();

            // set the mesh renderer's material
            meshRendererTrait.Material = mat;

            // and it's model
            meshRendererTrait.Mesh = modelpart;

            testGO.AddTrait<ColourChanger>();

            return testGO;
        }
    }

    public class Scene_Shapes1
        : Scene
    {
        List<Entity> _objects;

        Entity _alternateCamera;

        GridRenderer gr;

        const Single _cameraChangeTime = 5f;

        Single _timer = _cameraChangeTime;

        bool _defaultCamIsCurrent = true;

        Scene _returnScene;

        // GPU Resources.
        IShader shader = null;

        public override void Start ()
        {
            // set up the debug renderer
            ShaderAsset shaderAsset = this.Cor.Assets.Load<ShaderAsset>("unlit.cba");
            this.Blimey.DebugShapeRenderer.DebugShader = 
                this.Cor.Graphics.CreateShader (shaderAsset);

            gr = new GridRenderer(this.Blimey.DebugShapeRenderer, "Default");

            _alternateCamera = this.SceneGraph.CreateSceneObject("Alternate Camera");

            _alternateCamera.AddTrait<Camera>();

            _alternateCamera.Transform.Position = new Vector3(0.65f, 1f, -2.50f) * 3;
            _alternateCamera.Transform.LookAt(Vector3.Zero);

            _objects = RandomObjectHelper.Generate(this);

            var landmarkGo = this.CreateLandmark();
            _objects.Add(landmarkGo);

            _returnScene = this;

            this.Blimey.InputEventSystem.Tap += this.OnTap;
        }

        Entity CreateLandmark()
        {
            var landmarkGo = this.SceneGraph.CreateSceneObject("landmark");

            landmarkGo.Transform.LocalPosition = new Vector3(0f, 0f, 0f);
            landmarkGo.Transform.LocalScale = new Vector3(0.64f, 0.64f, 0.64f);

            var cowMesh = new TeapotPrimitive(this.Cor.Graphics);

            var mr = landmarkGo.AddTrait<MeshRenderer>();

            mr.Mesh = cowMesh;

            ShaderAsset shaderAsset = this.Cor.Assets.Load<ShaderAsset> ("pixel_lit.cba");
            shader = this.Cor.Graphics.CreateShader (shaderAsset);

            var mat = new Material("Default", shader);

            mat.SetColour("MaterialColour", Rgba32.CornflowerBlue);

            mr.Material = mat;

            return landmarkGo;
        }

        public override Scene Update(AppTime time)
        {
            if (Cor.Input.GenericGamepad.Buttons.East == ButtonState.Pressed ||
                Cor.Input.Keyboard.IsFunctionalKeyDown(FunctionalKey.Escape) ||
                Cor.Input.Keyboard.IsFunctionalKeyDown(FunctionalKey.Backspace))
            {
                _returnScene = new Scene_Shapes2();
            }

            gr.Update();

            _timer -= time.Delta;

            if( _timer < 0f )
            {
                _timer = _cameraChangeTime;

                if( _defaultCamIsCurrent )
                {
                    this.RuntimeConfiguration.SetRenderPassCameraTo("Default", _alternateCamera);
                }
                else
                {
                    this.RuntimeConfiguration.SetRenderPassCameraToDefault("Default");
                }

                _defaultCamIsCurrent = !_defaultCamIsCurrent;
            }

            return _returnScene;
        }

        public override void Shutdown ()
        {
            _objects = null;
            this.Blimey.InputEventSystem.Tap -= this.OnTap;

            // Clean up the things we allocated on the GPU.
            this.Cor.Graphics.DestroyShader (shader);
            shader = null;
        }

        void OnTap(Gesture gesture)
        {
            _returnScene = new Scene_Shapes2();
        }
    }

    public class Scene_Shapes2
        : Scene
    {
        Scene returnScene;
        Entity billboardGo;

        GridRenderer gr;

        LookAtSubject las;

        Entity cam;

        Transform target;


        float timer = timeWindow;
        const float timeWindow = 5f;
        bool x = true;
        bool goOut = true;

        Entity markerGo;

        // GPU Resources
        IShader unlitShader = null;

        public override void Start()
        {
            this.Configuration.BackgroundColour = Rgba32.LightSlateGrey;

            // set up the debug renderer
            ShaderAsset unlitShaderAsset = this.Cor.Assets.Load<ShaderAsset> ("unlit.cba");
            this.Blimey.DebugShapeRenderer.DebugShader = 
                this.Cor.Graphics.CreateShader (unlitShaderAsset);
            gr = new GridRenderer(this.Blimey.DebugShapeRenderer, "Default", 1f, 10);

            returnScene = this;

            // create a sprite
            var billboard = new BillboardPrimitive(this.Cor.Graphics);


            unlitShader = this.Cor.Graphics.CreateShader (unlitShaderAsset);
            billboardGo = this.SceneGraph.CreateSceneObject("billboard");

            var mr = billboardGo.AddTrait<MeshRenderer>();
            mr.Mesh = billboard;
            mr.Material = new Material("Default", unlitShader);
            mr.Material.SetColour("MaterialColour", RandomGenerator.Default.GetRandomColour());

            target = billboardGo.Transform;

            markerGo = this.SceneGraph.CreateSceneObject ("marker");

            markerGo.Transform.LocalScale = new Vector3 (0.05f, 0.05f, 0.05f);

            var markerMR = markerGo.AddTrait<MeshRenderer> ();
            markerMR.Mesh = new CubePrimitive(this.Cor.Graphics);
            markerMR.Material = new Material("Default", unlitShader);
            markerMR.Material.SetColour("MaterialColour", Rgba32.Red);

            cam = this.CameraManager.GetRenderPassCamera ("Default");

            this.SceneGraph.DestroySceneObject(this.CameraManager.GetRenderPassCamera ("Debug"));
            this.SceneGraph.DestroySceneObject(this.CameraManager.GetRenderPassCamera ("Gui"));

            this.RuntimeConfiguration.SetRenderPassCameraTo ("Debug", cam);
            cam.Transform.Position = new Vector3(2, 1, 5);
            cam.RemoveTrait<OrbitAroundSubject> ();

            las = cam.GetTrait<LookAtSubject> ();
            las.Subject = billboardGo.Transform;

            this.Blimey.InputEventSystem.Tap += this.OnTap;
        }

        public override void Shutdown()
        {
            this.Blimey.InputEventSystem.Tap -= this.OnTap;
        }

        public override Scene Update(AppTime time)
        {
            gr.Update ();

            timer -= time.Delta;

            if (timer < 0f)
            {
                timer = timeWindow;

                goOut = !goOut;

                if (goOut)
                    x = !x;
            }

            float f = timer / timeWindow;

            if (goOut)
                f = 1f - f;

            f = f * 2f;

            target.Position = new Vector3 (
                x ? f : 0f,
                0,
                x ? 0f : f);

            this.Blimey.DebugShapeRenderer.AddLine (
                "Default",
                target.Position,
                target.Position + new Vector3 (0f, 10f, 0f),
                Rgba32.Orange);

            this.Blimey.DebugShapeRenderer.AddLine (
                "Default",
                las.Subject.Position,
                new Vector3(cam.Transform.Position.X, 0f, cam.Transform.Position.Z),
                Rgba32.Lime);

            markerGo.Transform.Position = target.Position + new Vector3 (0f, 0.2f, 0f);

            if (Cor.Input.GenericGamepad.Buttons.East == ButtonState.Pressed ||
                Cor.Input.Keyboard.IsFunctionalKeyDown(FunctionalKey.Escape) ||
                    Cor.Input.Keyboard.IsFunctionalKeyDown(FunctionalKey.Backspace))
            {
                returnScene = new Scene_Shapes3();
            }

            return returnScene;
        }

        void OnTap(Gesture gesture)
        {
            returnScene = new Scene_Shapes3();

            // Clean up the things we allocated on the GPU.
            this.Cor.Graphics.DestroyShader (unlitShader);
            unlitShader = null;
        }
    }


    public class Scene_Shapes3
        : Scene
    {
        Scene _returnScene;

        Mesh teapotGPUMesh;
        Int32 teapotCounter;
        GridRenderer gr;

		
		// GPU Resources.
		IShader shader = null;

        public override void Start()
        {
			this.Configuration.BackgroundColour = Rgba32.Black;

            Entity camSo = SceneGraph.CreateSceneObject ("Scene 3 Camera");
			camSo.AddTrait <Camera> ();
            var lookatTrait = camSo.AddTrait<LookAtSubject>();
            lookatTrait.Subject = Transform.Origin;
            var orbitTrait = camSo.AddTrait<OrbitAroundSubject>();
            orbitTrait.CameraSubject = Transform.Origin;

            camSo.Transform.LocalPosition = new Vector3(1f,0.5f,5f);

			this.RuntimeConfiguration.SetRenderPassCameraTo("Debug", camSo);
			this.RuntimeConfiguration.SetRenderPassCameraTo("Default", camSo);

            _returnScene = this;
            this.Blimey.InputEventSystem.Tap += this.OnTap;

            teapotGPUMesh = new TeapotPrimitive(Cor.Graphics);

			// set up the debug renderer
			ShaderAsset shaderAsset = this.Cor.Assets.Load<ShaderAsset>("unlit.cba");
			this.Blimey.DebugShapeRenderer.DebugShader = 
				this.Cor.Graphics.CreateShader (shaderAsset);
            gr = new GridRenderer(this.Blimey.DebugShapeRenderer, "Debug");

            AddTeapot(0);
            AddTeapot(-1.5f);
            AddTeapot(1.5f);
        }

        void AddTeapot(Single z)
        {
            // create a game object
            Entity testGO = SceneGraph.CreateSceneObject ("teapot #" + ++teapotCounter);

            Single scale = 1f;


            // size it
            testGO.Transform.LocalPosition = new Vector3(
                0,
                0,
                z);

            testGO.Transform.LocalScale = new Vector3(scale, scale, scale);


			ShaderAsset shaderAsset = this.Cor.Assets.Load<ShaderAsset> ("pixel_lit.cba");

			shader = this.Cor.Graphics.CreateShader (shaderAsset);

            var mat = new Material("Default", shader);

            //mat.SetTexture("_texture", null);
            // add a mesh renderer
            var meshRendererTrait = testGO.AddTrait<MeshRenderer> ();

            // set the mesh renderer's material
            meshRendererTrait.Material = mat;

            meshRendererTrait.Material.SetColour("MaterialColour", RandomGenerator.Default.GetRandomColour());

            // and it's model
            meshRendererTrait.Mesh = teapotGPUMesh;
        }

        public override void Shutdown()
        {
            this.Blimey.InputEventSystem.Tap -= this.OnTap;
        }

        public override Scene Update(AppTime time)
        {
			if (Cor.Input.GenericGamepad.Buttons.East == ButtonState.Pressed ||
				Cor.Input.Keyboard.IsFunctionalKeyDown(FunctionalKey.Escape) ||
					Cor.Input.Keyboard.IsFunctionalKeyDown(FunctionalKey.Backspace))
            {
                _returnScene = new Scene_MainMenu();
            }

            gr.Update();

            this.Blimey.DebugShapeRenderer.AddLine(
                "Gui",
                new Vector3(-0.5f, -0.5f, 0),
                new Vector3(0.5f, 0.5f, 0),
                Rgba32.Yellow);

            return _returnScene;
        }

        void OnTap(Gesture gesture)
        {
			_returnScene = new Scene_MainMenu();

			// Clean up the things we allocated on the GPU.
			this.Cor.Graphics.DestroyShader (shader);
			shader = null;
        }
    }
}

