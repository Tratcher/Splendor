using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Splendor.Engine;

namespace Splendor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Splendor");

            string playerCountInput;
            int playerCount;
            do
            {
                Console.Write("How many players? (2-4) ");
                playerCountInput = Console.ReadLine();
            }
            while (!int.TryParse(playerCountInput.Trim(), out playerCount));

            var playerNames = new string[playerCount];
            for (int i = 0; i < playerCount; i++)
            {
                playerNames[i] =$"Player {i+1}";
            }
            var game = new Game(playerNames);

            while (!game.IsGameOver)
            {
                for (int i = 0; i < game.Players.Count; i++)
                {
                    // TODO: Create a player interaction abstraction and implement a console player vs random AI player
                    PlayTurn(game);
                }
            }
        }

        private static void PlayTurn(Game game)
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
            var builder = new StringBuilder($"Name: {player.Name}, Points: TODO, ");
            // TODO: Disk and bonus breakdowns
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
