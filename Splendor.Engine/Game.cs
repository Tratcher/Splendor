using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splendor.Engine
{
    public class Game
    {
        // TODO: What if we want to seed the board for testing?
        public Game(string[] playerNames)
        {
            if (playerNames == null)
            {
                throw new ArgumentNullException(nameof(playerNames));
            }

            if (playerNames.Length < 2 || playerNames.Length > 4)
            {
                throw new ArgumentOutOfRangeException(nameof(playerNames), playerNames.Length, "2-4 players");
            }

            // Create players
            var palayers = new List<Player>(playerNames.Length);
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

        public Player CurrentPlayer { get; }

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
        public void TakeDistinctGems(GemType[] types, IEnumerable<KeyValuePair<GemType, int>> dicards)
        {
            ThrowIfGameOver();

            // Can you request 0? E.g. pass

            // Verify The types are distinct

            // Verify not Gold

            // Verify The bank has sufficient quantity.

            // If discarding, verify new total is exactly 10. Can't discard and get below 10.

            // Verify the player owns specified discards.

            // Discard gems

            // Aquire gems

            AdvanceGame();
        }

        // Take 2 Gems of the same color.  Color must have at least 4 gems available. 
        // Can't take Gold
        // Disk limit
        public void TakeTwoGems(GemType type, IEnumerable<KeyValuePair<GemType, int>> dicards)
        {
            ThrowIfGameOver();

            // Verify not Gold

            // Verify The bank has sufficient quantity. Color must have at least 4 gems available. 

            // If discarding, verify new total is exactly 10. Can't discard and get below 10.

            // Verify the player owns specified discards.

            // Discard gems

            // Aquire gems

            AdvanceGame();
        }

        // Reserve 1 Card to your hand and take 1 gold.
        // Hand limit
        // Disk limit
        // You may discard the aquired gold (but why would you?)
        public void ReserveCard(string id, GemType? dicard)
        {
            ThrowIfGameOver();

            // Verify reserve limit (3)

            // Verify card is available

            // Verify disk limit 10 (if gold available then it will be taken and a discard must be specified).
            //  discard must not be specified if current disks < 10.

            // Verify player owns discard or is discarding the gold aquired.

            // Move card from available to reserve

            // Discard specified

            // Gain gold

            AdvanceGame();
        }

        // Draw secretly from any deck
        // Reserve 1 Card to your hand and take 1 gold.
        // Hand limit
        // Disk limit
        // Empty deck
        // You may discard the aquired gold (byt why would you?)
        public void ReserveSecret(int level, GemType? dicard)
        {
            ThrowIfGameOver();

            // Verify reserve limit (3)

            // Verify deck is not empty

            // Verify disk limit 10 (if gold available then it will be taken and a discard must be specified).
            //  discard must not be specified if current disks < 10.

            // Verify player owns discard or is discarding the gold aquired.

            // Move card from deck to reserve

            // Discard specified

            // Gain gold

            AdvanceGame();
        }

        // Buy 1 Card from the middle or from reserve.
        // Gold will be consumed automatically if required
        // Claim a noble if possible. If none is specified and you can afford one it will
        //  be claimed automatically.
        public void Purchase(string id, Noble noble)
        {
            ThrowIfGameOver();

            // Verify card is available or in reserve

            // Verify card is affordable (inlcuding gold as needed)

            // Verify noble is available

            // Verify noble requirements will be met after purchase

            // If no noble is specified, try selecting one who's requirements will be met after purchase

            // Pay price (e.g. return tokens to bank as needed)

            // Move card from reserve or store player's stack

            // Move noble from board to player, if any.

            AdvanceGame();
        }

        private void ThrowIfGameOver()
        {
            if (IsGameOver)
            {
                throw new InvalidOperationException("The game is over, no further actions can be taken.");
            }
        }

        // Check if final round
        // Check if game over, check winner
        // Advance to next player
        private void AdvanceGame()
        {
            throw new NotImplementedException();
        }
    }
}
