// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ __________.__  .__                                                     │ \\
// │ \______   \  | |__| _____   ____ ___.__.                               │ \\
// │  |    |  _/  | |  |/     \_/ __ <   |  |                               │ \\
// │  |    |   \  |_|  |  Y Y  \  ___/\___  |                               │ \\
// │  |______  /____/__|__|_|  /\___  > ____|                               │ \\
// │         \/              \/     \/\/                                    │ \\
// │                                                                        │ \\
// │ A `work in progress` native implementation of the Blimey Platform API. │ \\
// │                                                                        │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Copyright © 2012 - 2015 ~ Blimey Engine (http://www.blimey.io)         │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Authors:                                                               │ \\
// │ ~ Ash Pook (http://www.ajpook.com)                                     │ \\
// │ ~ Ryan Sullivan (http://ryanpsullivan.github.io)                       │ \\
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

#pragma once

#include <iostream>
#include <stdio.h>

#include <mono/jit/jit.h> //AssemblyWrapper
#include <mono/metadata/assembly.h> //AssemblyWrapper
#include <mono/metadata/debug-helpers.h> //AssemblyWrapper
//----------------------------------------------------------------------------//
// Configuration
//----------------------------------------------------------------------------//
//#define ENABLE_ASSERTS // todo fix this for linux
//#define SYSTEM_PRINTF


//----------------------------------------------------------------------------//
// Typedefs
//----------------------------------------------------------------------------//
typedef unsigned int        uint;
typedef std::ostream        CorWriter;
typedef std::istream        CorReader;



//----------------------------------------------------------------------------//
// Constants
//----------------------------------------------------------------------------//
const char eol = '\n';



//----------------------------------------------------------------------------//
// Macros
//----------------------------------------------------------------------------//
#define BREAK_CPU () __asm { int 3 }

#ifdef SYSTEM_PRINTF
    static void CorPrintDebug (const char * str)
    {
        // Windows:
        // OutputDebugStringA (str);

        // Linux:
        // qDebug (str);
    }
#else
    #define CorPrintDebug printf
#endif

#define TRACE(e) CorPrintDebug (e);

#define SAFE_DELETE(a) \
    { \
        delete (a); \
        (a) = NULL; \
    }

