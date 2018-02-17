using System;
using System.Collections.Generic;
using System.Linq;
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
            Card best = null;
            var bestCost = 0;
            // Purchase from available or reserve
            foreach (var card in game.Board.AvailableCards.Concat(game.CurrentPlayer.Reserve))
            {
                if (Utilities.CanAfford(card.Cost, game.CurrentPlayer.TotalGems))
                {
                    // More points?
                    if (best == null || card.PointValue > best.PointValue)
                    {
                        best = card;
                        bestCost = Utilities.TotalRemainingCost(card.Cost, game.CurrentPlayer.Bonuses);
                    }
                    // Or consumes fewer disks?
                    else if (card.PointValue == best.PointValue)
                    {
                        var diskCost = Utilities.TotalRemainingCost(card.Cost, game.CurrentPlayer.Bonuses);
                        if (diskCost < bestCost)
                        {
                            best = card;
                            bestCost = diskCost;
                        }
                    }
                }
            }

            if (best != null)
            {
                // TODO: Noble
                game.Purchase(best.Id);
                return;
            }

            // If I'm 1 disk away from anything on the board, reserve it.
            if (game.CurrentPlayer.Reserve.Count < 3)
            {
                foreach (var card in game.Board.AvailableCards)
                {
                    if (Utilities.TotalRemainingCost(card.Cost, game.CurrentPlayer.TotalGems) == 1)
                    {
                        // More points?
                        if (best == null || card.PointValue > best.PointValue)
                        {
                            best = card;
                        }
                    }
                }

                if (best != null)
                {
                    // TODO: Discard. Be careful not to discard anything needed for this card.
                    var discard = DiscardSingle(game, best);

                    game.ReserveCard(best.Id, discard);
                    return;
                }
            }

            // Can't afford anything. Take disks.
            // TODO: These are static, check if we can draw disks that will progress us towards anything.
            // Find the card that requires the fewest additional disks, if they're available.

            // Draw 3 unique
            // TODO: Modified random
            var types = Utilities.SelectableDisks.Where(d => game.Board.Bank.Available[d] > 0).ToList();
            while (types.Count > 3)
            {
                types.RemoveAt(Utilities.Random.Next(types.Count));
            }
            if (types.Count == 3)
            {
                var discards = Discard(game, types.Count);
                game.TakeDistinctGems(types, discards);
                return;
            }

            // Draw 2 of a kind
            // TODO: Modified random
            types = Utilities.SelectableDisks.Where(d => game.Board.Bank.Available[d] >= 4).ToList();
            if (types.Count > 0)
            {
                var type = types[Utilities.Random.Next(types.Count)];
                var discards = Discard(game, 2);
                game.TakeTwoGems(type, discards);
                return;
            }

            // Draw less than 3 unique
            types = Utilities.SelectableDisks.Where(d => game.Board.Bank.Available[d] > 0).ToList();
            while (types.Count > 3)
            {
                types.RemoveAt(Utilities.Random.Next(types.Count));
            }
            if (types.Count > 0)
            {
                var discards = Discard(game, types.Count);
                game.TakeDistinctGems(types, discards);
                return;
            }

            // No disks? Reserve something?
            if (game.CurrentPlayer.Reserve.Count < 3)
            {
                foreach (var card in game.Board.AvailableCards)
                {
                    var remainingCost = Utilities.TotalRemainingCost(card.Cost, game.CurrentPlayer.TotalGems);
                    // Cost less?
                    if (best == null || remainingCost < bestCost)
                    {
                        best = card;
                        remainingCost = bestCost;
                    }
                    else if (remainingCost == bestCost && best.PointValue < card.PointValue)
                    {
                        best = card;
                    }
                }

                if (best != null)
                {
                    // TODO: Discard. Be careful not to discard anything needed for this card.
                    var discard = DiscardSingle(game, best);

                    game.ReserveCard(best.Id, discard);
                    return;
                }
            }

            // Note this never reserves something from the draw piles

            // Pass
            game.TakeDistinctGems(new GemType[0], null);
        }

        // TODO Copied from Random. Be careful not to discard anything needed for this card.
        private GemType? DiscardSingle(Game game, Card best)
        {
            if (game.CurrentPlayer.TotalDisks < 10 || game.Board.Bank.Available[GemType.Gold] == 0)
            {
                return null;
            }

            return GetDiskAtOffset(game.CurrentPlayer.Disks, Utilities.Random.Next(10));
        }

        // TODO Copied from Random. Avoid discarding items needed for reserve cards
        private IEnumerable<KeyValuePair<GemType, int>> Discard(Game game, int added)
        {
            var toDiscard = game.CurrentPlayer.TotalDisks + added - 10;
            if (toDiscard <= 0)
            {
                return null;
            }

            // Expect 1-3 discards.
            var offset1 = Utilities.Random.Next(game.CurrentPlayer.TotalDisks);
            var type = GetDiskAtOffset(game.CurrentPlayer.Disks, offset1);

            if (toDiscard == 1)
            {
                return new[] { new KeyValuePair<GemType, int>(type, 1) };
            }

            var discards = new List<GemType>(toDiscard);
            discards.Add(type);

            var offset2 = Utilities.Random.Next(game.CurrentPlayer.TotalDisks);
            while (offset2 == offset1)
            {
                offset2 = Utilities.Random.Next(game.CurrentPlayer.TotalDisks);
            }
            type = GetDiskAtOffset(game.CurrentPlayer.Disks, offset2);
            discards.Add(type);

            if (toDiscard == 3)
            {
                var offset3 = Utilities.Random.Next(game.CurrentPlayer.TotalDisks);

                while (offset3 == offset1 || offset3 == offset2)
                {
                    offset3 = Utilities.Random.Next(game.CurrentPlayer.TotalDisks);
                }
                type = GetDiskAtOffset(game.CurrentPlayer.Disks, offset3);
                discards.Add(type);
            }

            return discards.GroupBy(g => g).Select(group => new KeyValuePair<GemType, int>(group.Key, group.Count()));
        }

        // Count through the disk types in a determanistic order.
        private static GemType GetDiskAtOffset(IReadOnlyDictionary<GemType, int> disks, int offset)
        {
            var i = disks[GemType.Diamond];
            if (i > offset) return GemType.Diamond;
            i += disks[GemType.Emerald];
            if (i > offset) return GemType.Emerald;
            i += disks[GemType.Gold];
            if (i > offset) return GemType.Gold;
            i += disks[GemType.Onyx];
            if (i > offset) return GemType.Onyx;
            i += disks[GemType.Ruby];
            if (i > offset) return GemType.Ruby;
            i += disks[GemType.Sapphire];
            if (i > offset) return GemType.Sapphire;

            throw new NotImplementedException("Bad random?");
        }
    }
}
