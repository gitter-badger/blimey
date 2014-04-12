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
using System.Runtime.InteropServices;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Abacus;
using Abacus.Packed;
using Abacus.SinglePrecision;
using Abacus.Int32Precision;
using System.Linq;
using Cor;

namespace Blimey
{
    public class BlimeyApp
        : IApp
    {
        Scene startScene;
        SceneManager sceneManager;

        FpsHelper fps;
        FrameBufferHelper frameBuffer;
        LoggingHelper log;

        public ICor cor;

        public BlimeyApp(Scene startScene)
        {
            this.startScene = startScene;
        }

        public void Initilise(ICor cor)
        {
            this.cor = cor;

            fps = new FpsHelper();
            frameBuffer = new FrameBufferHelper(this.cor.Graphics);
            log = new LoggingHelper(this.cor.System);
            log.Heading();
            log.SystemDetails();

            this.sceneManager = new SceneManager(this.cor, startScene);
        }

        public Boolean Update(AppTime time)
        {
            FrameStats.SlowLog ();
            FrameStats.Reset ();

            using (new ProfilingTimer(t => FrameStats.UpdateTime += t))
            {
                fps.Update(time);
                frameBuffer.Update(time);

                return this.sceneManager.Update(time);
            }
        }

        public void Render()
        {
            using (new ProfilingTimer(t => FrameStats.RenderTime += t))
            {
                fps.LogRender();
                frameBuffer.Clear();
                this.sceneManager.Render();
            }
        }
    }

    internal class BlimeyContext
        : IBlimey
    {
        ICor cor;

        internal BlimeyContext(ICor cor, SceneSettings settings)
        {
            this.cor = cor;

            this.InputEventSystem = new InputEventSystem(this.cor);
            this.DebugShapeRenderer = new DebugShapeRenderer(
                this.cor,
                settings.RenderPasses);

        }

        public InputEventSystem InputEventSystem { get; set; }
        public DebugShapeRenderer DebugShapeRenderer { get; set; }

        internal void PreUpdate (AppTime time)
        {
            this.DebugShapeRenderer.Update(time);
            this.InputEventSystem.Update(time);
        }

        internal void PostUpdate(AppTime time)
        {
            
        }

        internal void PreRender()
        {
        }

        internal void PostRender()
        {
            
        }
    }

    public static class BlimeyMathsHelper
    {
        public static Single Distance(Single value1, Single value2)
        {
            return Math.Abs((Single)(value1 - value2));
        }

        public static T Clamp<T>(T value, T min, T max)
            where T : System.IComparable<T>
        {
            T result = value;

            if (value.CompareTo(max) > 0)
                result = max;

            if (value.CompareTo(min) < 0)
                result = min;

            return result;
        }

        public static Single Limit(ref Single zItem, Single zLower, Single zUpper)
        {
            if (zItem < zLower)
            {
                zItem = zLower;
            }

            else if (zItem > zUpper)
            {
                zItem = zUpper;
            }

            return zItem;
        }

        public static Single Wrap(ref Single zItem, Single zLower, Single zUpper)
        {
            while (zItem < zLower)
            {
                zItem += (zUpper - zLower);
            }

            while (zItem >= zUpper)
            {
                zItem -= (zUpper - zLower);
            }

            return zItem;
        }

        public static Quaternion EulerToQuaternion(Vector3 e)
        {
            
            Single x = RealMaths.ToRadians(e.X);
            Single y = RealMaths.ToRadians(e.Y);
            Single z = RealMaths.ToRadians(e.Z);

            Quaternion result;
            Quaternion.CreateFromYawPitchRoll(ref x, ref y, ref z, out result);
            return result;
        }

        public static Vector3 QuaternionToEuler(Quaternion rotation)
        {
            // This bad boy works, taken from: 

            Single q2 = rotation.I;
            Single q1 = rotation.J;
            Single q3 = rotation.K;
            Single q0 = rotation.U;

            Vector3 angles = Vector3.Zero;

            // METHOD 1: http://forums.create.msdn.com/forums/p/28687/159870.aspx

            angles.X = (Single)Math.Atan2(2 * (q0 * q1 + q2 * q3), 1 - 2 * (Math.Pow(q1, 2) + Math.Pow(q2, 2)));
            angles.Y = (Single)Math.Asin(2 * (q0 * q2 - q3 * q1));
            angles.Z = (Single)Math.Atan2(2 * (q0 * q3 + q1 * q2), 1 - 2 * (Math.Pow(q2, 2) + Math.Pow(q3, 2)));


            // METHOD 2: http://forums.create.msdn.com/forums/p/4574/23763.aspx
            //angles.X = (Single)Math.Atan2(2 * q1 * q0 - 2 * q2 * q3, 1 - 2 * Math.Pow(q1, 2) - 2 * Math.Pow(q3, 2));
            //angles.Z = (Single)Math.Asin(2 * q2 * q1 + 2 * q3 * q0);
            //angles.Y = (Single)Math.Atan2(2 * q2 * q0 - 2 * q1 * q3, 1 - 2 * Math.Pow(q2, 2) - 2 * Math.Pow(q3, 2));
            //if (q2 * q1 + q3 * q0 == 0.5)
            //{
            //    angles.X = (Single)(2 * Math.Atan2(q2, q0));
            //    angles.Y = 0;
            //}
            //else if (q2 * q1 + q3 * q0 == -0.5)
            //{
            //    angles.X = (Single)(-2 * Math.Atan2(q2, q0));
            //    angles.Y = 0;
            //}

            // METHOD 3: http://forums.create.msdn.com/forums/p/4574/23763.aspx
            //const Single Epsilon = 0.0009765625f;
            //const Single Threshold = 0.5f - Epsilon;
            //Single XY = q2 * q1;
            //Single ZW = q3 * q0;
            //Single TEST = XY + ZW;
            //if (TEST < -Threshold || TEST > Threshold)
            //{
            //    int sign = Math.Sign(TEST);
            //    angles.X = sign * 2 * (Single)Math.Atan2(q2, q0);
            //    angles.Y = sign * MathHelper.PiOver2;
            //    angles.Z = 0;
            //}
            //else
            //{
            //    Single XX = q2 * q2;
            //    Single XZ = q2 * q3;
            //    Single XW = q2 * q0;
            //    Single YY = q1 * q1;
            //    Single YW = q1 * q0;
            //    Single YZ = q1 * q3;
            //    Single ZZ = q3 * q3;
            //    angles.X = (Single)Math.Atan2(2 * YW - 2 * XZ, 1 - 2 * YY - 2 * ZZ);
            //    angles.Y = (Single)Math.Atan2(2 * XW - 2 * YZ, 1 - 2 * XX - 2 * ZZ);
            //    angles.Z = (Single)Math.Asin(2 * TEST);
            //}


            angles.X = RealMaths.ToDegrees(angles.X);
            angles.Y = RealMaths.ToDegrees(angles.Y);
            angles.Z = RealMaths.ToDegrees(angles.Z);


            return angles;
        }



        public static Vector3 QuaternionToYawPitchRoll(Quaternion q)
        {

            const Single Epsilon = 0.0009765625f;
            const Single Threshold = 0.5f - Epsilon;

            Single yaw;
            Single pitch;
            Single roll;

            Single XY = q.I * q.J;
            Single ZW = q.K * q.U;

            Single TEST = XY + ZW;

            if (TEST < -Threshold || TEST > Threshold)
            {

                int sign = Math.Sign(TEST);

                yaw = sign * 2 * (Single)Math.Atan2(q.I, q.U);

                Single piOver2;
                RealMaths.Pi(out piOver2);
                piOver2 /= 2;

                pitch = sign * piOver2;

                roll = 0;

            }
            else
            {

                Single XX = q.I * q.I;
                Single XZ = q.I * q.K;
                Single XW = q.I * q.U;

                Single YY = q.J * q.J;
                Single YW = q.J * q.U;
                Single YZ = q.J * q.K;

                Single ZZ = q.K * q.K;

                yaw = (Single)Math.Atan2(2 * YW - 2 * XZ, 1 - 2 * YY - 2 * ZZ);

                pitch = (Single)Math.Atan2(2 * XW - 2 * YZ, 1 - 2 * XX - 2 * ZZ);

                roll = (Single)Math.Asin(2 * TEST);

            }

            return new Vector3(yaw, pitch, roll);

        }

        public static Boolean CheckThatAllComponentsAreValidNumbers(Vector3 zVec)
        {
            if (Single.IsNaN(zVec.X) || Single.IsNaN(zVec.Y) || Single.IsNaN(zVec.Z))
            {
                return false;
            }
            return true;
        }

        /// Return angle between two vectors. Used for visbility testing and
        /// for checking angles between vectors for the road sign generation.
        public static Single GetAngleBetweenVectors(Vector3 vec1, Vector3 vec2)
        {


            // See http://en.wikipedia.org/wiki/Vector_(spatial)
            // for help and check out the Dot Product section ^^
            // Both vectors are normalized so we can save deviding through the
            // lengths.

            Boolean isVec1Ok = CheckThatAllComponentsAreValidNumbers(vec1);
            Boolean isVec2Ok = CheckThatAllComponentsAreValidNumbers(vec2);
            System.Diagnostics.Debug.Assert(isVec1Ok && isVec2Ok);

            Vector3.Normalise(ref vec1, out vec1);
            Vector3.Normalise(ref vec2, out vec2);
            Single dot;
            Vector3.Dot(ref vec1, ref vec2, out dot);
            dot = Clamp(dot, -1.0f, 1.0f);
            Single result = (Single)System.Math.Acos(dot);
            System.Diagnostics.Debug.Assert(!Single.IsNaN(result));
            return result;
        }

        public static Single GetSignedAngleBetweenVectors(Vector3 vec1, Vector3 vec2)
        {
            // See http://en.wikipedia.org/wiki/Vector_(spatial)
            // for help and check out the Dot Product section ^^
            // Both vectors are normalized so we can save deviding through the
            // lengths.

            Single dot;
            Vector3.Dot(ref vec1, ref vec2, out dot);
            Single angle = RealMaths.ArcCos(dot);

            //check to see if the car->camera vector is to the left or right of the
            //inverse car.look vector using the cross product
            //to do this we can just check the sign of the y as we set the y
            //of the two input vector to zero
            Vector3 cross;
            Vector3.Cross(ref vec1, ref vec2, out cross);
            Single sign = 1.0f;

            if (cross.Y < 0.0f)
            {
                sign = -1.0f;
            }

            //check to see if the angle between the car->camera vector and the inverse car.look
            //vector is greater than our limiting angle
            angle *= sign;

            Single pi;
            RealMaths.Pi(out pi);

            Single tau;
            RealMaths.Tau(out tau);

            while (angle < -pi)
                angle += tau;
            while (angle >= pi)
                angle -= tau;

            return angle;
        }

        /// Distance from our point to the line described by linePos1 and linePos2.
        public static Single DistanceToLine(Vector3 point, Vector3 linePos1, Vector3 linePos2)
        {
            // For help check out this article:
            // http://mathworld.wolfram.com/Point-LineDistance3-Dimensional.html
            Vector3 lineVec = linePos2 - linePos1;
            Vector3 pointVec = linePos1 - point;

            Vector3 cross;
            Vector3.Cross(ref lineVec, ref pointVec, out cross);

            return cross.Length() / lineVec.Length();
        }

        /// Signed distance to plane
        public static Single SignedDistanceToPlane(Vector3 point, Vector3 planePosition, Vector3 planeNormal)
        {
            Vector3 pointVec = planePosition - point;

            Single dot;

            Vector3.Dot(ref planeNormal, ref pointVec, out dot);

            return dot;
        }


        public static string NiceMatrixString(Matrix44 mat)
        {
            return string.Format(
                "| {0:+00000.00;-00000.00;} {1:+00000.00;-00000.00;} {2:+00000.00;-00000.00;} {3:+00000.00;-00000.00;} |\n" +
                "| {4:+00000.00;-00000.00;} {5:+00000.00;-00000.00;} {6:+00000.00;-00000.00;} {7:+00000.00;-00000.00;} |\n" +
                "| {8:+00000.00;-00000.00;} {9:+00000.00;-00000.00;} {10:+00000.00;-00000.00;} {11:+00000.00;-00000.00;} |\n" +
                "| {12:+00000.00;-00000.00;} {13:+00000.00;-00000.00;} {14:+00000.00;-00000.00;} {15:+00000.00;-00000.00;} |\n",
                mat.R0C0, mat.R1C0, mat.R2C0, mat.R3C0,
                mat.R0C1, mat.R1C1, mat.R2C1, mat.R3C1,
                mat.R0C2, mat.R1C2, mat.R2C2, mat.R3C2,
                mat.R0C3, mat.R1C3, mat.R2C3, mat.R3C3) +

            "Translation: " + mat.Translation + "\n";
        }

        public static Single FastInverseSquareRoot(Single val)
        {
            if (Single.IsNaN(val))
                throw new Exception("FastInverseSquareRoot only works on numbers!");

            if (Single.IsInfinity(val))
                return 0f;

            if (val == 0f)
                return val;

            unsafe
            {
                Single halfVal = 0.5f * val;
                Int32 i = *(Int32*)&val;    // evil floating point bit level hacking
                i = 0x5f3759df - (i >> 1);  // what the fuck?
                val = *(Single*)&i;
                val = val * (1.5f - (halfVal * val * val));
                //val = val * (1.5f - (halfVal * val * val));
                return val;
            }
        }
    }

    internal class CameraManager
    {
        internal Camera GetActiveCamera(String RenderPass)
        { 
            return _activeCameras[RenderPass].GetTrait<Camera> (); 
        }

        Dictionary<String, SceneObject> _defaultCameras = new Dictionary<String,SceneObject>();
        Dictionary<String, SceneObject> _activeCameras = new Dictionary<String,SceneObject>();

        internal void SetDefaultCamera(String RenderPass)
        {
            _activeCameras[RenderPass] = _defaultCameras[RenderPass];
        }
        
        internal void SetMainCamera (String RenderPass, SceneObject go)
        {
            _activeCameras[RenderPass] = go;
        }

        internal CameraManager (Scene scene)
        {
            var settings = scene.Settings;

            foreach (String renderPass in settings.RenderPasses)
            {
                var renderPassSettings = settings.GetRenderPassSettings(renderPass);

                var go = scene.CreateSceneObject("RenderPass(" + renderPass + ") Provided Camera");

                var cam = go.AddTrait<Camera>();

                if (renderPassSettings.CameraProjectionType == CameraProjectionType.Perspective)
                {
                    go.Transform.Position = new Vector3(2, 1, 5);

                    var orbit = go.AddTrait<OrbitAroundSubject>();
                    orbit.CameraSubject = Transform.Origin;

                    var lookAtSub = go.AddTrait<LookAtSubject>();
                    lookAtSub.Subject = Transform.Origin;
                }
                else
                {
                    cam.Projection = CameraProjectionType.Orthographic;

                    go.Transform.Position = new Vector3(0, 0, 0.5f);
                    go.Transform.LookAt(Vector3.Zero);
                }

            
                _defaultCameras.Add(renderPass, go);
                _activeCameras.Add(renderPass, go);
            }
        }
    }
    internal class FpsHelper
    {
        Single fps = 0;
        TimeSpan sampleSpan;
        Stopwatch stopwatch;
        Int32 sampleFrames;

        internal Single Fps { get { return fps; } }

        internal FpsHelper()
        {
            sampleSpan = TimeSpan.FromSeconds(1);
            fps = 0;
            sampleFrames = 0;
            stopwatch = Stopwatch.StartNew();

        }

        internal void Update(AppTime time)
        {
            if (stopwatch.Elapsed > sampleSpan)
            {
                // Update FPS value and start next sampling period.
                fps = (Single)sampleFrames / (Single)stopwatch.Elapsed.TotalSeconds;

                stopwatch.Reset();
                stopwatch.Start();
                sampleFrames = 0;
            }

        }

        internal void LogRender()
        {
            sampleFrames++;
        }

    }

    internal class FrameBufferHelper
    {
        Rgba32 randomColour = Rgba32.CornflowerBlue;
        Single colourChangeTime = 5.0f;
        Single colourChangeTimer = 0.0f;

        IGraphicsManager gfx;

        internal FrameBufferHelper(IGraphicsManager gfx)
        {
            this.gfx = gfx;
        }

        internal void Update(AppTime time)
        {
            colourChangeTimer += time.Delta;

            if (colourChangeTimer > colourChangeTime)
            {
                colourChangeTimer = 0.0f;
                randomColour = RandomGenerator.Default.GetRandomColour();
            }
        }

        internal void Clear()
        {
            gfx.ClearColourBuffer(randomColour);
        }
    }

    public interface IBlimey
    {
        InputEventSystem InputEventSystem { get; }

        DebugShapeRenderer DebugShapeRenderer { get; }
    }

    internal class LoggingHelper
    {
        ISystemManager sys;

        internal LoggingHelper(ISystemManager sys)
        {
            this.sys = sys;
            Teletype.OpenChannel("Blimey");
            Teletype.OpenChannel("Blimey.Engine");
            Teletype.OpenChannel("Blimey.Input");
            Teletype.OpenChannel("Blimey.Graphics");
            Teletype.OpenChannel("Blimey.Resources");
            Teletype.OpenChannel("Blimey.Resources");
        }

        internal void Heading()
        {
            Teletype.WriteLine("Blimey", " #### #   # #   #    #### #####  ###  #   # #####");
            Teletype.WriteLine("Blimey", "#     #   # ##  #   #       #   #   # ##  #   #  ");
            Teletype.WriteLine("Blimey", "##### #   # # # #   #  ##   #   ##### # # #   #  ");
            Teletype.WriteLine("Blimey", "    # #   # #  ##   #   #   #   #   # #  ##   #  ");
            Teletype.WriteLine("Blimey", "####   ###  #   #    ###  ##### #   # #   #   #  ");
        }

        internal void SystemDetails()
        {
            Teletype.WriteLine("Blimey.Engine", "Operating System: " + sys.OperatingSystem);
            Teletype.WriteLine("Blimey.Engine", "Device Name: " + sys.DeviceName);
            Teletype.WriteLine("Blimey.Engine", "Device Model: " + sys.DeviceModel);
            Teletype.WriteLine("Blimey.Engine", "System Name: " + sys.SystemName);
            Teletype.WriteLine("Blimey.Engine", "System Version: " + sys.SystemVersion);
            Teletype.WriteLine(
                "Blimey.Engine",
                "Screen Spec: w: " + 
                sys.ScreenSpecification.ScreenResolutionWidth +
                " h: " + 
                sys.ScreenSpecification.ScreenResolutionHeight
                );

            Teletype.WriteLine("Blimey.Engine", "System Version: " + sys.SystemVersion);
        }
    }

