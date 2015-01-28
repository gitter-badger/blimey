using System;
using System.Runtime.InteropServices;

namespace AssImp
{
    public static class AssImp
    {
        [Flags]
        public enum PostProcessSteps
        {
            None = 0x0,
            CalculateTangentSpace = 0x1,
            JoinIdenticalVertices = 0x2,
            MakeLeftHanded = 0x4,
            Triangulate = 0x8,
            RemoveComponent = 0x10,
            GenerateNormals = 0x20,
            GenerateSmoothNormals = 0x40,
            SplitLargeMeshes = 0x80,
            PreTransformVertices = 0x100,
            LimitBoneWeights = 0x200,
            ValidateDataStructure = 0x400,
            ImproveCacheLocality = 0x800,
            RemoveRedundantMaterials = 0x1000,
            FixInFacingNormals = 0x2000,
            SortByPrimitiveType = 0x8000,
            FindDegenerates = 0x10000,
            FindInvalidData = 0x20000,
            GenerateUVCoords = 0x40000,
            TransformUVCoords = 0x80000,
            FindInstances = 0x100000,
            OptimizeMeshes = 0x200000,
            OptimizeGraph = 0x400000,
            FlipUVs = 0x800000,
            FlipWindingOrder = 0x1000000,
            SplitByBoneCount = 0x2000000,
            Debone = 0x4000000
        }
    
        public static class PostProcessPreset
        {
            /// <summary>
            /// PostProcess configuration for (some) Direct3D conventions,
            /// left handed geometry, upper left origin for UV coordinates,
            /// and clockwise face order, suitable for CCW culling.
            /// </summary>
            public static PostProcessSteps ConvertToLeftHanded
            {
                get
                {
                    return PostProcessSteps.MakeLeftHanded |
                        PostProcessSteps.FlipUVs |
                        PostProcessSteps.FlipWindingOrder;
                }
            }
    
            /// <summary>
            /// PostProcess configuration for optimizing data for real-time.
            /// Does the following steps:
            /// </summary>
            public static PostProcessSteps TargetRealTimeFast
            {
                get
                {
                    return PostProcessSteps.CalculateTangentSpace |
                        PostProcessSteps.GenerateNormals |
                        PostProcessSteps.JoinIdenticalVertices |
                        PostProcessSteps.Triangulate |
                        PostProcessSteps.GenerateUVCoords |
                        PostProcessSteps.SortByPrimitiveType;
                }
            }
    
            /// <summary>
            /// PostProcess configuration for optimizing
            /// data for real-time rendering. Does the following steps:
            /// </summary>
            public static PostProcessSteps TargetRealTimeQuality
            {
                get
                {
                    return PostProcessSteps.CalculateTangentSpace |
                        PostProcessSteps.GenerateSmoothNormals |
                        PostProcessSteps.JoinIdenticalVertices |
                        PostProcessSteps.LimitBoneWeights |
                        PostProcessSteps.RemoveRedundantMaterials |
                        PostProcessSteps.SplitLargeMeshes |
                        PostProcessSteps.Triangulate |
                        PostProcessSteps.GenerateUVCoords |
                        PostProcessSteps.SortByPrimitiveType |
                        PostProcessSteps.FindDegenerates |
                        PostProcessSteps.FindInvalidData;
                }
            }
    
            /// <summary>
            /// PostProcess configuration for heavily optimizing the data
            /// </summary>
            public static PostProcessSteps TargetRealTimeMaximumQuality
            {
                get
                {
                    return TargetRealTimeQuality |
                        PostProcessSteps.FindInstances |
                        PostProcessSteps.ValidateDataStructure |
                        PostProcessSteps.OptimizeMeshes;
                }
            }
        }

        #if DEBUG
        const String libassimp = "libassimpd";
        #else
        const String libassimp = "libassimp";
        #endif

        [DllImport (libassimp)]
        static extern int aiGetVersionMajor ();

        [DllImport (libassimp)]
        static extern int aiGetVersionMinor ();

        [DllImport (libassimp)]
        static extern int aiImportFile (String path);

        [DllImport (libassimp)]
        static extern IntPtr aiImportFile([In, MarshalAs(UnmanagedType.LPStr)] String file, UInt32 flags);
        
        public static void PrintVersion ()
        {
            try
            {
                Console.WriteLine (
                    "AssImp Version: " + aiGetVersionMajor () + "." + aiGetVersionMinor ());

                IntPtr seagull = aiImportFile ("seagull.fbx", (UInt32) PostProcessPreset.TargetRealTimeMaximumQuality);
                IntPtr duck = aiImportFile ("duck.dae", (UInt32) PostProcessPreset.TargetRealTimeMaximumQuality);

                Console.ReadLine ();
            }
            catch (Exception ex)
            {
                Console.WriteLine (ex.GetType () + " : " + ex.Message);
            }   
        }
    }
}