#ifdef ENABLE_ASSERTS

    #define ASSERT(e) \
        if (!(e)) \
        { \
            char buffer[1024]; \
            sprintf_s( buffer, "ASSERT: %s line (%d)\n", __FILE__, __LINE__ ); \
            CorPrintDebug(buffer); \
            BREAK_CPU(); \
        }

    #define ASSERTM(expr, m) \
        if (!(expr)) \
        { \
            char buffer[1024]; \
            sprintf_s(buffer, "ASSERT: %s line (%d)\nASSERT: %s\n", __FILE__, __LINE__, m); \
            CorPrintDebug(buffer); \
            BREAK_CPU(); \
        }

    #define VERIFY(e) \
        if (!(e)) \
        { \
            ASSERTM(false, "VERIFY(" #e ") failed.\n"); \
        }

#else
        
    #define NOTHING
    #define ASSERT NOTHING
    #define ASSERTM NOTHING
    #define VERIFY NOTHING

#endif



//----------------------------------------------------------------------------//
// Singleton
//----------------------------------------------------------------------------//

//! A user created / destroyed singleton. 
//! Derive from this class to create your singleton.
template <typename T> class cSingleton
{
private:
    static T* ms_Singleton;

public:
    cSingleton( void )
    {
        ASSERTM( !ms_Singleton, "Trying to create singleton twice" );

        size_t offset = reinterpret_cast<size_t>(reinterpret_cast<T*>(1)) -
            reinterpret_cast<size_t>(static_cast<cSingleton <T>*>(reinterpret_cast<T*>(1)));

        ms_Singleton = reinterpret_cast<T*>((reinterpret_cast<size_t>(this) + offset));
    }

    virtual ~cSingleton( void )
    {
        ASSERT( ms_Singleton );  
        ms_Singleton = 0;
    }

    static T& Instance( void )
    {
        ASSERTM( ms_Singleton != NULL, "No singleton at %p\n", &ms_Singleton );  
        return ( *ms_Singleton );
    }

    static T* InstancePtr( void )
    {
        return ( ms_Singleton );
    }

    //! A create function that can safely be called many times without 
    //! the possibility of multiple instantiation.  Useful when you don't 
    //! want the overhead of the autosingleton for every access to the 
    //! singleton pointer (e.g., when you store a pointer to a singleton 
    //! object - you can just create it once then avoid the test for existence)
    static T* Create()
    {
        if ( !InstancePtr() )
        {
            new T();
        }
        return InstancePtr();
    }

    //! Destroy this singleton
    static void Destroy()
    {
        delete ms_Singleton;
        ms_Singleton = 0;
    }
};

//----------------------------------------------------------------------------//
// Native calls
//----------------------------------------------------------------------------//

extern "C" void 	__sfx_SetVolume (float volume);
extern "C" float	__sfx_GetVolume ();
extern "C" void 	__gfx_ClearColourBuffer (byte r, byte g, byte b, byte a);
extern "C" void 	__gfx_ClearDepthBuffer (float depth);
extern "C" void 	__gfx_SetCullMode (int cullMode);
extern "C" void 	__gfx_SetBlendEquation (int rgbBlendFunction, int sourceRgb, int destinationRgb, int alphaBlendFunction, int sourceAlpha, int destinationAlpha);
extern "C" void 	__gfx_CreateVertexBuffer ();
extern "C" void 	__gfx_CreateIndexBuffer ();
extern "C" void 	__gfx_CreateTexture ();
extern "C" void 	__gfx_CreateShader ();
extern "C" void 	__gfx_DestroyVertexBuffer ();
extern "C" void 	__gfx_DestroyIndexBuffer ();
extern "C" void 	__gfx_DestroyTexture ();
extern "C" void 	__gfx_DestroyShader ();
extern "C" void 	__gfx_DrawPrimitives ();
extern "C" void 	__gfx_DrawIndexedPrimitives ();
extern "C" void 	__gfx_DrawUserPrimitives ();
extern "C" void 	__gfx_DrawUserIndexedPrimitives ();
extern "C" void 	__gfx_CompileShader ();
extern "C" void 	__gfx_dbg_BeginEvent ();
extern "C" void 	__gfx_dbg_EndEvent ();
extern "C" void 	__gfx_dbg_SetMarker ();
extern "C" void 	__gfx_dbg_SetRegion ();
extern "C" void 	__gfx_vbff_GetVertexCount ();
extern "C" void 	__gfx_vbff_GetVertexDeclaration ();
extern "C" void 	__gfx_vbff_SetData ();
extern "C" void 	__gfx_vbff_GetData ();
extern "C" void 	__gfx_vbff_Activate ();
extern "C" void 	__gfx_ibff_GetIndexCount ();
extern "C" void 	__gfx_ibff_SetData ();
extern "C" void 	__gfx_ibff_GetData ();
extern "C" void 	__gfx_ibff_Activate ();
extern "C" void 	__gfx_tex_GetWidth ();
extern "C" void 	__gfx_tex_GetHeight ();
extern "C" void 	__gfx_tex_GetTextureFormat ();
extern "C" byte[] 	__gfx_tex_GetData ();
extern "C" void 	__gfx_tex_Activate ();
extern "C" void 	__gfx_shdr_SetVariable ();
extern "C" void 	__gfx_shdr_SetSampler ();
extern "C" void 	__gfx_shdr_Activate ();
extern "C" void 	__gfx_shdr_GetVariantCount ();
extern "C" void 	__gfx_shdr_GetIdentifier ();
extern "C" void 	__gfx_shdr_GetInputs ();
extern "C" void 	__gfx_shdr_GetVariables ();
extern "C" void 	__gfx_shdr_GetSamplers ();
extern "C" void 	__res_GetFileStream ();
extern "C" char* 	__sys_GetMachineIdentifier ();
extern "C" char* 	__sys_GetOperatingSystemIdentifier ();
extern "C" char* 	__sys_GetVirtualMachineIdentifier ();
extern "C" int 		__sys_GetPrimaryScreenResolutionWidth ();
extern "C" int 		__sys_GetPrimaryScreenResolutionHeight ();
extern "C" void 	__sys_GetPrimaryPanelPhysicalSize ();
extern "C" void 	__sys_GetPrimaryPanelType ();
extern "C" int 		__app_IsFullscreen ();
extern "C" int 		__app_GetWidth ();
extern "C" int 		__app_GetHeight ();
extern "C" int 		__hid_GetCurrentOrientation ();
extern "C" void 	__hid_GetDigitalControlStates ();
extern "C" void 	__hid_GetAnalogControlStates ();
extern "C" void 	__hid_GetBinaryControlStates ();
extern "C" void 	__hid_GetPressedCharacters ();
extern "C" void 	__hid_GetActiveTouches ();


//----------------------------------------------------------------------------//
// Classes
//----------------------------------------------------------------------------//

class cEngine
    : public cSingleton<cEngine>
{
public:
    cEngine();
    ~cEngine();

    void Setup();
};

// A class that wraps up a given assembly life-cycle and provides a method
// to invoke functions within the assembly
class cAssemblyWrapper
{
protected:
	cAssemblyWrapper( std::string assemblyName )
	{
	   	MonoDomain *domain = mono_jit_init( "SOMEDOMAINNAME" ); //TODO:Pull this out

	    m_assembly = mono_domain_assembly_open( domain, assemblyName.c_str() );
	}
	
	~cAssemblyWrapper( )
	{
		mono_assembly_close( m_assembly );
	}
	
	void InvokeMethodOnAssembly( std::string nameSpace, 
	std::string className,
	std::string methodName, 
	void** args )
	{
		MonoImage* monoImage = mono_assembly_get_image( m_assembly );
	    MonoClass* monoClass = mono_class_from_name(monoImage, nameSpace.c_str(), className.c_str());
	    MonoMethodDesc* mono_method_desc = mono_method_desc_new( ( className + "::" + methodName ).c_str(), false);
	    MonoMethod* fooMethod = mono_method_desc_search_in_class( mono_method_desc, monoClass );
    
	    mono_runtime_invoke (fooMethod, NULL, args, NULL);
	}
	
private:
	MonoAssembly* m_assembly;	
};

// An implementation of the assembly wrapper that will be used to invoke methods within
// the linux binding dll
class cBindingAssemblyWrapper :
	cAssemblyWrapper
{
public:
	cBindingAssemblyWrapper( ) : cAssemblyWrapper( "LINUXBINDINGDLLNAME" )
	{
	}
};

int main()
{
	cBindingAssemblyWrapper* linuxBindingDLL = new cBindingAssemblyWrapper();
	
	//do some stuff with the dll
	
	delete linuxBindingDLL;
	return 0;
}


