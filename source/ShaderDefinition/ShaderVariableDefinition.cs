using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sungiant.Cor.MonoTouchRuntime
{
	public class ShaderVariableDefinition
	{
		public String NiceName { get; set; }
		public String Name { get; set; }
		public Type Type { get; set; }
		public Object DefaultValue { get; set; }
	}
}

