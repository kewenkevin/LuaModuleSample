// Copyright 2020 Yoozoo Net Inc.
// UMT Framework and corresponding source code is free
// software: you can redistribute it and/or modify it under the terms of
// the GNU General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
// 
// UMT Framework and corresponding source code is distributed
// in the hope that it will be useful, but with permitted additional restrictions
// under Section 7 of the GPL. See the GNU General Public License in LICENSE.TXT
// distributed with this program. You should have received a copy of the
// GNU General Public License along with permitted additional restrictions
// with this program. If not, see https://gitlab.uuzu.com/yoozooopensource/client/framework/core
// 
// ***********************************************************************************************
// ***                  C O N F I D E N T I A L  ---  U M T   T E A M                          ***
// ***********************************************************************************************
// 
//     Project Name        :        UMT Framework Core Library
// 
//     File Name           :        XRandom.cs
// 
//     Programmer          :        Wei Wei (Battle Mage Gandalf)
// 
//     Start Date          :        04/12/2021
// 
//     Last Update         :        04/12/2021 15:55 [Wei]
// 
//     Description         :        write here
// 
// =============================================================================================
// Contributors:
// ---------------------------------------------------------------------------------------------
// Battle Mage Gandalf                 wwei@yoozoo.com             Product technology Center
// =============================================================================================
 using System;

namespace ND.Core
{
    #region For Java Support

    /* //java support
    public class XRandom
    {
        private long seed;

        private static final long multiplier = 0x5DEECE66DL;
        private static final long addend = 0xBL;
        private static final long mask = (1L << 48) - 1;

        private static final double DOUBLE_UNIT = 1.0 / (1L << 53);

        static final String BadBound = "bound must be positive";

        public XRandom()
        {
            this(seedUniquifier() ^ System.nanoTime());
        }

        private static long seedUniquifier()
        {
            for (;;)
            {
                long current = _seedUniquifier;
                long next = current * 181783497276652981L;
                if (_seedUniquifier == current)
                {
                    _seedUniquifier = next;
                    return next;
                }
            }
        }

        private static long _seedUniquifier = 8682522807148012L;

        public XRandom(long seed)
        {
            if (getClass() == XRandom.class)
            this.seed = initialScramble(seed);
            else
            {
                this.seed = 0L;
                setSeed(seed);
            }
        }

        private static long initialScramble(long seed)
        {
            return (seed ^ multiplier) & mask;
        }

        synchronized public void setSeed(long seed)
        {
            this.seed = initialScramble(seed);
            haveNextNextGaussian = false;
        }

        protected int next(int bits)
        {
            long oldseed, nextseed;
            boolean isSeedEqual = false;
            do
            {
                oldseed = seed;
                nextseed = (oldseed * multiplier + addend) & mask;
                if (seed == oldseed)
                {
                    seed = nextseed;
                    isSeedEqual = true;
                }
            } while (!isSeedEqual);

            return (int) (nextseed >> > (48 - bits));
        }

        public void nextBytes(byte[] bytes)
        {
            for (int i = 0, len = bytes.length; i < len;)
            for (int rnd = nextInt(),
                n = Math.min(len - i, Integer.SIZE
                                      / Byte.SIZE);
                n-- > 0;
                rnd >>= Byte.SIZE)
                bytes[i++] = (byte) rnd;
        }

        public int nextInt()
        {
            return next(32);
        }

        public int nextInt(int bound)
        {
            if (bound <= 0)
                throw new IllegalArgumentException(BadBound);

            int r = next(31);
            int m = bound - 1;
            if ((bound & m) == 0)
                r = (int) ((bound * (long) r) >> 31);
            else
            {
                for (int u = r; u - (r = u % bound) + m < 0; u = next(31))
                    ;
            }

            return r;
        }

        public long nextLong()
        {
            return ((long) (next(32)) << 32) + next(32);
        }

        public boolean nextBoolean()
        {
            return next(1) != 0;
        }

        public float nextFloat()
        {
            return next(24) / ((float) (1 << 24));
        }

        public double nextDouble()
        {
            return (((long) (next(26)) << 27) + next(27)) * DOUBLE_UNIT;
        }

        private double nextNextGaussian;
        private boolean haveNextNextGaussian = false;

        synchronized public double nextGaussian()
        {
            if (haveNextNextGaussian)
            {
                haveNextNextGaussian = false;
                return nextNextGaussian;
            }
            else
            {
                double v1, v2, s;
                do
                {
                    v1 = 2 * nextDouble() - 1;
                    v2 = 2 * nextDouble() - 1;
                    s = v1 * v1 + v2 * v2;
                } while (s >= 1 || s == 0);

                double multiplier = StrictMath.sqrt(-2 * StrictMath.log(s) / s);
                nextNextGaussian = v2 * multiplier;
                haveNextNextGaussian = true;
                return v1 * multiplier;
            }
        }
    }
    */

