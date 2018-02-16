using System;
using System.Collections.Generic;
using System.Text;
using Splendor.Engine;

namespace Splendor
{
    // Short sighted. Buy if you can, take disks if you can't, reserve if you must.
    public class GreedyPlayer : IPlayerControl
    {
        public GreedyPlayer(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public int Wins { get; set; }

        public void PlayTurn(Game game)
        {
            throw new NotImplementedException();
        }
    }
}
