#ifndef __COR_PLATFORM_NATIVE_LINUX_H__
#define __COR_PLATFORM_NATIVE_LINUX_H__

class cNativeBindings
{
public:
    void Engine_Create ();

    void AudioManager_Create ();

    void GraphicsManager_Create ();

    void ResourceManager_Create ();

    void InputManager_Create ();

    void SystemManager_Create ();

    void DisplayStatus_Create ();

    void IndexBuffer_Create ();

    void PanelSpecification_Create ();

    void ScreenSpecification_Create ();

    void GeometryBuffer_Create ();

    void VertexBuffer_Create ();
};

#endif

