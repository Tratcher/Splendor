using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splendor.Engine
{
    public class Board
    {
        private readonly List<Card> _availableCards = new List<Card>(12);

        public Board(Bank bank, IReadOnlyList<Noble> nobles, IReadOnlyList<Card> cards)
        {
            Bank = bank;
            Nobles = nobles;
            Deck1 = new Stack<Card>(cards.Where(card => card.Level == 1));
            Deck2 = new Stack<Card>(cards.Where(card => card.Level == 2));
            Deck3 = new Stack<Card>(cards.Where(card => card.Level == 3));

            for (var i = 0; i < 4; i++)
            {
                _availableCards.Add(Deck1.Pop());
                _availableCards.Add(Deck2.Pop());
                _availableCards.Add(Deck3.Pop());
            }
        }

        public IReadOnlyList<Noble> Nobles { get; }

        public Bank Bank { get; }

        // The contents of these decks are not public. Thier count or empty/non-empty status may be.
        internal Stack<Card> Deck1 { get; }
        internal Stack<Card> Deck2 { get; }
        internal Stack<Card> Deck3 { get; }

        public IReadOnlyList<Card> AvailableCards => _availableCards;

        // Costs to be paid in advance (unless reserving)
        internal void TakeCard(Card card)
        {
            // Verify available
            // Remove from Available cards, deal replacement
        }

        internal void TakeNoble(Noble noble)
        {
            // Verify available
            // Remove from Nobles, no replacement
        }
    }
}
