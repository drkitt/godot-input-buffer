using Godot;
using System;

namespace ExtensionMethods
{
    /// <summary>
    /// Extension methods for the RandomNumberGenerator class.
    /// </summary>
    public static class RandomNumberGeneratorExtensions
    {
        /// Generates a pseudo-random 32-bit boolean.
        /// </summary>
        /// <param name="rng"> The object to call this method for. </param>
        /// <returns> True or False, each with equal probability. </returns>
        public static bool Randb(this RandomNumberGenerator rng)
        {
            return rng.RandiRange(0, 1) == 1;
        }
    }
}