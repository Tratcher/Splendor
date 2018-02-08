using System;
using System.Text;
using Splendor.Engine;

namespace Splendor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var game = new Game(new[] { "Player 1", "Player 2" });

            var nobles = game.Board.Nobles;
            Console.WriteLine($"{nobles.Count} nobles available:");
            foreach (var noble in nobles)
            {
                Console.WriteLine(ShowNoble(noble));
            }

            var cards = game.Board.AvailableCards;
            Console.WriteLine($"{cards.Count} cards available:");
            foreach (var card in cards)
            {
                Console.WriteLine(ShowCard(card));
            }
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
