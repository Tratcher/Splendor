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

            var loader = new ResourceLoader();
            var cards = loader.LoadCards();
            Console.WriteLine($"Loaded {cards.Count} cards");
            foreach (var card in cards)
            {
                Console.WriteLine(ShowCard(card));
            }

            var nobles = loader.LoadNobles();
            Console.WriteLine($"Loaded {nobles.Count} nobles");
            foreach (var noble in nobles)
            {
                Console.WriteLine(ShowNoble(noble));
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
