// ┌────────────────────────────────────────────────────────────────────────┐ \\
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
    using Fudge;
    using Abacus.SinglePrecision;
    using Blimey;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Linq;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class Scene_Go : Scene
    {
        Scene returnScene;
        Shader shader = null;
        Texture woodTex = null;
        Entity mush0 = null;
        Entity mush1 = null;
        Entity mush2 = null;
        VertexBuffer vb0;
        IndexBuffer ib0;
        VertexBuffer vb1;
        IndexBuffer ib1;
        VertexBuffer vb2;
        IndexBuffer ib2;

        public override void Start ()
        {
            this.Configuration.BackgroundColour = Rgba32.DarkSlateGrey;
            //var goBoardMesh = new CubePrimitive (this.Cor.Graphics).Mesh;

            MeshAsset mushMeshAsset0 = this.Blimey.Assets.Load<MeshAsset> ("assets/big_mushroom.bba");
            vb0 = Cor.Graphics.CreateVertexBuffer (mushMeshAsset0.VertexDeclaration, mushMeshAsset0.VertexData.Length);
            ib0 = Cor.Graphics.CreateIndexBuffer (mushMeshAsset0.IndexData.Length);
            vb0.SetDataEx (mushMeshAsset0.VertexData);
            ib0.SetData (mushMeshAsset0.IndexData);
            var mushMesh0 = new Mesh (vb0, ib0);

            MeshAsset mushMeshAsset1 = this.Blimey.Assets.Load<MeshAsset> ("assets/small_mushroom_1.bba");
            vb1 = Cor.Graphics.CreateVertexBuffer (mushMeshAsset1.VertexDeclaration, mushMeshAsset1.VertexData.Length);
            ib1 = Cor.Graphics.CreateIndexBuffer (mushMeshAsset1.IndexData.Length);
            vb1.SetDataEx (mushMeshAsset1.VertexData);
            ib1.SetData (mushMeshAsset1.IndexData);
            var mushMesh1 = new Mesh (vb1, ib1);

            MeshAsset mushMeshAsset2 = this.Blimey.Assets.Load<MeshAsset> ("assets/small_mushroom_2.bba");
            vb2 = Cor.Graphics.CreateVertexBuffer (mushMeshAsset2.VertexDeclaration, mushMeshAsset2.VertexData.Length);
            ib2 = Cor.Graphics.CreateIndexBuffer (mushMeshAsset2.IndexData.Length);
            vb2.SetDataEx (mushMeshAsset2.VertexData);
            ib2.SetData (mushMeshAsset2.IndexData);
            var mushMesh2 = new Mesh (vb2, ib2);

            // set up the debug renderer
            ShaderAsset unlitShaderAsset = this.Blimey.Assets.Load<ShaderAsset> ("assets/pixel_lit.bba");
            shader = this.Cor.Graphics.CreateShader (unlitShaderAsset);
            TextureAsset woodTexAsset = this.Blimey.Assets.Load <TextureAsset> ("assets/toadstool_diffuse.bba");
            woodTex = this.Cor.Graphics.CreateTexture (woodTexAsset);

            mush0 = SceneGraph.CreateSceneObject ("mush0");
            mush0.Transform.LocalPosition = new Vector3 (0f, 0f, 0f);
            mush0.Transform.LocalScale = new Vector3 (1f, 1f, 1f);

            mush1 = SceneGraph.CreateSceneObject ("mush1");
            mush1.Transform.LocalPosition = new Vector3 (0.8f, 0f, 0.8f);
            mush1.Transform.LocalScale = new Vector3 (1f, 1f, 1f);

            mush2 = SceneGraph.CreateSceneObject ("mush2");
            mush2.Transform.LocalPosition = new Vector3 (0.5f, 0f, 0f);
            mush2.Transform.LocalScale = new Vector3 (1f, 1f, 1f);

            var mat = new Material ("Default", shader);
            mat.SetTexture ("TextureSampler", woodTex);

            MeshRendererTrait meshRendererTrait0 = mush0.AddTrait<MeshRendererTrait> ();
            meshRendererTrait0.Material = mat;
            meshRendererTrait0.Mesh = mushMesh0;
            meshRendererTrait0.CullMode = CullMode.None;

            MeshRendererTrait meshRendererTrait1 = mush1.AddTrait<MeshRendererTrait> ();
            meshRendererTrait1.Material = mat;
            meshRendererTrait1.Mesh = mushMesh1;
            meshRendererTrait1.CullMode = CullMode.None;

            MeshRendererTrait meshRendererTrait2 = mush2.AddTrait<MeshRendererTrait> ();
            meshRendererTrait2.Material = mat;
            meshRendererTrait2.Mesh = mushMesh2;
            meshRendererTrait2.CullMode = CullMode.None;

            returnScene = this;

            var t = SceneGraph.CreateSceneObject ("T");
            t.Transform.LocalPosition = new Vector3 (0, 1f, 0);
            Entity camSo = SceneGraph.CreateSceneObject ("Scene X Camera");
            camSo.AddTrait<CameraTrait>();
            var lookatTrait = camSo.AddTrait<LookAtSubjectTrait>();
            lookatTrait.Subject = t.Transform;
            var orbitTrait = camSo.AddTrait<OrbitAroundSubjectTrait>();
            orbitTrait.CameraSubject = Transform.Origin;

            camSo.Transform.LocalPosition = new Vector3(6f,3f,6f);

            this.RuntimeConfiguration.SetRenderPassCameraTo("Debug", camSo);
            this.RuntimeConfiguration.SetRenderPassCameraTo("Default", camSo);

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

