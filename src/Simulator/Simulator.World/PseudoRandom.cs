using System;
using System.Security.Cryptography;


namespace Simulator.World
{
    /// <summary>
    /// A fast thread-safe wrapper for the default pseudo-random number generator.
    /// </summary>
    public static class PseudoRandom
    {
        // Global seed generator
        private static Random global;

        // Thread-local pseudo-random generator
        [ThreadStatic]
        private static Random local;

        // Gets or initializes a thread-local Random instance.
        private static Random Local
        {
            get
            {
                Random inst = local;
                if (inst == null)
                {
                    int seed;
                    lock (global) seed = global.Next();
                    local = inst = new Random(seed);
                }
                return inst;
            }
        }

        static PseudoRandom()
        {
            global = new Random(Storage.CurrentWorld.Seed);
        }

        /// <summary>
        /// Returns a nonnegative random number.
        /// </summary>
        public static int Next()
        {
            return Local.Next();
        }

        /// <summary>
        /// Returns a nonnegative random number less than the specified maximum.
        /// </summary>
        public static int Next(int maxValue)
        {
            return Local.Next(maxValue);
        }

        /// <summary>
        /// Returns a random number within a specified range.
        /// </summary>
        public static int Next(int minValue, int maxValue)
        {
            return Local.Next(minValue, maxValue);
        }

        /// <summary>
        /// Returns a random double between 0.0 and 1.0.
        /// </summary>
        public static double NextDouble()
        {
            return Local.NextDouble();
        }


        /// <summary>
        /// Returns a nonnegative random number less than the specified maximum.
        /// </summary>
        public static double NextDouble(double maxValue)
        {
            return (maxValue * NextDouble());
        }

        /// <summary>
        /// Returns a random number within the specified range.
        /// </summary>
        public static double NextDouble(double minValue, double maxValue)
        {
            double magnitude = maxValue - minValue;
            return minValue + (magnitude * NextDouble());
        }
    }
}