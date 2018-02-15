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
                if (game.Board.Bank.Available[type] > 1)
                {
                    actions.Add(TakeThree);
                    break;
                }
            }

            // Select one. Is it possible for this list to be empty?
            if (actions.Count == 0)
            {
                throw new InvalidOperationException("No available actions");
            }
            var action = actions[Utilities.Random.Next(actions.Count)];
            action(game);
        }

        private void TakeTwo(Game game)
        {
            var stacks = Utilities.SelectableDisks.Where(d => game.Board.Bank.Available[d] >= 4).ToList();
            var type = stacks[Utilities.Random.Next(stacks.Count)];

            // TODO: Discards
            game.TakeTwoGems(type, null);
        }

        private void TakeThree(Game game)
        {
            var stacks = Utilities.SelectableDisks.Where(d => game.Board.Bank.Available[d] > 0).ToList();
            while (stacks.Count > 3)
            {
                stacks.RemoveAt(Utilities.Random.Next(stacks.Count));
            }

            // TODO: discards
            game.TakeDistinctGems(stacks, null);
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

            // TODO: discard
            game.ReserveCard(card.Id, null);
        }

        // Assumes there will always be cards available in at least one deck
        private void ReserveSecret(Game game)
        {
            var level = Utilities.Random.Next(1, 4);
            while (game.Board.CheckLevelDeckIsEmpty(level))
            {
                level = Utilities.Random.Next(1, 4);
            }

            // TODO: discard
            game.ReserveSecret(level, null);
        }
    }
}
