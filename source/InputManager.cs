using System;
using Sungiant.Cor;

namespace Sungiant.Cor.Xna4Runtime
{
	public class InputManager
		: IInputManager
	{


		public MultiTouchController GetTouchScreen() { return _touchScreen; }
		public MultiTouchController GetRearTouchPanel() { return null; }
        public PsmGamepad GetPsmGamepad() { return null; }
		public GenericGamepad GetGenericGamepad(){ return _genericPad; }
		
		TouchScreenImplementation _touchScreen;

		GenericGamepad _genericPad;
		Xbox360ControllerImplementation _pad1;
		Xbox360ControllerImplementation _pad2;
		Xbox360ControllerImplementation _pad3;
		Xbox360ControllerImplementation _pad4;



		public Xbox360Gamepad GetXbox360Gamepad(PlayerIndex player)
		{
			switch(player)
			{
				case PlayerIndex.One: return _pad1;
				case PlayerIndex.Two: return _pad2;
				case PlayerIndex.Three: return _pad3;
				case PlayerIndex.Four: return _pad4;
				default: throw new System.NotSupportedException();
			}
			
		}

		public InputManager(ICor engine)
		{

#if WINDOWS || XBOX

			_pad1 = new Xbox360ControllerImplementation(Microsoft.Xna.Framework.PlayerIndex.One);
			_pad2 = new Xbox360ControllerImplementation(Microsoft.Xna.Framework.PlayerIndex.Two);
			_pad3 = new Xbox360ControllerImplementation(Microsoft.Xna.Framework.PlayerIndex.Three);
			_pad4 = new Xbox360ControllerImplementation(Microsoft.Xna.Framework.PlayerIndex.Four);

			_genericPad = new GenericGamepad(this);

#endif


#if WP7
			_touchScreen = new TouchScreenImplementation(engine);
#endif

#if WINDOWS
			if (engine.Settings.MouseGeneratesTouches)
			{
				_touchScreen = new TouchScreenImplementation(engine);
			}
#endif
		}

		public void Update(AppTime time)
		{

#if WINDOWS || XBOX
			_pad1.Update(time);
			_pad2.Update(time);
			_pad3.Update(time);
			_pad4.Update(time);

			_genericPad.Update(time);
#endif

#if WINDOWS
			if (_touchScreen != null)
			{
				_touchScreen.Update(time);
			}
#endif

#if WP7
			_touchScreen.Update(time);
#endif
		}
	}
}