    public class RandomGenerator
    {
        internal Random Random
        {
            get { return random; }
        }

        Random random;

        public static RandomGenerator Default
        {
            get { return defaultGenerator; }
        }

        static RandomGenerator defaultGenerator = new RandomGenerator();

        public RandomGenerator(Int32 seed)
        {
            random = SetSeed(0);
        }

        public RandomGenerator()
        {
            random = SetSeed(0);
        }

        public Random SetSeed(Int32 seed)
        {
            if (seed == 0)
            {
                random = new Random((Int32)DateTime.Now.Ticks);
            }
            else
            {
                random = new Random(seed);
            }

            return random;
        }

        public Single GetRandomSingle(Single min, Single max)
        {
            return ((Single)random.NextDouble() * (max - min)) + min;
        }

        public Int32 GetRandomInt32(Int32 max)
        {
            return random.Next(max);
        }

        public Byte GetRandomByte()
        {
            Byte[] b = new Byte[1];
            random.NextBytes(b);
            return b[0];
        }

        public Boolean GetRandomBoolean()
        {
            Int32 i = random.Next(2);
            if (i > 0)
            {
                return true;
            }

            return false;
        }

        [CLSCompliant(false)]
        public Rgba32 GetRandomColour()
        {
            Single min = 0.25f;
            Single max = 1f;

            Single r = (Single)random.NextDouble() * (max - min) + min;
            Single g = (Single)random.NextDouble() * (max - min) + min;
            Single b = (Single)random.NextDouble() * (max - min) + min;
            Single a = 1f;

            return new Rgba32(r, g, b, a);
        }

        [CLSCompliant(false)]
        public Vector2 GetRandomVector2(Single min, Single max)
        {
            Single x = (Single)random.NextDouble() * (max - min) + min;
            Single y = (Single)random.NextDouble() * (max - min) + min;

            return new Vector2(x, y);
        }

        [CLSCompliant(false)]
        public Vector3 GetRandomVector3(Single min, Single max)
        {
            Single x = (Single)random.NextDouble() * (max - min) + min;
            Single y = (Single)random.NextDouble() * (max - min) + min;
            Single z = (Single)random.NextDouble() * (max - min) + min;

            return new Vector3(x, y, z);
        }

        [CLSCompliant(false)]
        public Vector3 GetRandomNormalisedVector3()
        {
            Single max = 1f;
            Single min = 1f;

            Single x = (Single)random.NextDouble() * (max - min) + min;
            Single y = (Single)random.NextDouble() * (max - min) + min;
            Single z = (Single)random.NextDouble() * (max - min) + min;

            var result = new Vector3(x, y, z);

            Vector3.Normalise(ref result, out result);

            return result;
        }

        [CLSCompliant(false)]
        public Vector4 GetRandomVector4(Single min, Single max)
        {
            Single x = (Single)random.NextDouble() * (max - min) + min;
            Single y = (Single)random.NextDouble() * (max - min) + min;
            Single z = (Single)random.NextDouble() * (max - min) + min;
            Single w = (Single)random.NextDouble() * (max - min) + min;

            return new Vector4(x, y, z, w);
        }

        /// <summary>
        /// Pick a random element from an indexable collection
        /// </summary>
        /// <typeparam name="T">Return type</typeparam>
        /// <param name="random">'this' parameter</param>
        /// <param name="choices">Collection to pick an element from</param>
        /// <returns></returns>
        public object Choose(System.Collections.IList choices)
        {
            return choices[random.Next(choices.Count)];
        }

        /// <summary>
        /// Pick a random value from an enum
        /// </summary>
        /// <typeparam name="T">Enum type to pick from</typeparam>
        /// <param name="random">'this' parameter</param>
        /// <param name="enumType">the enum type to pick from</param>
        /// <returns></returns>
        public object ChooseFromEnum(Type enumType)
        {
            return Choose(Enum.GetValues(enumType));
        }
    }

    internal class SceneManager
    {
        Scene activeScene;
        ICor cor;

        SceneRenderManager renderManager;

        public event System.EventHandler SimulationStateChanged;

        public Scene ActiveState { get { return activeScene; } }

        public SceneManager (ICor cor, Scene startScene)
        {
            this.cor = cor;
            activeScene = startScene;
            activeScene.Initialize(cor);
            renderManager = new SceneRenderManager(cor);

        }

        public Boolean Update(AppTime time)
        {
            Scene a = activeScene.RunUpdate (time);

            // If the active state returns a game state other than itself then we need to shut
            // it down and start the returned state.  If a game state returns null then we need to
            // shut the engine down.

            //quitting the game
            if (a == null) 
            {
                activeScene.Uninitilise ();
                return true;
            } 
            else if (a != activeScene) 
            {
                activeScene.Uninitilise ();

                activeScene = a;

                this.cor.Graphics.Reset();

                GC.Collect();

                activeScene.Initialize (cor);

                if (SimulationStateChanged != null)
                {
                    SimulationStateChanged(this, System.EventArgs.Empty);
                }

                this.Update(time);

            }

            return false;

        }

        public void Render()
        {
            if (activeScene != null && activeScene.Active)
            {
                renderManager.Render(activeScene);
            }
            else
            {
                Teletype.WriteLine("Beep");
            }
        }
    }

    internal class SceneRenderManager
    {
        ICor Castle { get; set; }

        internal SceneRenderManager(ICor cor)
        {
            this.Castle = cor;
        }

        internal void Render(Scene scene)
        {
            var sceneSettings = scene.Settings;

            // Clear the background colour if the scene settings want us to.
            if (sceneSettings.StartByClearingBackBuffer)
            {
                this.Castle.Graphics.ClearColourBuffer(sceneSettings.BackgroundColour);
            }

            foreach (string renderPass in sceneSettings.RenderPasses)
            {
                this.RenderPass(scene, renderPass);
            }
        }

        List<MeshRenderer> list = new List<MeshRenderer>();
        List<MeshRenderer> GetMeshRenderersWithMaterials(Scene scene, string pass)
        {
            list.Clear ();
            foreach (var go in scene.SceneObjects)
            {
                var mr = go.GetTrait<MeshRenderer>();

                if (mr == null)
                {
                    continue;
                }

                if (mr.Material == null)
                {
                    continue;
                }

                // if the material is for this pass
                if (mr.Material.RenderPass == pass)
                {
                    list.Add(mr);
                }
            }

            return list;
        }

        void RenderPass(Scene scene, string pass)
        {
            // init pass
            var passSettings = scene.Settings.GetRenderPassSettings(pass);

            var gfxManager = this.Castle.Graphics;

            if (passSettings.ClearDepthBuffer)
            {
                gfxManager.ClearDepthBuffer();
            }

            var cam = scene.CameraManager.GetActiveCamera(pass);

            var meshRenderers = this.GetMeshRenderersWithMaterials(scene, pass);

            // TODO: big one
            // we really need to group the mesh renderers by material
            // and only make a new draw call when there are changes.
            foreach (var mr in meshRenderers)
            {
                mr.Render(gfxManager, cam.ViewMatrix44, cam.ProjectionMatrix44);
            }

            scene.Blimey.DebugShapeRenderer.Render(
                gfxManager, pass, cam.ViewMatrix44, cam.ProjectionMatrix44);

        }
    }

    public enum Space
    {
        World,
        Self
    }

    public static class Teletype
    {
        static HashSet<String> activeChannels = new HashSet<String>();

        static Teletype()
        {
            OpenChannel("Default");
        }

        [Conditional("DEBUG")]
        public static void OpenChannel(String channel)
        {
            activeChannels.Add(channel);
        }

        [Conditional("DEBUG")]
        public static void CloseChannel(String channel)
        {
            activeChannels.Remove(channel);
        }

        public static void WriteLine(String line)
        {
            WriteLine("Default", line);
        }

        
        public static void WriteLine(String line, params object[] args)
        {
            WriteLine("Default", line, args);
        }

        public static void WriteLine(String channel, String line)
        {
            if (!activeChannels.Contains(channel))
            {
                return;
            }

            String prepend = String.Format("[{0}] ", channel);

            Debug.WriteLine(prepend + line);
        }

        public static void WriteLine(String channel, String line, params object[] args)
        {
            String main = String.Format(line, args);

            WriteLine(channel, main);
            
        }
    }

    //
    // TRANSFORM
    //
    // Every object in a scene has a Transform. It's used to store and manipulate 
    // the position, rotation and scale of the object. Every Transforms can have a 
    // parent, which allows you to apply position, rotation and scale hierarchically. 
    // This is the hierarchy seen in the Hierarchy pane. They also support 
    // enumerators so you can loop through children using:
    public sealed class Transform
        : IEnumerable
    {

        public static Transform Origin = new Transform ();

        // DATA --------------------------------------------------
        Vector3 _localPosition = Vector3.Zero;
        Vector3 _localScale = new Vector3 (1, 1, 1);
        Quaternion _localRotation = Quaternion.Identity;
        List<Transform> _children = new List<Transform> ();
        List<Transform> _cachedHierarchyToRootParent = new List<Transform> (0);
        Transform _parent = null;
        //--------------------------------------------------------



        // The parent of the transform.
        public Transform Parent { 
            get { 
                return _parent; 
            } 
            set { 
                _parent = value;
                _cachedHierarchyToRootParent.Clear ();
                Transform temp = this.Parent;
                while (temp != null) {
                    _cachedHierarchyToRootParent.Add (temp);
                    temp = temp.Parent;
                }
            } 
        }

        // Returns the topmost transform in the hierarchy.
        public Transform Root { 
            get {
                Transform temp = this;
                while (temp.Parent != null) {
                    temp = temp.Parent;
                }

                return temp;
            } 
        }

        // How many child transforms?
        public int ChildCount { 
            get { 
                return _children.Count; 
            } 
        }

        // Matrix44 that transforms a point from local space into world space.
        internal Matrix44 LocalToWorld { 
            get {
                Matrix44 trans = Matrix44.Identity;
                Transform temp = this.Parent;
                while (temp != null) {
                    trans = trans * temp.LocalLocation;
                    temp = temp.Parent;
                }
                return trans; 
            } 
        }

        // Matrix44 that transforms a point from world space into local space.
        internal Matrix44 WorldToLocal { 
            get {
                // why doesn't this work
                //Matrix44 trans = Matrix44.Identity;
                //for (int i = _cachedHierarchyToRootParent.Count - 1; i > -1; --i)
                //{
                //    trans = _cachedHierarchyToRootParent[i].LocalLocation * trans;
                //}

                //use this for now
                Matrix44 loc2World = LocalToWorld;

                Matrix44 trans; Matrix44.Invert(ref loc2World, out trans);
                return trans; 
            } 
        }

        // In world space.
        public Vector3 Forward { get { return Location.Forward; } }

        public Vector3 Up { get { return Location.Up; } }

        public Vector3 Right { get { return Location.Right; } }

        public Vector3 Position { 
            get 
            {
                Vector3 localPos = LocalPosition;
                Matrix44 location; Matrix44.CreateTranslation(ref localPos, out location);
                Transform temp = this.Parent;
                while (temp != null) 
                {
                    Matrix44 rotMat;
                    Quaternion lr = temp.LocalRotation;
                    Matrix44.CreateFromQuaternion(ref lr, out rotMat);
                    Matrix44.Transform(ref location, ref lr, out location);
                    //rotMat * location;
                    

                    location.Translation += temp.LocalPosition;
                    temp = temp.Parent;
                }
                return location.Translation; 
            } 

            set 
            {
                Matrix44 trans;
                Matrix44.CreateTranslation(ref value, out trans);

                Matrix44 newMat;
                Matrix44 w2l = WorldToLocal;
                Matrix44.Multiply(ref trans, ref w2l, out newMat);

                LocalPosition = newMat.Translation;
            }
        }

        public Quaternion Rotation { 
            get {
                Quaternion rotation = LocalRotation;
                Transform temp = this.Parent;
                while (temp != null) {
                    rotation = rotation * temp.LocalRotation;
                    temp = temp.Parent;
                }
                return rotation; 
            }
            set
            {
                Quaternion q = value;
                q.Normalise ();

                if (WorldToLocal != Matrix44.Identity)
                {
                    Matrix44 mat;
                    Matrix44.CreateFromQuaternion (ref q, out mat);

                    Matrix44 r = WorldToLocal * mat;

                    Quaternion newRot;
                    Quaternion.CreateFromRotationMatrix (ref r, out newRot);
                    LocalRotation = newRot; 
                }
                else
                {
                    LocalRotation = q;
                }
            }
        }

        public Vector3 Scale { 
            get {
                Vector3 scale = this.LocalScale;
                Transform temp = this.Parent;
                while (temp != null) {
                    scale = scale * temp.LocalScale;
                    temp = temp.Parent;
                }
                return scale; 
            } 
        }

        public Vector3 EulerAngles { get { return BlimeyMathsHelper.QuaternionToEuler (Rotation); } }

        public Matrix44 Location {
            get {
                return LocalLocation * LocalToWorld;
            }
        }


        // Relative to the parent transform.
        public Vector3 LocalPosition { get { return _localPosition; } set { _localPosition = value; } }

        public Quaternion LocalRotation { get { return _localRotation; } set { _localRotation = value; _localRotation.Normalise(); } }

        public Vector3 LocalScale { get { return _localScale; } set { _localScale = value; } }

        public Vector3 LocalEulerAngles
        {
            get { return BlimeyMathsHelper.QuaternionToEuler(_localRotation); }
            set { _localRotation = BlimeyMathsHelper.EulerToQuaternion(value); } 
        }

        public Matrix44 LocalLocation
        {
            get
            {
                Matrix44 scale;
                Matrix44.CreateScale(ref _localScale, out scale);

                Matrix44 rotation;
                Matrix44.CreateFromQuaternion(ref _localRotation, out rotation);

                Matrix44 translation;
                Matrix44.CreateTranslation(ref _localPosition, out translation);

                Matrix44 result = scale * rotation * translation;
                return result;
            }
        }


        // Moves the transform in the direction and distance of translation.
        //
        // If relativeTo is left out or set to Space.Self the movement is applied 
        // relative to the transform's local axes. (the x, y and z axes shown when 
        // selecting the object inside the Scene View.) If relativeTo is Space.World 
        // the movement is applied relative to the world coordinate system.
        public void Translate (Vector3 translation)
        {
            Position += translation;
        }

        public void Translate (Vector3 translation, Space relativeTo)
        { 
            if (relativeTo == Space.World) 
                Position += translation;
            else 
                LocalPosition += translation; 
        }

        public void Translate (Vector3 translation, Transform relativeTo)
        {
            Vector3 pointInWorld = relativeTo.TransformPoint (translation);
            Vector3 worldTrans = pointInWorld - relativeTo.Position;
            this.Position += worldTrans;
        }

        public void Translate (Single x, Single y, Single z)
        {
            this.Translate (new Vector3 (x, y, z));
        }

        public void Translate (Single x, Single y, Single z, Space relativeTo)
        {
            this.Translate (new Vector3 (x, y, z), relativeTo);
        }

        public void Translate (Single x, Single y, Single z, Transform relativeTo)
        {
            this.Translate (new Vector3 (x, y, z), relativeTo);
        }

        // Applies a rotation of eulerAngles.z degrees around the z axis, 
        // eulerAngles.x degrees around the x axis, and eulerAngles.y 
        // degrees around the y axis (in that order).
        //
        // If relativeTo is left out or set to Space.Self the rotation is applied 
        // around the transform's local axes. (The x, y and z axes shown when 
        // selecting the object inside the Scene View.) If relativeTo is 
        // Space.World the rotation is applied around the world x, y, z axes.
        public void Rotate (Vector3 eulerAngles)
        {
            throw new System.NotImplementedException();
        }

        public void Rotate (Vector3 axis, Single angle)
        {
            throw new System.NotImplementedException();
        }

        public void Rotate (Vector3 eulerAngles, Space relativeTo)
        {
            throw new System.NotImplementedException();
        }

        public void Rotate (Single xAngle, Single yAngle, Single zAngle)
        {
            throw new System.NotImplementedException();
        }

        public void Rotate (Vector3 axis, Single angle, Space relativeTo)
        {
            throw new System.NotImplementedException();
        }

        public void Rotate (Single xAngle, Single yAngle, Single zAngle, Space relativeTo)
        {
            throw new System.NotImplementedException();
        }

        // Rotates the transform about axis passing through point 
        // in world coordinates by angle degrees.
        // This modifies both the position and the rotation of the transform.
        public void RotateAround (Vector3 axis, Single angle)
        {
            throw new System.NotImplementedException();
        }

        public void RotateAround (Vector3 point, Vector3 axis, Single angle)
        {
            throw new System.NotImplementedException();
        }

        public void RotateAroundLocal (Vector3 axis, Single angle)
        {
            throw new System.NotImplementedException();
        }

        //Rotates the transform so the forward vector points at /target/'s current position.
        //
        // Then it rotates the transform to point its up direction vector in the direction 
        // hinted at by the worldUp vector. If you leave out the worldUp parameter, the 
        // function will use the world y axis. worldUp is only a hint vector. The up 
        // vector of the rotation will only match the worldUp vector if the forward 
        // direction is perpendicular to worldUp
        public void LookAt (Transform target)
        {
            LookAt (target, Vector3.Up);
        }

        public void LookAt (Vector3 worldPosition)
        {
            LookAt (worldPosition, Vector3.Up);
        }

        public void LookAt (Transform target, Vector3 worldUp)
        {
            LookAt (target.Position, worldUp);
        }

        public void LookAt (Vector3 worldPosition, Vector3 worldUp)
        {
            Vector3 lookAtVector = worldPosition - this.Position;
            Vector3.Normalise(ref lookAtVector, out lookAtVector);

            Matrix44 newOrientation = Matrix44.Identity;

            newOrientation.Forward = lookAtVector;

            Vector3 newRight; 
            Vector3.Cross(ref lookAtVector, ref worldUp, out newRight);
            Vector3.Normalise(ref newRight, out newRight);

            newOrientation.Right = newRight;

            Vector3 newUp;
            Vector3.Cross(ref newRight, ref lookAtVector, out newUp);
            Vector3.Normalise(ref newUp, out newUp);
            newOrientation.Up = newUp;

            Quaternion rotation;
            Quaternion.CreateFromRotationMatrix(ref newOrientation, out rotation);

            this.Rotation = rotation;

            /*

            // A vector going from our parent game object to our Subject
            Vector3 lookAtVector = Subject.Position - this.Parent.Transform.Position;

            // A direction from our parent game object to our Subject
            Vector3.Normalise(ref lookAtVector, out lookAtVector);

            // Build a new orientation matrix
            Matrix44 newOrientation = Matrix44.Identity;

            Vector3 t1;
            Vector3.Normalise(ref lookAtVector, out t1);
            newOrientation.Forward = t1;

            if (LockToY) 
            {
                Vector3 t2 = Vector3.Up;
                Vector3.Normalise(ref t2, out t2);
                newOrientation.Up = t2;

                Vector3 b = newOrientation.Backward;
                Vector3 u = newOrientation.Up;

                Vector3 r;
                Vector3.Cross(ref b, ref u, out r);
                Vector3.Normalise(ref r, out r);
                newOrientation.Right = r;
            }
            else
            {
                Vector3 f = newOrientation.Forward;
                Vector3 u = Vector3.Up;
                Vector3 r;
                Vector3.Cross(ref f, ref u, out r);
                Vector3.Normalise(ref r, out r);
                newOrientation.Right = r;

                Vector3.Cross(ref r, ref f, out u);
                Vector3.Normalise(ref u, out u);
                newOrientation.Up = u;
            }

            Quaternion rotation;
            Quaternion.CreateFromRotationMatrix(ref newOrientation, out rotation);
            this.Parent.Transform.Rotation = rotation;
        }

            */
        
        }


        // Transforms direction from local space to world space.
        // This operation is not affected by scale or position of the transform. 
        // The returned vector has the same length as direction.
        public Vector3 TransformDirection (Vector3 direction)
        {
            Single length = direction.Length ();
            Vector3.Normalise(ref direction, out direction);
            var t = TransformPoint (direction);
            Vector3.Normalise(ref t, out t);
            
            return t * length; 
        }

        public Vector3 TransformDirection (Single x, Single y, Single z)
        {
            return TransformDirection (new Vector3 (x, y, z));
        }

        // Transforms a direction from world space to local space. 
        // The opposite of Transform.TransformDirection.
        // This operation is unaffected by scale.
        public Vector3 InverseTransformDirection (Vector3 direction)
        {
            Single length = direction.Length ();
            Vector3.Normalise(ref direction, out direction);
            var t = InverseTransformPoint(direction);
            Vector3.Normalise(ref t, out t);
            return t * length;
        }

        public Vector3 InverseTransformDirection (Single x, Single y, Single z)
        {
            return InverseTransformDirection (new Vector3 (x, y, z));
        }

        // Transforms position from local space to world space.
        // Note that the returned position is affected by scale. 
        // Use Transform.TransformDirection if you are dealing with directions.
        public Vector3 TransformPoint (Vector3 position)
        {
            Matrix44 trans;
            Matrix44.CreateTranslation(ref position, out trans);

            return (trans * LocalToWorld).Translation; 
        }

        public Vector3 TransformPoint (Single x, Single y, Single z)
        {
            return TransformPoint (new Vector3 (x, y, z));
        }

        // Transforms position from world space to local space. The 
        // opposite of Transform.TransformPoint.
        // Note that the returned position is affected by scale. Use 
        // Transform.InverseTransformDirection if you are dealing with directions.
        public Vector3 InverseTransformPoint (Vector3 position)
        {
            Matrix44 trans;
            Matrix44.CreateTranslation(ref position, out trans);

            return (trans * WorldToLocal).Translation;
        }

        public Vector3 InverseTransformPoint (Single x, Single y, Single z)
        {
            return InverseTransformPoint (new Vector3 (x, y, z));
        }

        // Unparents all children.
        // Useful if you want to destroy the root of a hierarchy without destroying the children.
        public void DetachChildren ()
        {
            while (_children.Count > 0) {
                _children [0].Parent = null;
                _children.RemoveAt (0);
            }
        }

        // Not sure if we want this?
        public Transform GetChild (int index)
        {
            if (_children.Count > index)
                return _children [index];

            return null;
        }



        // Returns a Booleanean value that indicates whether the transform 
        // is a child of a given transform. true if this transform is a child, 
        // deep child (child of a child...) or identical to this transform, otherwise false.
        public Boolean IsChildOf (Transform parent)
        {
            Transform temp = this;
            while (temp != null) {
                if (temp == parent)
                    return true;

                temp = temp.Parent;
            }

            return false;
        }

        public override String ToString ()
        {
            return
                "LOCAL \n" +
                string.Format (" - Position: |{0} {1} {2}|\n", LocalPosition.X, LocalPosition.Y, LocalPosition.Z) +
                string.Format (" - Rotation: |{0} {1} {2}|\n", LocalEulerAngles.X, LocalEulerAngles.Y, LocalEulerAngles.Z) +
                string.Format (" - Scale:    |{0} {1} {2}|\n", LocalScale.X, LocalScale.Y, LocalScale.Z) +

                "WORLD \n" +
                string.Format (" - Position: |{0} {1} {2}|\n", Position.X, Position.Y, Position.Z) +
                string.Format (" - Rotation: |{0} {1} {2}|\n", EulerAngles.X, EulerAngles.Y, EulerAngles.Z) +
                string.Format (" - Scale:    |{0} {1} {2}|\n", Scale.X, Scale.Y, Scale.Z);

        }
        //--------------------------------------------------------------------------

        // IEnumerable implementation
        public IEnumerator GetEnumerator ()
        {
            return new Enumerator (this);
        }

        // Nested Types
        sealed class Enumerator 
            : IEnumerator
        {
            // Fields
            int currentIndex = -1;
            Transform outer;

            // Methods
            internal Enumerator (Transform outer)
            {
                this.outer = outer;
            }

            public Boolean MoveNext ()
            {
                int childCount = this.outer.ChildCount;
                return (++this.currentIndex < childCount);
            }

            public void Reset ()
            {
                this.currentIndex = -1;
            }

            // Properties
            public object Current {
                get {
                    return this.outer.GetChild (this.currentIndex);
                }
            }


        }
    }

