
            this.sceneGraph = new StageStageGraph (this);
            this.cameraManager = new CameraManager(this);



        public StageStageGraph StageGraph { get { return sceneGraph; } }
        public CameraManager CameraManager { get { return cameraManager; } }

            this.engine.PreUpdate (time);

            foreach (Entity go in sceneGraph.GetAllObjects())
            {
                go.Update(time);
            }

            this.engine.PostUpdate (time);




            List<Entity> onesToDestroy = new List<Entity> ();

            foreach (Entity go in sceneGraph.GetAllObjects ())
            {
                if (go.Transform.Parent == null)
                {
                    onesToDestroy.Add (go);
                }
            }

            foreach (Entity go in onesToDestroy)
            {
                sceneGraph.DestroyStageObject (go);
            }

            Debug.Assert(sceneGraph.GetAllObjects ().Count == 0);



    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    internal class SceneManager
    {
        Platform platform;
        Engine engine;

        public SceneManager (Platform platform, Engine engine, Scene startScene)
        {
            this.platform = platform;
            this.engine = engine;
            nextScene = startScene;

        }

        public Boolean Update (AppTime time)
        {
            // If the active state returns a game state other than itself then we need to shut
            // it down and start the returned state.  If a game state returns null then we need to
            // shut the platform down.

            //quitting the game
            if (nextScene == null)
            {
                activeScene.Uninitilise ();
                activeScene = null;
                return true;
            }

            if (nextScene != activeScene && activeScene != null)
            {
                activeScene.Uninitilise ();
                activeScene = null;
                GC.Collect ();
                return false;
            }

            if (nextScene != activeScene)
            {
                activeScene = nextScene;
                activeScene.Initialize (platform, engine);
            }

            nextScene = activeScene.RunUpdate (time);

            return false;
        }


        Dictionary<Material, List<MeshRendererTrait>> GroupMeshRenderersByMaterials(Scene scene, String pass)
        {
            // TODO, make this fast
            var grouping = new Dictionary<Material, List<MeshRendererTrait>> ();
            foreach (var go in scene.SceneGraph.GetAllObjects())
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

        internal void Render (Platform platform, RenderPass pass)
        {

            var cam = scene.CameraManager.GetActiveCamera(pass.Name);

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

                    /*
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
                    */
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
        }
    }
