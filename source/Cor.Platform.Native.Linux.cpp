#include "Cor.Platform.Native.Linux.h"
#include <stdio>

void cNativeBindings::cNativeBindings ()
{

}

void cNativeBindings::~cNativeBindings ()
{

}

void cNativeBindings::Engine_Create ()
{
    std::cout << "NATIVE -> Engine -> Create()" << std::endl;
}

void cNativeBindings::AudioManager_Create ()
{
    std::cout << "NATIVE -> AudioManager -> Create()" << std::endl;
}

void cNativeBindings::GraphicsManager_Create ()
{
    std::cout << "NATIVE -> GraphicsManager -> Create()" << std::endl;
}

void cNativeBindings::ResourceManager_Create ()
{
    std::cout << "NATIVE -> ResourceManager -> Create()" << std::endl;
}

void cNativeBindings::InputManager_Create ()
{
    std::cout << "NATIVE -> InputManager -> Create()" << std::endl;
}

void cNativeBindings::SystemManager_Create ()
{
    std::cout << "NATIVE -> SystemManager -> Create()" << std::endl;
}

void cNativeBindings::DisplayStatus_Create ()
{
    std::cout << "NATIVE -> ATIVE -> Create()" << std::endl;
}

void cNativeBindings::IndexBuffer_Create ()
{
    std::cout << "NATIVE -> IndexBuffer -> Create()" << std::endl;
}

void cNativeBindings::PanelSpecification_Create ()
{
    std::cout << "NATIVE -> PanelSpecification -> Create()" << std::endl;
}

void cNativeBindings::ScreenSpecification_Create ()
{
    std::cout << "NATIVE -> ScreenSpecification -> Create()" << std::endl;
}

void cNativeBindings::GeometryBuffer_Create ()
{
    std::cout << "NATIVE -> GeometryBuffer -> Create()" << std::endl;
}

void cNativeBindings::VertexBuffer_Create ()
{
    std::cout << "NATIVE -> VertexBuffer -> Create()" << std::endl;
}

void cManagedBindings::cManagedBindings ()
{
    mono_add_internal_call ("ManagedBindings::BootstapApp", BootstapApp);
}

void cManagedBindings::~cManagedBindings ()
{

}

void cManagedBindings::Boot (char* settings, char* entryPoint)
{
    // this needs to make a call into:
    // - Sungiant.Cor.Platform.Native.Linux.Bindings.dll
    // to trigger starting the users app from their desired
    // entry point with their desired settings

    MonoString* monoSettingsString = mono_string_new (mono_domain_get (), settings);
    MonoString* monoEntryPointString = mono_string_new (mono_domain_get (), entryPoint);

    BootstapApp(monoSettingsString, monoEntryPointString);

}

