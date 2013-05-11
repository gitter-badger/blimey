using System;
using Sungiant.Abacus.SinglePrecision;
using Sungiant.Abacus.Packed;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Sungiant.Cor.MonoTouchRuntime
{
	/// <summary>
	/// Represents in individual pass of a Cor.Xios high level Shader object.
	/// </summary>
	public class ShaderPass
		: IShaderPass
		, IDisposable
	{
		/// <summary>
		/// A collection of OpenGL shaders, all with slight variations in their
		/// input parameters, that are suitable for rendering this ShaderPass object.
		/// </summary>
		List<OglesShader> Variants { get; set; }
		
		/// <summary>
		/// A nice name for the shader pass, for example: Main or Cel -> Outline.
		/// </summary>
		public string Name { get; private set; }
		
		/// <summary>
		/// Whenever this ShaderPass object gets asked to activate itself whilst a VertexDeclaration it has not seen
		/// before is active, the best matching shader pass variant is found and then stored in this map to fast
		/// access.
		/// </summary>
		Dictionary<VertexDeclaration, OglesShader> BestVariantMap { get; set; }

		Dictionary<String, Object>	currentSettings = new Dictionary<String, Object>();
		
		
		internal void SetVariable<T>(string name, T value)
		{
			currentSettings[name] = value; 
		}
		
		public ShaderPass(string passName, List<Tuple<string, ShaderVarientPassDefinition>> passVariants___Name_AND_passVariantDefinition)
		{
			Console.WriteLine("Creating ShaderPass: " + passName);
			this.Name = passName;
			this.Variants = 
				passVariants___Name_AND_passVariantDefinition
					.Select (x => new OglesShader (x.Item1, passName, x.Item2.PassDefinition))
					.ToList();
			
			this.BestVariantMap = new Dictionary<VertexDeclaration, OglesShader>();
		}
		
		internal void ValidateInputs(List<ShaderInputDefinition> definitions)
		{
			foreach(var variant in this.Variants)
			{
				variant.ValidateInputs(definitions);
			}
		}
		
		internal void ValidateVariables(List<ShaderVariableDefinition> definitions)
		{
			foreach(var variant in this.Variants)
			{
				variant.ValidateVariables(definitions);
			}
		}
		
		
		public void Activate(VertexDeclaration vertexDeclaration)
		{
			if (!BestVariantMap.ContainsKey (vertexDeclaration))
			{
				BestVariantMap[vertexDeclaration] = ShaderHelper.WorkOutBestVariantFor(vertexDeclaration, Variants);
			}
			var bestVariant = BestVariantMap[vertexDeclaration];
			// select the correct shader pass variant and then activate it
			bestVariant.Activate ();
			
			foreach (var key in currentSettings.Keys)
			{
				var variable = bestVariant
					.Variables
						.Find(x => x.NiceName == key || x.Name == key);
				
				if( variable == null )
				{
					Console.WriteLine("missing " + key);
				}
				else
				{
					var val = currentSettings[key];
					
					variable.Set(val);
				}
			}
			
		}
		
		public void Dispose()
		{
			foreach (var oglesShader in Variants)
			{
				oglesShader.Dispose ();
			}
		}
	}
	

}