    public static class LightingManager
    {
        public static Rgba32 ambientLightColour;
        public static Rgba32 emissiveColour;
        public static Rgba32 specularColour;
        public static Single specularPower;


        public static Boolean fogEnabled;
        public static Single fogStart;
        public static Single fogEnd;
        public static Rgba32 fogColour;
        

        public static Vector3 dirLight0Direction;
        public static Rgba32 dirLight0DiffuseColour;
        public static Rgba32 dirLight0SpecularColour;

        public static Vector3 dirLight1Direction;
        public static Rgba32 dirLight1DiffuseColour;
        public static Rgba32 dirLight1SpecularColour;

        public static Vector3 dirLight2Direction;
        public static Rgba32 dirLight2DiffuseColour;
        public static Rgba32 dirLight2SpecularColour;

        static LightingManager()
        {
            ambientLightColour = Rgba32.Black;
            emissiveColour = Rgba32.DarkSlateGrey;
            specularColour = Rgba32.DarkGrey;
            specularPower = 2f;

            fogEnabled = true;
            fogStart = 100f;
            fogEnd = 1000f;
            fogColour = Rgba32.BlueViolet;


            dirLight0Direction = new Vector3(-0.3f, -0.9f, +0.3f); 
            Vector3.Normalise(ref dirLight0Direction, out dirLight0Direction);
            dirLight0DiffuseColour = Rgba32.DimGrey;
            dirLight0SpecularColour = Rgba32.DarkGreen;

            dirLight1Direction = new Vector3(0.3f, 0.1f, -0.3f);
            Vector3.Normalise(ref dirLight1Direction, out dirLight1Direction);
            dirLight1DiffuseColour = Rgba32.DimGrey;
            dirLight1SpecularColour = Rgba32.DarkRed;

            dirLight2Direction = new Vector3( -0.7f, -0.3f, +0.1f);
            Vector3.Normalise(ref dirLight2Direction, out dirLight2Direction);
            dirLight2DiffuseColour = Rgba32.DimGrey;
            dirLight2SpecularColour = Rgba32.DarkBlue;

        }
    }

    #region DebugUtils

    /// <summary>
    /// A simple timer for collecting profiler data.  Usage:
    /// 
    ///     using(new ProfilingTimer(time => myTime = time))
    ///     {
    ///         // stuff
    ///     }
    ///
    /// </summary>
    internal struct ProfilingTimer 
        : IDisposable
    {
        public delegate void ResultHandler(double timeInMilliSeconds);

        public ProfilingTimer(ResultHandler resultHandler)
        {
            _stopWatch = Stopwatch.StartNew();
            _resultHandler = resultHandler;
        }

        public void Dispose()
        {
            double elapsedTime = (double)_stopWatch.ElapsedTicks / (double)Stopwatch.Frequency;
            _resultHandler(elapsedTime * 1000.0);
        }

        private Stopwatch _stopWatch;
        private ResultHandler _resultHandler;
    }

        public static class FrameStats
        {
            static FrameStats ()
            {
                Reset ();
            }

            public static void Reset ()
            {
                UpdateTime = 0.0;
                RenderTime = 0.0;

                SetCullModeTime = 0.0;
                ActivateGeomBufferTime = 0.0;
                MaterialTime = 0.0;
                ActivateShaderTime = 0.0;
                DrawTime = 0.0;

                DrawUserPrimitivesCount = 0;
                DrawIndexedPrimitivesCount = 0;
            }

            public static Double UpdateTime { get; set; }
            public static Double RenderTime { get; set; }

            public static Double SetCullModeTime { get; set; }
            public static Double ActivateGeomBufferTime { get; set; }
            public static Double MaterialTime { get; set; }
            public static Double ActivateShaderTime { get; set; }
            public static Double DrawTime { get; set; }

            public static Int32 DrawUserPrimitivesCount { get; set; }
            public static Int32 DrawIndexedPrimitivesCount { get; set; }

            public static Int32 DrawCallCount
            {
                get
                {
                    return 
                        DrawUserPrimitivesCount + 
                        DrawIndexedPrimitivesCount;
                }
            }

            public static void SlowLog ()
            {
                if (UpdateTime > 5.0)
                {
                    Console.WriteLine(
                        string.Format(
                            "UpdateTime -> {0:0.##}ms",
                            UpdateTime ));
                }

                if (RenderTime > 10.0)
                {
                    Console.WriteLine(
                        string.Format(
                            "RenderTime -> {0:0.##}ms",
                            RenderTime ));


                    Console.WriteLine(
                        string.Format(
                            "\tMeshRenderer -> SetCullModeTime -> {0:0.##}ms",
                            SetCullModeTime ));

                    Console.WriteLine(
                        string.Format(
                            "\tActivateGeomBufferTime -> DrawTime -> {0:0.##}ms",
                            ActivateGeomBufferTime ));

                    Console.WriteLine(
                        string.Format(
                            "\tMeshRenderer -> MaterialTime -> {0:0.##}ms",
                            MaterialTime ));

                    Console.WriteLine(
                        string.Format(
                            "\tMeshRenderer -> ActivateShaderTime -> {0:0.##}ms",
                            ActivateShaderTime ));
                    
                    Console.WriteLine(
                        string.Format(
                            "\tMeshRenderer -> DrawTime -> {0:0.##}ms",
                            DrawTime ));
                }

                if (DrawCallCount > 25)
                {
                    Console.WriteLine(
                        string.Format(
                            "Draw Call Count -> {0}",
                            DrawCallCount ));
                }
            }
        }

    /// <summary>
    /// A system for handling rendering of various debug shapes.
    /// </summary>
    /// <remarks>
    /// The DebugShapeRenderer allows for rendering line-base shapes in a batched fashion. Games
    /// will call one of the many Add* methods to add a shape to the renderer and then a call to
    /// Draw will cause all shapes to be rendered. This mechanism was chosen because it allows
    /// game code to call the Add* methods wherever is most convenient, rather than having to
    /// add draw methods to all of the necessary objects.
    ///
    /// Additionally the renderer supports a lifetime for all shapes added. This allows for things
    /// like visualization of raycast bullets. The game would call the AddLine overload with the
    /// lifetime parameter and pass in a positive value. The renderer will then draw that shape
    /// for the given amount of time without any more calls to AddLine being required.
    ///
    /// The renderer's batching mechanism uses a cache system to avoid garbage and also draws as
    /// many lines in one call to DrawUserPrimitives as possible. If the renderer is trying to draw
    /// more lines than are allowed in the Reach profile, it will break them up into multiple draw
    /// calls to make sure the game continues to work for any game.</remarks>
    public class DebugShapeRenderer
    {
        public static IShader DebugShader
        {
            get { return debugShader; }
            set { debugShader = value; }
        }

        static IShader debugShader;

        public int NumActiveShapes { get { return activeShapes.Count; } }

        // A single shape in our debug renderer
        class DebugShape
        {
            public string RenderPass = "Default";

            /// <summary>
            /// The array of vertices the shape can use.
            /// </summary>
            public VertexPositionColour[] Vertices;

            /// <summary>
            /// The number of lines to draw for this shape.
            /// </summary>
            public int LineCount;

            /// <summary>
            /// The length of time to keep this shape visible.
            /// </summary>
            public float Lifetime;
        }

        // We use a cache system to reuse our DebugShape instances to avoid creating garbage
        readonly List<DebugShape> cachedShapes = new List<DebugShape>();
        readonly List<DebugShape> activeShapes = new List<DebugShape>();

        // Allocate an array to hold our vertices; this will grow as needed by our renderer
        VertexPositionColour[] verts = new VertexPositionColour[64];

        // An array we use to get corners from frustums and bounding boxes
        Vector3[] corners = new Vector3[8];

        // This holds the vertices for our unit sphere that we will use when drawing bounding spheres
        const int sphereResolution = 30;
        const int sphereLineCount = (sphereResolution + 1) * 3;
        Vector3[] unitSphere;

        Dictionary<string, Material> materials = new Dictionary<string, Material>();

        ICor cor;

        public DebugShapeRenderer(ICor cor, List<string> renderPasses)
        {
            this.cor = cor;

            if (debugShader == null)
            {
                // todo, need a better way to configure this.
                throw new Exception ("DebugShapeRenderer.DebugShader must be set by user.");
            }

            foreach (string pass in renderPasses)
            {
                var m = new Material(pass, debugShader);

                m.BlendMode = BlendMode.Default;
                materials[pass] = m;
                m.SetColour("MaterialColour", Rgba32.White);

            }

            // Create our unit sphere vertices
            InitializeSphere();
        }

        public void AddQuad(string renderPass, Vector3 a, Vector3 b, Vector3 c, Vector3 d, Rgba32 rgba)
        {
            AddQuad (renderPass, a, b, c, d, rgba, 0f);
        }

        public void AddQuad(string renderPass, Vector3 a, Vector3 b, Vector3 c, Vector3 d, Rgba32 rgba, float life)
        {
            AddLine(renderPass, a, b, rgba, life);
            AddLine(renderPass, b, c, rgba, life);
            AddLine(renderPass, c, d, rgba, life);
            AddLine(renderPass, d, a, rgba, life);
        }


        public void AddLine(string renderPass, Vector3 a, Vector3 b, Rgba32 rgba)
        {
            AddLine(renderPass, a, b, rgba, 0f);
        }


        public void AddLine(string renderPass, Vector3 a, Vector3 b, Rgba32 rgba, float life)
        {
            // Get a DebugShape we can use to draw the line
            DebugShape shape = GetShapeForLines(1, life);
            shape.RenderPass = renderPass;

            // Add the two vertices to the shape
            shape.Vertices[0] = new VertexPositionColour(a, rgba);
            shape.Vertices[1] = new VertexPositionColour(b, rgba);
        }

        public void AddTriangle(string renderPass, Vector3 a, Vector3 b, Vector3 c, Rgba32 rgba)
        {
            AddTriangle(renderPass, a, b, c, rgba, 0f);
        }

