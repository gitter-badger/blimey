using System;
using System.Collections.Generic;

namespace Sungiant.Blimey.PsmRuntime
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class IndexBuffer
        : IIndexBuffer
    {
		Sce.Pss.Core.Graphics.VertexBuffer _pssBuffer;
		
        public IndexBuffer(Sce.Pss.Core.Graphics.VertexBuffer buffer)
        {
			_pssBuffer = buffer;
        }

        public void GetData(UInt16[] data)
        {
			throw new System.NotImplementedException();
        }

        public void GetData(UInt16[] data, int startIndex, int elementCount)
        {
			throw new System.NotImplementedException();
        }

        public void GetData(int offsetInBytes, UInt16[] data, int startIndex, int elementCount)
        {
			throw new System.NotImplementedException();
        }

        public void SetData(UInt16[] data)
        {
			_pssBuffer.SetIndices(data);
        }

        public void SetData(UInt16[] data, int startIndex, int elementCount)
        {
			throw new System.NotImplementedException();
        }

        public void SetData(int offsetInBytes, UInt16[] data, int startIndex, int elementCount)
        {
			throw new System.NotImplementedException();
        }

        public int IndexCount 
        { 
            get
            {
                return _pssBuffer.IndexCount;
            }
        }

    }
}
