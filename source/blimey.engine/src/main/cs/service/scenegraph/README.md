Blimey's scene graph service, inspired by Unity3D, provides the following types:

- Entity ~= Unity GameObects
- Trait ~= Unity MonoBehavior

The service enables hierarchical management of a Entities within a scene and additionally provides an abstraction over the rendering pipeline.

Rendering of the scene is managed internally by Blimey, which is achieved by collecting all Traits within the Scene of type `MeshRenderer`, batching them up and then rendering them in the RenderPass they were tagged for. 