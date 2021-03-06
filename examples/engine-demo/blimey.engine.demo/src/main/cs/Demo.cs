﻿// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ __________.__  .__                                                     │ \\
// │ \______   \  | |__| _____   ____ ___.__.                               │ \\
// │  |    |  _/  | |  |/     \_/ __ <   |  |                               │ \\
// │  |    |   \  |_|  |  Y Y  \  ___/\___  |                               │ \\
// │  |______  /____/__|__|_|  /\___  > ____|                               │ \\
// │         \/              \/     \/\/                                    │ \\
// │                                                                        │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Copyright © 2012 - 2015 ~ Blimey Engine (http://www.blimey.io)         │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Authors:                                                               │ \\
// │ ~ Ash Pook (http://www.ajpook.com)                                     │ \\
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

namespace EngineDemo
{
    using System;
    using Blimey.Platform;
    using Blimey.Asset;
    using Blimey.Engine;
    using System.Collections.Generic;
    using Abacus.SinglePrecision;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

	public class Demo : App
	{
        public Demo() : base (new Scene_Darius ()) {}
	}


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public static class RandomObjectHelper
    {
        public static List<Entity> Generate(Scene scene)
        {
            var objects = new List<Entity>();
            var cubeModel = new CubePrimitive(scene.Platform.Graphics);
            var billboardModel = new BillboardPrimitive(scene.Platform.Graphics);
            var teapotModel = new TeapotPrimitive(scene.Platform.Graphics);
            var cylinderModel = new CylinderPrimitive(scene.Platform.Graphics);
            var sphereModel = new SpherePrimitive(scene.Platform.Graphics);
            var torusModel = new TorusPrimitive(scene.Platform.Graphics);

            objects.Add(CreateShapeGO(scene, "Default", cubeModel.Mesh, 2));
            objects.Add(CreateShapeGO(scene, "Default", billboardModel.Mesh));
            objects.Add(CreateShapeGO(scene, "Default", teapotModel.Mesh, 1));
            objects.Add(CreateShapeGO(scene, "Default", cylinderModel.Mesh));
            objects.Add(CreateShapeGO(scene, "Default", sphereModel.Mesh));
            objects.Add(CreateShapeGO(scene, "Default", torusModel.Mesh, 1));
            objects.Add(CreateShapeGO(scene, "Default", torusModel.Mesh, 2));
            objects.Add(CreateShapeGO(scene, "Default", torusModel.Mesh, 0));
            return objects;
        }

        static int c = 0;

        public static Entity CreateShapeGO(Scene scene, string renderPass,  Mesh modelpart, int shaderIndex = 0)
        {
            // create a game object
            Entity testGO = scene.SceneGraph.CreateSceneObject ("test-" + c++);

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


            Shader shader = null;
            if (shaderIndex == 0) shader = CommonDemoResources.PixelLitShader;
            else if (shaderIndex == 1) shader = CommonDemoResources.VertexLitShader;
            else shader = CommonDemoResources.UnlitShader;

            // create a material on the fly
            var mat = new Material(renderPass, shader);
            //mat.SetTexture("_texture", tex);

            // add a mesh renderer
            MeshRendererTrait meshRendererTrait = testGO.AddTrait<MeshRendererTrait> ();

            // set the mesh renderer's material
            meshRendererTrait.Material = mat;

            // and it's model
            meshRendererTrait.Mesh = modelpart;

            testGO.AddTrait<ColourChanger>();

            return testGO;
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public static class CommonDemoResources
    {
        public static Shader UnlitShader;
        public static Shader VertexLitShader;
        public static Shader PixelLitShader;

        public static void Create (Platform platform, Engine engine)
        {
            var unlitShaderAsset = engine.Assets.Load<ShaderAsset>("assets/unlit.bba");
            var vertexLitShaderAsset = engine.Assets.Load<ShaderAsset>("assets/vertex_lit.bba");
            var pixelLitShaderAsset = engine.Assets.Load<ShaderAsset>("assets/pixel_lit.bba");

            UnlitShader = platform.Graphics.CreateShader (unlitShaderAsset);
            VertexLitShader = platform.Graphics.CreateShader (vertexLitShaderAsset);
            PixelLitShader = platform.Graphics.CreateShader (pixelLitShaderAsset);
        }

        public static void Destroy ()
        {
            UnlitShader.Dispose ();
            VertexLitShader.Dispose ();
            PixelLitShader.Dispose ();

            UnlitShader = null;
            VertexLitShader = null;
            PixelLitShader = null;
        }
    }

}

