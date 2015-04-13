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

namespace Blimey.Engine
{
    using System;
    using Fudge;
    using global::Blimey.Platform;
    using global::Blimey.Asset;
    using Abacus.SinglePrecision;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class SpriteTrait
        : Trait
    {
        public struct SpriteConfiguration
        {
            static SpriteConfiguration sprConf;

            static SpriteConfiguration()
            {
                Single piOver2;
                Maths.Pi(out piOver2);
                piOver2 /= 2;
                Single minusPiOver2 = -piOver2;
                Matrix44 rotation = Matrix44.Identity;
                Matrix44.CreateRotationX(ref minusPiOver2, out rotation);
                Quaternion q;
                Quaternion.CreateFromRotationMatrix(ref rotation, out q);


                sprConf = new SpriteConfiguration()
                {
                    SpriteSpaceScale = 100f,
                    SpriteSpaceOrientation = q
                };
            }

            public static SpriteConfiguration Default { get { return sprConf; } }

            // Defines the number of units in world
            // space a sprite takes up, perhaps this should be a member of each
            // sprite... Not sure yet...
            // so as it stands if your sprite has width of 256 and heigh of 128 in
            // world space, it will occupy 2.56 x 1.28 units on the face of the
            // plane it is defined to use.
            public Single SpriteSpaceScale { get; set; }

            public Quaternion SpriteSpaceOrientation { get; set; }

        }

    static class SpriteMesh
      {
          public static Mesh Create(Graphics gfx)
          {
                var spriteVerts = new VertexPositionTexture[]
                {
                    new VertexPositionTexture ((-Vector3.Right - Vector3.Forward) / 2, new Vector2(0f, 1f)),
                    new VertexPositionTexture ((-Vector3.Right + Vector3.Forward) / 2, new Vector2(0f, 0f)),
                    new VertexPositionTexture ((Vector3.Right + Vector3.Forward) / 2, new Vector2(1f, 0f)),
                    new VertexPositionTexture ((Vector3.Right - Vector3.Forward) / 2, new Vector2(1f, 1f))
                };

                var spriteIndices = new Int32[]
                {
                    0,1,2,
                    0,2,3
                };

                var vertexBuffer = gfx.CreateVertexBuffer(
                  VertexPositionTexture.Default.VertexDeclaration,
                  spriteVerts.Length);
                var indexBuffer = gfx.CreateIndexBuffer(
                    spriteIndices.Length);

              vertexBuffer.SetData(spriteVerts);
              indexBuffer.SetData(spriteIndices);

                return new Mesh (vertexBuffer, indexBuffer);
          }

      }

        static Mesh spriteMesh;

        // all sprites share a quad uploaded to the gpu.
        // right now we are using billboard, however, once
        // texture support is in the sprie will need to define
        // it's own vert data with a vertdecl that supports textures.
        //public static BillboardPrimitive Billboard { get { return billboard; } }
        //static BillboardPrimitive billboard;

        // they also share an unlit shader
        public static Shader SpriteShader
        {
            get { return spriteShader; }
            set { spriteShader = value; }
        }

        static Shader spriteShader;

        // defines how to move from world space to sprite space.
        Matrix44 spriteSpaceMatrix;
        public Matrix44 SpriteSpaceMatrix { get { return spriteSpaceMatrix; } }

        // defines how to move from sprite space to world space.
        Matrix44 inverseSpriteSpaceMatrix;
        public Matrix44 InverseSpriteSpaceMatrix { get { return inverseSpriteSpaceMatrix; } }

        SpriteConfiguration conf;

        public SpriteConfiguration Configuration
        {
            get { return conf; }
            set
            {
                conf = value;
                this.CalculateTransforms();
            }
        }


        // PRIVATES!

        MeshRendererTrait meshRendererTrait;

        // track the current status of the sprite
        Single      currentWidth;
        Single      currentHeight;
        Single      currentDepth;
        Vector2     currentPosition;
        Single      currentRotation;
        Single      currentScale;
        Boolean     currentFlipHorizontal;
        Boolean     currentFlipVertical;
        Rgba32      currentColour;
        Texture     currentTexture;

        // track the desired status of the sprite
        Single      desiredWidth;
        Single      desiredHeight;
        Single      desiredDepth;
        Vector2     desiredPosition;
        Single      desiredRotation;
        Single      desiredScale;
        Boolean     desiredFlipHorizontal;
        Boolean     desiredFlipVertical;
        Rgba32      desiredColour;
        Texture     desiredTexture;


        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         * PUBLIC TRAIT VARIABLES                                            *
         * ----------------------                                            *
         * These are how the user defines the state of the sprite.           *
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        // defines the name of the renderpass to perform a debug render, if null or empty then ignored
        public String DebugRender { get; set; }
        public Material Material { get { return meshRendererTrait.Material; } set { meshRendererTrait.Material = value; } }

        public Single Width { get { return desiredWidth; } set { desiredWidth = value; } }
        public Single Height { get { return desiredHeight; } set { desiredHeight = value; } }
        public Single Depth { get { return desiredDepth; } set { desiredDepth = value; } }
        public Vector2 Position { get { return desiredPosition; } set { desiredPosition = value; } }
        public Single Rotation { get { return desiredRotation; } set { desiredRotation = value; } }
        public Single Scale { get { return desiredScale; } set { desiredScale = value; } }
        public Boolean FlipHorizontal { get { return desiredFlipHorizontal; } set { desiredFlipHorizontal = value; } }
        public Boolean FlipVertical { get { return desiredFlipVertical; } set { desiredFlipVertical = value; } }
        public Rgba32 Colour { get { return desiredColour; } set { desiredColour = value; } }
        public Texture Texture { get { return desiredTexture; } set { desiredTexture = value; } }

        //--------------------------------------------------------------------//
        public SpriteTrait()
        {
            Configuration = SpriteConfiguration.Default;
            desiredColour = Rgba32.White;
            desiredScale = 1f;
        }

        void CalculateTransforms()
        {
            Matrix44 scale =
                Matrix44.CreateScale(Configuration.SpriteSpaceScale);

            Quaternion q = Configuration.SpriteSpaceOrientation;
            Matrix44 rotation;
            Matrix44.CreateFromQuaternion(
                ref q,
                out rotation);


            // defines how to move from world space to sprite space.
            spriteSpaceMatrix = rotation * scale;

            Matrix44.Invert(ref spriteSpaceMatrix, out inverseSpriteSpaceMatrix);
        }

        public override void OnAwake()
        {
            meshRendererTrait = this.Parent.GetTrait<MeshRendererTrait>();

            if(meshRendererTrait == null)
            {
                meshRendererTrait = this.Parent.AddTrait<MeshRendererTrait>();
            }

            if( spriteMesh == null )
            {
                spriteMesh = SpriteMesh.Create(this.Platform.Graphics);
            }

            if (spriteShader == null)
            {
                // todo, need a better way to configure this.
                throw new Exception ("Sprite.SpriteShader must be set by user.");
            }

            var mat = new Material("Default", spriteShader);

            meshRendererTrait.Mesh = spriteMesh;
            meshRendererTrait.Material = mat;

            ApplyChanges (true);
        }

        public override void OnUpdate(AppTime time)
        {
            ApplyChanges(false);

            if (!String.IsNullOrWhiteSpace (this.DebugRender))
            {
                var yScale =
                    this.Parent.Transform.Scale.Z / 2;

                // this is fucked.  shouldn't have to normalise here
                var up = this.Parent.Transform.Location.Forward;
                Vector3.Normalise(ref up, out up);

                var xScale =
                    this.Parent.Transform.Scale.X / 2;

                // this is fucked.  shouldn't have to normalise here
                var right = this.Parent.Transform.Location.Right;
                Vector3.Normalise(ref right, out right);

                var a =   (up * yScale) - (right * xScale);
                var b =   (up * yScale) + (right * xScale);
                var c = - (up * yScale) + (right * xScale);
                var d = - (up * yScale) - (right * xScale);

                a = this.Parent.Transform.LocalPosition + a;
                b = this.Parent.Transform.LocalPosition + b;
                c = this.Parent.Transform.LocalPosition + c;
                d = this.Parent.Transform.LocalPosition + d;

                this.Engine.DebugRenderer.AddQuad(
                    this.DebugRender,
                    a,
                    b,
                    c,
                    d,
                    Rgba32.Red
                    );
            }
        }


        // Called when something has changed to update the sprite's state.
        void ApplyChanges(bool forceApply)
        {
            if( currentWidth != desiredWidth ||
                currentHeight != desiredHeight ||
                currentDepth != desiredDepth ||
                currentPosition != desiredPosition ||
                currentRotation != desiredRotation ||
                currentScale != desiredScale ||
                forceApply)
            {
                //--------------------------------------------------------------
                // PT 1
                // work out where the object is in sprite space
                // from the sprites settingY
                Vector3 ssLocalPostion = new Vector3(desiredPosition.X, desiredDepth, -desiredPosition.Y);

                Quaternion ssLocalRotation;
                Vector3 rotationAxis = Vector3.Up;
                Quaternion.CreateFromAxisAngle(ref rotationAxis, ref desiredRotation, out ssLocalRotation);
                ssLocalRotation.Normalise();

                Vector3 ssLocalScale = new Vector3(
                    desiredWidth * desiredScale,
                    desiredScale,
                    desiredHeight * desiredScale
                    );


                //--------------------------------------------------------------
                // PT 2
                // Convert this to a Matrix44
                Matrix44 scale;
                Matrix44.CreateScale(ref ssLocalScale, out scale);

                Matrix44 rotation;
                Matrix44.CreateFromQuaternion(ref ssLocalRotation, out rotation);

                Matrix44 spriteSpaceLocalLocation =  rotation * scale;

                //Matrix44 translation;
                //Matrix44.CreateScale(ref ssLocalPostion, out translation);
                //spriteSpaceLocalLocation = translation * spriteSpaceLocalLocation;
                spriteSpaceLocalLocation.Translation = ssLocalPostion;

                //--------------------------------------------------------------
                // PT 3
                // next use the inverse SpriteSpace matrix to transform the above into world space
                Matrix44 newLocation =  spriteSpaceLocalLocation * inverseSpriteSpaceMatrix;


                //--------------------------------------------------------------
                // PT 4
                // Decompose the inverted matrix to get the result.
                Vector3 resultPos;
                Quaternion resultRot;
                Vector3 resultScale;
                Boolean decomposeOk;

                Matrix44.Decompose(
                    ref newLocation, out resultScale, out resultRot,
                    out resultPos, out decomposeOk);

                resultRot.Normalise();

                //--------------------------------------------------------------
                // PT 5
                // Apply the result to the parent Scene Object

                this.Parent.Transform.LocalScale = ssLocalScale / this.conf.SpriteSpaceScale; //why not resultScale!
                this.Parent.Transform.LocalRotation = resultRot;
                this.Parent.Transform.LocalPosition = resultPos;

                //this.Parent.Transform.Rotation = newLocation.

                currentWidth = desiredWidth;
                currentHeight = desiredHeight;
                currentDepth = desiredDepth;
                currentPosition = desiredPosition;
                currentRotation = desiredRotation;
                currentScale = desiredScale;
            }

            if(currentTexture != desiredTexture || forceApply)
            {
                currentTexture = desiredTexture;

                if(meshRendererTrait.Material == null)
                    return;

                // then we need to tell the shader which slot to look at
                meshRendererTrait.Material.SetTexture("TextureSampler", desiredTexture);

                // todo: this is all a bit hard coded, it would be good if Platform! had a way of requesting that
                // a texture gets about to an unused slot, then reporting the slot number so we can use it.
            }

            if(currentFlipHorizontal != desiredFlipHorizontal || forceApply)
            {
                currentFlipHorizontal = desiredFlipHorizontal;
            }

            if(currentFlipVertical != desiredFlipVertical || forceApply)
            {
                currentFlipVertical = desiredFlipVertical;
            }

            if(currentColour != desiredColour || forceApply)
            {
                if(meshRendererTrait.Material == null)
                {
                    return;
                }

                meshRendererTrait.Material.SetColour("MaterialColour", desiredColour);

                currentColour = desiredColour;
            }
        }
    }
}
