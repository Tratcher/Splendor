using System;
using System.Collections.Generic;
using System.Text;

namespace Splendor.Engine
{
    public class Board
    {
        public IReadOnlyList<Noble> Nobles { get; }

        public Bank Bank { get; }

        // The contents of these decks are not public. Thier count or empty/non-empty status may be.
        internal IList<Card> Deck1 { get; }
        internal IList<Card> Deck2 { get; }
        internal IList<Card> Deck3 { get; }

        public IReadOnlyList<Card> AvailableCards { get; }

        // Costs to be paid in advance (unless reverving)
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
