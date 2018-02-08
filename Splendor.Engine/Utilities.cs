using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splendor.Engine
{
    internal static class Utilities
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
    }
}