        public void AddTriangle(string renderPass, Vector3 a, Vector3 b, Vector3 c, Rgba32 rgba, float life)
        {
            // Get a DebugShape we can use to draw the triangle
            DebugShape shape = GetShapeForLines(3, life);
            shape.RenderPass = renderPass;

            // Add the vertices to the shape
            shape.Vertices[0] = new VertexPositionColour(a, rgba);
            shape.Vertices[1] = new VertexPositionColour(b, rgba);
            shape.Vertices[2] = new VertexPositionColour(b, rgba);
            shape.Vertices[3] = new VertexPositionColour(c, rgba);
            shape.Vertices[4] = new VertexPositionColour(c, rgba);
            shape.Vertices[5] = new VertexPositionColour(a, rgba);
        }
        /*
        public void AddBoundingFrustum(string renderPass, BoundingFrustum frustum, Rgba32 rgba)
        {
            AddBoundingFrustum(renderPass, frustum, rgba, 0f);
        }

        public void AddBoundingFrustum(string renderPass, BoundingFrustum frustum, Rgba32 rgba, float life)
        {
            // Get a DebugShape we can use to draw the frustum
            DebugShape shape = GetShapeForLines(12, life);
            shape.RenderPass = renderPass;

            // Get the corners of the frustum
            corners = frustum.GetCorners();

            // Fill in the vertices for the bottom of the frustum
            shape.Vertices[0] = new VertexPositionColour(corners[0], rgba);
            shape.Vertices[1] = new VertexPositionColour(corners[1], rgba);
            shape.Vertices[2] = new VertexPositionColour(corners[1], rgba);
            shape.Vertices[3] = new VertexPositionColour(corners[2], rgba);
            shape.Vertices[4] = new VertexPositionColour(corners[2], rgba);
            shape.Vertices[5] = new VertexPositionColour(corners[3], rgba);
            shape.Vertices[6] = new VertexPositionColour(corners[3], rgba);
            shape.Vertices[7] = new VertexPositionColour(corners[0], rgba);

            // Fill in the vertices for the top of the frustum
            shape.Vertices[8] = new VertexPositionColour(corners[4], rgba);
            shape.Vertices[9] = new VertexPositionColour(corners[5], rgba);
            shape.Vertices[10] = new VertexPositionColour(corners[5], rgba);
            shape.Vertices[11] = new VertexPositionColour(corners[6], rgba);
            shape.Vertices[12] = new VertexPositionColour(corners[6], rgba);
            shape.Vertices[13] = new VertexPositionColour(corners[7], rgba);
            shape.Vertices[14] = new VertexPositionColour(corners[7], rgba);
            shape.Vertices[15] = new VertexPositionColour(corners[4], rgba);

            // Fill in the vertices for the vertical sides of the frustum
            shape.Vertices[16] = new VertexPositionColour(corners[0], rgba);
            shape.Vertices[17] = new VertexPositionColour(corners[4], rgba);
            shape.Vertices[18] = new VertexPositionColour(corners[1], rgba);
            shape.Vertices[19] = new VertexPositionColour(corners[5], rgba);
            shape.Vertices[20] = new VertexPositionColour(corners[2], rgba);
            shape.Vertices[21] = new VertexPositionColour(corners[6], rgba);
            shape.Vertices[22] = new VertexPositionColour(corners[3], rgba);
            shape.Vertices[23] = new VertexPositionColour(corners[7], rgba);
        }

        public void AddBoundingBox(string renderPass, BoundingBox box, Rgba32 col)
        {
            AddBoundingBox(renderPass, box, col, 0f);
        }

        public void AddBoundingBox(string renderPass, BoundingBox box, Rgba32 col, float life)
        {
            // Get a DebugShape we can use to draw the box
            DebugShape shape = GetShapeForLines(12, life);
            shape.RenderPass = renderPass;

            // Get the corners of the box
            corners = box.GetCorners();

            // Fill in the vertices for the bottom of the box
            shape.Vertices[0] = new VertexPositionColour(corners[0], col);
            shape.Vertices[1] = new VertexPositionColour(corners[1], col);
            shape.Vertices[2] = new VertexPositionColour(corners[1], col);
            shape.Vertices[3] = new VertexPositionColour(corners[2], col);
            shape.Vertices[4] = new VertexPositionColour(corners[2], col);
            shape.Vertices[5] = new VertexPositionColour(corners[3], col);
            shape.Vertices[6] = new VertexPositionColour(corners[3], col);
            shape.Vertices[7] = new VertexPositionColour(corners[0], col);

            // Fill in the vertices for the top of the box
            shape.Vertices[8] = new VertexPositionColour(corners[4], col);
            shape.Vertices[9] = new VertexPositionColour(corners[5], col);
            shape.Vertices[10] = new VertexPositionColour(corners[5], col);
            shape.Vertices[11] = new VertexPositionColour(corners[6], col);
            shape.Vertices[12] = new VertexPositionColour(corners[6], col);
            shape.Vertices[13] = new VertexPositionColour(corners[7], col);
            shape.Vertices[14] = new VertexPositionColour(corners[7], col);
            shape.Vertices[15] = new VertexPositionColour(corners[4], col);

            // Fill in the vertices for the vertical sides of the box
            shape.Vertices[16] = new VertexPositionColour(corners[0], col);
            shape.Vertices[17] = new VertexPositionColour(corners[4], col);
            shape.Vertices[18] = new VertexPositionColour(corners[1], col);
            shape.Vertices[19] = new VertexPositionColour(corners[5], col);
            shape.Vertices[20] = new VertexPositionColour(corners[2], col);
            shape.Vertices[21] = new VertexPositionColour(corners[6], col);
            shape.Vertices[22] = new VertexPositionColour(corners[3], col);
            shape.Vertices[23] = new VertexPositionColour(corners[7], col);
        }

        public void AddBoundingSphere(string renderPass, BoundingSphere sphere, Rgba32 col)
        {
            AddBoundingSphere(renderPass, sphere, col, 0f);
        }

        public void AddBoundingSphere(string renderPass, BoundingSphere sphere, Rgba32 col, float life)
        {
            // Get a DebugShape we can use to draw the sphere
            DebugShape shape = GetShapeForLines(sphereLineCount, life);
            shape.RenderPass = renderPass;

            // Iterate our unit sphere vertices
            for (int i = 0; i < unitSphere.Length; i++)
            {
                // Compute the vertex position by transforming the point by the radius and center of the sphere
                Vector3 vertPos = unitSphere[i] * sphere.Radius + sphere.Center;

                // Add the vertex to the shape
                shape.Vertices[i] = new VertexPositionColour(vertPos, col);
            }
        }
        */
        /*
        public void AddRect(string renderPass, Rectangle rect, Single z, Rgba32 colour, Single life = 0f)
        {
            Int32 width = this.cor.Graphics.DisplayStatus.CurrentWidth;
            Int32 height = this.cor.Graphics.DisplayStatus.CurrentHeight;

            this.cor.System.GetEffectiveDisplaySize(ref width, ref height);

            Single l = (Single) rect.Left / (Single) width;
            Single r = (Single) rect.Right / (Single) width;
            Single t = (Single) rect.Top / (Single) height;
            Single b = (Single) rect.Bottom / (Single) height;

            this.AddLine(
                renderPass,
                new Vector3(l, t, z),
                new Vector3(r, t, z),
                colour,
                life );

            this.AddLine(
                renderPass,
                new Vector3(l, b, z),
                new Vector3(r, b, z),
                colour,
                life );

            this.AddLine(
                renderPass,
                new Vector3(l, b, z),
                new Vector3(l, t, z),
                colour,
                life );

            this.AddLine(
                renderPass,
                new Vector3(r, b, z),
                new Vector3(r, t, z),
                colour,
                life );


        }*/

        internal void Update(AppTime time)
        {
            // Go through our active shapes and retire any shapes that have expired to the
            // cache list.
            Boolean resort = false;
            for (int i = this.activeShapes.Count - 1; i >= 0; i--)
            {
                DebugShape s = activeShapes[i];

                if (s.Lifetime < 0)
                {
                    this.cachedShapes.Add(s);
                    this.activeShapes.RemoveAt(i);
                    resort = true;
                }
                else
                {
                    s.Lifetime -= time.Delta;
                }
            }

            // If we move any shapes around, we need to resort the cached list
            // to ensure that the smallest shapes are first in the list.
            if (resort)
                this.cachedShapes.Sort(CachedShapesSort);
        }


        internal void Render(IGraphicsManager zGfx, string pass, Matrix44 zView, Matrix44 zProjection)
        {
            if (!materials.ContainsKey(pass))
                return;

            var material = materials[pass];

            if( material == null )
                return;

            material.UpdateGpuSettings (zGfx);

            // Update our effect with the matrices.
            material.UpdateShaderVariables (
                Matrix44.Identity,
                zView,
                zProjection
                );

            var shader = material.GetShader ();

            if( shader == null )
                return;

            zGfx.GpuUtils.BeginEvent(Rgba32.Red, "DebugRenderer.Render");

            var shapesForThisPass = this.activeShapes.Where(e => pass == e.RenderPass);

            // Calculate the total number of vertices we're going to be rendering.
            int vertexCount = 0;
            foreach (var shape in shapesForThisPass)
                vertexCount += shape.LineCount * 2;

            // If we have some vertices to draw
            if (vertexCount > 0)
            {
                // Make sure our array is large enough
                if (verts.Length < vertexCount)
                {
                    // If we have to resize, we make our array twice as large as necessary so
                    // we hopefully won't have to resize it for a while.
                    verts = new VertexPositionColour[vertexCount * 2];
                }

                // Now go through the shapes again to move the vertices to our array and
                // add up the number of lines to draw.
                int lineCount = 0;
                int vertIndex = 0;
                foreach (DebugShape shape in shapesForThisPass)
                {
                    lineCount += shape.LineCount;
                    int shapeVerts = shape.LineCount * 2;
                    for (int i = 0; i < shapeVerts; i++)
                        verts[vertIndex++] = shape.Vertices[i];
                }

                // Start our effect to begin rendering.
                foreach (IShaderPass effectPass in shader.Passes)
                {
                    effectPass.Activate (VertexPositionColour.Default.VertexDeclaration);

                    // We draw in a loop because the Reach profile only supports 65,535 primitives. While it's
                    // not incredibly likely, if a game tries to render more than 65,535 lines we don't want to
                    // crash. We handle this by doing a loop and drawing as many lines as we can at a time, capped
                    // at our limit. We then move ahead in our vertex array and draw the next set of lines.
                    int vertexOffset = 0;
                    while (lineCount > 0)
                    {
                        // Figure out how many lines we're going to draw
                        int linesToDraw = Math.Min(lineCount, 65535);

                        FrameStats.DrawUserPrimitivesCount ++;
                        zGfx.DrawUserPrimitives(
                            PrimitiveType.LineList,
                            verts,
                            vertexOffset,
                            linesToDraw,
                            VertexPositionColour.Default.VertexDeclaration
                            );

                        // Move our vertex offset ahead based on the lines we drew
                        vertexOffset += linesToDraw * 2;

                        // Remove these lines from our total line count
                        lineCount -= linesToDraw;
                    }
                }

                zGfx.GpuUtils.EndEvent();
            }
        }


        void InitializeSphere()
        {
            // We need two vertices per line, so we can allocate our vertices
            unitSphere = new Vector3[sphereLineCount * 2];

            float tau; RealMaths.Tau(out tau);

            // Compute our step around each circle
            float step = tau / sphereResolution;

            // Used to track the index into our vertex array
            int index = 0;

            // Create the loop on the XY plane first
            for (float a = 0f; a < tau; a += step)
            {
                unitSphere[index++] = new Vector3((float)Math.Cos(a), (float)Math.Sin(a), 0f);
                unitSphere[index++] = new Vector3((float)Math.Cos(a + step), (float)Math.Sin(a + step), 0f);
            }

            // Next on the XZ plane
            for (float a = 0f; a < tau; a += step)
            {
                unitSphere[index++] = new Vector3((float)Math.Cos(a), 0f, (float)Math.Sin(a));
                unitSphere[index++] = new Vector3((float)Math.Cos(a + step), 0f, (float)Math.Sin(a + step));
            }

            // Finally on the YZ plane
            for (float a = 0f; a < tau; a += step)
            {
                unitSphere[index++] = new Vector3(0f, (float)Math.Cos(a), (float)Math.Sin(a));
                unitSphere[index++] = new Vector3(0f, (float)Math.Cos(a + step), (float)Math.Sin(a + step));
            }
        }

        static int CachedShapesSort(DebugShape s1, DebugShape s2)
        {
            return s1.Vertices.Length.CompareTo(s2.Vertices.Length);
        }

        DebugShape GetShapeForLines(int lineCount, float life)
        {
            DebugShape shape = null;

            // We go through our cached list trying to find a shape that contains
            // a large enough array to hold our desired line count. If we find such
            // a shape, we move it from our cached list to our active list and break
            // out of the loop.
            int vertCount = lineCount * 2;
            for (int i = 0; i < cachedShapes.Count; i++)
            {
                if (cachedShapes[i].Vertices.Length >= vertCount)
                {
                    shape = cachedShapes[i];
                    cachedShapes.RemoveAt(i);
                    activeShapes.Add(shape);
                    break;
                }
            }

            // If we didn't find a shape in our cache, we create a new shape and add it
            // to the active list.
            if (shape == null)
            {
                shape = new DebugShape { Vertices = new VertexPositionColour[vertCount] };
                activeShapes.Add(shape);
            }

            // Set the line count and lifetime of the shape based on our parameters.
            shape.LineCount = lineCount;
            shape.Lifetime = life;

            return shape;
        }
    }

    public class GridRenderer
    {
        public Rgba32 AxisColourX = Rgba32.Red; // going to the right of the screen
        public Rgba32 AxisColourY = Rgba32.Green; // up
        public Rgba32 AxisColourZ = Rgba32.Blue; // coming out of the screen

        float gridSquareSize = 0.50f;  // in meters
        int numberOfGridSquares = 10;

        Rgba32 gridColour = Rgba32.LightGrey;

        public bool ShowXZPlane = true;
        public bool ShowXYPlane = false;
        public bool ShowYZPlane = false;

        string renderPass;
        DebugShapeRenderer debugRenderer;

        public GridRenderer(DebugShapeRenderer debugRenderer, string renderPass, float gridSquareSize = 0.50f, int numberOfGridSquares = 10)
        {
            this.debugRenderer = debugRenderer;
            this.renderPass = renderPass;
            this.gridSquareSize = gridSquareSize;
            this.numberOfGridSquares = numberOfGridSquares;
        }

        public void Update()
        {
            float length = numberOfGridSquares * 2 * gridSquareSize;
            float halfLength = length / 2;

            if( ShowXZPlane )
            {
                for (int i = 0; i < (numberOfGridSquares * 2 + 1); i++)
                {
                    if (i * gridSquareSize - halfLength == 0)
                        continue;

                    debugRenderer.AddLine(
                        renderPass,
                        new Vector3(-halfLength, 0.0f, i * gridSquareSize - halfLength),
                        new Vector3(halfLength, 0.0f, i * gridSquareSize - halfLength),
                        gridColour);

                    debugRenderer.AddLine(
                        renderPass,
                        new Vector3(i * gridSquareSize - halfLength, 0.0f, -halfLength),
                        new Vector3(i * gridSquareSize - halfLength, 0.0f, halfLength),
                        gridColour);
                }
            }

            if( ShowXYPlane )
            {
                for (int i = 0; i < (numberOfGridSquares * 2 + 1); i++)
                {
                    if (i * gridSquareSize - halfLength == 0)
                        continue;
                    
                    debugRenderer.AddLine(
                        renderPass,
                        new Vector3(-halfLength, i * gridSquareSize - halfLength, 0f),
                        new Vector3(halfLength, i * gridSquareSize - halfLength, 0f),
                        gridColour);
                    
                    debugRenderer.AddLine(
                        renderPass,
                        new Vector3(i * gridSquareSize - halfLength, -halfLength, 0f),
                        new Vector3(i * gridSquareSize - halfLength, halfLength, 0f),
                        gridColour);
                }
            }

            if( ShowYZPlane )
            {
                for (int i = 0; i < (numberOfGridSquares * 2 + 1); i++)
                {
                    if (i * gridSquareSize - halfLength == 0)
                        continue;
                    
                    debugRenderer.AddLine(
                        renderPass,
                        new Vector3(0f, -halfLength, i * gridSquareSize - halfLength),
                        new Vector3(0f, halfLength, i * gridSquareSize - halfLength),
                        gridColour);
                    
                    debugRenderer.AddLine(
                        renderPass,
                        new Vector3(0f, i * gridSquareSize - halfLength, -halfLength),
                        new Vector3(0f, i * gridSquareSize - halfLength, halfLength),
                        gridColour);
                }
            }


            if( ShowXYPlane )
            {
                for (int i = 0; i < (numberOfGridSquares * 2 + 1); i++)
                {
                    if (i * gridSquareSize - halfLength == 0)
                        continue;
                    
                    debugRenderer.AddLine(
                        renderPass,
                        new Vector3(-halfLength, i * gridSquareSize - halfLength, 0f),
                        new Vector3(halfLength, i * gridSquareSize - halfLength, 0f),
                        gridColour);
                    
                    debugRenderer.AddLine(
                        renderPass,
                        new Vector3(i * gridSquareSize - halfLength, -halfLength, 0f),
                        new Vector3(i * gridSquareSize - halfLength, halfLength, 0f),
                        gridColour);
                }
            }

            debugRenderer.AddLine(
                    renderPass,
                new Vector3(-numberOfGridSquares * gridSquareSize, 0, 0),
                new Vector3(0, 0, 0),
                Rgba32.White);

            debugRenderer.AddLine(
                    renderPass,
                new Vector3(0, -numberOfGridSquares * gridSquareSize, 0),
                new Vector3(0, 0, 0),
                Rgba32.White);

            debugRenderer.AddLine(
                    renderPass,
                new Vector3(0, 0, -numberOfGridSquares * gridSquareSize),
                new Vector3(0, 0, 0),
                Rgba32.White);


            debugRenderer.AddLine(
                    renderPass,
                new Vector3(0, 0, 0),
                new Vector3(numberOfGridSquares * gridSquareSize, 0, 0),
                AxisColourX);

            debugRenderer.AddLine(
                    renderPass,
                new Vector3(0, 0, 0),
                new Vector3(0, numberOfGridSquares * gridSquareSize, 0),
                AxisColourY);

            debugRenderer.AddLine(
                    renderPass,
                new Vector3(0, 0, 0),
                new Vector3(0, 0, numberOfGridSquares * gridSquareSize),
                AxisColourZ);

        }
    }


    #endregion

    #region Enums

    public enum CameraProjectionType
    {
        Perspective,
        Orthographic,
    }

    public enum GestureType
    {
        Tap, // multiiple fingers
        DoubleTap, // multiple fingers
        Flick,
        Drag,
        DragUpdate,
        DragComplete,
        Pinch,
        Pivot,
    }

    public enum TouchPositionSpace
    {
        NormalisedEngine,
        Screen,
        RealWorld

    }


    #endregion

    #region Gestures

    public class Gesture
    {
        static int GestureIDAssigner = 0;

        InputEventSystem inputEventSystem;
        Int32 id;
        GestureType type;
        Int32[] touchIDs;

        public Vector2 GetFinishingPosition(TouchPositionSpace space)
        {

            Vector2 averageFinishPos = Vector2.Zero ;
            foreach (Int32 touchID in TouchIDs)
            {
                var tracker = inputEventSystem.GetTouchTracker(touchID);

                var p = tracker.GetPosition(space);

                averageFinishPos += p;
            }

            averageFinishPos /= TouchIDs.Length;

            return averageFinishPos;  
        }

        public Gesture(InputEventSystem inputEventSystem, GestureType type, Int32[] touchIDs)
        {
            this.inputEventSystem = inputEventSystem;
            this.id = GestureIDAssigner;
            GestureIDAssigner++;
            this.type = type;
            this.touchIDs = touchIDs;
        }

        public Int32 ID
        {
            get
            {
                return this.id;
            }
        }

        public GestureType Type
        {
            get
            {
                return this.type;
            }
        }

        public Int32[] TouchIDs
        {
            get
            {
                return this.touchIDs;
            }
        }

        public List<TouchTracker> TouchTrackers
        {
            get
            {
                var tt = new List<TouchTracker>();

                foreach (Int32 touchID in TouchIDs)
                {
                    var tracker = inputEventSystem.GetTouchTracker(touchID);
                    tt.Add(tracker);
                }

                return tt;
            }
        }
    }

