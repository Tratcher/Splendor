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
            Console.WriteLine("3) Take up to 3 distinct gems");
            Console.WriteLine("2) Take two gems of a kind");
            Console.WriteLine("r) Reserve card");
            Console.WriteLine("s) Reserve secret card");
            Console.WriteLine("p) Purchase");

            var key = Console.ReadKey();
            switch (key.KeyChar)
            {
                case '3':
                    Console.WriteLine("Select the disk types separated by spaces: (d)iamond, (s)apphire, (e)merald, (r)uby, (o)nyx");
                    var input = Console.ReadLine();
                    var selections = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    // TODO: Input validation:
                    // Not gold
                    // At least one exists in the store
                    // 1-3 count
                    // Distinct types
                    var types = new List<GemType>(3);
                    foreach (var ch in selections)
                    {
                        types.Add(LookupUpGemType(ch[0]));
                    }

                    // TODO: If this would put you over 10, discard

                    game.TakeDistinctGems(types, null);
                    return;
                case '2':
                    Console.WriteLine("Select the disk type: (d)iamond, (s)apphire, (e)merald, (r)uby, (o)nyx");
                    input = Console.ReadLine();
                    // TODO: Input validation:
                    // Not gold
                    // At least one exists in the store
                    // 1-3 count
                    // Distinct types
                    var type = LookupUpGemType(input[0]));

                    // TODO: If this would put you over 10, discard

                    game.TakeTwoGems(type, null);
                    return;
                case 'r':
                    Console.Write("Enter the card Id to reserve: ");
                    input = Console.ReadLine();

                    // Verify card available

                    // Check if at disk limit 10 and must discard (if there's gold left)

                    game.ReserveCard(input, null);
                    return;
                case 's':
                    Console.Write("Enter the card level to reserve: ");
                    input = Console.ReadLine();
                    var level = int.Parse(input);

                    // Verify level 1-3

                    // Verify level has cards

                    // Check if at disk limit 10 and must discard (if there's gold left)

                    game.ReserveSecret(level, null);
                    return;
                case 'p':
                    Console.Write("Enter the card id to purchase, this may be from the available or your reserve: ");
                    input = Console.ReadLine();

                    // Is this available or in my resurve
                    
                    // Can I afford it

                    // Does this earn me a noble

                    // Do I have the choice of multiple nobles? If so which one do I want?

                    game.Purchase(input, null);
                    return;
                default:
                    Console.WriteLine($"Invalid input '{key.KeyChar}'");
                        break;
            }

            throw new NotImplementedException();
        }

        private GemType LookupUpGemType(char ch)
        {
            switch(ch)
            {
                case 'd': return GemType.Diamond;
                case 's': return GemType.Sapphire;
                case 'e': return GemType.Emerald;
                case 'r': return GemType.Ruby;
                case 'o': return GemType.Onyx;
                case 'g': return GemType.Gold;
                default: throw new NotImplementedException(ch.ToString());
            }
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
            var builder = new StringBuilder($"Name: {player.Name}, Points: {player.Points}\r\nGems: ");
            ShowGemAmounts(GemType.Diamond, player, builder);
            ShowGemAmounts(GemType.Emerald, player, builder);
            ShowGemAmounts(GemType.Onyx, player, builder);
            ShowGemAmounts(GemType.Ruby, player, builder);
            ShowGemAmounts(GemType.Sapphire, player, builder);
            
            var disks = player.Disks[GemType.Gold];
            builder.Append($"{GemType.Gold} {disks}d, ");
            
            if (player.Reserve.Count > 0)
            {
                builder.AppendLine("\r\nReserve:");
                foreach (var item in player.Reserve)
                {
                    builder.AppendLine(ShowCard(item));
                }
            }
            return builder.ToString();
        }

        private static void ShowGemAmounts(GemType type, Player player, StringBuilder builder)
        {
            var bonus = player.Bonuses[type];
            var disks = player.Disks[type];
            var total = player.TotalGems[type];
            builder.Append($"{type} {bonus}b + {disks}d = {total}, ");
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
