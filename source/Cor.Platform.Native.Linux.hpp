#ifndef __COR_PLATFORM_NATIVE_LINUX_H__
#define __COR_PLATFORM_NATIVE_LINUX_H__

// These functions get called from managed code.
class cNativeBindings
{
public:
    cNativeBindings();
    
    ~cNativeBindings();

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

// These functions call into managed code.
class cManagedBindings
{
public:
    cManagedBindings();
    
    ~cManagedBindings();

    void Boot (char* settings, char* entryPoint);

private:

    // this calls into managed code.
    void BootstapApp(MonoString* settings, MonoString* entryPoint)
};

#endif

