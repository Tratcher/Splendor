using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splendor.Engine
{
    public class Player
    {
        private readonly Dictionary<GemType, int> _disks = new Dictionary<GemType, int>(6);
        private readonly List<Card> _cards = new List<Card>(20);
        private readonly List<Card> _reserve = new List<Card>(3);
        private readonly List<Noble> _nobles = new List<Noble>(3);

        internal Player(string playerName)
        {
            Name = playerName;
        }

        public string Name { get; }

        public IReadOnlyDictionary<GemType, int> Disks => _disks;

        public IReadOnlyList<Card> Cards => _cards;

        // Not really secret from other players, they watched you reserve each.
        public IReadOnlyList<Card> Reserve => _reserve;

        public IReadOnlyList<Noble> Nobles => _nobles;

        public int Points
        {
            get
            {
                var points = 0;
                if (Cards.Count > 0)
                {
                    points += Cards.Select(c => c.PointValue).Aggregate((a, b) => a + b);
                }
                if (Nobles.Count > 0)
                {
                    points += Nobles.Select(c => c.PointValue).Aggregate((a, b) => a + b);
                }
                return points;
            }
        }

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