    #endregion

    public class XRandom
    {
        private const double DoubleUnit = 1.0 / (1L << 53);
        private const String BadBound = "bound must be positive";

        private const long Multiplier = 0x5DEECE66DL;
        private const long Addend = 0xBL;
        private const long Mask = (1L << 48) - 1;


        private static long _seedUniquifier = 8682522807148012L;
        private long _seed;
        
        private double nextNextGaussian;
        private bool haveNextNextGaussian = false;

        public static long nanoTime()
        {
            return DateTime.Now.Ticks * 100L;
        }

        public XRandom() : this(seedUniquifier() ^ nanoTime())
        {
        }

        private static long seedUniquifier()
        {
            for (;;)
            {
                long current = _seedUniquifier;
                long next = current * 181783497276652981L;
                if (_seedUniquifier == current)
                {
                    _seedUniquifier = next;
                    return next;
                }
            }
        }


        public XRandom(long seed)
        {
            if (GetType() == typeof(XRandom))
                this._seed = initialScramble(seed);
            else
            {
                this._seed = 0L;
                Seed = seed;
            }
        }

        private static long initialScramble(long seed)
        {
            return (seed ^ Multiplier) & Mask;
        }


        public long Seed
        {
            get
            {
                lock (this)
                {
                    return _seed;
                }
            }
            set
            {
                lock (this)
                {
                    _seed = initialScramble(value);
                    haveNextNextGaussian = false;
                }
            }
        }
        
        protected int Next(int bits)
        {
            long oldseed, nextseed;
            bool isSeedEqual = false;
            do
            {
                oldseed = _seed;
                nextseed = (oldseed * Multiplier + Addend) & Mask;
                if (_seed == oldseed)
                {
                    _seed = nextseed;
                    isSeedEqual = true;
                }
            } while (!isSeedEqual);

            return (int) (moveToFill0(nextseed, 48 - bits));
        }


        public void NextBytes(byte[] bytes)
        {
            for (int i = 0, len = bytes.Length; i < len;)
            for (int rnd = NextInt(),
                n = Math.Min(len - i, 32 / 8);
                n-- > 0;
                rnd >>= 8)
                bytes[i++] = (byte) rnd;
        }

        public int NextInt()
        {
            return Next(32);
        }

        public int NextInt(int bound)
        {
            if (bound <= 0)
                throw new ArgumentException(BadBound);

            int r = Next(31);
            int m = bound - 1;
            if ((bound & m) == 0)
                r = (int) ((bound * (long) r) >> 31);
            else
            {
                for (int u = r; u - (r = u % bound) + m < 0; u = Next(31)) ;
            }

            return r;
        }

        public long NextLong()
        {
            return ((long) (Next(32)) << 32) + Next(32);
        }

        public bool NextBoolean()
        {
            return Next(1) != 0;
        }

        public float NextFloat()
        {
            return Next(24) / ((float) (1 << 24));
        }

        public double NextDouble()
        {
            return (((long) (Next(26)) << 27) + Next(27)) * DoubleUnit;
        }

        

        public double NextGaussian()
        {
            lock (this)
            {
                if (haveNextNextGaussian)
                {
                    haveNextNextGaussian = false;
                    return nextNextGaussian;
                }
                else
                {
                    double v1, v2, s;
                    do
                    {
                        v1 = 2 * NextDouble() - 1;
                        v2 = 2 * NextDouble() - 1;
                        s = v1 * v1 + v2 * v2;
                    } while (s >= 1 || s == 0);

                    double multiplier = Math.Sqrt(-2 * Math.Log(s) / s);
                    nextNextGaussian = v2 * multiplier;
                    haveNextNextGaussian = true;
                    return v1 * multiplier;
                }
            }
        }

        public static long moveToFill0(long value, int bits)
        {
            long mask = long.MaxValue;
            for (int i = 0; i < bits; i++)
            {
                value >>= 1;
                value &= mask;
            }

            return value;
        }


        public XRandom Clone()
        {
            XRandom random = new XRandom();
            random._seed = _seed;
            random.nextNextGaussian = nextNextGaussian;
            random.haveNextNextGaussian = haveNextNextGaussian;
            return random;
        }
    }
}