    public class InputEventSystem
    {
        IMultiTouchController controller;

        internal InputEventSystem(ICor engine)
        {
            this.engine = engine;

            this.controller = engine.Input.MultiTouchController;
        }

        ICor engine;

        public delegate void GestureDelegate(Gesture gesture);

        public event GestureDelegate Tap;
        public event GestureDelegate DoubleTap;
        public event GestureDelegate Flick;
        //public event GestureDelegate DragStart;
        //public event GestureDelegate DragUpdate;
        //public event GestureDelegate DragEnd;
        //public event GestureDelegate Pinch;


        internal TouchTracker GetTouchTracker(Int32 id)
        {
            var found = touchTrackers.Find(q => q.TouchID == id);

            return found;
        }

        List<TouchTracker> touchTrackers = new List<TouchTracker>();

        Queue<Gesture> gestureQueue = new Queue<Gesture>();

        List<PotentialGesture> potentialGestures = new List<PotentialGesture>();

        internal void Reset()
        {
            // release all listeners
            Tap = null;
            DoubleTap = null;
            Flick = null;
        }

        internal virtual void Update(AppTime time)
        {
            if( controller != null )
            {
                // before this the child should have updated this TouchCollection
                this.UpdateTouchTrackers(time);
                this.UpdateGestureDetection(time);
                this.InvokeGestureEvents(time);
            }
        }

        void UpdateTouchTrackers(AppTime time)
        {
            
            // delete all touch trackers that whose last touch was in the released state
            int num = touchTrackers.RemoveAll(x => (x.Phase == TouchPhase.JustReleased || x.Phase == TouchPhase.Invalid));          

            if( num > 0 )
            {
                Teletype.WriteLine("Blimey.Input", string.Format("Removing {0} touches.", num));
            }

            // go through all active touches
            foreach (var touch in controller.TouchCollection)
            {
                // find the corresponding tracker

                TouchTracker tracker = touchTrackers.Find(x => (x.TouchID == touch.ID));

                if (tracker == null)
                {
                    tracker = new TouchTracker(
                        this.engine, 
                        this.engine.System.ScreenSpecification,
                        this.engine.System.PanelSpecification, 
                        touch.ID );

                    touchTrackers.Add(tracker);
                }

                tracker.RegisterTouch(touch);
            }

            // assert if there are any trackers in the list that have not been updated this frame
            var problems = touchTrackers.FindAll(x => (x.LatestTouch.FrameNumber != time.FrameNumber));
            System.Diagnostics.Debug.Assert(problems.Count == 0);
        }

        void UpdateGestureDetection(AppTime time)
        {
            // Each frame we look for press combinations that could potentially
            // be the start of a gesture.
            foreach (var touchTracker in touchTrackers)
            {
                if( touchTracker.Phase == TouchPhase.JustPressed )
                {
                    // this could be the start of a tap
                    var potentialTapGesture = 
                        new PotentialTapGesture(
                            this,
                            new Int32[]{touchTracker.TouchID} );

                    var potentialDoubleTapGesture = 
                        new PotentialDoubleTapGesture(
                            this,
                            new Int32[]{touchTracker.TouchID} );

                    var potentialFlickGesture = 
                        new PotentialFlickGesture(
                            this,
                            new Int32[]{touchTracker.TouchID} );

                    potentialGestures.Add(potentialTapGesture);
                    potentialGestures.Add(potentialDoubleTapGesture);
                    potentialGestures.Add(potentialFlickGesture);

                }

                int enqueueCount = 0;

                foreach(var potentialGesture in potentialGestures)
                {
                    var gesture = potentialGesture.Update(time.Delta, touchTrackers);

                    if( gesture != null )
                    {
                        this.gestureQueue.Enqueue(gesture);
                        enqueueCount++;
                    }
                }

                int removeCount = potentialGestures.Count;
                potentialGestures.RemoveAll(x => x.Finished );
                removeCount -= potentialGestures.Count;

            }
        }

        void InvokeGestureEvents(AppTime time)
        {
            foreach (var gesture in gestureQueue)
            {
                string line = string.Format("({1}) {0}", gesture.Type, gesture.ID);
                switch (gesture.Type)
                {
                    case GestureType.Tap:
                        line += string.Format(", finishing position {0}", gesture.GetFinishingPosition(TouchPositionSpace.NormalisedEngine));
                        
                        if (this.Tap != null)
                        {
                            this.Tap(gesture);
                        }
                        break;

                    case GestureType.DoubleTap:
                        if (this.DoubleTap != null)
                        {
                            this.DoubleTap(gesture);
                        }
                        break;

                    case GestureType.Flick:
                        line += string.Format(", finishing position {0}", gesture.GetFinishingPosition(TouchPositionSpace.NormalisedEngine));
                        if (this.Flick != null)
                        {
                            this.Flick(gesture);
                        }
                        break;

                    default: throw new System.NotImplementedException();
                }

                Teletype.WriteLine("Blimey.Input", line);
            }

            gestureQueue.Clear();
        }
    }

    internal class PotentialDoubleTapGesture
        : PotentialGesture
    {
        internal PotentialDoubleTapGesture(
            InputEventSystem inputEventSystem,
            Int32[] touchIDs)
            : base(inputEventSystem, GestureType.DoubleTap, touchIDs)
        {

        }

        internal override Gesture Update(float dt, List<TouchTracker> touchTrackers)
        {
            var touchTracker = inputEventSystem.GetTouchTracker(touchIDs[0]);

            if (touchTracker == null)
            {
                failedGesture = true;
                return null;
            }

            failedGesture = true;
            return null;
        }
    }

    internal class PotentialFlickGesture
        : PotentialGesture
    {
        const float velocityRequired = 0.05f;
        const float displacementRequired = 0.01f;

        internal PotentialFlickGesture(
            InputEventSystem inputEventSystem,
            Int32[] touchIDs)
            : base(inputEventSystem, GestureType.Flick, touchIDs)
        {

        }

        internal override Gesture Update(
            float dt,
            List<TouchTracker> touchTrackers)
        {
            var touchTracker = inputEventSystem.GetTouchTracker(touchIDs[0]);

            if (touchTracker == null)
            {
                failedGesture = true;
                return null;
            }

            var velocity = touchTracker.GetVelocity(TouchPositionSpace.RealWorld).Length();


            float distanceTravelled = touchTracker.GetDistanceTraveled(TouchPositionSpace.RealWorld);

            if (velocity >= velocityRequired &&
                distanceTravelled >= displacementRequired &&
                touchTracker.Phase == TouchPhase.JustReleased )
            {
                completedGesture = true;
                return new Gesture(this.inputEventSystem, this.type, this.touchIDs);
            }

            if( touchTracker.Phase == TouchPhase.JustReleased )
            {
                failedGesture = true;
            }

            return null;
        }
    }

    internal abstract class PotentialGesture
    {

        // in meters
        const float DISPLACEMENT_REQUIRED_FOR_DRAGS = 0.01f;
        const float MAX_DISPLACEMENT_FOR_TAPS = 0.005f;

        protected InputEventSystem inputEventSystem;

        static int PotentialGestureIDAssigner = 0;

        internal PotentialGesture(
            InputEventSystem inputEventSystem,
            GestureType type, Int32[] touchIDs)
        {
            this.id = PotentialGestureIDAssigner;
            this.inputEventSystem = inputEventSystem;
            PotentialGestureIDAssigner++;

            this.type = type;
            this.touchIDs = touchIDs;
        }

        internal abstract Gesture Update(Single dt, List<TouchTracker> touchTrackers);

        internal bool Finished { get { return failedGesture || completedGesture; } }

        protected bool failedGesture = false;
        protected bool completedGesture = false;

        protected Int32 id;
        protected GestureType type;
        protected Int32[] touchIDs;

    }

    internal class PotentialTapGesture
        : PotentialGesture
    {
        const Single MaxHoldTimeForTap = 0.6f;
        const Single MaxDisplacementForTap = 0.005f;

        Single timer = 0f;

        internal PotentialTapGesture(InputEventSystem inputEventSystem, Int32[] touchIDs)
            : base(inputEventSystem, GestureType.Tap, touchIDs)
        {

        }

        internal override Gesture Update(float dt, List<TouchTracker> touchTrackers)
        {
            if( failedGesture )
                throw new Exception("wrong!");

            this.timer += dt;

            if( this.timer > MaxHoldTimeForTap)
                failedGesture = true;

            var touchTracker = inputEventSystem.GetTouchTracker(touchIDs[0]);

            if (touchTracker == null)
            {
                failedGesture = true;
                return null;
            }


            if( touchTracker.Phase == TouchPhase.JustReleased )
            {
                float distanceTravelled = touchTracker.GetDistanceTraveled(TouchPositionSpace.RealWorld);
                if (distanceTravelled <= MaxDisplacementForTap)
                {
                    completedGesture = true;
                    return new Gesture(this.inputEventSystem, this.type, this.touchIDs);
                }
                else
                {
                    failedGesture = true;
                }

            }

            return null;
        }
    }

    public class TouchTracker
    {
        const Int32 NumFramesPerTrackedTouch = 15;

        Int32 trackCounter = -1;
        Int32 id;
        List<Touch> samples = new List<Touch>();
        IScreenSpecification screenSpec;
        IPanelSpecification panelSpec;
        ICor engine;

        internal TouchTracker(
            ICor engine,
            IScreenSpecification displayMode,
            IPanelSpecification panelMode,
            Int32 id )
        {
            this.engine = engine;
            this.screenSpec = displayMode;
            this.panelSpec = panelMode;
            this.id = id;

        }

        internal void RegisterTouch(Touch t)
        {
            if( trackCounter == -1 )
            {
                this.samples.Add(t);
            }
            else
            {
                if( trackCounter % NumFramesPerTrackedTouch == 0 )
                {
                    this.samples.Add(t);
                }
                else
                {
                    this.samples[this.samples.Count -1] = t;
                }
            }


            trackCounter++;

        }

        internal Touch LatestTouch { get { return this.samples.Last(); } }

        internal Int32 TouchID { get { return this.id; } }

        internal TouchPhase Phase { get { return samples.Last().Phase; } }

        Vector2 GetPositionOfSampleAtIndex(int index, TouchPositionSpace space)
        {
            var pos = this.samples[index].Position;

            var multiplier = Vector2.One;
            switch (space)
            {
                case TouchPositionSpace.RealWorld:

                    if(engine.System.CurrentOrientation == DeviceOrientation.Default ||
                       engine.System.CurrentOrientation == DeviceOrientation.Upsidedown)
                    {
                        multiplier = new Vector2(panelSpec.PanelPhysicalSize.X, panelSpec.PanelPhysicalSize.Y);
                    }
                    else
                    {
                        multiplier = new Vector2(panelSpec.PanelPhysicalSize.Y, panelSpec.PanelPhysicalSize.X);
                    }

                    break;

                case TouchPositionSpace.Screen:

                    if (this.engine.System.CurrentOrientation == DeviceOrientation.Upsidedown )
                    {
                        pos.Y = - pos.Y;
                        pos.X = - pos.X;
                    }
                    else if (this.engine.System.CurrentOrientation == DeviceOrientation.Leftside )
                    {
                        Single temp = pos.X;
                        pos.X = -pos.Y;
                        pos.Y = temp;
                    }
                    else if(this.engine.System.CurrentOrientation == DeviceOrientation.Rightside )
                    {
                        Single temp = pos.X;
                        pos.X = pos.Y;
                        pos.Y = -temp;
                    }

                    Int32 w = this.engine.Graphics.DisplayStatus.CurrentWidth;
                    Int32 h = this.engine.Graphics.DisplayStatus.CurrentHeight;

                    //this.engine.System.GetEffectiveDisplaySize(ref w, ref h);

                    multiplier = new Vector2(w, h);

                    break;

            }
            pos *= multiplier;

            return pos;
        }

        public Vector2 GetPosition(TouchPositionSpace space)
        {
            int numSamples = samples.Count;

            var curPos = this.GetPositionOfSampleAtIndex(numSamples - 1, space);



            return curPos;
        }

        public Vector2 GetVelocity(TouchPositionSpace space)
        {
            int numSamples = samples.Count;

            if (numSamples > 1)
            {
                var currentTouch = this.samples[numSamples - 1];
                var previousTouch = this.samples[numSamples - 2];

                var currentPos = this.GetPositionOfSampleAtIndex(numSamples - 1, space);
                var previousPos = this.GetPositionOfSampleAtIndex(numSamples - 2, space);

                Single dt = currentTouch.Timestamp - previousTouch.Timestamp;

                return (currentPos - previousPos) / dt;
            }

            

            return Vector2.Zero;
        }

        public Single GetDistanceTraveled(TouchPositionSpace posType)
        {
            Single distance = 0f;

            for (Int32 i = 0; i < samples.Count; ++i)
            {
                if (i > 0)
                {
                    var currentPosition = this.GetPositionOfSampleAtIndex(i, posType);
                    var previousPosition = this.GetPositionOfSampleAtIndex(i - 1, posType);

                    Single mag = (currentPosition - previousPosition).Length();

                    distance += mag;
                }
            }

            return distance;
        }
    }


    #endregion

    #region Primitives

    public class BillboardPrimitive
        : GeometricPrimitive
    {
        public BillboardPrimitive (IGraphicsManager graphicsDevice)
        {

            // Six indices (two triangles) per face.
            AddIndex (0);
            AddIndex (1);
            AddIndex (2);

            AddIndex (0);
            AddIndex (2);
            AddIndex (3);


            // Four vertices per face.
            AddVertex ((-Vector3.Right - Vector3.Forward) / 2, Vector3.Up);
            AddVertex ((-Vector3.Right + Vector3.Forward) / 2, Vector3.Up);
            AddVertex ((Vector3.Right + Vector3.Forward) / 2, Vector3.Up);
            AddVertex ((Vector3.Right - Vector3.Forward) / 2, Vector3.Up);

            InitializePrimitive (graphicsDevice);
        }
    }

    public class CubePrimitive
        : GeometricPrimitive
    {
        /// <summary>
        /// Constructs a new cube primitive, with the specified size.
        /// </summary>
        public CubePrimitive (IGraphicsManager graphicsDevice)
        {
            // A cube has six faces, each one pointing in a different direction.
            Vector3[] normals =
            {
                new Vector3 (0, 0, 1),
                new Vector3 (0, 0, -1),
                new Vector3 (1, 0, 0),
                new Vector3 (-1, 0, 0),
                new Vector3 (0, 1, 0),
                new Vector3 (0, -1, 0),
            };

            // Create each face in turn.
            for (int i = 0; i < normals.Length; ++i )
            {
                Vector3 n = normals[i];

                // Get two vectors perpendicular to the face normal and to each other.
                Vector3 side1 = new Vector3(n.Y, n.Z, n.X);
                Vector3 side2;
                
                Vector3.Cross(ref n, ref side1, out side2);

                // Six indices (two triangles) per face.
                AddIndex(CurrentVertex + 0);
                AddIndex(CurrentVertex + 1);
                AddIndex(CurrentVertex + 2);

                AddIndex(CurrentVertex + 0);
                AddIndex(CurrentVertex + 2);
                AddIndex(CurrentVertex + 3);

                // Four vertices per face.
                AddVertex((n - side1 - side2) / 2, n);
                AddVertex((n - side1 + side2) / 2, n);
                AddVertex((n + side1 + side2) / 2, n);
                AddVertex((n + side1 - side2) / 2, n);
            }

            InitializePrimitive (graphicsDevice);
        }
    }

    public class CylinderPrimitive
        : GeometricPrimitive
    {
        const int _tessellation = 32;
        const float _height = 0.5f;
        const float _radius = 0.5f;

        public CylinderPrimitive (IGraphicsManager graphicsDevice)
        {
            Debug.Assert (_tessellation >= 3);

            // Create a ring of triangles around the outside of the cylinder.
            for (int i = 0; i <= _tessellation; i++) {
                Vector3 normal = GetCircleVector (i, _tessellation);

                Vector3 topPos = normal * _radius + Vector3.Up * _height;
                Vector3 botPos = normal * _radius + Vector3.Down * _height;

                AddVertex (topPos, normal);
                AddVertex (botPos, normal);
            }

            for (int i = 0; i < _tessellation; i++) {
                AddIndex (i * 2);
                AddIndex (i * 2 + 1);
                AddIndex ((i * 2 + 2));

                AddIndex (i * 2 + 1);
                AddIndex (i * 2 + 3);
                AddIndex (i * 2 + 2);
            }


            // Create flat triangle fan caps to seal the top and bottom.
            CreateCap (_tessellation, _height, _radius, Vector3.Up);
            CreateCap (_tessellation, _height, _radius, Vector3.Down);

            InitializePrimitive (graphicsDevice);
        }

        /// <summary>
        /// Helper method creates a triangle fan to close the ends of the cylinder.
        /// </summary>
        void CreateCap (int tessellation, float height, float radius, Vector3 normal)
        {
            // Create cap indices.
            for (int i = 0; i < tessellation - 2; i++) {
                if (normal.Y > 0) {
                    AddIndex (CurrentVertex);
                    AddIndex (CurrentVertex + (i + 1) % tessellation);
                    AddIndex (CurrentVertex + (i + 2) % tessellation);
                } else {
                    AddIndex (CurrentVertex);
                    AddIndex (CurrentVertex + (i + 2) % tessellation);
                    AddIndex (CurrentVertex + (i + 1) % tessellation);
                }
            }

            // Create cap vertices.
            for (int i = 0; i < tessellation; i++) {
                Vector3 circleVec = GetCircleVector (i, tessellation);
                Vector3 position = circleVec * radius +
                                   normal * height;

                AddVertex (position, normal);
            }
        }


        /// <summary>
        /// Helper method computes a point on a circle.
        /// </summary>
        static Vector3 GetCircleVector (int i, int tessellation)
        {
            float tau; RealMaths.Tau(out tau);
            float angle = i * tau / tessellation;

            float dx = (float)Math.Cos (angle);
            float dz = (float)Math.Sin (angle);

            return new Vector3 (dx, 0, dz);
        }
    }

