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
            // Availability check. Limit two at a time, and only if there's at least 4
            if (count > _available[type])
            {
                throw new InvalidOperationException($"Insuficient {type} disks");
            }
            if (count > 2)
            {
                throw new InvalidOperationException("Can't take more than two at a time.");
            }
            if (count > 1 && _available[type] < 4)
            {
                throw new InvalidOperationException("There must be at least 4 to take 2.");
            }

            _available[type] = _available[type] - count;
        }

        internal void Return(GemType type, int count)
        {
            // Limit check

            _available[type] = _available[type] + count;
        }
    }
}
