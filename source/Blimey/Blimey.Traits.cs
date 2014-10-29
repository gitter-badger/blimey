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

namespace Blimey
{
    using System;
    
    using Fudge;
    using Abacus.SinglePrecision;
    using Cor;
    using Cor.Platform;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class DebugRenderer
        : Trait
    {
        public Rgba32 Colour { get; set; }
        public String RenderPass { get; set; }

        public DebugRenderer ()
        {
            this.Colour = Rgba32.Red;
            this.RenderPass = "Debug";
        }

        public override void OnUpdate (Cor.AppTime time)
        {
            BoundingBox b;

            //fuck
            //this.Parent.Transform.Position
            b.Min = this.Parent.Transform.Location.Translation - (this.Parent.Transform.Scale / 2f);
            b.Max = this.Parent.Transform.Location.Translation + (this.Parent.Transform.Scale / 2f);

            this.Parent.Owner.Blimey.DebugShapeRenderer.AddBoundingBox (RenderPass, b, Colour);
        }
    }

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    //
    // LOOK AT SUBJECT
    //
    // This behaviour has many applications, it is very simple.  You must set the Subject
    // member variable and it will change its SceneObject's orientation to look at the subject.
    // Optionally you can set the LockToY member variable which will keep the SceneObjects
    // Up Vector as (0,1,0).  This is good for billboard sprites.
    //
    public sealed class LookAtSubject
        : Trait
    {
        #region SETTINGS (These are values that can be set per instance of this behaviour)

        // The target that this behaviour will look at.
        public Transform Subject = null;

        #endregion


        // UPDATE
        // Override update so that every frame we can alter our parent SceneObject's orientation.
        public override void OnUpdate(AppTime time)
        {
            // If the Subject has not been set then this behviour will just early
            // out without making any changes to the
            if (Subject == null)
                return;

            this.Parent.Transform.LookAt(Subject);
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    //
    // MESH RENDERER
    //
    // This behaviour takes a Blimey.Model and a Material, it then renders the models
    // at location, scale and orientaion of the parent SceneObject's Transform.
    //
    public sealed class MeshRenderer
        : Trait
    {

        public Mesh Mesh { get; set; }
        public Material Material { get; set; }
        public CullMode CullMode { get; set; }

        public MeshRenderer()
        {
            this.Mesh = null;
            this.Material = null;
            this.CullMode = CullMode.CW;
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    //
    // ORBIT AROUND SUBJECT
    //
    // This behaviour takes a Subject Transform and a Speed and uses to oribit it's parent SceneObject
    // around the Subject.  This radius of the orbit is the distance from the parent SceneObject
    // to the Subject.  If this distance changes at runtime the orbit radius will also change.
    //
    public sealed class OrbitAroundSubject
        : Trait
    {
        #region SETTINGS (These are values that can be set per instance of this behaviour)
        public Transform CameraSubject = null;
        public float Speed = 0.1f;
        #endregion

        // UPDATE
        // Override update so that every frame we move the parent SceneObjects transform a little.
        public override void OnUpdate(AppTime time)
        {
            Vector3 offset = this.Parent.Transform.LocalPosition - CameraSubject.Position;

            Matrix44 rotation =
                Matrix44.CreateRotationY(Speed * time.Delta);

            Vector3 offsetIn = offset;

            Vector3.Transform(ref offsetIn, ref rotation, out offset);

            this.Parent.Transform.LocalPosition = offset + CameraSubject.Position;

        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class PointLight
        : Trait
    {
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class SlowRotate
        : Trait
    {
        public override void OnUpdate(AppTime time)
        {
            Single x = time.Delta * Maths.ToRadians(10f);

            Quaternion rot = Quaternion.CreateFromYawPitchRoll(x, 0, 0);

            this.Parent.Transform.LocalRotation *= rot;
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class SpotLight
        : Trait
    {
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class ChaseSubject
        : Trait
    {
        Transform subject;
        Boolean dirty;
        Vector3 desiredPositionOffset;
        Vector3 velocity;

        // The target that this behaviour will chase.
        public Transform Subject
        {
            get { return subject; }
            set
            {
                subject = value;

                // When we set the subject we work out the current vector between us
                // and the target, so that we always try to keep this seperation
                // even if the subject moves.
                // This vector is in world space.
                desiredPositionOffset =
                    this.Parent.Transform.Position -
                    subject.Position;

                //Console.WriteLine(Parent.Name + ": ChaseSubject desiredPositionOffset=" + desiredPositionOffset);
            }
        }

        public float Mass { get; set; }
        public float Damping { get; set; }
        public float Stiffness { get; set; }
        public Boolean SpringEnabled { get; set; }

        public override void OnEnable()
        {
            this.ApplyDefaultSettings();
        }

        public void ApplyDefaultSettings()
        {
            this.ResetSpring();

            // Mass of the camera body.
            // Heaver objects require stiffer springs with less
            // damping to move at the same rate as lighter objects.
            this.Mass = 20.0f;
            this.Damping = 40.0f;
            this.Stiffness = 2000.0f;
            this.SpringEnabled = false;

            this.velocity = Vector3.Zero;
        }

        /// Forces camera to be at desired position and to stop moving. The is useful
        /// when the chased object is first created or after it has been teleported.
        /// Failing to call this after a large change to the chased object's position
        /// will result in the camera quickly flying across the world.
        public void ResetSpring()
        {
            this.dirty = true;
        }

        public override void OnUpdate(AppTime time)
        {
            Vector3 previousPosition = this.Parent.Transform.Position;

            Vector3 desiredPosition = Subject.Position + desiredPositionOffset;

            Vector3 stretch = previousPosition - desiredPosition;
            //Console.WriteLine(Parent.Name + ": ChaseSubject stretch=" + stretch + " - (" + previousPosition + " - " + desiredPosition + ")");

            if (this.dirty || ! SpringEnabled)
            {
                this.dirty = false;

                // Stop motion
                this.velocity = Vector3.Zero;

                // Force desired position
                this.Parent.Transform.Position = desiredPosition;
            }
            else
            {
                // Calculate spring force
                Vector3 force = -this.Stiffness * stretch - this.Damping * this.velocity;

                // Apply acceleration
                Vector3 acceleration = force / this.Mass;
                this.velocity += acceleration * time.Delta;

                // Apply velocity
                Vector3 deltaPosition = this.velocity * time.Delta;
                this.Parent.Transform.Position += deltaPosition;
            }
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class FreeTransform
        : Trait
    {
        public FreeTransform()
        {
        }
    }

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class FreeCam
        : Trait
    {
        public struct FreeCamInputs
        {
            public Vector3 mTranslation;
            public Vector3 mRotation;
            public float mTranslationSpeed; //in range 0-1
            public float mRotationSpeedScale;
            public bool mFixUp;
        }

        float localPitch;
        float localYaw;
        float localRoll;

        Vector3 oldPosition = Vector3.Zero;

        // inputs that come from the controller
        FreeCamInputs mInputs;

        // fre cam settings
        float mTranslationSpeedStandard = 10.0f;
        float mTranslationSpeedMaximum = 100.0f;
        float mRotationSpeed = 45.0f; //30 degrees per second

        public void WorkOutInputs()
        {
            var input = new FreeCamInputs();

            var xbox = this.Parent.Owner.Cor.Input.Xbox360Gamepad;
            var keyboard = this.Parent.Owner.Cor.Input.Keyboard;

            input.mTranslation = new Vector3(
                xbox.Thumbsticks.Left.X,
                0.0f,
                -xbox.Thumbsticks.Left.Y
                );

            input.mRotation = new Vector3(
                -xbox.Thumbsticks.Right.Y,
                -xbox.Thumbsticks.Right.X,
                0.0f
                );

            input.mTranslationSpeed = xbox.Triggers.Right;

            input.mRotationSpeedScale = 1.0f;

            input.mFixUp = keyboard.IsCharacterKeyUp('u');
            SetInputs(input);
        }

        public void SetInputs(FreeCamInputs zIn) { mInputs = zIn; }


        public void Reset()
        {
            //need to change this to that these values tie in with whatever the camera was looking at before
            localPitch=0.0f;
            localYaw = 0.0f;
            localRoll = 0.0f;
            oldPosition = Vector3.Zero;
        }

        public override void OnUpdate(AppTime time)
        {
            WorkOutInputs();

            float translationSpeed = mTranslationSpeedStandard
                + mInputs.mTranslationSpeed *
                (mTranslationSpeedMaximum - mTranslationSpeedStandard);

            Vector3 translation = mInputs.mTranslation * translationSpeed * time.Delta;

            Vector3 rotation =
                mInputs.mRotation *
                Maths.ToRadians(mRotationSpeed) *
                mInputs.mRotationSpeedScale * time.Delta;

            localPitch += rotation.X;
            localYaw += rotation.Y;
            localRoll += rotation.Z;

            Quaternion rotationFromInputs = Quaternion.CreateFromYawPitchRoll(localYaw, localPitch, localRoll);

            Quaternion currentOri = this.Parent.Transform.Rotation;

            this.Parent.Transform.Rotation = Quaternion.Multiply(currentOri, rotationFromInputs);

            float yTranslation = translation.Y;
            translation.Y = 0.0f;

            this.Parent.Transform.Position +=
                oldPosition +
                Vector3.Transform(translation, this.Parent.Transform.Rotation) +
                new Vector3(0.0f, yTranslation, 0.0f);

            //focusDistance = 3.0f;

            //update the old position for next time
            oldPosition = this.Parent.Transform.Position;
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class Sprite
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
	
	        public static SpriteMesh Create(Graphics gfx)
	        {
	            var sm = new SpriteMesh();
                sm.VertexBuffer = gfx.CreateVertexBuffer(
	                VertexPositionTexture.Default.VertexDeclaration,
	                sm.spriteVerts.Length);
                sm.IndexBuffer = gfx.CreateIndexBuffer(
                    sm.spriteIndices.Length);
	
	            sm.VertexBuffer.SetData(sm.spriteVerts);
	
	            sm.IndexBuffer.SetData(sm.spriteIndices);
	
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
		
        static SpriteMesh spriteMesh;

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
        public Sprite()
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
            meshRendererTrait = this.Parent.GetTrait<MeshRenderer>();

            if(meshRendererTrait == null)
            {
                meshRendererTrait = this.Parent.AddTrait<MeshRenderer>();
            }

            if( spriteMesh == null )
            {
                spriteMesh = SpriteMesh.Create(this.Cor.Graphics);
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

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class Camera
        : Trait
    {
        public CameraProjectionType Projection = CameraProjectionType.Perspective;

        // perspective settings
        public Single FieldOfView = Maths.ToRadians(45.0f);

        // orthographic settings
        public Single ortho_depth = 100f;
        public Single ortho_width = 1f;
        public Single ortho_height = 1f;
        public Single ortho_zoom = 1f;

        // clipping planes
        public Single NearPlaneDistance = 1.0f;
        public Single FarPlaneDistance = 10000.0f;

        public Matrix44 ViewMatrix44 { get { return _view; } }

        public Matrix44 ProjectionMatrix44 { get { return _projection; } }

        Matrix44 _projection;
        Matrix44 _view;

        // return this cameras bounding frustum
        //public BoundingFrustum BoundingFrustum { get { return new BoundingFrustum (ViewMatrix44 * ProjectionMatrix44); } }

        public bool TempWORKOUTANICERWAY = false;

        // Allows the game component to update itself.
        public override void OnUpdate (AppTime time)
        {
            var camUp = this.Parent.Transform.Up;

            var camLook = this.Parent.Transform.Forward;

            Vector3 pos = this.Parent.Transform.Position;
            Vector3 target = pos + (camLook * FarPlaneDistance);

            Matrix44.CreateLookAt(
                ref pos,
                ref target,
                ref camUp,
                out _view);

            Single width = (Single) this.Cor.Status.Width;
            Single height = (Single)this.Cor.Status.Height;

            if (Projection == CameraProjectionType.Orthographic)
            {
                if(TempWORKOUTANICERWAY)
                {
                    _projection =
                        Matrix44.CreateOrthographic(
                            width / Sprite.SpriteConfiguration.Default.SpriteSpaceScale,
                            height / Sprite.SpriteConfiguration.Default.SpriteSpaceScale,
                            1, -1);
                }
                else
                {
                    _projection =
                        Matrix44.CreateOrthographicOffCenter(
							-0.5f * ortho_width * ortho_zoom,  +0.5f * ortho_width * ortho_zoom, 
							-0.5f * ortho_height * ortho_zoom, +0.5f * ortho_height * ortho_zoom, 
							+0.5f * ortho_depth,  -0.5f * ortho_depth);
                }
            }
            else
            {
                _projection =
                    Matrix44.CreatePerspectiveFieldOfView (
                        FieldOfView,
                        width / height, // aspect ratio
                        NearPlaneDistance,
                        FarPlaneDistance);
            }
        }
    }
}
