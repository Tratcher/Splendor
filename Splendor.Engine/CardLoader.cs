using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

namespace Splendor.Engine
{
    public class CardLoader
    {
        public CardLoader()
        {

        }

        public IReadOnlyList<Card> LoadCards(string path)
        {
            var cards = new List<Card>();

            using (var reader = new StreamReader(new FileStream(path, FileMode.Open)))
            {
                // Skip the header row
                var line = reader.ReadLine();
                line = reader.ReadLine();
                while (line != null)
                {
                    var card = ParseLine(line);
                    cards.Add(card);

                    line = reader.ReadLine();
                }
            }

            return cards.AsReadOnly();
        }

        private Card ParseLine(string line)
        {
            var segments = line.Split(',');
            // Id,Level,White,Blue,Green,Red,Black,Points,Bonus
            Debug.Assert(segments.Length >= 9);

            var id = segments[0];
            var level = int.Parse(segments[1], CultureInfo.InvariantCulture);
            var diamonds = ParseIntOrDefault(segments[2]);
            var sapphires = ParseIntOrDefault(segments[3]);
            var emeralds = ParseIntOrDefault(segments[4]);
            var rubys = ParseIntOrDefault(segments[5]);
            var onyxs = ParseIntOrDefault(segments[6]);
            var pointValue = ParseIntOrDefault(segments[7]);
            var bonus = Enum.Parse<GemType>(segments[8], ignoreCase: true);

            var cost = new Dictionary<GemType, int>();
            if (diamonds > 0)
            {
                cost.Add(GemType.Diamond, diamonds);
            }
            if (sapphires > 0)
            {
                cost.Add(GemType.Sapphire, sapphires);
            }
            if (emeralds > 0)
            {
                cost.Add(GemType.Emerald, emeralds);
            }
            if (rubys > 0)
            {
                cost.Add(GemType.Ruby, rubys);
            }
            if (onyxs > 0)
            {
                cost.Add(GemType.Onyx, onyxs);
            }

            return new Card(id, level, cost, bonus, pointValue);
        }

        private int ParseIntOrDefault(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return 0;
            }

            return int.Parse(input, CultureInfo.InvariantCulture);
        }
    }
}
