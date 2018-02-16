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

            var statelmates = 0;
            for (int i = 0; i < 10000; i++)
            {
                if (i % 1000 == 0) Console.WriteLine("Game: " + i);
                PlayGame(playerControls, ref statelmates);
            }

            Console.WriteLine($"Stalemates: {statelmates}");
            Console.WriteLine("Wins:");
            foreach (var player in playerControls)
            {
                Console.WriteLine($"{player.Name}: {player.Wins}");
            }
        }

        public static void PlayGame(IList<IPlayerControl> playerControls, ref int stalemates)
        {
            var playerNames = playerControls.Select(p => p.Name).ToList();
            var game = new Game(playerNames);

            while (!game.IsGameOver)
            {
                for (int i = 0; i < playerControls.Count && !game.IsGameOver; i++)
                {
                    playerControls[i].PlayTurn(game);
                }
            }

            var winner = playerControls.Where(p => p.Name == game.Winner?.Name).SingleOrDefault();
            if (winner != null)
            {
                winner.Wins++;
            }
            else
            {
                stalemates++;
            }

            // ConsolePlayer.ShowGame(game);
            // Console.WriteLine($"Winner: {game.Winner.Name}");
        }
    }
}
