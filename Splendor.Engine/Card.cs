using System;
using System.Collections.Generic;
using System.Text;

namespace Splendor.Engine
{
    public class Card
    {
        public string Id { get; }

        public int Level { get; }

        public IReadOnlyDictionary<GemType, int> Cost { get; }

        public GemType Bonus { get; }

        public int PointValue { get; }
    }
}
