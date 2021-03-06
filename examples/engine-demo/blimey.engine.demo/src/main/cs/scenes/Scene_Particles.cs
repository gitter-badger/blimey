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

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class Scene_Particles
        : Scene
    {
        Scene returnScene = null;

        Triple q;
        SpritePrimitive s;
        PrimitiveParticleSystem ps;

        Texture tex1 = null;
        Texture tex2 = null;

        public override void Start ()
        {
            //var meshAsset = Engine.Assets.Load <MeshAsset> ();

            //var vb = Platform.Graphics.CreateVertexBuffer (meshAsset.VertexDeclaration, meshAsset.VertexCount);
            //vb.SetData <REFLECTION> ()


            var ta = Engine.Assets.Load <TextureAsset> ("assets/cvan01.bba");
            tex1 = Platform.Graphics.CreateTexture (ta);
            var tb = Engine.Assets.Load <TextureAsset> ("assets/bg2.bba");
            tex2 = Platform.Graphics.CreateTexture (tb);


            q = new Triple ();
            q.blend = BlendMode.Default;
            q.tex = tex1;
            q.v [0].Colour = Rgba32.Blue;
            q.v [0].Position.X = 0.0f;
            q.v [0].Position.Y = 0.0f;
            q.v [0].UV = new Vector2 (0, 0);
            q.v [1].Colour = Rgba32.Green;
            q.v [1].Position.X = 0.5f;
            q.v [1].Position.Y = 0.5f;
            q.v [1].UV = new Vector2 (1f, 1f);
            q.v [2].Colour = Rgba32.Red;
            q.v [2].Position.X = 0f;
            q.v [2].Position.Y = 0.5f;
            q.v [2].UV = new Vector2 (0, 1f);
            returnScene = this;

            s = new SpritePrimitive (this.Engine.PrimitiveRenderer, tex2, 64, 64, 256, 256);
            s.SetBlendMode (BlendMode.Default);



            var psi = new PrimitiveParticleSystemInfo ();
            psi.sprite = s;
            psi.fLifetime = 3f;
            psi.colColourStart = Rgba32.Red;
            psi.colColourEnd = Rgba32.Yellow;
            psi.nEmission = 10;
            psi.fSpinStart = 0.3f;
            psi.fRadialAccel = 0.1f;
            psi.fSpeed = 3f;
            psi.fSizeVar = 0.1f;


            ps = new PrimitiveParticleSystem (psi);

        }

        public override void Shutdown()
        {
            tex1.Dispose ();
            tex2.Dispose ();
        }

        public override Scene Update(AppTime time)
        {
            //this.Engine.PrimitiveRenderer.AddTriple ("Debug", q);
            //this.Engine.PrimitiveRenderer.AddTriple ("Gui", q);
            //s.Draw4V ("Gui",
            //    0.0f, 0.0f,
            //    0.5f, 0.0f,
            //    0.0f, 0.5f,
            //    0.5f, 0.5f);

            s.DrawEx ("Gui", 0f, 0f, 0.5f, 1f / 256f / 4f, 1f / 256f / 4f);

            //s.Draw ("Gui", 0f, 0f);
            ps.Fire ();
            ps.Draw ("Default");

            this.Engine.DebugRenderer.AddGrid ("Debug");
            if (Platform.Input.GenericGamepad.Buttons.East == ButtonState.Pressed ||
                Platform.Input.Keyboard.IsFunctionalKeyDown(FunctionalKey.Escape) ||
                Platform.Input.Keyboard.IsFunctionalKeyDown(FunctionalKey.Backspace))
            {
                returnScene = new Scene_MainMenu();
            }

            return returnScene;
        }
    }
}
