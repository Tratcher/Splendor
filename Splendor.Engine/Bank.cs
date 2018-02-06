using System;
using System.Collections.Generic;
using System.Text;

namespace Splendor.Engine
{
    public class Bank
    {
        public IReadOnlyDictionary<GemType, int> Available { get; }

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
