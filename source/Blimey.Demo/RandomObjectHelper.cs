// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ Blimey - Fast, efficient, high level engine built upon Cor & Abacus    │ \\
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
// │ Copyright © 2014 A.J.Pook (http://ajpook.github.io)                    │ \\
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
using Abacus;
using Abacus.Packed;
using Abacus.SinglePrecision;
using Cor;
using System.Collections.Generic;

namespace Blimey.Demo
{
    public static class RandomObjectHelper
    {
        public static List<SceneObject> Generate(Scene scene)
        {
            var objects = new List<SceneObject>();
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


        public static SceneObject CreateShapeGO(Scene scene, string renderPass,  Mesh modelpart, int shaderIndex = 0)
        {
            // create a game object
            SceneObject testGO = scene.CreateSceneObject ("test");


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

            IShader shader = null;

            if (shaderIndex == 0)
                shader = scene.Cor.Resources.LoadShader(ShaderType.VertexLit);
            else if (shaderIndex == 1)
                shader = scene.Cor.Resources.LoadShader(ShaderType.VertexLit);
            else
                shader = scene.Cor.Resources.LoadShader(ShaderType.Unlit);

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

}

