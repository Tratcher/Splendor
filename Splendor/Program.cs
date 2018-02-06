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

            var loader = new CardLoader();
            var cards = loader.LoadCards("../Splendor.Engine/Resources/Splendor_Card_Table.csv");
            Console.WriteLine($"Loaded {cards.Count} cards");
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
    }
}