    public abstract class BezierPrimitive
        : GeometricPrimitive
    {
        /// <summary>
        /// Creates indices for a patch that is tessellated at the specified level.
        /// </summary>
        protected void CreatePatchIndices(int tessellation, Boolean isMirrored)
        {
            int stride = tessellation + 1;

            for (int i = 0; i < tessellation; i++)
            {
                for (int j = 0; j < tessellation; j++)
                {
                    // Make a list of six index values (two triangles).
                    int[] indices =
                    {
                        i * stride + j,
                        (i + 1) * stride + j,
                        (i + 1) * stride + j + 1,

                        i * stride + j,
                        (i + 1) * stride + j + 1,
                        i * stride + j + 1,
                    };

                    // If this patch is mirrored, reverse the
                    // indices to keep the correct winding order.
                    if (isMirrored)
                    {
                        Array.Reverse(indices);
                    }

                    // Create the indices.
                    foreach (int index in indices)
                    {
                        AddIndex(CurrentVertex + index);
                    }
                }
            }
        }


        /// <summary>
        /// Creates vertices for a patch that is tessellated at the specified level.
        /// </summary>
        protected void CreatePatchVertices(Vector3[] patch, int tessellation, Boolean isMirrored)
        {
            Debug.Assert(patch.Length == 16);

            for (int i = 0; i <= tessellation; i++)
            {
                float ti = (float)i / tessellation;

                for (int j = 0; j <= tessellation; j++)
                {
                    float tj = (float)j / tessellation;

                    // Perform four horizontal bezier interpolations
                    // between the control points of this patch.
                    Vector3 p1 = Bezier(patch[0], patch[1], patch[2], patch[3], ti);
                    Vector3 p2 = Bezier(patch[4], patch[5], patch[6], patch[7], ti);
                    Vector3 p3 = Bezier(patch[8], patch[9], patch[10], patch[11], ti);
                    Vector3 p4 = Bezier(patch[12], patch[13], patch[14], patch[15], ti);

                    // Perform a vertical interpolation between the results of the
                    // previous horizontal interpolations, to compute the position.
                    Vector3 position = Bezier(p1, p2, p3, p4, tj);

                    // Perform another four bezier interpolations between the control
                    // points, but this time vertically rather than horizontally.
                    Vector3 q1 = Bezier(patch[0], patch[4], patch[8], patch[12], tj);
                    Vector3 q2 = Bezier(patch[1], patch[5], patch[9], patch[13], tj);
                    Vector3 q3 = Bezier(patch[2], patch[6], patch[10], patch[14], tj);
                    Vector3 q4 = Bezier(patch[3], patch[7], patch[11], patch[15], tj);

                    // Compute vertical and horizontal tangent vectors.
                    Vector3 tangentA = BezierTangent(p1, p2, p3, p4, tj);
                    Vector3 tangentB = BezierTangent(q1, q2, q3, q4, ti);

                    // Cross the two tangent vectors to compute the normal.
                    Vector3 normal;
                    Vector3.Cross(ref tangentA, ref tangentB, out normal);

                    if (normal.Length() > 0.0001f)
                    {
                        Vector3.Normalise(ref normal, out normal);

                        // If this patch is mirrored, we must invert the normal.
                        if (isMirrored)
                            normal = -normal;
                    }
                    else
                    {
                        // In a tidy and well constructed bezier patch, the preceding
                        // normal computation will always work. But the classic teapot
                        // model is not tidy or well constructed! At the top and bottom
                        // of the teapot, it contains degenerate geometry where a patch
                        // has several control points in the same place, which causes
                        // the tangent computation to fail and produce a zero normal.
                        // We 'fix' these cases by just hard-coding a normal that points
                        // either straight up or straight down, depending on whether we
                        // are on the top or bottom of the teapot. This is not a robust
                        // solution for all possible degenerate bezier patches, but hey,
                        // it's good enough to make the teapot work correctly!

                        if (position.Y > 0)
                            normal = Vector3.Up;
                        else
                            normal = Vector3.Down;
                    }

                    // Create the vertex.
                    AddVertex(position, normal);
                }
            }
        }


        /// <summary>
        /// Performs a cubic bezier interpolation between four scalar control
        /// points, returning the value at the specified time (t ranges 0 to 1).
        /// </summary>
        static float Bezier(float p1, float p2, float p3, float p4, float t)
        {
            return p1 * (1 - t) * (1 - t) * (1 - t) +
                   p2 * 3 * t * (1 - t) * (1 - t) +
                   p3 * 3 * t * t * (1 - t) +
                   p4 * t * t * t;
        }


        /// <summary>
        /// Performs a cubic bezier interpolation between four Vector3 control
        /// points, returning the value at the specified time (t ranges 0 to 1).
        /// </summary>
        static Vector3 Bezier(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float t)
        {
            Vector3 result = new Vector3();

            result.X = Bezier(p1.X, p2.X, p3.X, p4.X, t);
            result.Y = Bezier(p1.Y, p2.Y, p3.Y, p4.Y, t);
            result.Z = Bezier(p1.Z, p2.Z, p3.Z, p4.Z, t);

            return result;
        }


        /// <summary>
        /// Computes the tangent of a cubic bezier curve at the specified time,
        /// when given four scalar control points.
        /// </summary>
        static float BezierTangent(float p1, float p2, float p3, float p4, float t)
        {
            return p1 * (-1 + 2 * t - t * t) +
                   p2 * (1 - 4 * t + 3 * t * t) +
                   p3 * (2 * t - 3 * t * t) +
                   p4 * (t * t);
        }


        /// <summary>
        /// Computes the tangent of a cubic bezier curve at the specified time,
        /// when given four Vector3 control points. This is used for calculating
        /// normals (by crossing the horizontal and vertical tangent vectors).
        /// </summary>
        static Vector3 BezierTangent(Vector3 p1, Vector3 p2,
                                     Vector3 p3, Vector3 p4, float t)
        {
            Vector3 result = new Vector3();

            result.X = BezierTangent(p1.X, p2.X, p3.X, p4.X, t);
            result.Y = BezierTangent(p1.Y, p2.Y, p3.Y, p4.Y, t);
            result.Z = BezierTangent(p1.Z, p2.Z, p3.Z, p4.Z, t);

            try
            {
                Vector3.Normalise(ref result, out result);
            }
            catch
            {
                result = Vector3.Zero;
            }

            return result;
        }
    }


    public abstract class GeometricPrimitive
        : Mesh
    {
        public static IVertexType vertType = new VertexPositionNormal();

        public override VertexDeclaration VertDecl { get { return vertType.VertexDeclaration; } }

        // Once all the geometry has been specified by calling AddVertex and AddIndex,
        // this method copies the vertex and index data into GPU format buffers, ready
        // for efficient rendering.
        protected void InitializePrimitive(IGraphicsManager gfx)
        {

            GeomBuffer = gfx.CreateGeometryBuffer(VertDecl, vertices.Count, indices.Count);
            
            GeomBuffer.VertexBuffer.SetData(vertices.ToArray());

            GeomBuffer.IndexBuffer.SetData(indices.ToArray());

            //todo, move to base mesh abstract class
            TriangleCount = indices.Count / 3;
            VertexCount = vertices.Count;

        }

        /// <summary>
        /// Queries the index of the current vertex. This starts at
        /// zero, and increments every time AddVertex is called.
        /// </summary>
        protected Int32 CurrentVertex
        {
            get { return vertices.Count; }
        }

        /// <summary>
        /// Adds a new vertex to the primitive model. This should only be called
        /// during the initialization process, before InitializePrimitive.
        /// </summary>
        protected void AddVertex(Vector3 position, Vector3 normal)
        {
            var vertElement = new VertexPositionNormal(position, normal);
            vertices.Add(vertElement);
        }


        /// <summary>
        /// Adds a new index to the primitive model. This should only be called
        /// during the initialization process, before InitializePrimitive.
        /// </summary>
        protected void AddIndex(Int32 index)
        {
            if (index > UInt16.MaxValue)
                throw new ArgumentOutOfRangeException("index");

            indices.Add((UInt16)index);
        }

        // During the process of constructing a primitive model, vertex
        // and index data is stored on the CPU in these managed lists.
        List<VertexPositionNormal> vertices = new List<VertexPositionNormal>();
        List<Int32> indices = new List<Int32>();
    }

    public class SpherePrimitive
        : GeometricPrimitive
    {
        const float diameter = 1f;
        const int tessellation = 16;

        /// <summary>
        /// Constructs a new cube primitive, with the specified size.
        /// </summary>
        public SpherePrimitive(IGraphicsManager graphicsDevice)
        {
            Debug.Assert(tessellation >= 3);


            int verticalSegments = tessellation;
            int horizontalSegments = tessellation * 2;

            float radius = diameter / 2;

            // Start with a single vertex at the bottom of the sphere.
            AddVertex(Vector3.Down * radius, Vector3.Down);

            float pi; RealMaths.Pi(out pi);
            float piOver2 = pi / 2;
            // Create rings of vertices at progressively higher latitudes.
            for (int i = 0; i < verticalSegments - 1; i++)
            {
                float latitude = ((i + 1) * pi /
                                            verticalSegments) - piOver2;

                float dy = (float)Math.Sin(latitude);
                float dxz = (float)Math.Cos(latitude);

                float tau; RealMaths.Tau(out tau);

                // Create a single ring of vertices at this latitude.
                for (int j = 0; j < horizontalSegments; j++)
                {
                    float longitude = j * tau / horizontalSegments;

                    float dx = (float)Math.Cos(longitude) * dxz;
                    float dz = (float)Math.Sin(longitude) * dxz;

                    Vector3 normal = new Vector3(dx, dy, dz);

                    AddVertex(normal * radius, normal);
                }
            }

            // Finish with a single vertex at the top of the sphere.
            AddVertex(Vector3.Up * radius, Vector3.Up);

            // Create a fan connecting the bottom vertex to the bottom latitude ring.
            for (int i = 0; i < horizontalSegments; i++)
            {
                AddIndex(0);
                AddIndex(1 + (i + 1) % horizontalSegments);
                AddIndex(1 + i);
            }

            // Fill the sphere body with triangles joining each pair of latitude rings.
            for (int i = 0; i < verticalSegments - 2; i++)
            {
                for (int j = 0; j < horizontalSegments; j++)
                {
                    int nextI = i + 1;
                    int nextJ = (j + 1) % horizontalSegments;

                    AddIndex(1 + i * horizontalSegments + j);
                    AddIndex(1 + i * horizontalSegments + nextJ);
                    AddIndex(1 + nextI * horizontalSegments + j);

                    AddIndex(1 + i * horizontalSegments + nextJ);
                    AddIndex(1 + nextI * horizontalSegments + nextJ);
                    AddIndex(1 + nextI * horizontalSegments + j);
                }
            }

            // Create a fan connecting the top vertex to the top latitude ring.
            for (int i = 0; i < horizontalSegments; i++)
            {
                AddIndex(CurrentVertex - 1);
                AddIndex(CurrentVertex - 2 - (i + 1) % horizontalSegments);
                AddIndex(CurrentVertex - 2 - i);
            }

            InitializePrimitive(graphicsDevice);
        }
    }

    public class TeapotPrimitive
        : BezierPrimitive
    {
        const float size = 1; 
        const int tessellation = 8;
        /// <summary>
        /// Constructs a new cube primitive, with the specified size.
        /// </summary>
        public TeapotPrimitive(IGraphicsManager graphicsDevice)
        {
            Debug.Assert(tessellation >= 1);

            foreach (TeapotPatch patch in TeapotPatches)
            {
                // Because the teapot is symmetrical from left to right, we only store
                // data for one side, then tessellate each patch twice, mirroring in X.
                TessellatePatch(patch, tessellation, new Vector3(size, size, size));
                TessellatePatch(patch, tessellation, new Vector3(-size, size, size));

                if (patch.MirrorZ)
                {
                    // Some parts of the teapot (the body, lid, and rim, but not the
                    // handle or spout) are also symmetrical from front to back, so
                    // we tessellate them four times, mirroring in Z as well as X.
                    TessellatePatch(patch, tessellation, new Vector3(size, size, -size));
                    TessellatePatch(patch, tessellation, new Vector3(-size, size, -size));
                }
            }

            InitializePrimitive(graphicsDevice);
        }

        /// <summary>
        /// Tessellates the specified bezier patch.
        /// </summary>
        void TessellatePatch(TeapotPatch patch, int tessellation, Vector3 scale)
        {
            // Look up the 16 control points for this patch.
            Vector3[] controlPoints = new Vector3[16];

            for (int i = 0; i < 16; i++)
            {
                int index = patch.Indices[i];
                controlPoints[i] = TeapotControlPoints[index] * scale;
            }

            // Is this patch being mirrored?
            Boolean isMirrored = Math.Sign(scale.X) != Math.Sign(scale.Z);

            // Create the index and vertex data.
            CreatePatchIndices(tessellation, isMirrored);
            CreatePatchVertices(controlPoints, tessellation, isMirrored);
        }


        /// <summary>
        /// The teapot model consists of 10 bezier patches. Each patch has 16 control
        /// points, plus a flag indicating whether it should be mirrored in the Z axis
        /// as well as in X (all of the teapot is symmetrical from left to right, but
        /// only some parts are symmetrical from front to back). The control points
        /// are stored as integer indices into the TeapotControlPoints array.
        /// </summary>
        class TeapotPatch
        {
            public readonly int[] Indices;
            public readonly Boolean MirrorZ;


            public TeapotPatch(Boolean mirrorZ, int[] indices)
            {
                Debug.Assert(indices.Length == 16);

                this.Indices = indices;
                this.MirrorZ = mirrorZ;
            }
        }


        /// <summary>
        /// Static data array defines the bezier patches that make up the teapot.
        /// </summary>
        static TeapotPatch[] TeapotPatches =
        {
            // Rim.
            new TeapotPatch(true, new int[]
            {
                102, 103, 104, 105, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15
            }),

            // Body.
            new TeapotPatch (true, new int[]
            { 
                12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27
            }),

            new TeapotPatch(true, new int[]
            { 
                24, 25, 26, 27, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40
            }),

            // Lid.
            new TeapotPatch(true, new int[]
            { 
                96, 96, 96, 96, 97, 98, 99, 100, 101, 101, 101, 101, 0, 1, 2, 3
            }),
            
            new TeapotPatch(true, new int[]
            {
                0, 1, 2, 3, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117
            }),

            // Handle.
            new TeapotPatch(false, new int[]
            {
                41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56
            }),

            new TeapotPatch(false, new int[]
            { 
                53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 28, 65, 66, 67
            }),

            // Spout.
            new TeapotPatch(false, new int[]
            { 
                68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83
            }),

            new TeapotPatch(false, new int[]
            { 
                80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95
            }),

            // Bottom.
            new TeapotPatch(true, new int[]
            { 
                118, 118, 118, 118, 124, 122, 119, 121,
                123, 126, 125, 120, 40, 39, 38, 37
            }),
        };


        /// <summary>
        /// Static array defines the control point positions that make up the teapot.
        /// </summary>
        static Vector3[] TeapotControlPoints = 
        {
            new Vector3(0f, 0.345f, -0.05f),
            new Vector3(-0.028f, 0.345f, -0.05f),
            new Vector3(-0.05f, 0.345f, -0.028f),
            new Vector3(-0.05f, 0.345f, -0f),
            new Vector3(0f, 0.3028125f, -0.334375f),
            new Vector3(-0.18725f, 0.3028125f, -0.334375f),
            new Vector3(-0.334375f, 0.3028125f, -0.18725f),
            new Vector3(-0.334375f, 0.3028125f, -0f),
            new Vector3(0f, 0.3028125f, -0.359375f),
            new Vector3(-0.20125f, 0.3028125f, -0.359375f),
            new Vector3(-0.359375f, 0.3028125f, -0.20125f),
            new Vector3(-0.359375f, 0.3028125f, -0f),
            new Vector3(0f, 0.27f, -0.375f),
            new Vector3(-0.21f, 0.27f, -0.375f),
            new Vector3(-0.375f, 0.27f, -0.21f),
            new Vector3(-0.375f, 0.27f, -0f),
            new Vector3(0f, 0.13875f, -0.4375f),
            new Vector3(-0.245f, 0.13875f, -0.4375f),
            new Vector3(-0.4375f, 0.13875f, -0.245f),
            new Vector3(-0.4375f, 0.13875f, -0f),
            new Vector3(0f, 0.007499993f, -0.5f),
            new Vector3(-0.28f, 0.007499993f, -0.5f),
            new Vector3(-0.5f, 0.007499993f, -0.28f),
            new Vector3(-0.5f, 0.007499993f, -0f),
            new Vector3(0f, -0.105f, -0.5f),
            new Vector3(-0.28f, -0.105f, -0.5f),
            new Vector3(-0.5f, -0.105f, -0.28f),
            new Vector3(-0.5f, -0.105f, -0f),
            new Vector3(0f, -0.105f, 0.5f),
            new Vector3(0f, -0.2175f, -0.5f),
            new Vector3(-0.28f, -0.2175f, -0.5f),
            new Vector3(-0.5f, -0.2175f, -0.28f),
            new Vector3(-0.5f, -0.2175f, -0f),
            new Vector3(0f, -0.27375f, -0.375f),
            new Vector3(-0.21f, -0.27375f, -0.375f),
            new Vector3(-0.375f, -0.27375f, -0.21f),
            new Vector3(-0.375f, -0.27375f, -0f),
            new Vector3(0f, -0.2925f, -0.375f),
            new Vector3(-0.21f, -0.2925f, -0.375f),
            new Vector3(-0.375f, -0.2925f, -0.21f),
            new Vector3(-0.375f, -0.2925f, -0f),
            new Vector3(0f, 0.17625f, 0.4f),
            new Vector3(-0.075f, 0.17625f, 0.4f),
            new Vector3(-0.075f, 0.2325f, 0.375f),
            new Vector3(0f, 0.2325f, 0.375f),
            new Vector3(0f, 0.17625f, 0.575f),
            new Vector3(-0.075f, 0.17625f, 0.575f),
            new Vector3(-0.075f, 0.2325f, 0.625f),
            new Vector3(0f, 0.2325f, 0.625f),
            new Vector3(0f, 0.17625f, 0.675f),
            new Vector3(-0.075f, 0.17625f, 0.675f),
            new Vector3(-0.075f, 0.2325f, 0.75f),
            new Vector3(0f, 0.2325f, 0.75f),
            new Vector3(0f, 0.12f, 0.675f),
            new Vector3(-0.075f, 0.12f, 0.675f),
            new Vector3(-0.075f, 0.12f, 0.75f),
            new Vector3(0f, 0.12f, 0.75f),
            new Vector3(0f, 0.06375f, 0.675f),
            new Vector3(-0.075f, 0.06375f, 0.675f),
            new Vector3(-0.075f, 0.007499993f, 0.75f),
            new Vector3(0f, 0.007499993f, 0.75f),
            new Vector3(0f, -0.04875001f, 0.625f),
            new Vector3(-0.075f, -0.04875001f, 0.625f),
            new Vector3(-0.075f, -0.09562501f, 0.6625f),
            new Vector3(0f, -0.09562501f, 0.6625f),
            new Vector3(-0.075f, -0.105f, 0.5f),
            new Vector3(-0.075f, -0.18f, 0.475f),
            new Vector3(0f, -0.18f, 0.475f),
            new Vector3(0f, 0.02624997f, -0.425f),
            new Vector3(-0.165f, 0.02624997f, -0.425f),
            new Vector3(-0.165f, -0.18f, -0.425f),
            new Vector3(0f, -0.18f, -0.425f),
            new Vector3(0f, 0.02624997f, -0.65f),
            new Vector3(-0.165f, 0.02624997f, -0.65f),
            new Vector3(-0.165f, -0.12375f, -0.775f),
            new Vector3(0f, -0.12375f, -0.775f),
            new Vector3(0f, 0.195f, -0.575f),
            new Vector3(-0.0625f, 0.195f, -0.575f),
            new Vector3(-0.0625f, 0.17625f, -0.6f),
            new Vector3(0f, 0.17625f, -0.6f),
            new Vector3(0f, 0.27f, -0.675f),
            new Vector3(-0.0625f, 0.27f, -0.675f),
            new Vector3(-0.0625f, 0.27f, -0.825f),
            new Vector3(0f, 0.27f, -0.825f),
            new Vector3(0f, 0.28875f, -0.7f),
            new Vector3(-0.0625f, 0.28875f, -0.7f),
            new Vector3(-0.0625f, 0.2934375f, -0.88125f),
            new Vector3(0f, 0.2934375f, -0.88125f),
            new Vector3(0f, 0.28875f, -0.725f),
            new Vector3(-0.0375f, 0.28875f, -0.725f),
            new Vector3(-0.0375f, 0.298125f, -0.8625f),
            new Vector3(0f, 0.298125f, -0.8625f),
            new Vector3(0f, 0.27f, -0.7f),
            new Vector3(-0.0375f, 0.27f, -0.7f),
            new Vector3(-0.0375f, 0.27f, -0.8f),
            new Vector3(0f, 0.27f, -0.8f),
            new Vector3(0f, 0.4575f, -0f),
            new Vector3(0f, 0.4575f, -0.2f),
            new Vector3(-0.1125f, 0.4575f, -0.2f),
            new Vector3(-0.2f, 0.4575f, -0.1125f),
            new Vector3(-0.2f, 0.4575f, -0f),
            new Vector3(0f, 0.3825f, -0f),
            new Vector3(0f, 0.27f, -0.35f),
            new Vector3(-0.196f, 0.27f, -0.35f),
            new Vector3(-0.35f, 0.27f, -0.196f),
            new Vector3(-0.35f, 0.27f, -0f),
            new Vector3(0f, 0.3075f, -0.1f),
            new Vector3(-0.056f, 0.3075f, -0.1f),
            new Vector3(-0.1f, 0.3075f, -0.056f),
            new Vector3(-0.1f, 0.3075f, -0f),
            new Vector3(0f, 0.3075f, -0.325f),
            new Vector3(-0.182f, 0.3075f, -0.325f),
            new Vector3(-0.325f, 0.3075f, -0.182f),
            new Vector3(-0.325f, 0.3075f, -0f),
            new Vector3(0f, 0.27f, -0.325f),
            new Vector3(-0.182f, 0.27f, -0.325f),
            new Vector3(-0.325f, 0.27f, -0.182f),
            new Vector3(-0.325f, 0.27f, -0f),
            new Vector3(0f, -0.33f, -0f),
            new Vector3(-0.1995f, -0.33f, -0.35625f),
            new Vector3(0f, -0.31125f, -0.375f),
            new Vector3(0f, -0.33f, -0.35625f),
            new Vector3(-0.35625f, -0.33f, -0.1995f),
            new Vector3(-0.375f, -0.31125f, -0f),
            new Vector3(-0.35625f, -0.33f, -0f),
            new Vector3(-0.21f, -0.31125f, -0.375f),
            new Vector3(-0.375f, -0.31125f, -0.21f),
        };
    }

