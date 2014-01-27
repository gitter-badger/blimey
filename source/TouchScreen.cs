using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sungiant.Abacus;

namespace Sungiant.Blimey.PsmRuntime
{
	public class TouchScreen
		: MultiTouchController
		, IScreenSpecification
		, IPanelSpecification
	{
		Int32 touchIDCounter = -1;
		
		HashSet<Int32> previousActiveFingerIDs = new HashSet<Int32>();
		HashSet<Int32> currentActiveFingerIDs = new HashSet<Int32>();
		
		Dictionary<Int32, Int32> currentFingerIDToTouchID = new Dictionary<Int32, Int32>();
		
		public override IPanelSpecification PanelSpecification { get{ return this; } }
		Sce.Pss.Core.Graphics.GraphicsContext gfx;
		
		internal TouchScreen(IEngine engine, Sce.Pss.Core.Graphics.GraphicsContext gfx)
			: base(engine)
		{
			this.gfx = gfx;
		}

		internal override void Update(GameTime time)
		{
			this.touchCollection.ClearBuffer();
			
			previousActiveFingerIDs = currentActiveFingerIDs;
			currentActiveFingerIDs = new HashSet<Int32>();

			var data = Sce.Pss.Core.Input.Touch.GetData(0);
			
			var keys = currentFingerIDToTouchID.Keys.ToArray();
			foreach(var fingerId in keys)
			{
				var selectResult = data.Select(x => x.ID == fingerId);
				if( selectResult.Count() == 0 )
					currentFingerIDToTouchID.Remove(fingerId);
			}
				
			foreach (var touchData in data) 
			{		
				var phase = EnumConverter.ToBlimey(touchData.Status);

				if( phase != TouchPhase.Invalid )
				{
					Int32 fingerID = touchData.ID;
					currentActiveFingerIDs.Add(fingerID);
				}
			}
			
			foreach (var touchData in data) 
			{
				var phase = EnumConverter.ToBlimey(touchData.Status);

				if( phase != TouchPhase.Invalid )
				{
					Int32 fingerID = touchData.ID;

					if( !previousActiveFingerIDs.Contains(fingerID) &&
					   currentActiveFingerIDs.Contains(fingerID) )
					{
						currentFingerIDToTouchID.Add(fingerID, ++touchIDCounter);						
					}
					
					if( previousActiveFingerIDs.Contains(fingerID) &&
					   !currentActiveFingerIDs.Contains(fingerID) )
					{
						currentFingerIDToTouchID.Remove(fingerID);						
					}
				}
			}
			
			foreach (var touchData in data)
			{
				var phase = EnumConverter.ToBlimey(touchData.Status);

				if( phase != TouchPhase.Invalid )
				{
					Int32 fingerID = touchData.ID;
					
					Int32 touchID = currentFingerIDToTouchID[fingerID];
					
					Vector2 pos = new Vector2(touchData.X, touchData.Y);

					this.touchCollection.RegisterTouch(touchID, pos, phase, time.FrameNumber, time.Elapsed);
				}
        	}
			
			base.Update(time);
		}
		
		
		public Single ScreenResolutionAspectRatio
		{ 
			get
			{
				return ScreenResolutionWidth / ScreenResolutionHeight;
			}
		}
		public Int32 ScreenResolutionWidth
		{ 
			get
			{
				return gfx.Screen.Width;
			}
		}
		public Int32 ScreenResolutionHeight
		{ 
			get
			{
				return gfx.Screen.Height;
			}
		}
		
		public Vector2 PanelPhysicalSize
		{ 
			get
			{
				return new Vector2(0.11f, 0.63f);
			}
		}
		public Single PanelPhysicalAspectRatio
		{ 
			get
			{
				return PanelPhysicalSize.X / PanelPhysicalSize.Y;
			}
		}

		public PanelType PanelType
		{ 
			get
			{
				return PanelType.TouchScreen;
			}
		}
	}
}

