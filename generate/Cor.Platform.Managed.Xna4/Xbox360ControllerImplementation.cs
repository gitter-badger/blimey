using System;
using Sungiant.Cor;

namespace Sungiant.Cor.Platform.Managed.Xna4
{
	public class Xbox360ControllerImplementation
		: Xbox360Gamepad
	{
		Microsoft.Xna.Framework.PlayerIndex _playerIndex;
		internal Xbox360ControllerImplementation(Microsoft.Xna.Framework.PlayerIndex playerIndex)
		{
			_playerIndex = playerIndex;
		}

		internal void Update(AppTime time)
		{
			base.Reset();

			var state = Microsoft.Xna.Framework.Input.GamePad.GetState(_playerIndex);
			var chatPad = Microsoft.Xna.Framework.Input.Keyboard.GetState(_playerIndex);

			if (state.DPad.Down == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
				base.DPad.Down = ButtonState.Pressed;

			if (state.DPad.Up == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
				base.DPad.Up = ButtonState.Pressed;

			if (state.DPad.Left == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
				base.DPad.Left = ButtonState.Pressed;

			if (state.DPad.Right == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
				base.DPad.Right = ButtonState.Pressed;

			if (state.Buttons.A == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
				base.Buttons.A = ButtonState.Pressed;

			if (state.Buttons.B == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
				base.Buttons.B = ButtonState.Pressed;

			if (state.Buttons.X == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
				base.Buttons.X = ButtonState.Pressed;

			if (state.Buttons.Y == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
				base.Buttons.Y = ButtonState.Pressed;

			if (state.Buttons.Start == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
				base.Buttons.Start = ButtonState.Pressed;

			if (state.Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
				base.Buttons.Back = ButtonState.Pressed;

			if (state.Buttons.RightShoulder == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
				base.Buttons.RightShoulder = ButtonState.Pressed;

			if (state.Buttons.LeftShoulder == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
				base.Buttons.LeftShoulder = ButtonState.Pressed;

			if (state.Buttons.RightStick == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
				base.Buttons.RightStick = ButtonState.Pressed;

			if (state.Buttons.LeftStick == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
				base.Buttons.LeftStick = ButtonState.Pressed;

		}
	}
}