    public class TorusPrimitive
        : GeometricPrimitive
    {
        const float diameter = 1f;
        const float thickness = 0.333f;
        const int tessellation = 32;

        /// <summary>
        /// Constructs a new cube primitive, with the specified size.
        /// </summary>
        public TorusPrimitive(IGraphicsManager graphicsDevice)
        {
            Debug.Assert(tessellation >= 3);

            // First we loop around the main ring of the torus.
            for (int i = 0; i < tessellation; i++)
            {
                float tau; RealMaths.Tau(out tau);
                float outerAngle = i * tau / tessellation;

                // Create a transform matrix that will align geometry to
                // slice perpendicularly though the current ring position.
                Matrix44 translation = Matrix44.CreateTranslation(diameter / 2, 0, 0);

                Matrix44 rotationY = Matrix44.CreateRotationY(outerAngle);

                Matrix44 transform = translation * rotationY;

                // Now we loop along the other axis, around the side of the tube.
                for (int j = 0; j < tessellation; j++)
                {
                    float innerAngle = j * tau / tessellation;

                    float dx = (float)Math.Cos(innerAngle);
                    float dy = (float)Math.Sin(innerAngle);

                    // Create a vertex.
                    Vector3 normalIn = new Vector3(dx, dy, 0);
                    Vector3 positionIn = normalIn * thickness / 2;

                    Vector3 position;
                    Vector3.Transform(ref positionIn, ref transform, out position);

                    Vector3 normal;
                    Vector3.TransformNormal(ref normalIn, ref transform, out normal);

                    AddVertex(position, normal);

                    // And create indices for two triangles.
                    int nextI = (i + 1) % tessellation;
                    int nextJ = (j + 1) % tessellation;

                    AddIndex(i * tessellation + j);
                    AddIndex(i * tessellation + nextJ);
                    AddIndex(nextI * tessellation + j);

                    AddIndex(i * tessellation + nextJ);
                    AddIndex(nextI * tessellation + nextJ);
                    AddIndex(nextI * tessellation + j);
                }
            }

            InitializePrimitive(graphicsDevice);
        }
    }


    #endregion

    #region Traits

