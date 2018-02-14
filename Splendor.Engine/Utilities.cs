using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splendor.Engine
{
    public static class Utilities
    {
        private static readonly Random _random = new Random();

        public static IReadOnlyList<T> Shuffle<T>(this IEnumerable<T> input)
        {
            var array = input.ToArray();

            for (int i = 0; i < array.Length; i++)
            {
                var temp = array[i];
                var dest = _random.Next(array.Length);
                array[i] = array[dest];
                array[dest] = temp;
            }

            return array;
        }

        public static bool RequirementsMet(IReadOnlyDictionary<GemType, int> requirements, IReadOnlyDictionary<GemType, int> bonuses)
        {
            foreach (var requirement in requirements)
            {
                if (requirement.Value - bonuses[requirement.Key] > 0)
                {
                    return false;
                }
            }

            return true;
        }

        // Will I have met the requirements after purchasing the additional bonus card?
        public static bool WillRequirementsBeMet(IReadOnlyDictionary<GemType, int> requirements, IReadOnlyDictionary<GemType, int> bonuses,
            GemType purchase)
        {
            foreach (var requirement in requirements)
            {
                var diff = requirement.Value - bonuses[requirement.Key];
                if (requirement.Key == purchase)
                {
                    diff--;
                }

                if (diff > 0)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool CanAfford(IReadOnlyDictionary<GemType, int> costs, IReadOnlyDictionary<GemType, int> available)
        {
            var gold = available[GemType.Gold];
            foreach (var cost in costs)
            {
                var diff = cost.Value - available[cost.Key];
                if (diff > 0)
                {
                    gold -= diff;
                    if (gold < 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
