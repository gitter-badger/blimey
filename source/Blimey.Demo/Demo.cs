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
    using Cor;
    using System.Collections.Generic;
    using Abacus.SinglePrecision;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

	public class Demo : App
	{
        public Demo() : base (new Scene_MainMenu()) {}
	}


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

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


            Shader shader = null;
            if (shaderIndex == 0) shader = CommonDemoResources.VertexLitShader;
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

        public static void Create (Engine cor, Blimey blimey)
        {
            var unlitShaderAsset = blimey.Assets.Load<ShaderAsset>("unlit.bba");
            var vertexLitShaderAsset = blimey.Assets.Load<ShaderAsset>("vertex_lit.bba");
            var pixelLitShaderAsset = blimey.Assets.Load<ShaderAsset>("pixel_lit.bba");

            UnlitShader = cor.Graphics.CreateShader (unlitShaderAsset);
            VertexLitShader = cor.Graphics.CreateShader (vertexLitShaderAsset);
            PixelLitShader = cor.Graphics.CreateShader (pixelLitShaderAsset);
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