    public sealed class Camera
        : Trait
    {
        public CameraProjectionType Projection = CameraProjectionType.Perspective;

        // perspective settings
        public Single FieldOfView = RealMaths.ToRadians(45.0f);

        // orthographic settings
        public Single size = 100f;

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

            Single width = (Single) this.Cor.System.CurrentDisplaySize.X;
            Single height = (Single) this.Cor.System.CurrentDisplaySize.Y;

            if (Projection == CameraProjectionType.Orthographic)
            {
                if(TempWORKOUTANICERWAY)
                {
                    _projection =
                        Matrix44.CreateOrthographic(
                            width / SpriteConfiguration.Default.SpriteSpaceScale, 
                            height / SpriteConfiguration.Default.SpriteSpaceScale, 
                            1, -1);
                }
                else
                {
                    _projection = 
                        Matrix44.CreateOrthographicOffCenter(
                            -0.5f, 0.5f, -0.5f, 0.5f, 0.5f * size, -0.5f * size);
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
    }    public class FreeTransform
        : Trait
    {
        public FreeTransform()
        {
        }
    }

/*


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace SunGiant.Framework.Ophelia.Cameras
{

    public class FreeCamInputs
    {
        public Vector3 mTranslation;
        public Vector3 mRotation;
        public float mTranslationSpeed; //in range 0-1
        public float mRotationSpeedScale;
        public bool mFixUp;
    }

    public class FreeCamBehavior
        : Behavior
    {

        Vector3 oldPosition = Vector3.Zero;
        KeyboardState currentKeyboardState = new KeyboardState();
        GamePadState currentGamePadState = new GamePadState();

        // inputs that come from the controller
        FreeCamInputs mInputs;

        // fre cam settings
        float mTranslationSpeedStandard = 10.0f;
        float mTranslationSpeedMaximum = 100.0f;
        float mRotationSpeed = 45.0f; //30 degrees per second

        public void WorkOutInputs()
        {
            currentKeyboardState = Keyboard.GetState();
            currentGamePadState = GamePad.GetState(PlayerIndex.One);

            FreeCamInputs input = new FreeCamInputs();

            input.mTranslation = new Vector3(
                currentGamePadState.ThumbSticks.Left.X,
                0.0f,
                -currentGamePadState.ThumbSticks.Left.Y
                );

            input.mRotation = new Vector3(
                -currentGamePadState.ThumbSticks.Right.Y,
                -currentGamePadState.ThumbSticks.Right.X,
                0.0f
                );

            input.mTranslationSpeed = currentGamePadState.Triggers.Right;

            input.mRotationSpeedScale = 1.0f;

           input.mFixUp = currentKeyboardState.IsKeyDown(Keys.U);
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

        float localPitch;
        float localYaw;
        float localRoll;

        public void Apply(float zDt, CameraState zState, CameraState zPreviousCameraState)
        {
            WorkOutInputs();

            float translationSpeed = mTranslationSpeedStandard
                + mInputs.mTranslationSpeed *
                (mTranslationSpeedMaximum - mTranslationSpeedStandard);

            Vector3 translation = mInputs.mTranslation * translationSpeed * zDt;

            Vector3 rotation =
                mInputs.mRotation *
                MathHelper.ToRadians(mRotationSpeed) *
                mInputs.mRotationSpeedScale * zDt;

            localPitch += rotation.X;
            localYaw += rotation.Y;
            localRoll += rotation.Z;

            Quaternion rotationFromInputs = Quaternion.CreateFromYawPitchRoll(localYaw, localPitch, localRoll);

            Quaternion currentOri = zState.Orientation;

            zState.Orientation = Quaternion.Multiply( currentOri, rotationFromInputs);

            float yTranslation = translation.Y;
            translation.Y = 0.0f;

            zState.Position += oldPosition + Vector3.Transform(translation, zState.Orientation) + new Vector3(0.0f, yTranslation, 0.0f);
            zState.focusDistance = 3.0f;

            //update the old position for next time
            oldPosition = zState.Position;
            mInputs = null;

        }
    }
}*/    //
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

        internal override void Render (IGraphicsManager zGfx, Matrix44 zView, Matrix44 zProjection)
        {
            if (!Active)
                return;

            zGfx.GpuUtils.BeginEvent(Rgba32.Red, "MeshRenderer.Render");

            using (new ProfilingTimer(t => FrameStats.SetCullModeTime += t))
            {
                zGfx.SetCullMode(this.CullMode);
            }
            
            using (new ProfilingTimer(t => FrameStats.ActivateGeomBufferTime += t))
            {
                // Set our vertex declaration, vertex buffer, and index buffer.
                zGfx.SetActiveGeometryBuffer(Mesh.GeomBuffer);
            }

            using (new ProfilingTimer(t => FrameStats.MaterialTime += t))
            {
                Material.UpdateGpuSettings (zGfx);

                // The lighing manager right now just grabs the shader and tries to set
                // all variables to do with lighting, without even knowing if the shader
                // supports lighting.
                Material.SetColour( "AmbientLightColour", LightingManager.ambientLightColour );
                Material.SetColour( "EmissiveColour", LightingManager.emissiveColour );
                Material.SetColour( "SpecularColour", LightingManager.specularColour );
                Material.SetFloat( "SpecularPower", LightingManager.specularPower );

                Material.SetFloat( "FogEnabled", LightingManager.fogEnabled ? 1f : 0f );
                Material.SetFloat( "FogStart", LightingManager.fogStart );
                Material.SetFloat( "FogEnd", LightingManager.fogEnd );
                Material.SetColour( "FogColour", LightingManager.fogColour );

                Material.SetVector3( "DirectionalLight0Direction", LightingManager.dirLight0Direction );
                Material.SetColour( "DirectionalLight0DiffuseColour", LightingManager.dirLight0DiffuseColour );
                Material.SetColour( "DirectionalLight0SpecularColour", LightingManager.dirLight0SpecularColour );

                Material.SetVector3( "DirectionalLight1Direction", LightingManager.dirLight1Direction );
                Material.SetColour( "DirectionalLight1DiffuseColour", LightingManager.dirLight1DiffuseColour );
                Material.SetColour( "DirectionalLight1SpecularColour", LightingManager.dirLight1SpecularColour );

                Material.SetVector3( "DirectionalLight2Direction", LightingManager.dirLight2Direction );
                Material.SetColour( "DirectionalLight2DiffuseColour", LightingManager.dirLight2DiffuseColour );
                Material.SetColour( "DirectionalLight2SpecularColour", LightingManager.dirLight2SpecularColour );

                Material.SetVector3( "EyePosition", zView.Translation );

                // Get the material's shader and apply all of the settings
                // it needs.
                Material.UpdateShaderVariables (
                    this.Parent.Transform.Location,
                    zView,
                    zProjection
                    );
            }

            var shader = Material.GetShader ();

            if( shader != null)
            {
                foreach (var effectPass in shader.Passes)
                {
                    using (new ProfilingTimer(t => FrameStats.ActivateShaderTime += t))
                    {
                        effectPass.Activate (Mesh.GeomBuffer.VertexBuffer.VertexDeclaration);
                    }
                    using (new ProfilingTimer(t => FrameStats.DrawTime += t))
                    {
                        FrameStats.DrawIndexedPrimitivesCount ++;
                        zGfx.DrawIndexedPrimitives (
                            PrimitiveType.TriangleList, 0, 0,
                            Mesh.VertexCount, 0, Mesh.TriangleCount);
                    }
                }
            }

            zGfx.GpuUtils.EndEvent();

        }
    }

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

    public class PointLight
        : Trait
    {
    }

    public class SlowRotate
        : Trait
    {
        public override void OnUpdate(AppTime time)
        {
            Single x = time.Delta * RealMaths.ToRadians(10f);

            Quaternion rot = Quaternion.CreateFromYawPitchRoll(x, 0, 0);

            this.Parent.Transform.LocalRotation *= rot;
        }
    }

    public class SpotLight
        : Trait
    {
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

    public struct SpriteConfiguration
    {
        static SpriteConfiguration sprConf;

        static SpriteConfiguration()
        {
            Single piOver2;
            RealMaths.Pi(out piOver2);
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
        public static IShader SpriteShader
        {
            get { return spriteShader; }
            set { spriteShader = value; }
        }

        static IShader spriteShader;

        // defines how to move from world space to sprite space.
        Matrix44 spriteSpaceMatrix;
        public Matrix44 SpriteSpaceMatrix { get { return spriteSpaceMatrix; } }

        // defines how to move from sprite space to world space.
        Matrix44 inverseSpriteSpaceMatrix;
        public Matrix44 InverseSpriteSpaceMatrix { get { return inverseSpriteSpaceMatrix; } }

        SpriteConfiguration conf;

        public SpriteConfiguration SpriteConfiguration
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
        ITexture   currentTexture;

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
        ITexture   desiredTexture;


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
        public ITexture Texture { get { return desiredTexture; } set { desiredTexture = value; } }

        //--------------------------------------------------------------------//
        public Sprite()
        {
            SpriteConfiguration = SpriteConfiguration.Default;
            desiredColour = Rgba32.White;
            desiredScale = 1f;
        }

        void CalculateTransforms()
        {
            Matrix44 scale =
                Matrix44.CreateScale(SpriteConfiguration.SpriteSpaceScale);

            Quaternion q = SpriteConfiguration.SpriteSpaceOrientation;
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


    #endregion

    #region Types

    public abstract class Mesh
    {
        /// <summary>
        /// todo
        /// </summary>
        public Int32 TriangleCount;

        /// <summary>
        /// todo
        /// </summary>
        public Int32 VertexCount;

        /// <summary>
        /// todo
        /// </summary>
        public abstract VertexDeclaration VertDecl { get; }

        /// <summary>
        /// todo
        /// </summary>
        public IGeometryBuffer GeomBuffer;
    }
    // high level wrapper for blending stuff
    public struct BlendMode
        : IEquatable<BlendMode>
    {
        BlendFunction rgbBlendFunction;
        BlendFactor sourceRgb;
        BlendFactor destinationRgb;

        BlendFunction alphaBlendFunction;
        BlendFactor sourceAlpha;
        BlendFactor destinationAlpha;

        public override String ToString ()
        {
            return string.Format (
                "{{rgbBlendFunction:{0} sourceRgb:{1} destinationRgb:{2} alphaBlendFunction:{3} sourceAlpha:{4} destinationAlpha:{5}}}"
                , new Object[]
                    { 
                        rgbBlendFunction.ToString (), sourceRgb.ToString (), destinationRgb.ToString (),
                        alphaBlendFunction.ToString (), sourceAlpha.ToString (), destinationAlpha.ToString () 
                    }
            );
        }
        
        public Boolean Equals (BlendMode other)
        {
            return this == other;
        }
        
        public override Boolean Equals (Object obj)
        {
            Boolean flag = false;
            if (obj is BlendMode) {
                flag = this.Equals ((BlendMode)obj);
            }
            return flag;
        }
        
        public override Int32 GetHashCode ()
        {
            int a = (int) rgbBlendFunction.GetHashCode();
            int b = (int) sourceRgb.GetHashCode();
            int c = (int) destinationRgb.GetHashCode();
            
            int d = (int) alphaBlendFunction.GetHashCode();
            int e = (int) sourceAlpha.GetHashCode();
            int f = (int) destinationAlpha.GetHashCode();


            return a + b + c + d + e + f;
        }

        public static Boolean operator != (BlendMode value1, BlendMode value2)
        {
            return !(value1 == value2); 
        }

        public static Boolean operator == (BlendMode value1, BlendMode value2)
        {
            if (value1.rgbBlendFunction != value2.rgbBlendFunction) return false;
            if (value1.sourceRgb != value2.sourceRgb) return false;
            if (value1.destinationRgb != value2.destinationRgb) return false;
            if (value1.alphaBlendFunction != value2.alphaBlendFunction) return false;
            if (value1.sourceAlpha != value2.sourceAlpha) return false;
            if (value1.destinationAlpha != value2.destinationAlpha) return false;

            return true;
        }

        public static BlendMode Default
        {
            get
            {
                var blendMode = new BlendMode();
                
                blendMode.rgbBlendFunction =    BlendFunction.Add;
                blendMode.sourceRgb =           BlendFactor.SourceAlpha;
                blendMode.destinationRgb =      BlendFactor.InverseSourceAlpha;
                
                blendMode.alphaBlendFunction =  BlendFunction.Add;
                blendMode.sourceAlpha =         BlendFactor.One;
                blendMode.destinationAlpha =    BlendFactor.InverseSourceAlpha;
                
                return blendMode;
            }
        }

        public static BlendMode Opaque
        {
            get
            {
                var blendMode = new BlendMode();
                
                blendMode.rgbBlendFunction =    BlendFunction.Add;
                blendMode.sourceRgb =           BlendFactor.One;
                blendMode.destinationRgb =      BlendFactor.Zero;
                
                blendMode.alphaBlendFunction =  BlendFunction.Add;
                blendMode.sourceAlpha =         BlendFactor.One;
                blendMode.destinationAlpha =    BlendFactor.Zero;
                
                return blendMode;
            }
        }
        
        public static BlendMode Subtract
        {
            get
            {
                var blendMode = new BlendMode();

                blendMode.rgbBlendFunction =    BlendFunction.ReverseSubtract;
                blendMode.sourceRgb =           BlendFactor.SourceAlpha;
                blendMode.destinationRgb =      BlendFactor.One;

                blendMode.alphaBlendFunction =  BlendFunction.ReverseSubtract;
                blendMode.sourceAlpha =         BlendFactor.SourceAlpha;
                blendMode.destinationAlpha =    BlendFactor.One;

                return blendMode;
            }
        }

        public static BlendMode Additive
        {
            get
            {
                var blendMode = new BlendMode();
                
                blendMode.rgbBlendFunction =    BlendFunction.Add;
                blendMode.sourceRgb =           BlendFactor.SourceAlpha;
                blendMode.destinationRgb =      BlendFactor.One;
                
                blendMode.alphaBlendFunction =  BlendFunction.Add;
                blendMode.sourceAlpha =         BlendFactor.SourceAlpha;
                blendMode.destinationAlpha =    BlendFactor.One;
                
                return blendMode;
            }
        }

        static BlendMode lastSet = BlendMode.Default;
        static Boolean neverSet = true;

        public static void Apply(BlendMode blendMode, IGraphicsManager graphics)
        {
            if (neverSet || lastSet != blendMode)
            {
                graphics.SetBlendEquation (
                    blendMode.rgbBlendFunction, blendMode.sourceRgb, blendMode.destinationRgb,
                    blendMode.alphaBlendFunction, blendMode.sourceAlpha, blendMode.destinationAlpha
                    );

                lastSet = blendMode;
            }
        }
    }

    public class Material
    {
        IShader shader;
        string renderPass;

        public BlendMode BlendMode { get; set; }
        public string RenderPass { get { return renderPass; } }

        public Material(string renderPass, IShader shader)
        {
            this.BlendMode = BlendMode.Default;

            this.renderPass = renderPass;
            this.shader = shader;
        }

        internal void UpdateShaderVariables(Matrix44 world, Matrix44 view, Matrix44 proj)
        {
            if(shader == null)
                return;

            // Right now we need to make sure that the shader variables are all set with this
            // settings this material has defined.

            // We don't know if the shader being used is exclusive to this material, or if it
            // is shared between many.

            // Therefore to be 100% sure we could reset every variable on the shader to the defaults,
            // then set the ones that this material knows about, thus avoiding running with shader settings
            // that this material doesn't know about that are being changed by something else that
            // shares the shader.  This would be bad, as it will likely involve setting the same variable multiple times

            // So instead, as an optimisation, iterate over all settings that this material knows about,
            // and ask the shader to change them, this compare those changes against a full list of
            // all of the shader's variables, if any were missed by the material, then set them to
            // their default values.

            // Right now, just use the easy option and optimise later ;-D

            shader.ResetVariables();

            shader.SetVariable ("World", world);
            shader.SetVariable ("View", view);
            shader.SetVariable ("Projection", proj);

            foreach(var propertyName in colourSettings.Keys)
            {
                shader.SetVariable (propertyName, colourSettings[propertyName]);
            }

            foreach(var propertyName in floatSettings.Keys)
            {
                shader.SetVariable (propertyName, floatSettings[propertyName]);
            }

            foreach(var propertyName in matrixSettings.Keys)
            {
                shader.SetVariable (propertyName, matrixSettings[propertyName]);
            }

            foreach(var propertyName in vector3Settings.Keys)
            {
                shader.SetVariable (propertyName, vector3Settings[propertyName]);
            }

            foreach(var propertyName in vector4Settings.Keys)
            {
                shader.SetVariable (propertyName, vector4Settings[propertyName]);
            }

            foreach(var propertyName in scaleSettings.Keys)
            {
                shader.SetVariable (propertyName, scaleSettings[propertyName]);
            }

            foreach(var propertyName in textureOffsetSettings.Keys)
            {
                shader.SetVariable (propertyName, textureOffsetSettings[propertyName]);
            }

            shader.ResetSamplerTargets();

            int i = 0;
            foreach(var key in textureSamplerSettings.Keys)
            {
                shader.SetSamplerTarget (key, i);
                i++;
            }
        }

        internal IShader GetShader()
        {
            return shader;
        }

        public Vector2 Tiling { get; set; }
        public Vector2 Offset { get; set; }

        internal void UpdateGpuSettings(IGraphicsManager graphics)
        {
            // Update the render states on the gpu
            BlendMode.Apply (this.BlendMode, graphics);

            graphics.SetActiveTexture (0, null);

            // Set the active textures on the gpu
            int i = 0;
            foreach(var key in textureSamplerSettings.Keys)
            {
                graphics.SetActiveTexture (i, textureSamplerSettings[key]);
                i++;
            }
        }

        Dictionary<string, Rgba32> colourSettings = new Dictionary<string, Rgba32>();
        Dictionary<string, Single> floatSettings = new Dictionary<string, Single>();
        Dictionary<string, Matrix44> matrixSettings = new Dictionary<string, Matrix44>();
        Dictionary<string, Vector3> vector3Settings = new Dictionary<string, Vector3>();
        Dictionary<string, Vector4> vector4Settings = new Dictionary<string, Vector4>();
        Dictionary<string, Vector2> scaleSettings = new Dictionary<string, Vector2>();
        Dictionary<string, Vector2> textureOffsetSettings = new Dictionary<string, Vector2>();
        Dictionary<string, ITexture> textureSamplerSettings = new Dictionary<string, ITexture>();

        public void SetColour(string propertyName, Rgba32 colour)
        {
            colourSettings[propertyName] = colour;
        }

        public void SetFloat(string propertyName, Single value)
        {
            floatSettings[propertyName] = value;
        }

        public void SetMatrix(string propertyName, Matrix44 matrix)
        {
            matrixSettings[propertyName] = matrix;
        }

        public void SetVector4(string propertyName, Vector4 vector)
        {
            vector4Settings[propertyName] = vector;
        }

        public void SetVector3(string propertyName, Vector3 vector)
        {
            vector3Settings[propertyName] = vector;
        }

        public void SetTextureOffset(string propertyName, Vector2 offset)
        {
            textureOffsetSettings[propertyName] = offset;
        }

        public void SetTextureScale(string propertyName, Vector2 scale)
        {
            scaleSettings[propertyName] = scale;
        }


        public void SetTexture(string propertyName, ITexture texture)
        {
            textureSamplerSettings[propertyName] = texture;
        }
    }

    // THe user creates game states and uses them to interact with the engine
    public abstract class Scene
    {
        public virtual SceneSettings Settings
        { 
            get
            {
                return SceneSettings.Default;
            }
        }

        internal void RegisterDrawCall() { drawCalls++; }
        int drawCalls = 0;

        internal void RegisterTriangles(int count) { triCount += count; }
        int triCount = 0;


        BlimeyContext blimey;

        public ICor Cor { get; set;}

        public IBlimey Blimey { get { return blimey; } }

        public Boolean Active { get { return _active; } }

        Boolean _active;
        List<SceneObject> _gameObjects = new List<SceneObject> ();

        public List<SceneObject> SceneObjects { get { return _gameObjects; } }

        CameraManager cameraManager;

        public SceneObject CreateSceneObject (string zName)
        {
            var go = new SceneObject (this, zName);
            _gameObjects.Add (go);
            return go;
        }

        public void DestroySceneObject (SceneObject zGo)
        {
            zGo.Shutdown ();
            foreach (SceneObject go in zGo.Children) {
                this.DestroySceneObject (go);
                _gameObjects.Remove (go);
            }
            _gameObjects.Remove (zGo);

            zGo = null;
        }

        public abstract void Start ();

        public abstract Scene Update(AppTime time);

        public abstract void Shutdown ();

        public void Initialize(ICor cor)
        {
            this.Cor = cor;
            this.blimey = new BlimeyContext(this.Cor, this.Settings); ;

            cameraManager = new CameraManager(this);
            _active = true;

            this.Start();
        }

        internal Scene RunUpdate(AppTime time)
        {
            drawCalls = 0;
            triCount = 0;

            this.blimey.PreUpdate(time);
            

            foreach (SceneObject go in _gameObjects) {
                go.Update(time);
            }

            

            var ret =  this.Update(time);

            this.blimey.PostUpdate(time);


            return ret;
        }


        
        internal CameraManager CameraManager { get { return cameraManager; } }

        public void SetRenderPassCameraToDefault(string renderPass)
        {
            cameraManager.SetDefaultCamera (renderPass);
        }
        
        public void SetRenderPassCameraTo (string renderPass, SceneObject go)
        {
            cameraManager.SetMainCamera(renderPass, go);
        }

        public SceneObject GetRenderPassCamera(string renderPass)
        {
            return cameraManager.GetActiveCamera(renderPass).Parent;
        }
        public virtual void Uninitilise ()
        {
            this.Shutdown ();

            _active = false;

            List<SceneObject> onesToDestroy = new List<SceneObject> ();

            foreach (SceneObject go in _gameObjects) {
                if (go.Transform.Parent == null)
                    onesToDestroy.Add (go);
            }

            foreach (SceneObject go in onesToDestroy) {
                DestroySceneObject (go);
            }

            System.Diagnostics.Debug.Assert(_gameObjects.Count == 0);

            this.blimey = null;
            this.cameraManager = null;
        }
    }

    // Game Object
    // -----------
    //
    public sealed class SceneObject
    {
        List<Trait> behaviours = new List<Trait> ();
        Transform location = new Transform ();
        String name = "SceneObject";
        readonly Scene containedBy;
        Boolean enabled = false;
        
        public Scene Owner { get { return containedBy; } }
        
        // Used to define where in the game engine's hierarchy this
        // game object exists.
        public Transform Transform { get { return location; } }
        
        // The name of the this game object, defaults to "SceneObject"
        // can be set upon creation or changed at anytime.  Only real
        // use is to doing lazy searches of the hierachy by name
        // and also making the hierachy look neat.
        public string Name { get { return name; } set { name = value; } }
        
        // Defines whether or not the SceneObject's behaviours should be updated
        // and rendered.
        public Boolean Enabled
        { 
            get 
            {
                return enabled; 
            }
            set 
            {
                if( enabled == value )
                    return;
                
                Boolean changeFlag = false;
            
                changeFlag = true;
                enabled = value;
                
                if( changeFlag )
                {
                    foreach (var behaviour in behaviours) 
                    {
                        if(enabled)
                            behaviour.OnEnable();
                        else
                            behaviour.OnDisable();
                    }
                }   
            } 
        }
        
        
        internal List<SceneObject> Children {
            get {
                List<SceneObject> kids = new List<SceneObject> ();
                foreach (SceneObject go in containedBy.SceneObjects) {
                    if (go.Transform.Parent == this.Transform)
                        kids.Add (go);
                }
                return kids;
            }
        }

        
        public T AddTrait<T> ()
            where T : Trait, new()
        {
            
            if( this.GetTrait<T>() != null )
                throw new Exception("This Trait already exists on the gameobject");
            
            T behaviour = new T ();
            behaviours.Add (behaviour);
            behaviour.Initilise(containedBy.Cor, containedBy.Blimey, this);
            
            behaviour.OnAwake();
            
            if( this.Enabled )
                behaviour.OnEnable();
            else
                behaviour.OnDisable();
            
            return behaviour;

        }

        public void RemoveTrait<T> ()
            where T : Trait
        {
            Trait trait = behaviours.Find(x => x is T );
            trait.OnDestroy();
            behaviours.Remove(trait);
        }

        public T GetTrait<T> ()
            where T : Trait
        {
            foreach (Trait b in behaviours) {
                if (b as T != null)
                    return b as T;
            }

            return null;
        }

        public T GetTraitInChildren<T> ()
            where T : Trait
        {
            foreach (var go in Children) {
                foreach (var b in go.behaviours) {
                    if (b as T != null)
                        return b as T;
                }
            }

            return null;
        }

        internal SceneObject (Scene containedBy, string name)
        {
            this.Name = name;
            
            this.containedBy = containedBy;
            
            // directly set _enabled to false, don't want any callbacks yet
            this.enabled = true;
            
        }

        internal void Update(AppTime time)
        {
            if (!Enabled)
                return;
            
            foreach (Trait behaviour in behaviours) {
                if (behaviour.Active)
                {
                    behaviour.OnUpdate(time);
                }
            }
        }
        
        internal void Render(IGraphicsManager zGfx, Matrix44 zView, Matrix44 zProjection){
            if (!Enabled)
                return;
            
            foreach (Trait behaviour in behaviours) {
                if (behaviour.Active)
                    behaviour.Render(zGfx, zView, zProjection);
            }
        }

        internal void Shutdown ()
        {
            foreach (var behaviour in behaviours) 
            {
                behaviour.OnDestroy ();
            }
        }
    }

    public struct RenderPassSettings
    {
        public static readonly RenderPassSettings Default;

        static RenderPassSettings()
        {
            Default.ClearDepthBuffer = false;
            Default.FogEnabled = false;
            Default.FogColour = Rgba32.CornflowerBlue;
            Default.FogStart = 300.0f;
            Default.FogEnd = 550.0f;
            Default.EnableDefaultLighting = true;
            Default.CameraProjectionType = CameraProjectionType.Perspective;
        }

        public Boolean ClearDepthBuffer;
        public Boolean FogEnabled;
        public Rgba32 FogColour;
        public Single FogStart;
        public Single FogEnd;
        public Boolean EnableDefaultLighting;
        public CameraProjectionType CameraProjectionType;
    }
    
    //
    // Game Scene Settings
    // -------------------
    // Game scene settings are used by the engine to detemine how
    // to manage an associated scene.  For example the game scene 
    // settings are used to define the render pass for the scene.
    public class SceneSettings
    {
        Dictionary<String, RenderPassSettings> renderPassSettings;
        List<String> renderPassOrder;
        Boolean startByClearingBackBuffer;
        Rgba32 clearBackBufferColour;

        readonly static SceneSettings defaultSettings = new SceneSettings();

        internal static SceneSettings Default
        {
            get
            {
                return defaultSettings;
            }
        }

        static SceneSettings()
        {
            defaultSettings.InitDefault();
        }

        public SceneSettings()
        {
            renderPassSettings = new Dictionary<String, RenderPassSettings>();
            renderPassOrder = new List<String>();
            startByClearingBackBuffer = false;
            clearBackBufferColour = Rgba32.CornflowerBlue;
        }

        void InitDefault()
        {
            /// BIG TODO, ADD AWAY TO MAKE RENDER PASSES SHARE THE SAME CAMERA!!!!!
            /// SO DEBUG AND DEFAULT CAN USER THE SAME
            AddRenderPass("Debug", RenderPassSettings.Default);

            var defaultPassSettings = RenderPassSettings.Default;
            defaultPassSettings.EnableDefaultLighting = true;
            defaultPassSettings.FogEnabled = true;
            AddRenderPass("Default", defaultPassSettings);

            var guiPassSettings = RenderPassSettings.Default;
            guiPassSettings.ClearDepthBuffer = true;
            guiPassSettings.CameraProjectionType = CameraProjectionType.Orthographic;
            AddRenderPass("Gui", guiPassSettings);
        }

        public void AddRenderPass(String passID, RenderPassSettings settings)
        {
            if (renderPassOrder.Contains(passID))
            {
                throw new Exception("Can't have render passes with the same name");
            }

            renderPassOrder.Add(passID);
            renderPassSettings.Add(passID, settings);
        }

        public RenderPassSettings GetRenderPassSettings(String passName)
        {
            return renderPassSettings[passName];
        }

        public List<String> RenderPasses
        { 
            get 
            { 
                return renderPassOrder; 
            } 
        }

        // Debug Background Rgba
        public bool StartByClearingBackBuffer
        { 
            get 
            { 
                return startByClearingBackBuffer; 
            }
            set
            {
                startByClearingBackBuffer = value;
            }
        }

        public Rgba32 BackgroundColour 
        { 
            get 
            { 
                return clearBackBufferColour; 
            } 
            set 
            { 
                startByClearingBackBuffer = true; 
                clearBackBufferColour = value; 
            } 
        }   
    }

    //
    // BEHAVIOUR
    //
    // A game object can exhibit a number of behaviours.  The behaviours are defined as children of
    // this abstract class.
    //
    public abstract class Trait
    {
        protected ICor Cor { get; set; }
        protected IBlimey Blimey { get; set; }

        public SceneObject Parent { get; set; }

        public Boolean Active { get; set; }
    
        // INTERNAL METHODS
        // called after constructor and before awake
        internal void Initilise (ICor cor, IBlimey blimey, SceneObject parent)
        {
            Cor = cor;
            Blimey = blimey;
            Parent = parent;

            Active = true;
        }
        
        // INTERNAL CALLBACKS
        internal virtual void Render(IGraphicsManager zGfx, Matrix44 zView, Matrix44 zProjection) {}

        //TODO MAKE THESE ABSTRACT

        // PUBLIC CALLBACKS
        public virtual void OnAwake () {}
        
        public virtual void OnUpdate (AppTime time) {}
        
        // Called when the Enabled state of the parent gameobject changes
        public virtual void OnEnable() {}
        public virtual void OnDisable() {}
        
        public virtual void OnDestroy () {}
    }


    #endregion
}
