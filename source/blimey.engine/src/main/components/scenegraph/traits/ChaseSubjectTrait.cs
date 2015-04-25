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

    public class ChaseSubjectTrait
        : Trait
    {
        Transform subject;
        Boolean dirty;
        Vector3 desiredPositionOffset;
        Vector3 velocity;

        // The target that this behaviour will chase.
        public Transform Subject
        {
            get { return subject; }
            set
            {
                subject = value;

                // When we set the subject we work out the current vector between us
                // and the target, so that we always try to keep this seperation
                // even if the subject moves.
                // This vector is in world space.
                desiredPositionOffset =
                    this.Parent.Transform.Position -
                    subject.Position;

                //Console.WriteLine(Parent.Name + ": ChaseSubject desiredPositionOffset=" + desiredPositionOffset);
            }
        }

        public float Mass { get; set; }
        public float Damping { get; set; }
        public float Stiffness { get; set; }
        public Boolean SpringEnabled { get; set; }

        public override void OnEnable()
        {
            this.ApplyDefaultSettings();
        }

        public void ApplyDefaultSettings()
        {
            this.ResetSpring();

            // Mass of the camera body.
            // Heaver objects require stiffer springs with less
            // damping to move at the same rate as lighter objects.
            this.Mass = 20.0f;
            this.Damping = 40.0f;
            this.Stiffness = 2000.0f;
            this.SpringEnabled = false;

            this.velocity = Vector3.Zero;
        }

        /// Forces camera to be at desired position and to stop moving. The is useful
        /// when the chased object is first created or after it has been teleported.
        /// Failing to call this after a large change to the chased object's position
        /// will result in the camera quickly flying across the world.
        public void ResetSpring()
        {
            this.dirty = true;
        }

        public override void OnUpdate(AppTime time)
        {
            Vector3 previousPosition = this.Parent.Transform.Position;

            Vector3 desiredPosition = Subject.Position + desiredPositionOffset;

            Vector3 stretch = previousPosition - desiredPosition;
            //Console.WriteLine(Parent.Name + ": ChaseSubject stretch=" + stretch + " - (" + previousPosition + " - " + desiredPosition + ")");

            if (this.dirty || ! SpringEnabled)
            {
                this.dirty = false;

                // Stop motion
                this.velocity = Vector3.Zero;

                // Force desired position
                this.Parent.Transform.Position = desiredPosition;
            }
            else
            {
                // Calculate spring force
                Vector3 force = -this.Stiffness * stretch - this.Damping * this.velocity;

                // Apply acceleration
                Vector3 acceleration = force / this.Mass;
                this.velocity += acceleration * time.Delta;

                // Apply velocity
                Vector3 deltaPosition = this.velocity * time.Delta;
                this.Parent.Transform.Position += deltaPosition;
            }
        }
    }
}
