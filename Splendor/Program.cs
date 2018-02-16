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

            var playerControls = new[]
            {
                new RandomPlayer("Random 1"),
                new RandomPlayer("Random 2"),
                new RandomPlayer("Random 3"),
            };

            for (int i = 0; i < 1000; i++)
            {
                Console.WriteLine("Game: " + i);
                PlayGame(playerControls);
            }

            Console.WriteLine("Wins:");
            foreach (var player in playerControls)
            {
                Console.WriteLine($"{player.Name}: {player.Wins}");
            }
        }

        public static void PlayGame(IList<IPlayerControl> playerControls)
        {
            var playerNames = playerControls.Select(p => p.Name).ToList();
            var game = new Game(playerNames);

            while (!game.IsGameOver)
            {
                for (int i = 0; i < playerControls.Count; i++)
                {
                    playerControls[i].PlayTurn(game);
                }
            }

            var winner = playerControls.Where(p => p.Name == game.Winner.Name).Single();
            winner.Wins++;

            // ConsolePlayer.ShowGame(game);
            // Console.WriteLine($"Winner: {game.Winner.Name}");
        }
    }
}
