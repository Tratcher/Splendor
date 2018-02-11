using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Splendor.Engine;

namespace Splendor
{
    public class ConsolePlayer : IPlayerControl
    {
        public ConsolePlayer(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public void PlayTurn(Game game)
        {
            ShowGame(game);
            Console.WriteLine($"{game.CurrentPlayer.Name}'s turn");
            Console.WriteLine(ShowPlayer(game.CurrentPlayer));
            Console.WriteLine("Select an action: ");
            Console.WriteLine("A) TakeDistinctGems");
            Console.WriteLine("B) TakeTwoGems");
            Console.WriteLine("C) ReserveCard");
            Console.WriteLine("D) ReserveSecret");
            Console.WriteLine("D) Purchase");

            Console.ReadKey();
            throw new NotImplementedException();
        }

        private static void ShowGame(Game game)
        {
            var players = game.Players;
            Console.WriteLine($"{players.Count} players:");
            foreach (var player in players)
            {
                Console.WriteLine(ShowPlayer(player));
            }

            var nobles = game.Board.Nobles;
            Console.WriteLine($"{nobles.Count} nobles available:");
            foreach (var noble in nobles)
            {
                Console.WriteLine(ShowNoble(noble));
            }

            var cards = game.Board.AvailableCards;
            Console.WriteLine($"{cards.Count} cards available:");
            foreach (var card in cards.OrderBy(card => card.Level))
            {
                Console.WriteLine(ShowCard(card));
            }

            var bank = game.Board.Bank;
            var builder = new StringBuilder($"Bank: ");
            foreach (var item in bank.Available)
            {
                builder.Append($"{item.Key}={item.Value}, ");
            }
            Console.WriteLine(builder.ToString());
        }

        private static string ShowPlayer(Player player)
        {
            var builder = new StringBuilder($"Name: {player.Name}, Points: {player.Points}\r\n");
            // TODO: Combine these cleaner
            builder.Append("Disks: ");
            foreach (var item in player.Disks)
            {
                builder.Append($"{item.Key}={item.Value}, ");
            }
            builder.Append("\r\nBonuses: ");
            foreach (var group in player.Cards.GroupBy(card => card.Bonus))
            {
                builder.Append($"{group.Key}={group.Count()}, ");
            }
            return builder.ToString();
        }

        private static string ShowCard(Card card)
        {
            var builder = new StringBuilder($"ID: {card.Id}, Level: {card.Level}, Points: {card.PointValue}, Bonus: {card.Bonus}, Cost: ");
            foreach (var item in card.Cost)
            {
                builder.Append($"{item.Key}={item.Value}, ");
            }
            return builder.ToString();
        }

        private static string ShowNoble(Noble noble)
        {
            var builder = new StringBuilder($"ID: {noble.Id}, Name: {noble.Name}, Points: {noble.PointValue}, Requirements: ");
            foreach (var item in noble.Requirements)
            {
                builder.Append($"{item.Key}={item.Value}, ");
            }
            return builder.ToString();
        }
    }
}
