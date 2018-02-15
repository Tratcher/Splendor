using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Splendor.Engine
{
    public class Player
    {
        private readonly Dictionary<GemType, int> _disks = new Dictionary<GemType, int>(6)
        {
            { GemType.Gold, 0 },
            { GemType.Diamond, 0 },
            { GemType.Emerald, 0 },
            { GemType.Onyx, 0 },
            { GemType.Ruby, 0 },
            { GemType.Sapphire, 0 },
        };
        private readonly Dictionary<GemType, int> _bonses = new Dictionary<GemType, int>(5)
        {
            { GemType.Diamond, 0 },
            { GemType.Emerald, 0 },
            { GemType.Onyx, 0 },
            { GemType.Ruby, 0 },
            { GemType.Sapphire, 0 },
        };
        private readonly Dictionary<GemType, int> _totalGems = new Dictionary<GemType, int>(6)
        {
            { GemType.Gold, 0 },
            { GemType.Diamond, 0 },
            { GemType.Emerald, 0 },
            { GemType.Onyx, 0 },
            { GemType.Ruby, 0 },
            { GemType.Sapphire, 0 },
        };
        private readonly List<Card> _cards = new List<Card>(20);
        private readonly List<Card> _reserve = new List<Card>(3);
        private readonly List<Noble> _nobles = new List<Noble>(3);

        internal Player(string playerName)
        {
            Name = playerName;
        }

        public string Name { get; }

        public IReadOnlyDictionary<GemType, int> Disks => _disks;

        public int TotalDisks { get; private set; }

        public IReadOnlyDictionary<GemType, int> Bonuses => _bonses;

        public IReadOnlyDictionary<GemType, int> TotalGems => _totalGems;

        public IReadOnlyList<Card> Cards => _cards;

        // Not really secret from other players, they watched you reserve each.
        // Except for the random draws?
        public IReadOnlyList<Card> Reserve => _reserve;

        public IReadOnlyList<Noble> Nobles => _nobles;

        public int Points { get; private set; }

        internal void AddDisks(GemType type, int count)
        {
            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, "");
            }
            // Limit 10 total
            if (count + TotalDisks > 10)
            {
                throw new InvalidOperationException("The player cannot hold more than 10 disks.");
            }
            TotalDisks += count;
            _disks[type] = _disks[type] + count;
            _totalGems[type] = _totalGems[type] + count;
        }

        internal void RemoveDisks(GemType type, int count)
        {
            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, "");
            }
            if (count > _disks[type])
            {
                throw new InvalidOperationException("Attempting to remove more discs than preasent");
            }
            TotalDisks -= count;
            _disks[type] = _disks[type] - count;
            _totalGems[type] = _totalGems[type] - count;
        }

        internal void AddCard(Card card)
        {
            Points += card.PointValue;
            _bonses[card.Bonus] = _bonses[card.Bonus] + 1;
            _totalGems[card.Bonus] = _totalGems[card.Bonus] + 1;
            _cards.Add(card);
        }

        internal void AddNoble(Noble noble)
        {
            Points += noble.PointValue;
            _nobles.Add(noble);
        }

        internal void AddReserve(Card card)
        {
            // Limit 3
            if (_reserve.Count >= 3)
            {
                throw new InvalidOperationException("The reserve is already full.");
            }
            _reserve.Add(card);
        }

        // Remove from reserve and add to cards. Costs are paid separately.
        internal void TransferFromReserve(Card card)
        {
            if (!_reserve.Remove(card))
            {
                throw new InvalidOperationException($"Reserve card {card.Id} not found");
            }

            AddCard(card);
        }
    }
}
