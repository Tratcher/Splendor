using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Splendor.Engine;

namespace Splendor
{
    public class RandomPlayer : IPlayerControl
    {
        public RandomPlayer(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public int Wins { get; set; }

        public void PlayTurn(Game game)
        {
            // Generate a list of valid actions and randomly select one.
            var actions = new List<Action<Game>>();

            // Purchase from available or reserve
            foreach (var card in game.Board.AvailableCards.Concat(game.CurrentPlayer.Reserve))
            {
                if (Utilities.CanAfford(card.Cost, game.CurrentPlayer.TotalGems))
                {
                    actions.Add(Purchase(card));
                }
            }

            // Reserve public or secret
            if (game.CurrentPlayer.Reserve.Count < 3)
            {
                actions.Add(Reserve);
                actions.Add(ReserveSecret);
            }

            // Draw 2 of a kind
            foreach (var type in Utilities.SelectableDisks)
            {
                if (game.Board.Bank.Available[type] >= 4)
                {
                    actions.Add(TakeTwo);
                    break;
                }
            }

            // Draw 3 unique
            foreach (var type in Utilities.SelectableDisks)
            {
                if (game.Board.Bank.Available[type] >= 1)
                {
                    actions.Add(TakeThree);
                    break;
                }
            }

            // Select one. Is it possible for this list to be empty?
            if (actions.Count == 0)
            {
                // throw new InvalidOperationException("No available actions");
                // Pass, take no disks
                game.TakeDistinctGems(new GemType[0], null);
                return;
            }
            var action = actions[Utilities.Random.Next(actions.Count)];
            action(game);
        }

        private void TakeTwo(Game game)
        {
            var stacks = Utilities.SelectableDisks.Where(d => game.Board.Bank.Available[d] >= 4).ToList();
            var type = stacks[Utilities.Random.Next(stacks.Count)];
            
            game.TakeTwoGems(type, Discard(game, 2));
        }

        private void TakeThree(Game game)
        {
            var stacks = Utilities.SelectableDisks.Where(d => game.Board.Bank.Available[d] > 0).ToList();
            while (stacks.Count > 3)
            {
                stacks.RemoveAt(Utilities.Random.Next(stacks.Count));
            }
            
            game.TakeDistinctGems(stacks, Discard(game, stacks.Count));
        }

        private Action<Game> Purchase(Card card)
        {
            return game =>
            {
                // TODO: Noble
                game.Purchase(card.Id);
            };
        }

        // Assumes there will always be cards available
        private void Reserve(Game game)
        {
            var card = game.Board.AvailableCards[Utilities.Random.Next(game.Board.AvailableCards.Count)];

            var discard = DiscardSingle(game);
            game.ReserveCard(card.Id, discard);
        }

        // Assumes there will always be cards available in at least one deck
        private void ReserveSecret(Game game)
        {
            var level = Utilities.Random.Next(1, 4);
            while (game.Board.CheckLevelDeckIsEmpty(level))
            {
                level = Utilities.Random.Next(1, 4);
            }

            var discard = DiscardSingle(game);
            game.ReserveSecret(level, discard);
        }

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

        // Discard zero or 1, of gold is available to take, and if we're at exactly 10
        private GemType? DiscardSingle(Game game)
        {
            if (game.CurrentPlayer.TotalDisks < 10 || game.Board.Bank.Available[GemType.Gold] == 0)
            {
                return null;
            }

            return GetDiskAtOffset(game.CurrentPlayer.Disks, Utilities.Random.Next(10));
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
