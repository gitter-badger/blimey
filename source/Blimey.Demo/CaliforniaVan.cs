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
    public class CaliforniaVan
        : Trait
    {
        Vector2 velocity;
        Single deltaScale;
        Single deltaRotation;

        Sprite sprite;

        public override void OnAwake()
        {
            this.sprite = this.Parent.AddTrait<Sprite>();

            this.sprite.Texture =
                RandomGenerator.Default.GetRandomBoolean() ?
                    Scene4.texVan1 :
                    Scene4.texVan2;

            var bm = BlendMode.Default;

            switch (RandomGenerator.Default.GetRandomInt32 (2))
            {
                case 1: bm = BlendMode.Subtract; break;
                case 2: bm = BlendMode.Additive; break;
                case 3: bm = BlendMode.Opaque; break;
            }

            this.sprite.Material.BlendMode = bm;

            this.sprite.DebugRender = null;

            Single width = (Single) Scene4.screenWidth / 2f;
            Single height = (Single) Scene4.screenHeight / 2f;

            this.velocity = RandomGenerator.Default.GetRandomVector2(-200, 200);
            this.deltaRotation = RandomGenerator.Default.GetRandomSingle(-0.5f, 0.5f);

            this.deltaScale = RandomGenerator.Default.GetRandomSingle(-0.2f, 0.2f);

            Single pi;
            RealMaths.Pi(out pi);

            this.sprite.Rotation = RandomGenerator.Default.GetRandomSingle(0, pi);
            Single x = RandomGenerator.Default.GetRandomSingle( 0, width );
            Single y = RandomGenerator.Default.GetRandomSingle( 0 , height );
            this.sprite.Position = new Vector2(x, y);
            this.sprite.Scale = RandomGenerator.Default.GetRandomSingle(0.8f, 1.2f);

            this.sprite.Width = 128f;
            this.sprite.Height = 64f;

            this.sprite.Colour = Rgba32.White; //RandomGenerator.Default.GetRandomColour();
            //this.sprite.FlipVertical = RandomGenerator.Default.GetRandomBoolean();
            //this.sprite.FlipHorizontal = RandomGenerator.Default.GetRandomBoolean();
        }

        public void EnabledDebugRenderer (Boolean on)
        {
            this.sprite.DebugRender = on ? "Default" : null;
        }

        public override void OnUpdate(AppTime time)
        {
            Single width = (Single) Scene4.screenWidth / 2f;
            Single height = (Single) Scene4.screenHeight / 2f;

            this.sprite.Position += this.velocity * time.Delta;

            if (this.sprite.Position.X > width || this.sprite.Position.X < -width)
            {
                this.velocity.X = -this.velocity.X;
            }

            if (this.sprite.Position.Y > height || this.sprite.Position.Y < -height)
            {
                this.velocity.Y = -this.velocity.Y;
            }

            this.sprite.Scale += this.deltaScale * time.Delta;

            if (this.sprite.Scale > 1.2f || this.sprite.Scale < 0.8f)
            {
                this.deltaScale = -this.deltaScale;
            }

            this.sprite.Rotation += this.deltaRotation * time.Delta;


        }

        public override void OnDestroy ()
        {

        }
    }
}

