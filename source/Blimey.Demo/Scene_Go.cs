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
    using System.Reflection;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class Scene_Go
    : Scene
    {
        Scene returnScene;
        Shader shader = null;
        Texture woodTex = null;
        Entity goBoard = null;

        public override void Start ()
        {
            this.Configuration.BackgroundColour = Rgba32.DarkSlateGrey;
            var goBoardMesh = new CubePrimitive (this.Cor.Graphics);

            MeshAsset mushMeshAsset = this.Blimey.Assets.Load<MeshAsset> ("big_mushroom.bba");
            var vb = Cor.Graphics.CreateVertexBuffer (mushMeshAsset.VertexDeclaration, mushMeshAsset.VertexData.Length);
            var ib = Cor.Graphics.CreateIndexBuffer (mushMeshAsset.IndexData.Length);

            MethodInfo mi = typeof(VertexBuffer).GetMethod ("SetDataR");

            var vertType = mushMeshAsset.VertexData [0].GetType ();
            var gmi = mi.MakeGenericMethod(vertType);

            try
            {
                gmi.Invoke(vb, new [] { mushMeshAsset.VertexData });
            }
            catch (Exception ex)
            {
                throw new Exception (
                    "Failed to invoke SetData for type [" + vertType + "]" +
                    "\n" + ex.Message + 
                    "\n" + ex.InnerException.Message);
            }

            ib.SetData (mushMeshAsset.IndexData);
            var mushMesh = new Mesh (vb, ib);

            // set up the debug renderer
            ShaderAsset unlitShaderAsset = this.Blimey.Assets.Load<ShaderAsset> ("pixel_lit.bba");
            shader = this.Cor.Graphics.CreateShader (unlitShaderAsset);
            TextureAsset woodTexAsset = this.Blimey.Assets.Load <TextureAsset> ("cvan01.bba");
            woodTex = this.Cor.Graphics.CreateTexture (woodTexAsset);
            goBoard = SceneGraph.CreateSceneObject ("go-board");
            goBoard.Transform.LocalPosition = new Vector3 (0f, 0f, 0f);
            goBoard.Transform.LocalScale = new Vector3 (1f, 0.1f, 1f);

            var mat = new Material ("Default", shader);
            mat.SetTexture ("TextureSampler", woodTex);
            MeshRendererTrait meshRendererTrait = goBoard.AddTrait<MeshRendererTrait> ();
            meshRendererTrait.Material = mat;
            meshRendererTrait.Mesh = mushMesh;

            returnScene = this;

            var cam = this.CameraManager.GetRenderPassCamera ("Default");
            cam.GetTrait <OrbitAroundSubjectTrait> ().Active = false;

            this.Blimey.InputEventSystem.Tap += this.OnTap;
        }

        public override void Shutdown ()
        {
            this.Blimey.InputEventSystem.Tap -= this.OnTap;
        }

        public override Scene Update (AppTime time)
        {
            this.Blimey.DebugRenderer.AddGrid ("Debug", 1f, 10);

            if (Cor.Input.GenericGamepad.Buttons.East == ButtonState.Pressed ||
            Cor.Input.Keyboard.IsFunctionalKeyDown (FunctionalKey.Escape) ||
            Cor.Input.Keyboard.IsFunctionalKeyDown (FunctionalKey.Backspace))
            {
                returnScene = new Scene_MainMenu ();
            }

            return returnScene;
        }

        void OnTap (Gesture gesture)
        {
            returnScene = new Scene_MainMenu ();

            woodTex.Dispose ();
            // Clean up the things we allocated on the GPU.
            this.Cor.Graphics.DestroyShader (shader);
            shader = null;
        }
    }
}

