#include <iostream>
#include <../../submodules/cor.platform.native.linux/source/Cor.Platform.Native.Linux.hpp>

use namespace std;

int main (int argc, char *argv[])
{
	cout << "This is where the user boots the app" << endl;

    // create vm here

    // These c# libraries need to be built into dlls:
    // - Abacus.dll
    // - Cor.dll
    // - Cor.Platform.Native.Linux.Bindings.dll
    // - Cor.Demo.dll

    // The dlls then get loaded into the mono virtual machine.

    // We then start evething like this:

    cManagedBindings managedBindings = new cManagedBindings();

    managedBindings->Boot(
        "Cor.Demo.Demo.GetAppSettings()",
        "Cor.Demo.Demo.GetAppEntryPoint()");

    // RUN

    SAFE_DELETE (managedBindings);

	return 0;
}

