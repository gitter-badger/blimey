using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sungiant.Cor.MonoTouchRuntime
{
	public class ShaderVariantDefinition
	{
		public string VariantName { get; set; }
		public List<ShaderVarientPassDefinition> VariantPassDefinitions { get; set; }
	}
}

