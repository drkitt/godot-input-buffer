using Godot;
using System;
using System.Collections.Generic;

namespace ExtensionMethods
{
    /// <summary>
    /// Extension methods for the RandomNumberGenerator class.
    /// </summary>
    static class RandomNumberGeneratorExtensions
    {
        /// <summary>
        /// Executes one of the given actions based on the given properties.
        /// </summary>
        /// <param name="rng"> The object to call this method for. </param>
        /// <param name="probabiltiesAndActions"> 
        /// List of Tuples containing the relative probability of each action being called along with the action itself.
        /// For any entry (uint p, Action a), the probability of a being called is p / (sum of all p in dictionary).
        /// </param>
        /// I chose to use ushort instead of int in order to prevent some idiot 'programmer' from the future (me) from 
        /// accidentally including an outcome with negative probability (which would mess up this algorithm) while also
        /// avoiding the need for a potentially dangerous cast.
        public static void RandomAction(
            this RandomNumberGenerator rng, List<(ushort probability, Action action)> outcomes)
        {
            int sumOfProbabilities = 0;
            foreach ((ushort probability, Action) outcome in outcomes)
            {
                sumOfProbabilities += outcome.probability;
            }
            int choice = rng.RandiRange(1, sumOfProbabilities);
            uint accumulator = 0;

            foreach ((uint probability, Action action) outcome in outcomes)
            {
                accumulator += outcome.probability;
                if (accumulator >= choice)
                {
                    outcome.action();
                    break;
                }
            }
        }
    }
}
