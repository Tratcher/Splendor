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
                new RandomPlayer("Random 1"),
                new RandomPlayer("Random 1"),
            };

            var playerNames = playerControls.Select(p => p.Name).ToList();
            var game = new Game(playerNames);

            while (!game.IsGameOver)
            {
                for (int i = 0; i < playerControls.Length; i++)
                {
                    playerControls[i].PlayTurn(game);
                }
            }
        }
    }
}
