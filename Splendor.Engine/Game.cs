using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splendor.Engine
{
    public class Game
    {
        private int _currentPlayerIndex;
        private int _passCount;

        // TODO: What if we want to seed the board for testing?
        public Game(IList<string> playerNames)
        {
            if (playerNames == null)
            {
                throw new ArgumentNullException(nameof(playerNames));
            }

            if (playerNames.Count < 2 || playerNames.Count > 4)
            {
                throw new ArgumentOutOfRangeException(nameof(playerNames), playerNames.Count, "2-4 players");
            }

            // Create players
            var palayers = new List<Player>(playerNames.Count);
            foreach (var playerName in playerNames)
            {
                palayers.Add(new Player(playerName));
            }
            Players = palayers;

            // Load bank: Use all 5 gold. Gems- 2 players: 4 gems;  3 players: 5;  4 players: 7
            var gems = Players.Count == 2 ? 4
                : Players.Count == 3 ? 5
                : 7;
            var bank = new Bank(5, gems);

            var loader = new ResourceLoader();
            var nobles = loader.LoadNobles().Shuffle().Take(Players.Count + 1).ToList();
            var cards = loader.LoadCards().Shuffle();

            // Board
            Board = new Board(bank, nobles, cards);
        }

        // In turn order, 0 is start player
        public IReadOnlyList<Player> Players { get; }

        public Player CurrentPlayer => Players[_currentPlayerIndex];

        public Board Board { get; }

        public bool IsFinalRound { get; private set; }
        
        public bool IsGameOver { get; private set; }

        public Player Winner { get; private set; }

        // Verify everything is as it should be. Call as frequently as neccessary durring development, disable for 
        // No duplicate card assignments
        // No missing cards
        // No duplicate noble assignments
        // No missing nobles
        // Expected total number of gem disks in play
        // No player with more than 10 disks
        // No player with more than 3 reserve cards
        public void Validate()
        {
            
        }

        // The current player can take one of these actions. Doing so automaticlly advances play to the next player.

        // Take 3 Gems of different colors.  As many as possible if 3 colors not available.
        // Can't take Gold
        // Disk limit
        public void TakeDistinctGems(IList<GemType> types, IEnumerable<KeyValuePair<GemType, int>> discards, Noble noble = null)
        {
            ThrowIfGameOver();

            // Verify not Gold
            if (types.Where(type => type == GemType.Gold).Any())
            {
                throw new InvalidOperationException("Gold cannot be selected.");
            }

            // Verify The types are distinct
            if (types.Distinct().Count() < types.Count)
            {
                throw new InvalidOperationException("Types must be unique.");
            }

            // Can you request 0? E.g. pass. It seems like it's possible to pass because the bank may run out of disks
            //  and your reserve could be full, yet you may still not be able to afford anything.
            // 0-3 count
            if (types.Count > 3)
            {
                throw new InvalidOperationException("Too many types selected.");
            }

            // TODO: You can only take less than three if there aren't three distinct disks in the bank.

            // At least one exists in the bank
            foreach (var disk in types)
            {
                if (Board.Bank.Available[disk] == 0)
                {
                    throw new InvalidOperationException($"No {disk} in the bank.");
                }
            }

            // If discarding, verify new total is exactly 10. Can't discard and get below 10.
            var totalDiscards = discards?.Select(pair => pair.Value).Sum() ?? 0;
            var newTotal = CurrentPlayer.TotalDisks + types.Count - totalDiscards;
            if (newTotal > 10)
            {
                throw new InvalidOperationException("Insufficient discards.");
            }

            // Verify the player owns specified discards.
            if (discards != null)
            {
                foreach (var set in discards)
                {
                    if (CurrentPlayer.Disks[set.Key] < set.Value)
                    {
                        throw new InvalidOperationException("Discard count mismatch.");
                    }
                }
            }

            ClaimNoble(noble);

            // Discard gems
            if (discards != null)
            {
                foreach (var type in discards)
                {
                    CurrentPlayer.RemoveDisks(type.Key, type.Value);
                    Board.Bank.Return(type.Key, type.Value);
                }
            }

            // Aquire gems
            foreach (var type in types)
            {
                Board.Bank.Take(type, 1);
                CurrentPlayer.AddDisks(type, 1);
            }

            if (types.Count == 0)
            {
                _passCount++;
            }
            else
            {
                _passCount = 0;
            }

            AdvanceGame();
        }

        // Take 2 Gems of the same color.  Color must have at least 4 gems available. 
        // Can't take Gold
        // Disk limit
        public void TakeTwoGems(GemType type, IEnumerable<KeyValuePair<GemType, int>> discards, Noble noble = null)
        {
            ThrowIfGameOver();

            // Verify not Gold
            if (type == GemType.Gold)
            {
                throw new InvalidOperationException("Gold cannot be selected.");
            }

            // Verify The bank has sufficient quantity. Color must have at least 4 gems available. 
            if (Board.Bank.Available[type] < 4)
            {
                throw new InvalidOperationException($"Insufficient {type} in the bank, there must be at least 4.");
            }

            // If discarding, verify new total is exactly 10. Can't discard and get below 10.
            var totalDiscards = discards?.Select(pair => pair.Value).Sum() ?? 0;
            var newTotal = CurrentPlayer.TotalDisks + 2 - totalDiscards;
            if (newTotal > 10)
            {
                throw new InvalidOperationException("Insufficient discards.");
            }

            // Verify the player owns specified discards.
            if (discards != null)
            {
                foreach (var set in discards)
                {
                    if (CurrentPlayer.Disks[set.Key] < set.Value)
                    {
                        throw new InvalidOperationException("Discard count mismatch.");
                    }
                }
            }

            ClaimNoble(noble);

            // Discard gems
            if (discards != null)
            {
                foreach (var group in discards)
                {
                    CurrentPlayer.RemoveDisks(group.Key, group.Value);
                    Board.Bank.Return(group.Key, group.Value);
                }
            }

            // Aquire gems
            Board.Bank.Take(type, 2);
            CurrentPlayer.AddDisks(type, 2);

            _passCount = 0;

            AdvanceGame();
        }

        // Reserve 1 Card to your hand and take 1 gold.
        // Hand limit
        // Disk limit
        // You may discard the aquired gold (but why would you?)
        public void ReserveCard(string id, GemType? discard, Noble noble = null)
        {
            ThrowIfGameOver();

            // Verify reserve limit (3)
            if (CurrentPlayer.Reserve.Count == 3)
            {
                throw new InvalidOperationException($"Too many cards already reserved.");
            }

            // Verify card is available
            if (Board.AvailableCards.Where(c => c.Id == id).SingleOrDefault() == null)
            {
                throw new InvalidOperationException($"Card id {id} not found.");
            }

            // Verify disk limit 10 (if gold available then it will be taken and a discard must be specified).
            //  discard must not be specified if current disks < 10.
            if (Board.Bank.Available[GemType.Gold] > 0 && CurrentPlayer.TotalDisks == 10)
            {
                if (!discard.HasValue)
                {
                    throw new InvalidOperationException("Discard required.");
                }
                // Verify player owns discard or is discarding the gold aquired.
                if (CurrentPlayer.Disks[discard.Value] == 0)
                {
                    throw new InvalidOperationException($"{discard.Value} is not available to discard.");
                }
            }

            ClaimNoble(noble);

            // Move card from available to reserve
            var card = Board.AvailableCards.Where(c => c.Id == id).Single();
            Board.TakeCard(card);
            CurrentPlayer.AddReserve(card);

            // Discard specified
            if (discard.HasValue)
            {
                CurrentPlayer.RemoveDisks(discard.Value, 1);
                Board.Bank.Return(discard.Value, 1);
            }

            // Gain gold
            if (Board.Bank.Available[GemType.Gold] > 0)
            {
                Board.Bank.Take(GemType.Gold, 1);
                CurrentPlayer.AddDisks(GemType.Gold, 1);
            }

            _passCount = 0;

            AdvanceGame();
        }

        // Draw secretly from any deck
        // Reserve 1 Card to your hand and take 1 gold.
        // Hand limit
        // Disk limit
        // Empty deck
        // You may discard the aquired gold (byt why would you?)
        public void ReserveSecret(int level, GemType? discard, Noble noble = null)
        {
            ThrowIfGameOver();

            // Verify reserve limit (3)
            if (CurrentPlayer.Reserve.Count == 3)
            {
                throw new InvalidOperationException($"Too many cards already reserved.");
            }


            // Verify level 1-3
            if (level < 1 || level > 3)
            {
                throw new InvalidOperationException($"Invlaid level {level} selected.");
            }

            // Verify level has cards
            if (Board.CheckLevelDeckIsEmpty(level))
            {
                throw new InvalidOperationException($"Level {level} deck is empty.");
            }

            // Verify disk limit 10 (if gold available then it will be taken and a discard must be specified).
            //  discard must not be specified if current disks < 10.
            if (Board.Bank.Available[GemType.Gold] > 0 && CurrentPlayer.TotalDisks == 10)
            {
                if (!discard.HasValue)
                {
                    throw new InvalidOperationException("Discard required.");
                }
                // Verify player owns discard or is discarding the gold aquired.
                if (CurrentPlayer.Disks[discard.Value] == 0)
                {
                    throw new InvalidOperationException($"{discard.Value} is not available to discard.");
                }
            }

            ClaimNoble(noble);

            // Move card from deck to reserve
            var card = Board.TakeSecret(level);
            CurrentPlayer.AddReserve(card);

            // Discard specified
            if (discard.HasValue)
            {
                CurrentPlayer.RemoveDisks(discard.Value, 1);
                Board.Bank.Return(discard.Value, 1);
            }

            // Gain gold
            if (Board.Bank.Available[GemType.Gold] > 0)
            {
                Board.Bank.Take(GemType.Gold, 1);
                CurrentPlayer.AddDisks(GemType.Gold, 1);
            }

            _passCount = 0;

            AdvanceGame();
        }

        // Buy 1 Card from the middle or from reserve.
        // Gold will be consumed automatically if required
        // Claim a noble if possible. If none is specified and you can afford one it will
        //  be claimed automatically.
        public void Purchase(string id, Noble noble = null)
        {
            ThrowIfGameOver();

            // Verify card is available or in reserve
            var fromReserve = CurrentPlayer.Reserve.Where(c => c.Id == id).SingleOrDefault();
            var fromAvailable = Board.AvailableCards.Where(c => c.Id == id).SingleOrDefault();
            var card = fromReserve ?? fromAvailable ?? throw new InvalidOperationException($"Card {id} could not be found.");

            // Verify card is affordable (inlcuding gold as needed)
            if (!Utilities.CanAfford(card.Cost, CurrentPlayer.TotalGems))
            {
                throw new InvalidOperationException($"Cannot afford card id {id}.");
            }

            // Pay price (e.g. return tokens to bank as needed)
            foreach (var cost in card.Cost)
            {
                var diff = cost.Value - CurrentPlayer.Bonuses[cost.Key];
                if (diff > 0)
                {
                    var goldNeeded = diff - CurrentPlayer.Disks[cost.Key];
                    if (goldNeeded > 0)
                    {
                        CurrentPlayer.RemoveDisks(GemType.Gold, goldNeeded);
                        Board.Bank.Return(GemType.Gold, goldNeeded);
                        diff -= goldNeeded;
                    }
                    if (diff > 0)
                    {
                        CurrentPlayer.RemoveDisks(cost.Key, diff);
                        Board.Bank.Return(cost.Key, diff);
                    }
                }
            }

            // Move card from reserve or available to player's stack
            if (fromReserve != null)
            {
                CurrentPlayer.TransferFromReserve(card);
            }
            else
            {
                Board.TakeCard(card);
                CurrentPlayer.AddCard(fromAvailable);
            }

            ClaimNoble(noble);
            //  Design bug: Claiming nobles is not part of the purchase action. Since it's possible for you
            //  to be able to afford more than one when you're only allowed to take one, on your subsequent turn
            //  you may claim another without an additional purchase. On that turn you may still have a choice,
            //  so you need to be able specify a noble on any type of action.
            //  Proposal: Many different actions require similar parameters. Make a general state object that
            //  includes disk aquisitions, discards, noble selection, and a card selection id. The secret reservation
            //  level is the only unique parameter.

            _passCount = 0;

            AdvanceGame();
        }

        private void ClaimNoble(Noble noble)
        {
            if (noble == null)
            {
                // If no noble is specified, try selecting one who's requirements have or will be met.
                // Most of the time there won't be any. Sometimes there will be one. Rarely there will be multiple.
                noble = Board.Nobles.Where(n => Utilities.RequirementsMet(n.Requirements, CurrentPlayer.Bonuses)).FirstOrDefault();
            }
            else
            {
                // Verify noble is available
                if (!Board.Nobles.Contains(noble))
                {
                    throw new InvalidOperationException($"Noble {noble.Id} not found.");
                }

                // Verify noble requirements have been met
                if (!Utilities.RequirementsMet(noble.Requirements, CurrentPlayer.Bonuses))
                {
                    throw new InvalidOperationException($"The requirements for noble {noble.Id} have not been met.");
                }
            }

            // Claim it
            if (noble != null)
            {
                Board.TakeNoble(noble);
                CurrentPlayer.AddNoble(noble);
            }
        }

        private void ThrowIfGameOver()
        {
            if (IsGameOver)
            {
                throw new InvalidOperationException("The game is over, no further actions can be taken.");
            }
        }

        private void AdvanceGame()
        {
            // Check if final round
            IsFinalRound = Players.Where(p => p.Points >= 15).Any();

            // Check if game over, check winner
            if (IsFinalRound && _currentPlayerIndex == Players.Count - 1)
            {
                IsGameOver = true;
                // Most points with fewest cards
                Winner = Players.OrderByDescending(p => p.Cards.Count).OrderBy(p => p.Points).First();
            }
            else if (_passCount == Players.Count)
            {
                // Stalemate
                IsFinalRound = true;
                IsGameOver = true;
                // No winner. We could see who has the most points, but it's more interesting to track stalemates.
            }

            // Advance to next player
            _currentPlayerIndex = (_currentPlayerIndex + 1) % Players.Count;
        }
    }
}
