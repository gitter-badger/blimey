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

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    internal static class FrameStats
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


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

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


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

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


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

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


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

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


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

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


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

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
                Console.WriteLine("Beep");
            }
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

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

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

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


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

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

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    internal class SpriteMesh
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
}
