using System;
using System.Collections.Generic;
using System.Text;

namespace Splendor.Engine
{
    public class Bank
    {
        private Dictionary<GemType, int> _available = new Dictionary<GemType, int>(6);

        public Bank(int gold, int otherGems)
        {
            var limits = new Dictionary<GemType, int>(5);
            limits[GemType.Gold] = gold;
            limits[GemType.Diamond] = otherGems;
            limits[GemType.Emerald] = otherGems;
            limits[GemType.Onyx] = otherGems;
            limits[GemType.Ruby] = otherGems;
            limits[GemType.Sapphire] = otherGems;
            Limits = limits;

            foreach (var pair in Limits)
            {
                _available.Add(pair.Key, pair.Value);
            }
        }

        public IReadOnlyDictionary<GemType, int> Available => _available;

        public IReadOnlyDictionary<GemType, int> Limits { get; }

        internal void Take(GemType type, int count)
        {
            // Availability check. Limit one at a time, or two if the stack is less than 2
        }

        internal void Return(GemType type, int count)
        {
            // Limit check
        }
    }
}
