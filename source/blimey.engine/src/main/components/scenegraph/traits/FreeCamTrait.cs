// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ __________.__  .__                                                     │ \\
// │ \______   \  | |__| _____   ____ ___.__.                               │ \\
// │  |    |  _/  | |  |/     \_/ __ <   |  |                               │ \\
// │  |    |   \  |_|  |  Y Y  \  ___/\___  |                               │ \\
// │  |______  /____/__|__|_|  /\___  > ____|                               │ \\
// │         \/              \/     \/\/                                    │ \\
// │                                                                        │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Copyright © 2012 - 2015 ~ Blimey Engine (http://www.blimey.io)         │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Authors:                                                               │ \\
// │ ~ Ash Pook (http://www.ajpook.com)                                     │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Permission is hereby granted, free of charge, to any person obtaining  │ \\
// │ a copy of this software and associated documentation files (the        │ \\
// │ "Software"), to deal in the Software without restriction, including    │ \\
// │ without limitation the rights to use, copy, modify, merge, publish,    │ \\
// │ distribute, sublicense, and/or sellcopies of the Software, and to      │ \\
// │ permit persons to whom the Software is furnished to do so, subject to  │ \\
// │ the following conditions:                                              │ \\
// │                                                                        │ \\
// │ The above copyright notice and this permission notice shall be         │ \\
// │ included in all copies or substantial portions of the Software.        │ \\
// │                                                                        │ \\
// │ THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,        │ \\
// │ EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF     │ \\
// │ MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. │ \\
// │ IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY   │ \\
// │ CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,   │ \\
// │ TORT OR OTHERWISE, ARISING FROM,OUT OF OR IN CONNECTION WITH THE       │ \\
// │ SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.                 │ \\
// └────────────────────────────────────────────────────────────────────────┘ \\

namespace Blimey.Engine
{
    using System;
    using Fudge;
    using global::Blimey.Platform;
    using global::Blimey.Asset;
    using Abacus.SinglePrecision;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class FreeCamTrait
        : Trait
    {
        public struct FreeCamInputs
        {
            public Vector3 mTranslation;
            public Vector3 mRotation;
            public float mTranslationSpeed; //in range 0-1
            public float mRotationSpeedScale;
            public bool mFixUp;
        }

        float localPitch;
        float localYaw;
        float localRoll;

        Vector3 oldPosition = Vector3.Zero;

        // inputs that come from the controller
        FreeCamInputs mInputs;

        // fre cam settings
        float mTranslationSpeedStandard = 10.0f;
        float mTranslationSpeedMaximum = 100.0f;
        float mRotationSpeed = 45.0f; //30 degrees per second

        public void WorkOutInputs()
        {
            var input = new FreeCamInputs();

            var xbox = this.Platform.Input.Xbox360Gamepad;
            var keyboard = this.Platform.Input.Keyboard;

            input.mTranslation = new Vector3(
                xbox.Thumbsticks.Left.X,
                0.0f,
                -xbox.Thumbsticks.Left.Y
                );

            input.mRotation = new Vector3(
                -xbox.Thumbsticks.Right.Y,
                -xbox.Thumbsticks.Right.X,
                0.0f
                );

            input.mTranslationSpeed = xbox.Triggers.Right;

            input.mRotationSpeedScale = 1.0f;

            input.mFixUp = keyboard.IsCharacterKeyUp('u');
            SetInputs(input);
        }

        public void SetInputs(FreeCamInputs zIn) { mInputs = zIn; }


        public void Reset()
        {
            //need to change this to that these values tie in with whatever the camera was looking at before
            localPitch=0.0f;
            localYaw = 0.0f;
            localRoll = 0.0f;
            oldPosition = Vector3.Zero;
        }

        public override void OnUpdate(AppTime time)
        {
            WorkOutInputs();

            float translationSpeed = mTranslationSpeedStandard
                + mInputs.mTranslationSpeed *
                (mTranslationSpeedMaximum - mTranslationSpeedStandard);

            Vector3 translation = mInputs.mTranslation * translationSpeed * time.Delta;

            Vector3 rotation =
                mInputs.mRotation *
                Maths.ToRadians(mRotationSpeed) *
                mInputs.mRotationSpeedScale * time.Delta;

            localPitch += rotation.X;
            localYaw += rotation.Y;
            localRoll += rotation.Z;

            Quaternion rotationFromInputs = Quaternion.CreateFromYawPitchRoll(localYaw, localPitch, localRoll);

            Quaternion currentOri = this.Parent.Transform.Rotation;

            this.Parent.Transform.Rotation = Quaternion.Multiply(currentOri, rotationFromInputs);

            float yTranslation = translation.Y;
            translation.Y = 0.0f;

            this.Parent.Transform.Position +=
                oldPosition +
                Vector3.Transform(translation, this.Parent.Transform.Rotation) +
                new Vector3(0.0f, yTranslation, 0.0f);

            //focusDistance = 3.0f;

            //update the old position for next time
            oldPosition = this.Parent.Transform.Position;
        }
    }
}
