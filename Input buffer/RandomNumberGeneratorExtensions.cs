using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExtensionMethods
{
    /// <summary>
    /// Extension methods for the RandomNumberGenerator class.
    /// </summary>
    static class RandomNumberGeneratorExtensions
    {
        /// <summary>
        /// Executes one of the given actions based on the given probabilities.
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
            this RandomNumberGenerator rng, List<(ushort Probability, Action Action)> outcomes)
        {
            int sumOfProbabilities = 0;
            foreach ((ushort Probability, Action) outcome in outcomes)
            {
                sumOfProbabilities += outcome.Probability;
            }
            int choice = rng.RandiRange(1, sumOfProbabilities);
            uint accumulator = 0;

            foreach ((uint Probability, Action Action) outcome in outcomes)
            {
                accumulator += outcome.Probability;
                if (accumulator >= choice)
                {
                    outcome.Action();
                    break;
                }
            }
        }

        /// <summary>
        /// Returns a value based on the given probabilities.
        /// </summary>
        /// <param name="rng"> The object to call this method for. </param>
        /// <param name="outcomes"> 
        /// List of Tuples containing the relative probability of returning each value along with the value itself. 
        /// See the description for the RandomAction method for more info.
        /// </param>
        /// <typeparam name="T"> The type of the values given and the value to return. </typeparam>
        /// <returns> Random value according to the outcomes parameter. </returns>
        public static T RandomValue<T>(this RandomNumberGenerator rng, List<(ushort Probability, T Value)> outcomes)
        {
            /* 
            I made this method after realizing that RandomAction was overkill for its intended use case (i.e. selecting 
            values!), but this one uses RandomAction internally so it works out pretty well.
            */
            T selectedValue = default(T);
            var actionOutcomes = new List<(ushort Probability, Action Action)>();
            foreach ((ushort Probability, T Value) outcome in outcomes)
            {
                actionOutcomes.Add((outcome.Probability, () => selectedValue = outcome.Value));
            }
            RandomAction(rng, actionOutcomes);
            return selectedValue;
        }
    }
}
