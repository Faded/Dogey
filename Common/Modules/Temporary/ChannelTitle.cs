using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Common.Modules
{
    public static class ChannelTitle
    {
        private static string[] _clocks =
        {
            "🕐",
            "🕑",
            "🕒",
            "🕓",
            "🕔",
            "🕕",
            "🕖",
            "🕗",
            "🕘",
            "🕙",
            "🕚",
            "🕛"
        };

        private static string[] _describers =
        {
            "Flying",
            "Walking",
            "Rotten",
            "Shiny",
            "Golden",
            "Blue",
            "Red",
            "Green",
            "Brown",
            "Black",
            "White",
            "Metal",
            "Spotted",
            "Random",
            "Silly",
            "Dancing",
            "Typing",
            "Rockin'",
            "Frozen",
            "Tired",
            "Sleepy",
            "Scary",
            "Crazy",
            "Dangerous",
            "Normal"
        };

        private static string[] _nouns =
        {
            "Bird",
            "Dog",
            "Doge",
            "Cat",
            "House",
            "Tree",
            "Star",
            "Fruit",
            "Garbage",
            "Beach",
            "Turtle",
            "Lion",
            "Giraffe",
            "Snake",
            "Raven",
            "Unicorn",
            "Robot",
            "Kitten",
            "Grass",
            "Mountain",
            "River",
            "Stream",
            "Gamer",
            "Person",
            "People"
        };
        
        public static string GetTitle()
        {
            var r = new Random();

            string clocks = _clocks[r.Next(0, _clocks.Count())];
            string describer = _describers[r.Next(0, _describers.Count())];
            string noun = _nouns[r.Next(0, _nouns.Count())];

            return $"{clocks} {describer} {noun}";
        }

        public static string GetDescriber()
        {
            var r = new Random();
            return _describers[r.Next(0, _describers.Count())];
        }

        public static string GetNoun()
        {
            var r = new Random();
            return _nouns[r.Next(0, _nouns.Count())];
        }
    }
}
