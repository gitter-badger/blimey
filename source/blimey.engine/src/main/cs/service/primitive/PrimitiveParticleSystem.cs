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
    using System.Collections.Generic;
    using System.Linq;
    using Abacus.SinglePrecision;
    using Fudge;
    using global::Blimey.Platform;
    using global::Blimey.Asset;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class PrimitiveParticleSystemInfo
    {
        public SpritePrimitive sprite;    // texture + blend mode
        public int nEmission; // particles per sec
        public float fLifetime;

        public float fParticleLifeMin;
        public float fParticleLifeMax;

        public float fDirection;
        public float fSpread;
        public bool bRelative;

        public float fSpeed;
        public float fGravity;
        public float fRadialAccel;
        public float fTangentialAccel;

        public float fSizeStart;
        public float fSizeEnd;
        public float fSizeVar;

        public float fSpinStart;
        public float fSpinEnd;
        public float fSpinVar;

        public Rgba32 colColourStart; // + alpha
        public Rgba32 colColourEnd;
        public float fColourStartVar;
        public float fColourEndVar;
    }

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class PrimitiveParticleSystem
    {
        static int MAX_PARTICLES = 5000;
        //static int MAX_PSYSTEMS   = 100;

        public PrimitiveParticleSystemInfo info = new PrimitiveParticleSystemInfo();

        BoundingRectangle rectBoundingBox = new BoundingRectangle();
        bool bUpdateBoundingBox;

        float               fAge;
        float               fEmissionResidue;

        Vector2             vecPrevLocation;
        Vector2             vecLocation;
        float               fTx, fTy;
        float               fScale;

        int                 nParticlesAlive;

        ParticlePrimitive[]          particles = new ParticlePrimitive[MAX_PARTICLES];

        public PrimitiveParticleSystem(PrimitiveParticleSystemInfo psi)
        {
            for (int i = 0; i < MAX_PARTICLES; i++)
            {
                particles[i] = new ParticlePrimitive();
            }

            info = psi;

            vecLocation.X=vecPrevLocation.X=0.0f;
            vecLocation.Y=vecPrevLocation.Y=0.0f;
            fTx=fTy=0;
            fScale = 1.0f;

            fEmissionResidue=0.0f;
            nParticlesAlive=0;
            fAge=-2.0f;
        }

        public void TrackBoundingBox(bool bTrack) { bUpdateBoundingBox = bTrack; }

        public BoundingRectangle GetBoundingBox(ref BoundingRectangle rect)
        {
            rect = rectBoundingBox;

            rect.x1 *= fScale;
            rect.y1 *= fScale;
            rect.x2 *= fScale;
            rect.y2 *= fScale;

            return rect;
        }

        public void Draw(String renderPass)
        {
            Rgba32 col = info.sprite.GetColour();

            for (int i = 0; i < nParticlesAlive; i++ )
            {
                ParticlePrimitive par = particles[i];

                System.Diagnostics.Debug.Assert(par != null);
                //Rgba32 temp = info.sprite.GetColour();
                //temp.A = par.colColour.A;
                info.sprite.SetColour(par.colColour);

                info.sprite.DrawEx(renderPass,
                    par.vecLocation.X * fScale + fTx,
                    par.vecLocation.Y * fScale + fTy,
                    par.fSpin * par.fAge,
                    par.fSize * fScale);
            }

            // set the particle system's sprite back to the colour it was
            info.sprite.SetColour(col);
        }

        public void FireAt(float x, float y)
        {
            Stop();
            MoveTo(x, y);
            Fire();
        }

        public void Fire()
        {
            if (info.fLifetime == -1.0f)
                fAge = -1.0f;
            else
                fAge = 0.0f;
        }

        public void Stop()
        {
            Stop(false);
        }

        public void Stop(bool bKillParticles)
        {
            fAge = -2.0f;
            if (bKillParticles)
            {
                nParticlesAlive = 0;
                rectBoundingBox.Clear();
            }
        }

        public void Update(float fDeltaTime)
        {
            int i;
            float ang;
            ParticlePrimitive par = null;
            Vector2 vecAccel, vecAccel2;

            if(fAge >= 0)
            {
                fAge += fDeltaTime;
                if(fAge >= info.fLifetime)
                    fAge = -2.0f;
            }

            // update all alive particles
            if (bUpdateBoundingBox)
                rectBoundingBox.Clear();

            for(i=0; i<nParticlesAlive; i++)
            {
                par=particles[i];

                par.fAge += fDeltaTime;

                //need to kill this particle
                if(par.fAge >= par.fTerminalAge)
                {
                    nParticlesAlive--;
                    particles[i] = new ParticlePrimitive (particles[nParticlesAlive]);
                    i--;
                    continue;
                }

                vecAccel = par.vecLocation-vecLocation;
                vecAccel.Normalise ();
                vecAccel2 = vecAccel;
                vecAccel *= par.fRadialAccel;

                // vecAccel2.Rotate(M_PI_2);
                // the following is faster
                ang = vecAccel2.X;
                vecAccel2.X = -vecAccel2.Y;
                vecAccel2.X = ang;

                vecAccel2 *= par.fTangentialAccel;
                par.vecVelocity += (vecAccel+vecAccel2)*fDeltaTime;
                par.vecVelocity.Y += par.fGravity*fDeltaTime;

                par.vecLocation += par.vecVelocity*fDeltaTime;

                par.fSpin += par.fSpinDelta*fDeltaTime;
                par.fSize += par.fSizeDelta*fDeltaTime;

                float factor = par.fAge / par.fTerminalAge;
                par.colColour = Rgba32.Lerp(par.colColourStart, par.colColourEnd, factor);
                //par.colColour = new Rgba32(par.colColour.ToVector4() + (par.colColourEnd.ToVector4() * fDeltaTime));
            }

            if (bUpdateBoundingBox)
                rectBoundingBox.Encapsulate(par.vecLocation.X, par.vecLocation.Y);

            // generate new particles

            if(fAge != -2.0f)
            {
                float fParticlesNeeded = ((float)info.nEmission)*fDeltaTime + fEmissionResidue;
                int nParticlesCreated = (int) fParticlesNeeded;
                fEmissionResidue=fParticlesNeeded-((float)nParticlesCreated);

                for(i=0; i<nParticlesCreated; i++)
                {
                    if(nParticlesAlive>=MAX_PARTICLES) break;

                    par = particles[nParticlesAlive];

                    par.fAge = 0.0f;
                    par.fTerminalAge = RandomGenerator.Default.GetRandomSingle(info.fParticleLifeMin, info.fParticleLifeMax);

                    par.vecLocation = vecPrevLocation + (vecLocation - vecPrevLocation) * RandomGenerator.Default.GetRandomSingle(0.0f, 1.0f);
                    par.vecLocation.X += RandomGenerator.Default.GetRandomSingle(-2.0f, 2.0f);
                    par.vecLocation.Y += RandomGenerator.Default.GetRandomSingle(-2.0f, 2.0f);

                    ang = info.fDirection - ((float)Math.PI / 2.0f) + RandomGenerator.Default.GetRandomSingle(0.0f, info.fSpread) - info.fSpread / 2.0f;

                    if(info.bRelative)
                        ang += (  (float) Math.Atan2( (vecPrevLocation-vecLocation).Y, (vecPrevLocation-vecLocation).X )    )+( (float)Math.PI / 2.0f );

                    par.vecVelocity.X = (float) Math.Cos(ang);
                    par.vecVelocity.Y = (float) Math.Sin(ang);
                    par.vecVelocity *= info.fSpeed;

                    par.fGravity = info.fGravity;
                    par.fRadialAccel = info.fRadialAccel;
                    par.fTangentialAccel = info.fTangentialAccel;

                    par.fSize = RandomGenerator.Default.GetRandomSingle(info.fSizeStart, info.fSizeStart + (info.fSizeEnd - info.fSizeStart) * info.fSizeVar);
                    par.fSizeDelta = (info.fSizeEnd-par.fSize) / par.fTerminalAge;

                    par.fSpin = RandomGenerator.Default.GetRandomSingle(info.fSpinStart, info.fSpinStart + (info.fSpinEnd - info.fSpinStart) * info.fSpinVar);
                    par.fSpinDelta = (info.fSpinEnd-par.fSpin) / par.fTerminalAge;


                    //Vector4 start = info.colColourStart.ToVector4();
                    //Vector4 finish = info.colColourEnd.ToVector4();

                    //Vector4 colColourV = new Vector4(
                    //    Euclid.RandomHelper.Random_Float(start.W, finish.W + (finish.W - start.W) * info.fColourEndVar),
                    //    Euclid.RandomHelper.Random_Float(start.X, start.X + (finish.X - start.X) * info.fColourStartVar),
                    //    Euclid.RandomHelper.Random_Float(start.Y, finish.Y + (finish.Y - start.Y) * info.fColourStartVar),
                    //    Euclid.RandomHelper.Random_Float(start.Z, finish.Z + (finish.Z - start.Z) * info.fColourStartVar)
                    //    );
                    //par.colColourStart = new Rgba32(colColourV);

                    //Vector4 colColourDeltaV = new Vector4();

                    //colColourDeltaV.W = (finish.W - start.W) / par.fTerminalAge;
                    //colColourDeltaV.X = (finish.X - start.X) / par.fTerminalAge;
                    //colColourDeltaV.Y = (finish.Y - start.Y) / par.fTerminalAge;
                    //colColourDeltaV.Z = (finish.Z - start.Z) / par.fTerminalAge;
                    //par.colColourEnd = new Rgba32(colColourDeltaV);

                    par.colColourStart = RandomGenerator.Default.GetRandomColourNearby(info.colColourStart, info.fColourStartVar);
                    par.colColourEnd = RandomGenerator.Default.GetRandomColourNearby(info.colColourEnd, info.fColourEndVar);

                    if (bUpdateBoundingBox)
                        rectBoundingBox.Encapsulate(par.vecLocation.X, par.vecLocation.Y);

                    nParticlesAlive++;
                }
            }

            vecPrevLocation=vecLocation;
        }

        public void MoveTo(float x, float y)
        {
            MoveTo(x, y, false);
        }

        public void MoveTo(float x, float y, bool bMoveParticles)
        {
            int i;
            float dx, dy;

            if (bMoveParticles)
            {
                dx = x - vecLocation.X;
                dy = y - vecLocation.Y;

                for (i = 0; i < nParticlesAlive; i++)
                {
                    particles[i].vecLocation.X += dx;
                    particles[i].vecLocation.Y += dy;
                }

                vecPrevLocation.X = vecPrevLocation.X + dx;
                vecPrevLocation.Y = vecPrevLocation.Y + dy;
            }
            else
            {
                if (fAge == -2.0) { vecPrevLocation.X = x; vecPrevLocation.Y = y; }
                else { vecPrevLocation.X = vecLocation.X; vecPrevLocation.Y = vecLocation.Y; }
            }

            vecLocation.X = x;
            vecLocation.Y = y;
        }

        public void Transpose(float x, float y)
        {
            fTx=x;
            fTy=y;
        }

        public void SetScale(float scale)
        {
            fScale = scale;
        }

        public int GetParticlesAlive()
        {
            return nParticlesAlive;
        }

        public float GetAge()
        {
            return fAge;
        }

        public void GetPosition(ref float x, ref float y)
        {
            x = vecLocation.X;
            y = vecLocation.Y;
        }

        public void GetTransposition(ref float x, ref float y)
        {
            x = fTx;
            y = fTy;
        }

        public float GetScale()
        {
            return fScale;
        }
    }
}
