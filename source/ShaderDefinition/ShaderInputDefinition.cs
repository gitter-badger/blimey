using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sungiant.Cor.MonoTouchRuntime
{
	public class ShaderInputDefinition
	{
		public String Name { get; set; }
		public Type Type { get; set; }
		public VertexElementUsage Usage { get; set; }
		public Object DefaultValue { get; set; }
		public Boolean Optional { get; set; }
	}
}

