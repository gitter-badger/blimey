#pragma once

#include <iostream>
#include <stdio.h>
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

        size_t offset = reinterpret_cast<size_t>(
            reinterpret_cast<T*>(1)) -
            reinterpret_cast<size_t>(
                static_cast<cSingleton <T>*>(
                    reinterpret_cast<T*>(1)
                )
            );

        ms_Singleton = reinterpret_cast<T*>(
            (reinterpret_cast<size_t>(this) + offset));
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
// Classes
//----------------------------------------------------------------------------//

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
    //void BootstapApp(MonoString* settings, MonoString* entryPoint)
};


class cCorEngine
    : public cSingleton<cCorEngine>
{
public:
    cCorEngine();
    ~cCorEngine();

    void Setup();
};


