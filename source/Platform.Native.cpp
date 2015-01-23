// ┌────────────────────────────────────────────────────────────────────────────────────────────────────────────────┐ \\
// │ Cor! Native Implementation                                                                                     │ \\
// ├────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤ \\
// │                     Brought to you by:                                                                         │ \\
// │                              _________                    .__               __                                 │ \\
// │                             /   _____/__ __  ____    ____ |__|____    _____/  |_                               │ \\
// │                             \_____  \|  |  \/    \  / ___\|  \__  \  /    \   __\                              │ \\
// │                             /        \  |  /   |  \/ /_/  >  |/ __ \|   |  \  |                                │ \\
// │                            /_______  /____/|___|  /\___  /|__(____  /___|  /__|                                │ \\
// │                                    \/           \//_____/         \/     \/                                    │ \\
// │                                                                                                                │ \\
// ├────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤ \\
// │ Copyright © 2008-2014 Sungiant ~ http://www.blimey3d.com ~ Authors: A.J.Pook                                   │ \\
// ├────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤ \\
// │ Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated   │ \\
// │ documentation files (the "Software"), to deal in the Software without restriction, including without           │ \\
// │ limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sellcopies of the   │ \\
// │ Software, and to permit persons to whom the Software is furnished to do so, subject to the following           │ \\
// │ conditions:                                                                                                    │ \\
// │                                                                                                                │ \\
// │ The above copyright notice and this permission notice shall be included in all copies or substantial portions  │ \\
// │ of the Software.                                                                                               │ \\
// │                                                                                                                │ \\
// │ THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED  │ \\
// │ TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.  IN NO EVENT SHALL │ \\
// │ THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF  │ \\
// │ CONTRACT, TORT OR OTHERWISE, ARISING FROM,OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER        │ \\
// │ DEALINGS IN THE SOFTWARE.                                                                                      │ \\
// └────────────────────────────────────────────────────────────────────────────────────────────────────────────────┘ \\

#include "Platform.Native.hpp"

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
    // - Platform.Native.Linux.Bindings.dll
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

