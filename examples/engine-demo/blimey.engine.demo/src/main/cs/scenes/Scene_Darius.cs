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
    using Blimey.Platform;
    using Blimey.Asset;
    using Blimey.Engine;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Linq;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class Scene_Darius : Scene
    {
        Scene returnScene;
        Shader shader = null;
        Texture tex = null;
        Entity entity = null;
        VertexBuffer vb;
        IndexBuffer ib;

        public override void Start ()
        {
            this.Configuration.BackgroundColour = Rgba32.DarkSlateGrey;

            MeshAsset meshAsset = this.Engine.Assets.Load<MeshAsset> ("assets/darius.bba");
            vb = Platform.Graphics.CreateVertexBuffer (meshAsset.VertexDeclaration, meshAsset.VertexData.Length);
            ib = Platform.Graphics.CreateIndexBuffer (meshAsset.IndexData.Length);
            vb.SetDataEx (meshAsset.VertexData);
            ib.SetData (meshAsset.IndexData);
            var mushMesh0 = new Mesh (vb, ib);

            // set up the debug renderer
            ShaderAsset unlitShaderAsset = this.Engine.Assets.Load<ShaderAsset> ("assets/vertex_lit.bba");
            shader = this.Platform.Graphics.CreateShader (unlitShaderAsset);
            TextureAsset texAsset = this.Engine.Assets.Load <TextureAsset> ("assets/bg1.bba");
            tex = this.Platform.Graphics.CreateTexture (texAsset);

            entity = SceneGraph.CreateSceneObject ("entity");
            entity.Transform.LocalPosition = new Vector3 (0f, 0f, 0f);
            entity.Transform.LocalScale = new Vector3 (0.1f, 0.1f, 0.1f);

            var mat = new Material ("Default", shader);
            mat.SetTexture ("TextureSampler", tex);

            MeshRendererTrait meshRendererTrait0 = entity.AddTrait<MeshRendererTrait> ();
            meshRendererTrait0.Material = mat;
            meshRendererTrait0.Mesh = mushMesh0;
            meshRendererTrait0.CullMode = CullMode.None;

            returnScene = this;

            var t = SceneGraph.CreateSceneObject ("T");
            t.Transform.LocalPosition = new Vector3 (0, 2f, 0);
            Entity camSo = SceneGraph.CreateSceneObject ("Scene X Camera");
            camSo.AddTrait<CameraTrait> ();
            var lookatTrait = camSo.AddTrait<LookAtSubjectTrait> ();
            lookatTrait.Subject = t.Transform;
            var orbitTrait = camSo.AddTrait<OrbitAroundSubjectTrait> ();
            orbitTrait.CameraSubject = Transform.Origin;

            camSo.Transform.LocalPosition = new Vector3 (6f,3f,6f);

            this.RuntimeConfiguration.SetRenderPassCameraTo("Debug", camSo);
            this.RuntimeConfiguration.SetRenderPassCameraTo("Default", camSo);

            this.Engine.InputEventSystem.Tap += this.OnTap;
        }

        public override void Shutdown ()
        {
            this.Engine.InputEventSystem.Tap -= this.OnTap;
        }

        public override Scene Update (AppTime time)
        {
            this.Engine.DebugRenderer.AddGrid ("Debug", 1f, 10);

            if (Platform.Input.GenericGamepad.Buttons.East == ButtonState.Pressed ||
                Platform.Input.Keyboard.IsFunctionalKeyDown (FunctionalKey.Escape) ||
                Platform.Input.Keyboard.IsFunctionalKeyDown (FunctionalKey.Backspace))
            {
                returnScene = new Scene_MainMenu ();
            }

            return returnScene;
        }

        void OnTap (Gesture gesture)
        {
            returnScene = new Scene_MainMenu ();

            tex.Dispose ();
            // Clean up the things we allocated on the GPU.
            this.Platform.Graphics.DestroyShader (shader);
            shader = null;
        }
    }
}

