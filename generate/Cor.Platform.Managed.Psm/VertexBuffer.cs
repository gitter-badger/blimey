using System;
using System.Linq;
using Sungiant.Blimey.Helpers;
using Sungiant.Abacus;
using System.Collections.Generic;

namespace Sungiant.Blimey.PsmRuntime
{
    public class VertexBuffer
        : IVertexBuffer
    {
		Sce.Pss.Core.Graphics.VertexBuffer _pssBuffer;
		VertexDeclaration _vertexDeclaration;

        public VertexBuffer(Sce.Pss.Core.Graphics.VertexBuffer buffer, VertexDeclaration vertexDeclaration)
        {
			_vertexDeclaration = vertexDeclaration;
			_pssBuffer = buffer;
        }
		public VertexDeclaration VertexDeclaration 
		{ 
			get
			{
				return _vertexDeclaration;
			}
		}

        public void SetData<T>(T[] data) where T : struct, IVertexType
        {
			// TODO: FIX THIS METHOD, IT IS SHIT AND SLOW, SHOULD NOT BE HARDCODED
			
			var vertPosData = data as VertexPosition[];
			if( vertPosData != null ){ this.SetVertPosData( vertPosData ); return; }
			
			var vertPosColData = data as VertexPositionColour[];
			if( vertPosColData != null ){ this.SetVertPosColData( vertPosColData ); return; }
			
			var vertPosNormData = data as VertexPositionNormal[];
			if( vertPosNormData != null ){ this.SetVertPosNormData( vertPosNormData ); return; }
			
			var vertPosNormColData = data as VertexPositionNormalColour[];
			if( vertPosNormColData != null ){ this.SetVertPosNormColData( vertPosNormColData ); return; }
			
			var vertPosNormTexData = data as VertexPositionNormalTexture[];
			if( vertPosNormTexData != null ){ this.SetVertPosNormTexData( vertPosNormTexData ); return; }
			
			var vertPosNormTexColData = data as VertexPositionNormalTextureColour[];
			if( vertPosNormTexColData != null ){ this.SetVertPosNormTexColData( vertPosNormTexColData ); return; }
			
			throw new System.NotImplementedException();
        }
		
		public void GetData<T>(T[] data) where T : struct, IVertexType
        {
			throw new System.NotImplementedException();
		}

		void SetVertPosData(VertexPosition[] data)
		{
			var _pos = new List<Sce.Pss.Core.Vector3>();
			
			foreach(var dat in data)
			{
				_pos.Add(dat.Position.ToPSS());
			}
			
			_pssBuffer.SetVertices(0, _pos.ToArray());
		}
		
		void SetVertPosColData(VertexPositionColour[] data)
		{
			var _pos = new List<Sce.Pss.Core.Vector3>();
			var _colour = new List<Sce.Pss.Core.Vector4>();
			
			foreach(var dat in data)
			{
				_pos.Add(dat.Position.ToPSS());
				_colour.Add(dat.Colour.ToVector4().ToPSS());
			}
			
			_pssBuffer.SetVertices(0, _pos.ToArray());
			_pssBuffer.SetVertices(1, _colour.ToArray());
		}
		
		void SetVertPosNormData(VertexPositionNormal[] data)
		{
			var _pos = new List<Sce.Pss.Core.Vector3>();
			var _normal = new List<Sce.Pss.Core.Vector3>();
			
			foreach(var dat in data)
			{
				_pos.Add(dat.Position.ToPSS());
				_normal.Add(dat.Normal.ToPSS());
			}
			
			_pssBuffer.SetVertices(0, _pos.ToArray());
			_pssBuffer.SetVertices(1, _normal.ToArray());
		}
		
		void SetVertPosNormColData(VertexPositionNormalColour[] data)
		{
			var _pos = new List<Sce.Pss.Core.Vector3>();
			var _normal = new List<Sce.Pss.Core.Vector3>();
			var _colour = new List<Sce.Pss.Core.Vector4>();
			
			foreach(var dat in data)
			{
				_pos.Add(dat.Position.ToPSS());
				_normal.Add(dat.Normal.ToPSS());
				_colour.Add(dat.Colour.ToVector4().ToPSS());
			}
			
			_pssBuffer.SetVertices(0, _pos.ToArray());
			_pssBuffer.SetVertices(1, _normal.ToArray());
			_pssBuffer.SetVertices(2, _colour.ToArray());
			
		}
		
		void SetVertPosNormTexData(VertexPositionNormalTexture[] data)
		{
			var _pos = new List<Sce.Pss.Core.Vector3>();
			var _normal = new List<Sce.Pss.Core.Vector3>();
			var _texture = new List<Sce.Pss.Core.Vector2>();
			
			foreach(var dat in data)
			{
				_pos.Add(dat.Position.ToPSS());
				_normal.Add(dat.Normal.ToPSS());
				_texture.Add(dat.UV.ToPSS());
			}
			
			_pssBuffer.SetVertices(0, _pos.ToArray());
			_pssBuffer.SetVertices(1, _normal.ToArray());
			_pssBuffer.SetVertices(2, _texture.ToArray());
		}
		
		void SetVertPosNormTexColData(VertexPositionNormalTextureColour[] data)
		{
			var _pos = new List<Sce.Pss.Core.Vector3>();
			var _normal = new List<Sce.Pss.Core.Vector3>();
			var _texture = new List<Sce.Pss.Core.Vector2>();
			var _colour = new List<Sce.Pss.Core.Vector4>();
			
			foreach(var dat in data)
			{
				_pos.Add(dat.Position.ToPSS());
				_normal.Add(dat.Normal.ToPSS());
				_texture.Add(dat.UV.ToPSS());
				_colour.Add(dat.Colour.ToVector4().ToPSS());
			}
			
			_pssBuffer.SetVertices(0, _pos.ToArray());
			_pssBuffer.SetVertices(1, _normal.ToArray());
			_pssBuffer.SetVertices(2, _texture.ToArray());
			_pssBuffer.SetVertices(3, _colour.ToArray());
		}
		
        public int VertexCount
        {
            get
            {
                return _pssBuffer.VertexCount;
            }
        }
		
		
		
		
		
		
		
		
		
		

    }
}
