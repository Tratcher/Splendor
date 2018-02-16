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

        public int Wins { get; set; }
        
        public void PlayTurn(Game game)
        {
            ShowGame(game);
            Console.WriteLine($"{game.CurrentPlayer.Name}'s turn");
            Console.WriteLine(ShowPlayer(game.CurrentPlayer));

            while (true)
            {
                Console.WriteLine("Select an action: ");
                Console.WriteLine("3) Take 0-3 distinct gems");
                Console.WriteLine("2) Take two gems of a kind, minimum 4");
                Console.WriteLine("r) Reserve card");
                Console.WriteLine("s) Reserve secret card");
                Console.WriteLine("p) Purchase");

                var key = Console.ReadKey();
                switch (key.KeyChar)
                {
                    case '3':
                        Console.WriteLine("Select the disk types with no spaces: (d)iamond, (s)apphire, (e)merald, (r)uby, (o)nyx");
                        var input = Console.ReadLine();
                        var types = new List<GemType>(3);
                        for (int i = 0; i < input.Length; i++)
                        {
                            if (!TryLookupUpGemType(input[i], out var type))
                            {
                                Console.WriteLine($"'{input[i]}' is not a gem type.");
                                continue;
                            }
                            // Not gold
                            if (type == GemType.Gold)
                            {
                                Console.WriteLine("Gold cannot be selected.");
                                continue;
                            }
                            types.Add(type);
                        }

                        // Distinct types
                        if (types.Distinct().Count() < types.Count)
                        {
                            Console.WriteLine("Types must be unique.");
                            continue;
                        }

                        // 0-3 count
                        if (types.Count > 3)
                        {
                            Console.WriteLine("Too many types selected.");
                            continue;
                        }

                        // At least one exists in the bank
                        foreach (var disk in types)
                        {
                            if (game.Board.Bank.Available[disk] == 0)
                            {
                                Console.WriteLine($"No {disk} in the bank.");
                                continue;
                            }
                        }

                        // If this would put you over 10, discard
                        IEnumerable<KeyValuePair<GemType, int>> discards = null;
                        var toDiscard = game.CurrentPlayer.TotalDisks + types.Count - 10;
                        if (toDiscard > 0)
                        {
                            Console.WriteLine($"Select {toDiscard} disk(s) to discard.");
                            input = Console.ReadLine();
                            var discardTypes = new List<GemType>(toDiscard);
                            for (int i = 0; i < input.Length; i++)
                            {
                                if (!TryLookupUpGemType(input[i], out var discardSelection))
                                {
                                    Console.WriteLine($"'{input[i]}' is not a gem type.");
                                    break;
                                }
                                discardTypes.Add(discardSelection);
                            }
                            if (discardTypes.Count != input.Length)
                            {
                                continue;
                            }

                            if (discardTypes.Count != toDiscard)
                            {
                                Console.WriteLine("Wrong number of disks discarded.");
                                continue;
                            }

                            discards = discardTypes.GroupBy(t => t).Select(group => new KeyValuePair<GemType, int>(group.Key, group.Count()));
                        }

                        game.TakeDistinctGems(types, discards);
                        return;
                    case '2':
                        Console.WriteLine("Select the disk type: (d)iamond, (s)apphire, (e)merald, (r)uby, (o)nyx");
                        input = Console.ReadLine();
                        if (input.Length != 1)
                        {
                            Console.WriteLine("Only enter one character.");
                            continue;
                        }
                        
                        if (!TryLookupUpGemType(input[0], out var selection))
                        {
                            Console.WriteLine($"'{input[0]}' is not a gem type.");
                            continue;
                        }
                        // Not gold
                        if (selection == GemType.Gold)
                        {
                            Console.WriteLine("Gold cannot be selected.");
                            continue;
                        }

                        // At least 4 of that type exist in the bank
                        if (game.Board.Bank.Available[selection] < 4)
                        {
                            Console.WriteLine($"The bank must have at least for disks of that type to take two.");
                            continue;
                        }

                        // If this would put you over 10, discard
                        discards = null;
                        toDiscard = game.CurrentPlayer.TotalDisks + 2 - 10;
                        if (toDiscard > 0)
                        {
                            Console.WriteLine($"Select {toDiscard} disk(s) to discard.");
                            input = Console.ReadLine();
                            var discardTypes = new List<GemType>(toDiscard);
                            for (int i = 0; i < input.Length; i++)
                            {
                                if (!TryLookupUpGemType(input[i], out var discardSelection))
                                {
                                    Console.WriteLine($"'{input[i]}' is not a gem type.");
                                    break;
                                }
                                discardTypes.Add(discardSelection);
                            }
                            if (discardTypes.Count != input.Length)
                            {
                                continue;
                            }

                            if (discardTypes.Count != toDiscard)
                            {
                                Console.WriteLine("Wrong number of disks discarded.");
                                continue;
                            }

                            discards = discardTypes.GroupBy(t => t).Select(group => new KeyValuePair<GemType, int>(group.Key, group.Count()));
                        }

                        game.TakeTwoGems(selection, discards);
                        return;
                    case 'r':
                        // Verify reserve limit (3)
                        if (game.CurrentPlayer.Reserve.Count == 3)
                        {
                            Console.WriteLine($"Too many cards already reserved.");
                            continue;
                        }

                        Console.Write("Enter the card Id to reserve: ");
                        input = Console.ReadLine();

                        // Verify card available
                        if (game.Board.AvailableCards.Where(c => c.Id == input).SingleOrDefault() == null)
                        {
                            Console.WriteLine($"Card id {input} not found.");
                            continue;
                        }

                        // Check if at disk limit 10 and must discard (if there's gold left)
                        GemType? discardType = null;
                        if (game.Board.Bank.Available[GemType.Gold] > 0 && game.CurrentPlayer.TotalDisks == 10)
                        {
                            Console.WriteLine($"Select 1 disk to discard.");
                            input = Console.ReadLine();
                            if (input.Length != 1)
                            {
                                Console.WriteLine("Wrong number of disks discarded.");
                                continue;
                            }

                            if (!TryLookupUpGemType(input[0], out var type))
                            {
                                Console.WriteLine($"'{input[0]}' is not a gem type.");
                                continue;
                            }
                            discardType = type;
                        }

                        game.ReserveCard(input, discardType);
                        return;
                    case 's':
                        // Verify reserve limit (3)
                        if (game.CurrentPlayer.Reserve.Count == 3)
                        {
                            Console.WriteLine($"Too many cards already reserved.");
                            continue;
                        }

                        Console.Write("Enter the card level to reserve: ");
                        input = Console.ReadLine();
                        var level = int.Parse(input);

                        // Verify level 1-3
                        if (level < 1 || level > 3)
                        {
                            Console.WriteLine("Invlaid level selected.");
                            continue;
                        }

                        // Verify level has cards
                        if (game.Board.CheckLevelDeckIsEmpty(level))
                        {
                            Console.WriteLine($"Level {level} deck is empty.");
                            continue;
                        }

                        // Check if at disk limit 10 and must discard (if there's gold left)
                        discardType = null;
                        if (game.Board.Bank.Available[GemType.Gold] > 0 && game.CurrentPlayer.TotalDisks == 10)
                        {
                            Console.WriteLine($"Select 1 disk to discard.");
                            input = Console.ReadLine();
                            if (input.Length != 1)
                            {
                                Console.WriteLine("Wrong number of disks discarded.");
                                continue;
                            }
                            
                            if (!TryLookupUpGemType(input[0], out var type))
                            {
                                Console.WriteLine($"'{input[0]}' is not a gem type.");
                                continue;
                            }
                            discardType = type;
                        }

                        game.ReserveSecret(level, discardType);
                        return;
                    case 'p':
                        Console.Write("Enter the card id to purchase, this may be from the available or your reserve: ");
                        input = Console.ReadLine();

                        // Is this available or in my reserve
                        var card = game.Board.AvailableCards.Where(c => c.Id == input).SingleOrDefault()
                            ?? game.CurrentPlayer.Reserve.Where(c => c.Id == input).SingleOrDefault();
                        if (card == null)
                        {
                            Console.WriteLine($"Card id {input} not found.");
                            continue;
                        }

                        // Can I afford it
                        if (!Utilities.CanAfford(card.Cost, game.CurrentPlayer.TotalGems))
                        {
                            Console.WriteLine($"Cannot afford card id {input}.");
                            continue;
                        }

                        // Does this earn me a noble
                        var nobles = game.Board.Nobles.Where(n =>
                            Utilities.WillRequirementsBeMet(n.Requirements, game.CurrentPlayer.Bonuses, card.Bonus));

                        // Do I have the choice of multiple nobles? If so which one do I want?
                        Noble noble = nobles.FirstOrDefault();
                        if (nobles.Count() > 1)
                        {
                            Console.WriteLine("Select a noble id:");
                            foreach (var n in nobles)
                            {
                                Console.WriteLine(ShowNoble(n));
                            }
                            var nobleId = input = Console.ReadLine();
                            noble = nobles.Where(n => n.Id == nobleId).FirstOrDefault();
                        }

                        game.Purchase(input, noble);
                        return;
                    default:
                        Console.WriteLine($"Invalid input '{key.KeyChar}'");
                        continue;
                }
            }

            throw new NotImplementedException();
        }

        private bool TryLookupUpGemType(char ch, out GemType type)
        {
            switch(ch)
            {
                case 'd': type = GemType.Diamond; return true;
                case 's': type = GemType.Sapphire; return true;
                case 'e': type = GemType.Emerald; return true;
                case 'r': type = GemType.Ruby; return true;
                case 'o': type = GemType.Onyx; return true;
                case 'g': type = GemType.Gold; return true;
                default: type = GemType.None; return false;
            }
        }

        public static void ShowGame(Game game)
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
            foreach (var card in cards.OrderByDescending(card => card.PointValue).OrderByDescending(card => card.Level))
            {
                Console.WriteLine(ShowCard(card, Utilities.CanAfford(card.Cost, game.CurrentPlayer.TotalGems)));
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
            var builder = new StringBuilder($"Name: {player.Name}, Points: {player.Points}, Nobles: {player.Nobles.Count}\r\nGems:\r\n");
            ShowGemAmounts(GemType.Diamond, player, builder);
            ShowGemAmounts(GemType.Emerald, player, builder);
            ShowGemAmounts(GemType.Onyx, player, builder);
            ShowGemAmounts(GemType.Ruby, player, builder);
            ShowGemAmounts(GemType.Sapphire, player, builder);
            
            var disks = player.Disks[GemType.Gold];
            builder.Append($"{GemType.Gold.ToString().PadRight(13)} {disks}d = {disks}");
            
            if (player.Reserve.Count > 0)
            {
                builder.AppendLine("\r\nReserve:");
                foreach (var card in player.Reserve)
                {
                    builder.AppendLine(ShowCard(card, Utilities.CanAfford(card.Cost, player.TotalGems)));
                }
            }
            return builder.ToString();
        }

        private static void ShowGemAmounts(GemType type, Player player, StringBuilder builder)
        {
            var bonus = player.Bonuses[type];
            var disks = player.Disks[type];
            var total = player.TotalGems[type];
            if (total > 0)
            {
                builder.AppendLine($"{type.ToString().PadRight(8)} {bonus}b + {disks}d = {total}");
            }
        }

        private static string ShowCard(Card card, bool canAfford)
        {
            var builder = new StringBuilder();
            if (canAfford)
            {
                builder.Append("P ");
            }
            builder.Append($"ID: {card.Id.ToString().PadLeft(2)}, Level: {card.Level}, Points: {card.PointValue}, Bonus: {card.Bonus.ToString().PadRight(8)} Cost: ");
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
