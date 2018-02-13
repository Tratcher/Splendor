using System;
using System.Collections.Generic;
using System.Linq;

namespace Splendor.Engine
{
    public class Board
    {
        private readonly List<Card> _availableCards = new List<Card>(12);
        private readonly List<Noble> _nobles;

        public Board(Bank bank, IReadOnlyList<Noble> nobles, IReadOnlyList<Card> cards)
        {
            Bank = bank;
            _nobles = nobles.ToList();
            Deck1 = new Stack<Card>(cards.Where(card => card.Level == 1));
            Deck2 = new Stack<Card>(cards.Where(card => card.Level == 2));
            Deck3 = new Stack<Card>(cards.Where(card => card.Level == 3));
            Decks = new List<Stack<Card>>() { Deck1, Deck2, Deck3 };

            for (var i = 0; i < 4; i++)
            {
                _availableCards.Add(Deck1.Pop());
                _availableCards.Add(Deck2.Pop());
                _availableCards.Add(Deck3.Pop());
            }
        }

        public IReadOnlyList<Noble> Nobles => _nobles;

        public Bank Bank { get; }

        // The contents of these decks are not public. Thier count or empty/non-empty status may be.
        private Stack<Card> Deck1 { get; }
        private Stack<Card> Deck2 { get; }
        private Stack<Card> Deck3 { get; }

        private IReadOnlyList<Stack<Card>> Decks { get; }

        public IReadOnlyList<Card> AvailableCards => _availableCards;

        private Stack<Card> GetDeck(int level) => Decks[level - 1];

        public bool LevelDeckIsEmpty(int level)
        {
            return GetDeck(level).Count == 0;
        }

        // Costs to be paid in advance (unless reserving)
        internal void TakeCard(Card card)
        {
            // Remove from Available cards
            var removed = _availableCards.Remove(card);
            // Verify available
            if (!removed) throw new InvalidOperationException($"Missing card {card.Id}");

            // Deal replacement
            var deck = GetDeck(card.Level);
            if (deck.Count > 0)
            {
                var replacement = deck.Pop();
                _availableCards.Add(replacement);
            }
        }

        // Reserve directly off the draw pile for a given level
        internal Card TakeSecret(int level)
        {
            // Verify level has card
            return GetDeck(level).Pop();
        }

        internal void TakeNoble(Noble noble)
        {
            // Remove from Nobles, no replacement
            var removed = _nobles.Remove(noble);
            // Verify available
            if (!removed) throw new InvalidOperationException($"Missing noble {noble.Id}");
        }
    }
}
