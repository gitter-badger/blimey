using System;
using Sungiant.Abacus.Packed;
using Sungiant.Abacus.SinglePrecision;
using Sungiant.Cor;

namespace Sungiant.Cor.Xna4Runtime
{
	public class TouchScreenImplementation
		: MultiTouchController
	{

#if WINDOWS

		Microsoft.Xna.Framework.Input.MouseState previousMouseState;
		bool doneFirstUpdateFlag = false;

#endif


		ScreenImplementation screen;

		internal TouchScreenImplementation(ICor engine)
			: base(engine)
		{

			this.screen = (engine.SystemManager as SystemManager).MainDisplayPanel;
		}

		public override IPanelSpecification PanelSpecification { get { return screen; } }

		internal override void Update(AppTime time)
		{

			this.collection.ClearBuffer();

#if WP7
			foreach (var xnaTouch in Microsoft.Xna.Framework.Input.Touch.TouchPanel.GetState())
			{
				Int32 id = xnaTouch.Id;
				Vector2 pos = xnaTouch.Position.ToBlimey();


				pos.X = pos.X / (Single) Microsoft.Xna.Framework.Input.Touch.TouchPanel.DisplayWidth;
				pos.Y = pos.Y / (Single) Microsoft.Xna.Framework.Input.Touch.TouchPanel.DisplayHeight;

				pos -= new Vector2(0.5f, 0.5f);

				var state = EnumConverter.ToBlimey(xnaTouch.State);

				this.touchCollection.RegisterTouch(id, pos, state, time.FrameNumber, time.Elapsed);
			}
#endif


#if WINDOWS

			var mouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();

			if( doneFirstUpdateFlag )
			{

				bool pressedThisFrame = (mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed);
				bool pressedLastFrame = (previousMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed);


				Int32 id = -42;
				Vector2 pos = new Vector2(mouseState.X, mouseState.Y);

				Int32 w = engine.GraphicsManager.DisplayStatus.CurrentWidth;
				Int32 h = engine.GraphicsManager.DisplayStatus.CurrentHeight;

				pos.X = pos.X / (Single)w;
				pos.Y = pos.Y / (Single)h;

				pos -= new Vector2(0.5f, 0.5f);

				pos.Y = -pos.Y;

				var state = TouchPhase.Invalid;
				
				if (pressedThisFrame && !pressedLastFrame)
				{
					// new press
					state = TouchPhase.JustPressed;
				}
				else if (pressedLastFrame && pressedThisFrame)
				{
					// press in progress
					state = TouchPhase.Active;
				}
				else if (pressedLastFrame && !pressedThisFrame)
				{
					// released
					state = TouchPhase.JustReleased;
				}

				if (state != TouchPhase.Invalid)
				{
					this.collection.RegisterTouch(id, pos, state, time.FrameNumber, time.Elapsed);
				}


			}
			else
			{
				doneFirstUpdateFlag = true;
			}

			previousMouseState = mouseState;

#endif
		}
	}
}