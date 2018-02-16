using System;
using System.Collections.Generic;
using System.Text;
using Splendor.Engine;

namespace Splendor
{
    public interface IPlayerControl
    {
        string Name { get; }
        int Wins { get; set; }
        void PlayTurn(Game game);
    }
}
