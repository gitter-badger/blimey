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

namespace Blimey
{
    using System;
    using System.Runtime.InteropServices;
    using System.Globalization;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.IO;

    using Abacus.SinglePrecision;
    using Fudge;
    
    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Provides a means to interact with a shader loaded on the GPU.
    /// This class is a little comlicated because it provides an abstraction that hides shader variants.
    /// </summary>
    public sealed class Shader
        : IDisposable
        , ICorResource
        , IEquatable <Shader>
    {
        readonly IApi platform;
        readonly Handle shaderHandle;

        public Handle Handle { get { return shaderHandle; } }

        // For each vert decl seen, defines the index of the most suitable shader variant.
        readonly Dictionary<VertexDeclaration, Int32> bestVariantMap = new Dictionary<VertexDeclaration, Int32>();
        readonly Dictionary<VertexDeclaration, Int32[]> bestVariantMapVertexIndicies = new Dictionary<VertexDeclaration, Int32[]>();

        // Current state (acts as a buffer for adjusting shader settings, only when needed will the changes
        // be applied to the GPU).
        readonly Dictionary<String, Object> currentVariables = new Dictionary<String, Object>();
        readonly Dictionary<String, Int32> currentSamplerTargets = new Dictionary<String, Int32>();

        // Debug
        readonly Dictionary<String, Boolean> logHistory = new Dictionary<String, Boolean>();

        // IDisposable
        Boolean disposed;

        static Int32 shaderCount = 0;
        static System.Collections.Concurrent.ConcurrentQueue <Handle> shadersToClean =
            new System.Collections.Concurrent.ConcurrentQueue<Handle> ();

        public override int GetHashCode ()
        {
            return Handle.GetHashCode ();
        }

        internal static void CollectGpuGarbage (IApi platform)
        {
            Handle handle = null;
            while (shadersToClean.TryDequeue (out handle))
            {
                platform.gfx_DestroyShader (handle);
                --shaderCount;
                InternalUtils.Log.Info ("GFX", "Shader destroyed: " + handle.Identifier);
            }
        }

        // Returned, in order, only the shader input declaration supported by the given variant.
        ShaderInputDeclaration[] GetSupportedInputs (VertexDeclaration vertexDeclaration)
        {
            if (!bestVariantMap.ContainsKey (vertexDeclaration))
            {
                WorkOutBestVariantFor (vertexDeclaration);
            }

            Int32 variantIndex = bestVariantMap[vertexDeclaration];

            Int32 length = variantInputInfos [variantIndex].Length;
            var result = new ShaderInputDeclaration [length];
            for(Int32 i = 0; i < length; ++i)
            {
                var inputInfo = variantInputInfos [variantIndex][i];
                result [i] = inputActualNameToDeclaration[inputInfo.Name];
            }
            return result;
        }

        public Int32[] GetElementsIndicesToEnable (VertexDeclaration vertexDeclaration)
        {
            if (!bestVariantMap.ContainsKey (vertexDeclaration))
            {
                WorkOutBestVariantFor (vertexDeclaration);
            }

            return bestVariantMapVertexIndicies[vertexDeclaration];
        }

        // Variant Tracking
        readonly Int32 variantCount;
        readonly Dictionary <Int32, String> variantIdentifiers = new Dictionary <Int32, String> ();
        readonly Dictionary <Int32, ShaderInputInfo[]> variantInputInfos = new Dictionary<Int32, ShaderInputInfo[]> ();
        readonly Dictionary <Int32, ShaderVariableInfo[]> variantVariableInfos = new Dictionary<Int32, ShaderVariableInfo[]> ();
        readonly Dictionary <Int32, ShaderSamplerInfo[]> variantSamplerInfos = new Dictionary<Int32, ShaderSamplerInfo[]> ();

        // Definition tracking.
        readonly Dictionary <String, ShaderInputDeclaration> inputActualNameToDeclaration;
        readonly Dictionary <String, ShaderVariableDeclaration> variableActualNameToDeclaration;
        readonly Dictionary <String, String> variableNiceNameToActualName;
        readonly Dictionary <String, String> samplerNiceNameToActualName;

        Boolean iAmActive = false;

        void OnActiveShaderChanged (Handle previousActive, Handle newActiveShader)
        {
            if (previousActive == Handle)
            {
                iAmActive = false;
                // I've just lost focus.
            }

            if (newActiveShader == Handle)
            {
                iAmActive = true;
                // I've just gained focus.
            }
        }

        public Shader (IApi platform, ShaderDeclaration shaderDeclaration, ShaderFormat shaderFormat, Byte[] source)
        {
            this.platform = platform;

            // Get the platform implementation to build create the shader on the GPU.
            this.shaderHandle = platform.gfx_CreateShader (shaderDeclaration, shaderFormat, source);
            ++shaderCount;
            InternalUtils.Log.Info ("GFX", "Shader created: " + shaderHandle.Identifier);

            // Cache off constants from the API so we don't need to hit the API each time we need the same info.
            this.variantCount = platform.gfx_shdr_GetVariantCount (shaderHandle);

            for (Int32 i = 0; i < variantCount; ++i)
            {
                this.variantIdentifiers [i] = platform.gfx_shdr_GetIdentifier (shaderHandle, i);
                this.variantInputInfos [i] = platform.gfx_shdr_GetInputs (shaderHandle, i);
                this.variantVariableInfos [i] = platform.gfx_shdr_GetVariables (shaderHandle, i);
                this.variantSamplerInfos [i] = platform.gfx_shdr_GetSamplers (shaderHandle, i);
            }

            // Useful look-up tables relating to the shader declaration.
            this.inputActualNameToDeclaration = shaderDeclaration.InputDeclarations
                .ToDictionary (x => x.Name, x => x);

            this.variableActualNameToDeclaration = shaderDeclaration.VariableDeclarations
                .ToDictionary (x => x.Name, x => x);

            this.variableNiceNameToActualName = shaderDeclaration.VariableDeclarations
                .ToDictionary (x => x.NiceName, x => x.Name);

            this.samplerNiceNameToActualName = shaderDeclaration.SamplerDeclarations
                .ToDictionary (x => x.NiceName, x => x.Name);

            // Checks that all variants of the shader match up with
            // the provided shader declaration.
            for (Int32 i = 0; i < variantCount; ++i)
            {
                Console.WriteLine ("Validating " + variantIdentifiers [i]);

                ValidateShaderInputs (shaderDeclaration.InputDeclarations, variantInputInfos [i]);
                ValidateShaderVariables (shaderDeclaration.VariableDeclarations, variantVariableInfos [i]);
                ValidateShaderSamplers (shaderDeclaration.SamplerDeclarations, variantSamplerInfos [i]);
            }
        }

        public override Boolean Equals (Object obj)
        {
            Boolean flag = false;
            if (obj is Shader) flag = this.Equals ((Shader) obj);
            return flag;
        }

        public Boolean Equals (Shader other)
        {
            if (this.Handle != other.Handle)
                return false;

            return true;
        }

        public static Boolean operator == (Shader a, Shader b) { return Equals (a, b); }
        public static Boolean operator != (Shader a, Shader b) { return !Equals (a, b); }

        void ValidateShaderInputs (List<ShaderInputDeclaration> inputDeclarations, ShaderInputInfo[] inputInfos)
        {
            // Make sure that this shader implements all of the non-optional defined inputs.
            var nonOptionalDefinitions = inputDeclarations.Where (y => !y.Optional).ToList ();

            foreach (var definition in nonOptionalDefinitions)
            {
                var find = inputInfos.ToList().Find (x => x.Name == definition.Name/* && x.Type == definition.Type */);

                if (find == null)
                {
                    throw new Exception ("problem");
                }
            }

            // Make sure that every implemented input is defined.
            foreach (var input in inputInfos)
            {
                var find = inputDeclarations.Find (x => x.Name == input.Name
                    /*&& (x.Type == input.Type || (x.Type == typeof (Rgba32) && input.Type == typeof (Vector4)))*/
                );

                if (find == null)
                {
                    throw new Exception ("problem");
                }
            }
        }

        void ValidateShaderVariables (List<ShaderVariableDeclaration> variableDeclarations, ShaderVariableInfo[] variableInfos)
        {
            // Make sure that every variable is defined.
            foreach (var variable in variableInfos)
            {
                var find = variableDeclarations.Find (
                    x =>
                    x.Name == variable.Name //&&
                    //(x.Type == variable.Type || (x.Type == typeof (Rgba32) && variable.Type == typeof (Vector4)))
                );

                if (find == null)
                {
                    throw new Exception ("problem");
                }
            }
        }

        void ValidateShaderSamplers (List<ShaderSamplerDeclaration> samplerDeclarations, ShaderSamplerInfo[] samplerInfos)
        {
            var nonOptionalSamplers =
                samplerDeclarations
                    .Where (y => !y.Optional)
                    .ToList ();

            foreach (var definition in nonOptionalSamplers)
            {
                var find = samplerInfos.ToList().Find (x => x.Name == definition.Name);

                if (find == null)
                {
                    throw new Exception ("problem");
                }
            }

            // Make sure that every implemented input is defined.
            foreach (var sampler in samplerInfos)
            {
                var find = samplerDeclarations.Find (x => x.Name == sampler.Name);

                if (find == null)
                {
                    throw new Exception ("problem");
                }
            }
        }



        // This finalizer will run only if the Dispose method
        // does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide destructors in types derived from this class.
        ~Shader ()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose (false);
        }

        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose ()
        {
            Dispose (true);

            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize (this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        /*protected virtual*/ void Dispose (bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.
                InternalUtils.Log.Info ("GFX", "Enqueuing shader for destruction: " + shaderHandle.Identifier);
                shadersToClean.Enqueue (shaderHandle);

                // Note disposing has been done.
                disposed = true;
            }
        }

        /// <summary>
        /// Resets all the shader's variables to their default values.
        /// </summary>
        public void ResetVariables ()
        {
            currentVariables.Clear ();

            foreach (var kvp in variableActualNameToDeclaration)
            {
                currentVariables.Add (kvp.Key, kvp.Value.DefaultValue);
            }
        }

        /// <summary>
        /// Resets all the shader's samplers.
        /// </summary>
        public void ResetSamplers ()
        {
            currentSamplerTargets.Clear ();

            //foreach (var v in samplerNiceNameToActualName.Values)
            //{
            //    currentSamplerTargets.Add (v, null);
            //}
        }

        /// <summary>
        /// Sets the texture slot that a texture sampler should sample from.
        /// </summary>
        public void SetSamplerTarget (String name, Int32 slot)
        {
            if (!samplerNiceNameToActualName.ContainsKey (name)) {
                return;
            }
            String actualName = samplerNiceNameToActualName [name];
            currentSamplerTargets[actualName] = slot;
        }

        /// <summary>
        /// Sets the value of a specified shader variable.
        /// </summary>
        public void SetVariable<T>(String name, T value)
        {
            if (!variableNiceNameToActualName.ContainsKey (name)) {
                return;
            }
            String actualName = variableNiceNameToActualName [name];
            currentVariables[actualName] = value;
        }

        /// <summary>
        /// For the given vertex declaration, picks the most appropriate shader variant, activates applies
        /// this shader's current state to the GPU.  This must be called before using the
        /// GPU to draw primitives.
        /// </summary>
        internal void Activate (VertexDeclaration vertexDeclaration)
        {
            if (!bestVariantMap.ContainsKey (vertexDeclaration))
            {
                WorkOutBestVariantFor (vertexDeclaration);
            }

            Int32 bestVariantIndex = bestVariantMap[vertexDeclaration];

            // select the correct shader pass variant and then activate it
            platform.gfx_shdr_Activate (shaderHandle, bestVariantIndex);

            // For all current cached variables.
            foreach (var key1 in currentVariables.Keys)
            {
                var variable = variantVariableInfos [bestVariantIndex].ToList ().Find (x => x.Name == key1);

                if (variable == null)
                {
                    string warning = "WARNING: missing variable: " + key1;

                    if ( !logHistory.ContainsKey (warning) )
                    {
                        InternalUtils.Log.Info ("GFX", warning);

                        logHistory.Add (warning, true);
                    }
                }
                else
                {
                    var val = currentVariables[key1];
                    platform.gfx_shdr_SetVariable (shaderHandle, bestVariantIndex, key1, val);
                }
            }

            foreach (var key2 in currentSamplerTargets.Keys)
            {
                var sampler = variantSamplerInfos [bestVariantIndex].ToList ().Find (x => x.Name == key2);
                if (sampler == null)
                {
                    //InternalUtils.Log.Info ("GFX", "missing sampler: " + key2);
                }
                else
                {
                    var textureSlot = currentSamplerTargets[key2];

                    platform.gfx_shdr_SetSampler (shaderHandle, bestVariantIndex, key2, textureSlot);
                }
            }
        }

        /// <summary>
        /// Defines which vertex elements are required by this shader.
        /// </summary>
        public VertexElementUsage[] RequiredVertexElements { get { throw new NotImplementedException (); } }

        /// <summary>
        /// Defines which vertex elements are optionally used by this shader if they happen to be present.
        /// </summary>
        public VertexElementUsage[] OptionalVertexElements { get { throw new NotImplementedException (); } }

        /// <summary>
        /// The name of this shader.
        /// </summary>
        public String Name { get { throw new NotImplementedException (); } }


        /// <summary>
        /// This function takes a VertexDeclaration and a collection of
        /// OpenGL shader passes and works out which
        /// pass is the best fit for the VertexDeclaration.
        /// </summary>
        internal Int32 WorkOutBestVariantFor (VertexDeclaration vertexDeclaration)
        {
            InternalUtils.Log.Info ("GFX", "\n");
            InternalUtils.Log.Info ("GFX", "\n");
            InternalUtils.Log.Info ("GFX", "=====================================================================");
            InternalUtils.Log.Info ("GFX", "Working out the best shader variant for: " + vertexDeclaration);
            InternalUtils.Log.Info ("GFX", "Possible variants:");

            int best = 0;

            int bestNumMatchedVertElems = 0;
            int bestNumUnmatchedVertElems = 0;
            int bestNumMissingNonOptionalInputs = 0;

            var matches = new CompareShaderInputsResult [variantCount];

            for (int i = 0; i < variantCount; ++i)
            {
                matches [i] = CompareShaderInputs (vertexDeclaration, i);
            }

            // foreach variant
            for (int i = 0; i < variantCount; ++i)
            {
                // work out how many vert inputs match
                int numMatchedVertElems = matches[i].NumMatchedInputs;
                int numUnmatchedVertElems = matches[i].NumUnmatchedInputs;
                int numMissingNonOptionalInputs = matches[i].NumUnmatchedRequiredInputs;

                InternalUtils.Log.Info ("GFX", String.Format ("[{3}] {4} ~ matched:{0}, unmatched:{1}, missing:{2}",
                    numMatchedVertElems, numUnmatchedVertElems, numMissingNonOptionalInputs, i, variantIdentifiers[i] ));

                if (i == 0 )
                {
                    bestNumMatchedVertElems = numMatchedVertElems;
                    bestNumUnmatchedVertElems = numUnmatchedVertElems;
                    bestNumMissingNonOptionalInputs = numMissingNonOptionalInputs;
                }
                else
                {
                    if (
                        (
                            numMatchedVertElems > bestNumMatchedVertElems &&
                            bestNumMissingNonOptionalInputs == 0
                        )
                        ||
                        (
                            numMatchedVertElems == bestNumMatchedVertElems &&
                            bestNumMissingNonOptionalInputs == 0 &&
                            numUnmatchedVertElems < bestNumUnmatchedVertElems
                        )
                    )
                    {
                        bestNumMatchedVertElems = numMatchedVertElems;
                        bestNumUnmatchedVertElems = numUnmatchedVertElems;
                        bestNumMissingNonOptionalInputs = numMissingNonOptionalInputs;
                        best = i;
                    }
                }
            }

            InternalUtils.Log.Info ("GFX", "Chosen variant = [" + best +"]");

            bestVariantMap[vertexDeclaration] = best;

            var matchShaderIndices = matches [best].ShaderInputIndexToVertexIndex.Keys.ToList ();

            matchShaderIndices.Sort ();

            bestVariantMapVertexIndicies[vertexDeclaration] = new Int32[matchShaderIndices.Count];

            try
            {
                for (Int32 i = 0; i < matchShaderIndices.Count; ++i)
                {
                    bestVariantMapVertexIndicies[vertexDeclaration][i] =  matches [best].ShaderInputIndexToVertexIndex[i];
                }
            }
            catch (Exception)
            {
                throw new NotImplementedException ();
            }

            return best;
        }

        internal CompareShaderInputsResult CompareShaderInputs (
            VertexDeclaration vertexDeclaration, Int32 variantIndex)
        {
            var result = new CompareShaderInputsResult ();

            var inputsUsed = new List<ShaderInputInfo>();

            var vertElems = vertexDeclaration.GetVertexElements ();

            // shader index - vert index
            Dictionary<Int32, Int32> usedVertIndices = new  Dictionary<Int32, Int32>();
            Int32 i = 0;
            // itterate over each input defined in the vert decl
            foreach (var vertElem in vertElems)
            {
                var usage = vertElem.VertexElementUsage;
                var format = vertElem.VertexElementFormat;
                /*
                foreach (var input in oglesShader.Inputs)
                {
                    // the vertDecl knows what each input's intended use is,
                    // so lets match up
                    if (input.Usage == usage)
                    {
                        // intended use seems good
                    }
                }

                // find all inputs that could match
                var matchingInputs = oglesShader.Inputs.FindAll (
                    x =>
                        x.Usage == usage &&
                        (x.Type == VertexElementFormatHelper.FromEnum (format) ||
                        ( (x.Type.GetType () == typeof (Vector4)) && (format == VertexElementFormat.Colour) ))
                );
                */

                var matchingInputs = variantInputInfos [variantIndex]
                    .ToList ()
                    .FindAll (x => inputActualNameToDeclaration[x.Name].Usage == usage);

                // now make sure it's not been used already

                while (matchingInputs.Count > 0)
                {
                    var potentialInput = matchingInputs[0];

                    if (inputsUsed.Find (x => x == potentialInput) != null)
                    {
                        matchingInputs.RemoveAt (0);
                    }
                    else
                    {
                        inputsUsed.Add (potentialInput);
                        usedVertIndices [potentialInput.Index] = i;
                    }
                }

                ++i;
            }

            result.ShaderInputIndexToVertexIndex = usedVertIndices;

            result.NumMatchedInputs = inputsUsed.Count;

            result.NumUnmatchedInputs = vertElems.Length - result.NumMatchedInputs;

            result.NumUnmatchedRequiredInputs = 0;

            foreach (var input in variantInputInfos [variantIndex])
            {
                if (!inputsUsed.Contains (input) )
                {
                    if ( !inputActualNameToDeclaration[input.Name].Optional)
                    {
                        result.NumUnmatchedRequiredInputs++;
                    }
                }

            }

            return result;
        }

        internal struct CompareShaderInputsResult
        {
            public Dictionary<Int32, Int32> ShaderInputIndexToVertexIndex;

            // the nume
            public int NumMatchedInputs;
            public int NumUnmatchedInputs;
            public int NumUnmatchedRequiredInputs;
        }
    }
}