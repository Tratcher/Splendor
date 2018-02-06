using System;
using System.Collections.Generic;
using System.Text;

namespace Splendor.Engine
{
    public class Noble
    {
        public string Name { get; }

        // TODO: Cities have more complex requirements. E.g. 6 of an unspecified type. Specify with Gold/Wild?
        public IReadOnlyDictionary<GemType, int> Requirements { get; }

        public int PointValue { get; }
    }
}
