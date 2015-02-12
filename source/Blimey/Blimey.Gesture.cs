// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ __________.__  .__                                                     │ \\
// │ \______   \  | |__| _____   ____ ___.__.                               │ \\
// │  |    |  _/  | |  |/     \_/ __ <   |  |                               │ \\
// │  |    |   \  |_|  |  Y Y  \  ___/\___  |                               │ \\
// │  |______  /____/__|__|_|  /\___  > ____|                               │ \\
// │         \/              \/     \/\/                                    │ \\
// │                                                                        │ \\
// │ Cor, Blimey!                                                           │ \\
// │                                                                        │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Copyright © 2012 - 2015 ~ Blimey3D (http://www.blimey3d.com)           │ \\
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

namespace Blimey
{
    using System;
    using System.Runtime.InteropServices;
    using System.Globalization;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    
    using Fudge;
    using Abacus.SinglePrecision;
    
    using System.Linq;
    using Cor;
    using Platform;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public enum GestureType
    {
        Tap, // multiiple fingers
        DoubleTap, // multiple fingers
        Flick,
        Drag,
        DragUpdate,
        DragComplete,
        Pinch,
        Pivot,
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public enum TouchPositionSpace
    {
        NormalisedEngine,
        Screen,
        RealWorld

    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class Gesture
    {
        static int GestureIDAssigner = 0;

        InputEventSystem inputEventSystem;
        Int32 id;
        GestureType type;
        String[] touchIDs;

        public Vector2 GetFinishingPosition(TouchPositionSpace space)
        {

            Vector2 averageFinishPos = Vector2.Zero ;
            foreach (String touchID in TouchIDs)
            {
                var tracker = inputEventSystem.GetTouchTracker(touchID);

                var p = tracker.GetPosition(space);

                averageFinishPos += p;
            }

            averageFinishPos /= TouchIDs.Length;

            return averageFinishPos;
        }

        public Gesture(InputEventSystem inputEventSystem, GestureType type, String[] touchIDs)
        {
            this.inputEventSystem = inputEventSystem;
            this.id = GestureIDAssigner;
            GestureIDAssigner++;
            this.type = type;
            this.touchIDs = touchIDs;
        }

        public Int32 ID
        {
            get
            {
                return this.id;
            }
        }

        public GestureType Type
        {
            get
            {
                return this.type;
            }
        }

        public String[] TouchIDs
        {
            get
            {
                return this.touchIDs;
            }
        }

        public List<TouchTracker> TouchTrackers
        {
            get
            {
                var tt = new List<TouchTracker>();

                foreach (String touchID in TouchIDs)
                {
                    var tracker = inputEventSystem.GetTouchTracker(touchID);
                    tt.Add(tracker);
                }

                return tt;
            }
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class InputEventSystem
    {
        MultiTouchController controller;

        internal InputEventSystem(Engine  engine)
        {
            this.engine = engine;

            this.controller = engine.Input.MultiTouchController;
        }

        Engine engine;

        public delegate void GestureDelegate(Gesture gesture);

        public event GestureDelegate Tap;
        public event GestureDelegate DoubleTap;
        public event GestureDelegate Flick;
        //public event GestureDelegate DragStart;
        //public event GestureDelegate DragUpdate;
        //public event GestureDelegate DragEnd;
        //public event GestureDelegate Pinch;


        internal TouchTracker GetTouchTracker(String id)
        {
            var found = touchTrackers.Find(q => q.TouchID == id);

            return found;
        }

        List<TouchTracker> touchTrackers = new List<TouchTracker>();

        Queue<Gesture> gestureQueue = new Queue<Gesture>();

        List<PotentialGesture> potentialGestures = new List<PotentialGesture>();

        internal void Reset()
        {
            // release all listeners
            Tap = null;
            DoubleTap = null;
            Flick = null;
        }

        internal virtual void Update(AppTime time)
        {
            if( controller != null )
            {
                // before this the child should have updated this TouchCollection
                this.UpdateTouchTrackers(time);
                this.UpdateGestureDetection(time);
                this.InvokeGestureEvents(time);
            }
        }

        void UpdateTouchTrackers(AppTime time)
        {

            // delete all touch trackers that whose last touch was in the released state
            int num = touchTrackers.RemoveAll(x => (x.Phase == TouchPhase.JustReleased || x.Phase == TouchPhase.Invalid));

            if( num > 0 )
            {
                Console.WriteLine("Blimey.Input", string.Format("Removing {0} touches.", num));
            }

            // go through all active touches
            foreach (var touch in controller.TouchCollection)
            {
                // find the corresponding tracker

                TouchTracker tracker = touchTrackers.Find(x => (x.TouchID == touch.ID));

                if (tracker == null)
                {
                    tracker = new TouchTracker(
                        this.engine,
                        this.engine.Host.ScreenSpecification,
                        this.engine.Host.PanelSpecification,
                        touch.ID );

                    touchTrackers.Add(tracker);
                }

                tracker.RegisterTouch(touch);
            }

            // assert if there are any trackers in the list that have not been updated this frame
            var problems = touchTrackers.FindAll(x => (x.LatestTouch.FrameNumber != time.FrameNumber));
            if (problems.Count != 0)
                throw new Exception ();
        }

        void UpdateGestureDetection(AppTime time)
        {
            // Each frame we look for press combinations that could potentially
            // be the start of a gesture.
            foreach (var touchTracker in touchTrackers)
            {
                if( touchTracker.Phase == TouchPhase.JustPressed )
                {
                    // this could be the start of a tap
                    var potentialTapGesture =
                        new PotentialTapGesture(
                            this,
                            new String[]{touchTracker.TouchID} );

                    var potentialDoubleTapGesture =
                        new PotentialDoubleTapGesture(
                            this,
                            new String[]{touchTracker.TouchID} );

                    var potentialFlickGesture =
                        new PotentialFlickGesture(
                            this,
                            new String[]{touchTracker.TouchID} );

                    potentialGestures.Add(potentialTapGesture);
                    potentialGestures.Add(potentialDoubleTapGesture);
                    potentialGestures.Add(potentialFlickGesture);

                }

                int enqueueCount = 0;

                foreach(var potentialGesture in potentialGestures)
                {
                    var gesture = potentialGesture.Update(time.Delta, touchTrackers);

                    if( gesture != null )
                    {
                        this.gestureQueue.Enqueue(gesture);
                        enqueueCount++;
                    }
                }

                int removeCount = potentialGestures.Count;
                potentialGestures.RemoveAll(x => x.Finished );
                removeCount -= potentialGestures.Count;

            }
        }

        void InvokeGestureEvents(AppTime time)
        {
            foreach (var gesture in gestureQueue)
            {
                string line = string.Format("({1}) {0}", gesture.Type, gesture.ID);
                switch (gesture.Type)
                {
                    case GestureType.Tap:
                        line += string.Format(", finishing position {0}", gesture.GetFinishingPosition(TouchPositionSpace.NormalisedEngine));

                        if (this.Tap != null)
                        {
                            this.Tap(gesture);
                        }
                        break;

                    case GestureType.DoubleTap:
                        if (this.DoubleTap != null)
                        {
                            this.DoubleTap(gesture);
                        }
                        break;

                    case GestureType.Flick:
                        line += string.Format(", finishing position {0}", gesture.GetFinishingPosition(TouchPositionSpace.NormalisedEngine));
                        if (this.Flick != null)
                        {
                            this.Flick(gesture);
                        }
                        break;

                    default: throw new System.NotImplementedException();
                }

                Console.WriteLine("Blimey.Input", line);
            }

            gestureQueue.Clear();
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    internal class PotentialDoubleTapGesture
        : PotentialGesture
    {
        internal PotentialDoubleTapGesture(
            InputEventSystem inputEventSystem,
            String[] touchIDs)
            : base(inputEventSystem, GestureType.DoubleTap, touchIDs)
        {

        }

        internal override Gesture Update(float dt, List<TouchTracker> touchTrackers)
        {
            var touchTracker = inputEventSystem.GetTouchTracker(touchIDs[0]);

            if (touchTracker == null)
            {
                failedGesture = true;
                return null;
            }

            failedGesture = true;
            return null;
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    internal class PotentialFlickGesture
        : PotentialGesture
    {
        const float velocityRequired = 0.05f;
        const float displacementRequired = 0.01f;

        internal PotentialFlickGesture(
            InputEventSystem inputEventSystem,
            String[] touchIDs)
            : base(inputEventSystem, GestureType.Flick, touchIDs)
        {

        }

        internal override Gesture Update(
            float dt,
            List<TouchTracker> touchTrackers)
        {
            var touchTracker = inputEventSystem.GetTouchTracker(touchIDs[0]);

            if (touchTracker == null)
            {
                failedGesture = true;
                return null;
            }

            var velocity = touchTracker.GetVelocity(TouchPositionSpace.RealWorld).Length();


            float distanceTravelled = touchTracker.GetDistanceTraveled(TouchPositionSpace.RealWorld);

            if (velocity >= velocityRequired &&
                distanceTravelled >= displacementRequired &&
                touchTracker.Phase == TouchPhase.JustReleased )
            {
                completedGesture = true;
                return new Gesture(this.inputEventSystem, this.type, this.touchIDs);
            }

            if( touchTracker.Phase == TouchPhase.JustReleased )
            {
                failedGesture = true;
            }

            return null;
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    internal abstract class PotentialGesture
    {

        // in meters
        const float DISPLACEMENT_REQUIRED_FOR_DRAGS = 0.01f;
        const float MAX_DISPLACEMENT_FOR_TAPS = 0.005f;

        protected InputEventSystem inputEventSystem;

        static int PotentialGestureIDAssigner = 0;

        internal PotentialGesture(
            InputEventSystem inputEventSystem,
            GestureType type, String[] touchIDs)
        {
            this.id = PotentialGestureIDAssigner;
            this.inputEventSystem = inputEventSystem;
            PotentialGestureIDAssigner++;

            this.type = type;
            this.touchIDs = touchIDs;
        }

        internal abstract Gesture Update(Single dt, List<TouchTracker> touchTrackers);

        internal bool Finished { get { return failedGesture || completedGesture; } }

        protected bool failedGesture = false;
        protected bool completedGesture = false;

        protected Int32 id;
        protected GestureType type;
        protected String[] touchIDs;
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    internal class PotentialTapGesture
        : PotentialGesture
    {
        const Single MaxHoldTimeForTap = 0.6f;
        const Single MaxDisplacementForTap = 0.005f;

        Single timer = 0f;

        internal PotentialTapGesture(InputEventSystem inputEventSystem, String[] touchIDs)
            : base(inputEventSystem, GestureType.Tap, touchIDs)
        {

        }

        internal override Gesture Update(float dt, List<TouchTracker> touchTrackers)
        {
            if( failedGesture )
                throw new Exception("wrong!");

            this.timer += dt;

            if( this.timer > MaxHoldTimeForTap)
                failedGesture = true;

            var touchTracker = inputEventSystem.GetTouchTracker(touchIDs[0]);

            if (touchTracker == null)
            {
                failedGesture = true;
                return null;
            }


            if( touchTracker.Phase == TouchPhase.JustReleased )
            {
                float distanceTravelled = touchTracker.GetDistanceTraveled(TouchPositionSpace.RealWorld);
                if (distanceTravelled <= MaxDisplacementForTap)
                {
                    completedGesture = true;
                    return new Gesture(this.inputEventSystem, this.type, this.touchIDs);
                }
                else
                {
                    failedGesture = true;
                }

            }

            return null;
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class TouchTracker
    {
        const Int32 NumFramesPerTrackedTouch = 15;

        Int32 trackCounter = -1;
        string id;
        List<Touch> samples = new List<Touch>();
        ScreenSpecification screenSpec;
        PanelSpecification panelSpec;
        Engine engine;

        internal TouchTracker(
            Engine engine,
            ScreenSpecification displayMode,
            PanelSpecification panelMode,
            string id )
        {
            this.engine = engine;
            this.screenSpec = displayMode;
            this.panelSpec = panelMode;
            this.id = id;

        }

        internal void RegisterTouch(Touch t)
        {
            if( trackCounter == -1 )
            {
                this.samples.Add(t);
            }
            else
            {
                if( trackCounter % NumFramesPerTrackedTouch == 0 )
                {
                    this.samples.Add(t);
                }
                else
                {
                    this.samples[this.samples.Count -1] = t;
                }
            }


            trackCounter++;

        }

        internal Touch LatestTouch { get { return this.samples.Last(); } }

        internal String TouchID { get { return this.id; } }

        internal TouchPhase Phase { get { return samples.Last().Phase; } }

        Vector2 GetPositionOfSampleAtIndex(int index, TouchPositionSpace space)
        {
            var pos = this.samples[index].Position;

            var multiplier = Vector2.One;
            switch (space)
            {
                case TouchPositionSpace.RealWorld:

                    if(engine.Host.CurrentOrientation == DeviceOrientation.Default ||
                       engine.Host.CurrentOrientation == DeviceOrientation.Upsidedown)
                    {
                multiplier = new Vector2(panelSpec.PanelPhysicalSize.Value.X, panelSpec.PanelPhysicalSize.Value.Y);
                    }
                    else
                    {
                multiplier = new Vector2(panelSpec.PanelPhysicalSize.Value.Y, panelSpec.PanelPhysicalSize.Value.X);
                    }

                    break;

                case TouchPositionSpace.Screen:

                    if (this.engine.Host.CurrentOrientation == DeviceOrientation.Upsidedown )
                    {
                        pos.Y = - pos.Y;
                        pos.X = - pos.X;
                    }
                    else if (this.engine.Host.CurrentOrientation == DeviceOrientation.Leftside )
                    {
                        Single temp = pos.X;
                        pos.X = -pos.Y;
                        pos.Y = temp;
                    }
                    else if(this.engine.Host.CurrentOrientation == DeviceOrientation.Rightside )
                    {
                        Single temp = pos.X;
                        pos.X = pos.Y;
                        pos.Y = -temp;
                    }

                    Int32 w = this.engine.Status.Width;
                    Int32 h = this.engine.Status.Height;

                    //this.engine.System.GetEffectiveDisplaySize(ref w, ref h);

                    multiplier = new Vector2(w, h);

                    break;

            }
            pos *= multiplier;

            return pos;
        }

        public Vector2 GetPosition(TouchPositionSpace space)
        {
            int numSamples = samples.Count;

            var curPos = this.GetPositionOfSampleAtIndex(numSamples - 1, space);



            return curPos;
        }

        public Vector2 GetVelocity(TouchPositionSpace space)
        {
            int numSamples = samples.Count;

            if (numSamples > 1)
            {
                var currentTouch = this.samples[numSamples - 1];
                var previousTouch = this.samples[numSamples - 2];

                var currentPos = this.GetPositionOfSampleAtIndex(numSamples - 1, space);
                var previousPos = this.GetPositionOfSampleAtIndex(numSamples - 2, space);

                Single dt = currentTouch.Timestamp - previousTouch.Timestamp;

                return (currentPos - previousPos) / dt;
            }



            return Vector2.Zero;
        }

        public Single GetDistanceTraveled(TouchPositionSpace posType)
        {
            Single distance = 0f;

            for (Int32 i = 0; i < samples.Count; ++i)
            {
                if (i > 0)
                {
                    var currentPosition = this.GetPositionOfSampleAtIndex(i, posType);
                    var previousPosition = this.GetPositionOfSampleAtIndex(i - 1, posType);

                    Single mag = (currentPosition - previousPosition).Length();

                    distance += mag;
                }
            }

            return distance;
        }
    }
}
