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

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

	public class Boid
    {
        public float influenceTracker;
        public Vector2 position;
        public Vector2 velocity;
        public float angle;

        public Boid(Vector2 startPos)
        {

            angle = 0;

            position = startPos;

            velocity = new Vector2();
			velocity.X = RandomHelper.Random_Float(30f, 50f);
            velocity.Y = RandomHelper.Random_Float(30f, 50f);
        }

        public void Update(float zDt)
        {
			Single pi; Maths.Pi(out pi);

            angle = pi / 2.0f;

            if (velocity.X == 0.0f)
            {
                velocity.X = 2.0f;
            }

            if (velocity.Y == 0.0f)
            {
                velocity.Y = 2.0f;
            }

            if (velocity.X > 0.0f && velocity.Y > 0.0f)
            {
                angle = angle + (float)Math.Atan(Math.Abs(velocity.Y) / Math.Abs(velocity.X));
            }
            else if (velocity.X < 0.0f && velocity.Y > 0.0f)
            {
                angle = angle + (float)Math.Atan(Math.Abs(velocity.X) / Math.Abs(velocity.Y)) + pi / 2;
            }
            else if (velocity.X < 0.0f && velocity.Y < 0.0f)
            {
                angle = angle + (float)Math.Atan(Math.Abs(velocity.Y) / Math.Abs(velocity.X)) + pi;
            }
            else if (velocity.X > 0.0f && velocity.Y < 0.0f)
            {
                angle = angle + (float)Math.Atan(Math.Abs(velocity.X) / Math.Abs(velocity.Y)) + 3 * pi / 2;
            }


        }
    }

	public static class RandomHelper
    {
        internal static Random randomGenerator = null;

        /// Sets the seed for the random generator.  If zero is used as the
        /// parameter then the current time is used as the seed.
        public static Random Random_Seed(int _seed)
        {
            if (_seed == 0)
                randomGenerator = new Random((int)DateTime.Now.Ticks);
            else
                randomGenerator = new Random(_seed);

            return randomGenerator;
        }

        /// Returns a random float.
        public static float Random_Float(float min, float max)
        {
            if (randomGenerator == null)
                randomGenerator = Random_Seed(0);

            return (float)randomGenerator.NextDouble() * (max - min) + min;
        }
    }


	public class BoidManager
    {
		readonly Single cageLeft, cageRight, cageTop, cageBottom;
        public Vector2 averageVelocity;
        List<Boid> prey = new List<Boid>();

		public List <Boid> Prey { get { return prey; } }

		public BoidManager(Int32 _numboids, Single cageLeft, Single cageRight, Single cageTop, Single cageBottom)
        {
			this.cageLeft = cageLeft;
			this.cageRight = cageRight;
			this.cageTop = cageTop;
			this.cageBottom = cageBottom;

            for (int i = 0; i < _numboids; i++)
            {
				Vector2 startPos;
            	startPos.X = RandomHelper.Random_Float(cageLeft, cageRight);
            	startPos.Y = RandomHelper.Random_Float(cageTop, cageBottom);
                prey.Add(new Boid(startPos));
            }
        }

        public void Update(float _dt)
        {
            averageVelocity.X = 0.0f;
            averageVelocity.Y = 0.0f;

            for (int i = 0; i < prey.Count; i++)
            {
                prey[i].velocity = prey[i].velocity
                    + Rule1(prey[i])
                    + Rule2(prey[i])
                    + Rule3(prey[i])
                    //+ FollowMouse(prey[i])
                    + Cage(prey[i])
                    ;

                LimitVelocity(prey[i]);

                prey[i].position = prey[i].position + prey[i].velocity * _dt;
                prey[i].Update(_dt);

                averageVelocity = averageVelocity + prey[i].velocity;
            }
            averageVelocity = averageVelocity / (float)prey.Count;
        }

        public static float awarenessRadius = 600.0f;


        // MOVE TOWARDS THE CENTER OF THE FLOCK
        Vector2 Rule1(Boid _boid)
        {
            _boid.influenceTracker = 0.0f;
            Vector2 PercievedCenterOfMass = new Vector2();
            PercievedCenterOfMass.X = _boid.position.X;
            PercievedCenterOfMass.Y = _boid.position.Y;

            int effectedBy = 0;
            for (int i = 0; i < prey.Count; i++)
            {
                if (prey[i] != _boid)
                {
                    if ((_boid.position - prey[i].position).Length() < awarenessRadius)
                    {
                        PercievedCenterOfMass = PercievedCenterOfMass + prey[i].position;
                        _boid.influenceTracker += 1.0f / 20.0f;
                        effectedBy++;
                    }
                }
            }
            if (_boid.influenceTracker > 1.0f)
                _boid.influenceTracker = 1.0f;


            PercievedCenterOfMass = PercievedCenterOfMass / (float)(effectedBy + 1);

            return (PercievedCenterOfMass - _boid.position) / 500.0f;
        }


        // MOVE AWAY FROM ONE ANOTHER
        Vector2 Rule2(Boid _boid)
        {
            Vector2 c = new Vector2(); ;
            for (int i = 0; i < prey.Count; i++)
            {
                if (prey[i] != _boid)
                {
                    if ((_boid.position - prey[i].position).Length() < awarenessRadius)
                    {
                        Vector2 temp = prey[i].position - _boid.position;

                        if (temp.Length() < 50.0f)
                        {
                            c = c - (prey[i].position - _boid.position);
                        }
                    }

                }
            }

            return c / 10.0f;
        }


        // MATCH VELOCITY WITH THE FLOCK
        Vector2 Rule3(Boid _boid)
        {
            Vector2 PercievedVelocity = new Vector2();
            PercievedVelocity.X = _boid.velocity.X;
            PercievedVelocity.Y = _boid.velocity.Y;

            int effectedBy = 0;
            for (int i = 0; i < prey.Count; i++)
            {
                if (prey[i] != _boid)
                {
                    if ((prey[i].position - _boid.position).Length() < awarenessRadius)
                    {
                        PercievedVelocity = PercievedVelocity + prey[i].velocity;
                        effectedBy++;
                    }
                }
            }
            PercievedVelocity = PercievedVelocity / (float)(effectedBy + 1);

            return (PercievedVelocity - _boid.velocity) / 500.0f;
        }



        Vector2 FollowMouse(Boid _boid)
        {
			Vector2 mouse = Vector2.Zero; //new Vector2(Input.MousePos.X, Input.MousePos.Y);
            return (mouse - _boid.position) / 100.0f;
        }

        Vector2 Cage(Boid _boid)
        {
            Vector2 v = new Vector2();
            if (_boid.position.X < cageLeft)
                v.X = 100.0f;

            if (_boid.position.X > cageRight)
                v.X = -100.0f;

            if (_boid.position.Y < cageTop)
                v.Y = 100.0f;

            if (_boid.position.Y > cageBottom)
                v.Y = -100.0f;


            return v;

        }

        void LimitVelocity(Boid _boid)
        {
            float magmax = 200.0f;

            if (_boid.velocity.Length() > magmax)
            {
                if (_boid.velocity.X == 0.0f || _boid.velocity.Y == 0.0f)
                    return;

                float xsign = _boid.velocity.X / Math.Abs(_boid.velocity.X);
                float ysign = _boid.velocity.Y / Math.Abs(_boid.velocity.Y);

                float r = Math.Abs(_boid.velocity.X / _boid.velocity.Y);

                float tempy = (float)Math.Sqrt((magmax * magmax) / (1.0f + (r * r)));
                float tempx = tempy * r;

                _boid.velocity.Y = tempy * ysign;
                _boid.velocity.X = tempx * xsign;

            }
                 }

    }


    public class Scene_Boids
        : Scene
    {
		BoidManager boidManager;

	    Int32 levelleft;
		Int32 levelright;
        Int32 leveltop;
        Int32 levelbottom;

        Int32 cageleft;
        Int32 cageright;
        Int32 cagetop;
        Int32 cagebottom;

		Int32 numBoids = 100;

        public override void Start()
        {
			var newCamSo = this.SceneGraph.CreateSceneObject("ortho");
            newCamSo.Transform.LocalPosition = new Vector3(0, 0, 1);

            var orthoCam = newCamSo.AddTrait<CameraTrait>();
            orthoCam.NearPlaneDistance = 0;
            orthoCam.FarPlaneDistance = 2;
            orthoCam.Projection = CameraProjectionType.Orthographic;
			orthoCam.ortho_width = this.Cor.Status.Width;
			orthoCam.ortho_height = this.Cor.Status.Height;
			orthoCam.ortho_zoom = 8f;

            this.RuntimeConfiguration.SetRenderPassCameraTo("Default", newCamSo);
            this.RuntimeConfiguration.SetRenderPassCameraTo("Gui", newCamSo);
            this.RuntimeConfiguration.SetRenderPassCameraTo("Debug", newCamSo);
			this.Configuration.BackgroundColour = Rgba32.CornflowerBlue;

			Int32 wpadding = 0;
            Int32 hpadding = 0;

            Int32 extraW = 2000;
            Int32 extraH = 1500;

            levelleft = -wpadding - extraW;
			levelright = this.Cor.Status.Width + wpadding + extraW;
            leveltop = -hpadding - extraH;
            levelbottom = this.Cor.Status.Height + hpadding + extraH;

            cageleft = levelleft + 250;
            cageright = levelright - 250;
            cagetop = leveltop + 250;
            cagebottom = levelbottom - 250;

			boidManager = new BoidManager (numBoids, cageleft, cageright, cagetop, cagebottom);
        }

        public override void Shutdown()
        {
        }

		void UpdateDebugRenderer ()
		{
			float left = (float) levelleft;
            float right = (float) levelright;
            float top = (float) leveltop;
            float bottom = (float)levelbottom;

            this.Blimey.DebugRenderer.AddLine(
                "Gui",
				new Vector3(left, top, 0),
				new Vector3(right, top, 0),
				Rgba32.Red);

            this.Blimey.DebugRenderer.AddLine(
                "Gui",
				new Vector3(left, top, 0),
				new Vector3(left, bottom, 0),
				Rgba32.Red);

            this.Blimey.DebugRenderer.AddLine(
                "Gui",
				new Vector3(left, bottom, 0),
				new Vector3(right, bottom, 0),
				Rgba32.Red);

            this.Blimey.DebugRenderer.AddLine(
                "Gui",
				new Vector3(right, top, 0),
				new Vector3(right, bottom, 0),
				Rgba32.Red);

            left = (float) cageleft;
            right = (float) cageright;
            top = (float) cagetop;
            bottom = (float) cagebottom;

            this.Blimey.DebugRenderer.AddLine(
                "Gui",
				new Vector3(left, top, 0),
				new Vector3(right, top, 0),
				Rgba32.Yellow);

            this.Blimey.DebugRenderer.AddLine(
                "Gui",
				new Vector3(left, top, 0),
				new Vector3(left, bottom, 0),
				Rgba32.Yellow);

            this.Blimey.DebugRenderer.AddLine(
                "Gui",
				new Vector3(left, bottom, 0),
				new Vector3(right, bottom, 0),
				Rgba32.Yellow);

            this.Blimey.DebugRenderer.AddLine(
                "Gui",
				new Vector3(right, top, 0),
				new Vector3(right, bottom, 0),
				Rgba32.Yellow);

			Single boidHalfSize = 15f;
			foreach (var boid in boidManager.Prey)
			{
				this.Blimey.DebugRenderer.AddQuad(
	                "Gui",
					new Vector3(boid.position.X - boidHalfSize, boid.position.Y - boidHalfSize, 0),
					new Vector3(boid.position.X + boidHalfSize, boid.position.Y - boidHalfSize, 0),
					new Vector3(boid.position.X + boidHalfSize, boid.position.Y + boidHalfSize, 0),
					new Vector3(boid.position.X - boidHalfSize, boid.position.Y + boidHalfSize, 0),
					Rgba32.Green);
			}
		}

        public override Scene Update(AppTime time)
        {
			if (Cor.Input.GenericGamepad.Buttons.East == ButtonState.Pressed ||
				Cor.Input.Keyboard.IsFunctionalKeyDown(FunctionalKey.Escape) ||
				Cor.Input.Keyboard.IsFunctionalKeyDown(FunctionalKey.Backspace))
			{
				return new Scene_MainMenu();
			}

			UpdateDebugRenderer();
			boidManager.Update (time.Delta);

			return this;
        }
    }
}
