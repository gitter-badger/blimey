using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sungiant.Cor.MonoTouchRuntime
{
	public class ShaderVariableDefinition
	{
		public String NiceName { get; set; }

		String name;
		public String Name
		{ 
			get { return name; }
			set { 
				if (value.Length > 16)
					name = value.Substring (0, 16);
				else
					name = value;
			}
		}
		public Type Type { get; set; }
		public Object DefaultValue { get; set; }
	}
}

