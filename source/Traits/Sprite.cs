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
// │ Copyright © 2013 A.J.Pook (http://sungiant.github.com)                 │ \\
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
using Sungiant.Abacus;
using Sungiant.Abacus.Packed;
using Sungiant.Abacus.SinglePrecision;
using Sungiant.Cor;

/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * LORD ISHMET, PROTECTOR OF THE ANCIENT REALM OF CAMEL                    3UU *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 *                                 __
 *                     .--.      .'  `.
 *                   .' . :\    /   :  L
 *                   F     :\  /   . : |        .-._
 *                  /     :  \/        J      .' ___\
 *                 J     :   /      : : L    /--'   ``.
 *                 F      : J           |  .<'.o.  `-'>
 *                /        J             L \_>.   .--w)
 *               J        /              \_/|   . `-__|
 *               F                        / `    -' /|)
 *              |   :                    J   '        |
 *             .'   ':                   |    .    :  \
 *            /                          J      :     |L
 *           F                              |     \   ||
 *          F .                             |   :      |
 *         F  |                             ; .   :  : F
 *        /   |                                     : J
 *       J    J             )                ;        F
 *       |     L           /      .:'                J
 *    .-'F:     L        ./       :: :       .       F
 *    `-'F:     .\    `:.J         :::.             J
 *      J       ::\    `:|        |::::\            |
 *      J        |:`.    J        :`:::\            F
 *       L   :':/ \ `-`.  \       : `:::|        .-'
 *       |     /   L    >--\         :::|`.    .-'
 *       J    J    |    |   L     .  :::: :`, /
 *        L   F    J    )   |        >::   : /
 *        |  J      L   F   \     .-.:'   . /
 *        ): |     J   /     `-   | |   .--'
 *        /  |     |: J        L  J J   )
 *        L  |     |: |        L   F|   /
 *        \: J     \:  L       \  /  L |
 *         L |      \  |        F|   | )
 *         J F       \ J       J |   |J
 *          L|        \ \      | |   | L
 *          J L        \ \     F \   F |
 *           L\         \ \   J   | J   L
 *          /__\_________)_`._)_  |_/   \_____
 *                              ""   `"""
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 
 * 
 * 
 * TYPE: CREATURE - CAMEL LORD
 * ATTACK:      50
 * HP:        2800
 * 
 * SPECIAL POWER: SPITTLE BOMB ( Deal 900 damage to any )
 * BEST FEATURE: TOES
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
namespace Sungiant.Blimey
{

    class SpriteMesh
        : Mesh
    {
        VertexPositionTexture[] spriteVerts;
        Int32[] spriteIndices;

        private SpriteMesh()
        {
            spriteVerts = new VertexPositionTexture[]
            {
                new VertexPositionTexture ((-Vector3.Right - Vector3.Forward) / 2, new Vector2(0f, 1f)),
                new VertexPositionTexture ((-Vector3.Right + Vector3.Forward) / 2, new Vector2(0f, 0f)),
                new VertexPositionTexture ((Vector3.Right + Vector3.Forward) / 2, new Vector2(1f, 0f)),
                new VertexPositionTexture ((Vector3.Right - Vector3.Forward) / 2, new Vector2(1f, 1f))
            };
            
            spriteIndices = new Int32[]
            {
                0,1,2,
                0,2,3
            };
        }

        public static SpriteMesh Create(IGraphicsManager gfx)
        {
            var sm = new SpriteMesh();
            sm.GeomBuffer = gfx.CreateGeometryBuffer(
                VertexPositionTexture.Default.VertexDeclaration, 
                sm.spriteVerts.Length,
                sm.spriteIndices.Length);
            
            sm.GeomBuffer.VertexBuffer.SetData(sm.spriteVerts);
            
            sm.GeomBuffer.IndexBuffer.SetData(sm.spriteIndices);
            
            sm.TriangleCount = 2;
            sm.VertexCount = 4;
            return sm;
        }

        public override VertexDeclaration VertDecl
        {
            get
            {
                return VertexPositionTexture.Default.VertexDeclaration;
            }
        }
    }
	public class Sprite
		: Trait
	{

        SpriteMesh spriteMesh;

        // all sprites share a quad uploaded to the gpu.
        // right now we are using billboard, however, once
        // texture support is in the sprie will need to define
        // it's own vert data with a vertdecl that supports textures.
        //public static BillboardPrimitive Billboard { get { return billboard; } }
        //static BillboardPrimitive billboard;

        // they also share an unlit shader
		public static IShader UnlitShader { get { return unlitShader; } }
        static IShader unlitShader;

        // Currently this shared constant defines the number of units in world 
        // space a sprite takes up, perhaps this should be a member of each
        // sprite... Not sure yet...
        // so as it stands if your sprite has width of 256 and heigh of 128 in 
        // world space, it will occupy 2.56 x 1.28 units on the face of the
        // plane it is defined to use.
        public const Single cSpriteSpaceScale = 100f;

        public readonly Matrix44 cSpriteSpaceMatrix;
        public readonly Matrix44 cInverseSpriteSpaceMatrix;



        // PRIVATES!

        MeshRenderer meshRendererTrait;

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
        Texture2D   currentTexture;

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
        Texture2D   desiredTexture;


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
        public Texture2D Texture { get { return desiredTexture; } set { desiredTexture = value; } } 

        //--------------------------------------------------------------------//
        public Sprite()
        {
            Matrix44 scale;
            Matrix44.CreateScale(cSpriteSpaceScale, out scale);

			Single piOver2; RealMaths.Pi(out piOver2); piOver2 /= 2;
            //piOver2 /= 2f;
            Matrix44 rotation = Matrix44.Identity;
            Matrix44.CreateRotationX(-piOver2, out rotation);

            // yxz
            //Matrix44.CreateFromYawPitchRoll(0, piOver2, -piOver2, out rotation);

            // defines how to move from world space to sprite space.
            cSpriteSpaceMatrix = rotation * scale;

            Matrix44.Invert(ref cSpriteSpaceMatrix, out cInverseSpriteSpaceMatrix);

            /*

                Matrix44.CreateScale(1f / cSpriteSpaceScale, out a);
                Matrix44.CreateRotationX(-piOver2, out b);

            Matrix44 parp = a * b;

            */
            desiredScale = 1f;			desiredColour = Rgba32.White;
        }

		public override void OnAwake()
		{
            meshRendererTrait = this.Parent.GetTrait<MeshRenderer>();

            if(meshRendererTrait == null)
            {
                meshRendererTrait = this.Parent.AddTrait<MeshRenderer>();
            }

            if( spriteMesh == null )
            {
                spriteMesh = SpriteMesh.Create(this.Cor.Graphics);
            }

            if( unlitShader == null )
            {
                unlitShader = this.Cor.Resources.LoadShader(
                    ShaderType.Unlit
				);
            }

            var mat = new Material("Default", unlitShader);

            meshRendererTrait.Mesh = spriteMesh;
            meshRendererTrait.Material = mat;

			ApplyChanges (true);
		}

        public override void OnUpdate(AppTime time)
        {
            ApplyChanges(false);

			if (!String.IsNullOrWhiteSpace (this.DebugRender)) 
			{
				var halfScale = new Vector3(0.64f, 0.32f, 0);//this.Parent.Transform.Scale / 2f;

				// this is fucked.  shouldn't have to normalise here
				var up = this.Parent.Transform.Up;
				Vector3.Normalise(ref up, out up);

				var right = this.Parent.Transform.Right;
				Vector3.Normalise(ref right, out right);

				var a =   up - right;
				var b =   up + right;
				var c = - up + right;
				var d = - up - right;

				a = this.Parent.Transform.Position + (a * halfScale);
				b = this.Parent.Transform.Position + (b * halfScale);
				c = this.Parent.Transform.Position + (c * halfScale);
				d = this.Parent.Transform.Position + (d * halfScale);

				this.Blimey.DebugShapeRenderer.AddQuad(
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
                Quaternion.CreateFromAxisAngle(ref rotationAxis, desiredRotation, out ssLocalRotation);
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
                Matrix44 newLocation =  spriteSpaceLocalLocation * cInverseSpriteSpaceMatrix;


                //--------------------------------------------------------------
                // PT 4
                // Decompose the inverted matrix to get the result.
                Vector3 resultPos;
                Quaternion resultRot;
                Vector3 resultScale;

                newLocation.Decompose(out resultScale, out resultRot, out resultPos);

                resultRot.Normalise();

                //--------------------------------------------------------------
                // PT 5
                // Apply the result to the parent Scene Object


                this.Parent.Transform.LocalScale = ssLocalScale / cSpriteSpaceScale;//why not resultScale!
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
                {
                    return;
                }

				// then we need to tell the shader which slot to look at
				meshRendererTrait.Material.SetTexture("TextureSampler", desiredTexture);

				// todo: this is all a bit hard coded, it would be good if Cor! had a way of requesting that
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

