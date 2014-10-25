#include "Cor.Platform.Native.hpp"

#include <iostream>
#include <list>
#include <vector>
#include <string>
#include <sstream>
#include <fstream>
#include <iomanip>
#include <deque>
#include <algorithm>
#include <ctime>
#include <cstdio>
#include <cstdlib>
#include <ctime>
#include <cmath>

//----------------------------------------------------------------------------//
// cSingleton
//----------------------------------------------------------------------------//

template <typename T> T* cSingleton<T>::ms_Singleton = NULL;


//void cManagedBindings::Boot (char* settings, char* entryPoint)
//{
    // this needs to make a call into:
    // - Cor.Platform.Native.Linux.Bindings.dll
    // to trigger starting the users app from their desired
    // entry point with their desired settings

//    MonoString* monoSettingsString = mono_string_new (mono_domain_get (), settings);
//    MonoString* monoEntryPointString = mono_string_new (mono_domain_get (), entryPoint);

//    BootstapApp(monoSettingsString, monoEntryPointString);

//}


//----------------------------------------------------------------------------//
// cEngine
//----------------------------------------------------------------------------//

cEngine::cEngine ()
{
    std::cout << "Engine Created" << std::endl;
}

cEngine::~cEngine ()
{
    std::cout << "Engine Destroyed" << std::endl;
}

void cEngine::Setup ()
{
    std::cout << "Engine Setup" << std::endl;
}

