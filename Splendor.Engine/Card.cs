using System;
using System.Collections.Generic;
using System.Text;

namespace Splendor.Engine
{
    public class Card
    {
        internal Card(string id, int level, IReadOnlyDictionary<GemType, int> cost, GemType bonus, int pointValue)
        {
            Id = id;
            Level = level;
            Cost = cost;
            Bonus = bonus;
            PointValue = pointValue;
        }

        public string Id { get; }

        public int Level { get; }

        public IReadOnlyDictionary<GemType, int> Cost { get; }

        public GemType Bonus { get; }

        public int PointValue { get; }
    }
}
