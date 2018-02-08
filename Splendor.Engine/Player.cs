using System;
using System.Collections.Generic;
using System.Text;

namespace Splendor.Engine
{
    public class Player
    {
        internal Player(string playerName)
        {
            Name = playerName;
        }

        public string Name { get; }

        public IReadOnlyDictionary<GemType, int> Disks { get; }

        public IReadOnlyList<Card> Cards { get; }
        
        // Not really secret from other players, they watched you reserve each.
        public IReadOnlyList<Card> Reserve { get; }

        public IReadOnlyList<Noble> Nobles { get; }

        internal void AddDisks(GemType type, int count)
        {
            // Limit 10
        }

        internal void RemoveDisks(GemType type, int count)
        {

        }

        internal void AddCard(Card card)
        {

        }

        internal void AddNoble(Noble noble)
        {

        }

        internal void AddReserve(Card card)
        {
            // Limit 3
        }

        // Remove from reserve and add to cards. Costs are paid separately.
        internal void TransferFromReserve(Card card)
        {

        }
    }
}
