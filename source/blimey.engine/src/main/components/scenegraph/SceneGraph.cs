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
    using System.Collections.Generic;
    using System.Linq;
    using Abacus.SinglePrecision;
    using Fudge;
    using global::Blimey.Platform;
    using global::Blimey.Asset;


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //
    // Blimey Engine's Scene Graph
    //
    public sealed class SceneGraph
        : Engine.Component
    {
        readonly Platform platform;

        // Data structure that stores all entities in the scene graph.
        // Currently this is just as list, it should really be an oct-tree.
        readonly List<Entity> data = new List<Entity> ();

        // Cache
        readonly Dictionary<String, Entity> defaultCameras = new Dictionary<String,Entity>();
        readonly Dictionary<String, Entity> activeCameras = new Dictionary<String,Entity>();


        internal SceneGraph (Platform platform)
        {
            this.platform = platform;

            //this.CreateDefaultCameras (config.RenderPasses);
            //this.CreateDefaultLights ();
        }

        public List<Entity> GetAllEntities ()
        {
            return data;
        }

        public Entity CreateEntity (String zName)
        {
            var go = new Entity (parent, zName);
            data.Add (go);
            return go;
        }

        public void DestroyEntity (Entity zEntity)
        {
            zEntity.Shutdown ();
            foreach (Entity e in zEntity.Children) {
                this.DestroyStageObject (e);
                data.Remove (e);
            }
            data.Remove (zEntity);

            zEntity = null;
        }

        public Entity GetRenderPassCamera (String renderPass)
        {
            return GetActiveCamera (renderPass).Parent;
        }

        internal CameraTrait GetActiveCamera (String renderPass)
        {
            return activeCameras[renderPass].GetTrait<CameraTrait> ();
        }


        internal void SetDefaultCamera(String renderPass)
        {
            activeCameras[renderPass] = defaultCameras[renderPass];
        }

        internal void SetMainCamera (String renderPass, Entity e)
        {
            activeCameras[renderPass] = e;
        }

        void CreateDefaultCameras (List<RenderPass> renderPasses)
        {
            foreach (var renderPass in renderPasses)
            {
                var go = CreateSceneObject("RenderPass(" + renderPass + ") Provided Camera");

                var cam = go.AddTrait<CameraTrait>();

                if (renderPass.Configuration.CameraProjectionType == CameraProjectionType.Perspective)
                {
                    go.Transform.Position = new Vector3(2, 1, 5);

                    var orbit = go.AddTrait<OrbitAroundSubjectTrait>();
                    orbit.CameraSubject = Transform.Origin;

                    var lookAtSub = go.AddTrait<LookAtSubjectTrait>();
                    lookAtSub.Subject = Transform.Origin;
                }
                else
                {
                    cam.Projection = CameraProjectionType.Orthographic;

                    go.Transform.Position = new Vector3(0, 0, 1f);
                    go.Transform.LookAt(Vector3.Zero);
                }


                _defaultCameras.Add(renderPass.Name, go);
                _activeCameras.Add(renderPass.Name, go);
            }
        }

        Dictionary<Material, List<MeshRendererTrait>> GroupMeshRenderersByMaterials (String pass)
        {
            // TODO, make this fast
            var grouping = new Dictionary<Material, List<MeshRendererTrait>> ();
            foreach (var go in GetAllObjects())
            {
                if (!go.Enabled) continue;

                var mr = go.GetTrait<MeshRendererTrait>();

                if (mr == null) continue;
                if (!mr.Active) continue;
                if (mr.Material == null) continue;

                // if the material is for this pass
                if (mr.Material.RenderPass == pass)
                {
                    if (!grouping.ContainsKey (mr.Material))
                        grouping [mr.Material] = new List<MeshRendererTrait> ();

                    grouping [mr.Material].Add (mr);
                }
            }

            return grouping;
        }

        public virtual void Render (Platform platform, RenderPass pass)
        {
/*
            var cam = GetActiveCamera (pass.Name);

            // todo, move this to the update loop
            // #1 Render everything in the scene graph that has a material on this pass.
            var groupedMeshRenderers = this.GroupMeshRenderersByMaterials(scene, pass.Name);

            // Group all mesh renderers by material.  If the material is the same, the shader is the same.
            foreach (var material in groupedMeshRenderers.Keys)
            {
                // skip nulls
                if (material.Shader == null)
                    continue;

                // materials define render state, set it now, just the once
                using (new ProfilingTimer (t => FrameStats.Add ("MaterialUpdateRenderStateTime", t)))
                {
                    material.UpdateRenderState (platform.Graphics);
                }

                // perhaps something else has used this shader before us.  for now, to be sure
                // just reset all variables to their defaults.  this can be optimised later.
                using (new ProfilingTimer (t => FrameStats.Add ("ResetShaderTime", t)))
                {
                    material.Shader.ResetVariables ();
                }

                using (new ProfilingTimer(t => FrameStats.Add ("MaterialUpdateShaderTime", t)))
                {
                    material.UpdateShaderState ();
                    material.Shader.SetVariable ("View", cam.ViewMatrix44);
                    material.Shader.SetVariable ("Projection", cam.ProjectionMatrix44);


                    // The lighing manager right now just grabs the shader and tries to set
                    // all variables to do with lighting, without even knowing if the shader
                    // supports lighting.
                    material.SetColour( "AmbientLightColour", LightingManager.ambientLightColour );
                    material.SetColour( "EmissiveColour", LightingManager.emissiveColour );
                    material.SetColour( "SpecularColour", LightingManager.specularColour );
                    material.SetFloat( "SpecularPower", LightingManager.specularPower );

                    material.SetFloat( "FogEnabled", LightingManager.fogEnabled ? 1f : 0f );
                    material.SetFloat( "FogStart", LightingManager.fogStart );
                    material.SetFloat( "FogEnd", LightingManager.fogEnd );
                    material.SetColour( "FogColour", LightingManager.fogColour );

                    material.SetVector3( "DirectionalLight0Direction", LightingManager.dirLight0Direction );
                    material.SetColour( "DirectionalLight0DiffuseColour", LightingManager.dirLight0DiffuseColour );
                    material.SetColour( "DirectionalLight0SpecularColour", LightingManager.dirLight0SpecularColour );

                    material.SetVector3( "DirectionalLight1Direction", LightingManager.dirLight1Direction );
                    material.SetColour( "DirectionalLight1DiffuseColour", LightingManager.dirLight1DiffuseColour );
                    material.SetColour( "DirectionalLight1SpecularColour", LightingManager.dirLight1SpecularColour );

                    material.SetVector3( "DirectionalLight2Direction", LightingManager.dirLight2Direction );
                    material.SetColour( "DirectionalLight2DiffuseColour", LightingManager.dirLight2DiffuseColour );
                    material.SetColour( "DirectionalLight2SpecularColour", LightingManager.dirLight2SpecularColour );

                    material.SetVector3( "EyePosition", zView.Translation );

                    // Get the material's shader and apply all of the settings
                    // it needs.
                }


                // TODO: big one
                // we really need to group the mesh renderers by material
                // and only make a new draw call when there are changes.
                foreach (var mr in groupedMeshRenderers[material])
                {
                    material.Shader.SetVariable ("World", mr.Parent.Transform.Location);

                    using (new ProfilingTimer(t => FrameStats.Add ("SetCullModeTime", t)))
                    {
                        platform.Graphics.SetCullMode(mr.CullMode);
                    }

                    using (new ProfilingTimer(t => FrameStats.Add ("ActivateVertexBufferTime", t)))
                    {
                        // Set our vertex declaration, vertex buffer, and index buffer.
                        platform.Graphics.SetActive(mr.Mesh.VertexBuffer);
                    }

                    using (new ProfilingTimer(t => FrameStats.Add ("ActivateIndexBufferTime", t)))
                    {
                        // Set our vertex declaration, vertex buffer, and index buffer.
                        platform.Graphics.SetActive(mr.Mesh.IndexBuffer);
                    }

                    using (new ProfilingTimer(t => FrameStats.Add ("ActivateShaderTime", t)))
                    {
                        platform.Graphics.SetActive (material.Shader);
                    }

                    using (new ProfilingTimer(t => FrameStats.Add ("DrawTime", t)))
                    {
                        FrameStats.Add ("DrawIndexedPrimitivesCount", 1);
                        platform.Graphics.DrawIndexedPrimitives (
                            PrimitiveType.TriangleList, 0, 0,
                            mr.Mesh.VertexBuffer.VertexCount, 0, mr.Mesh.TriangleCount);
                    }
                }
            }
            */
        }
    }
}
