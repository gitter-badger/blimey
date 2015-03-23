using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("Platform")]
[assembly: AssemblyDescription("Platform, built with PlayStation Studio, targetting Mono for PlayStation Mobile.")]
[assembly: ComVisible(false)]
[assembly: AssemblyVersion("0.0.0.1")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: CLSCompliant(true)]

[assembly: InternalsVisibleTo("Platform.Xios")]
[assembly: InternalsVisibleTo("Platform.Xna4")]
[assembly: InternalsVisibleTo("Platform.Psm")]
[assembly: InternalsVisibleTo("Platform.MonoMac")]
[assembly: InternalsVisibleTo("Platform.Stub")]
[assembly: InternalsVisibleTo("Platform.Lib.Khronos")]

