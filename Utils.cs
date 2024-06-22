using System;
using System.Collections.Generic;

namespace DragonsAndDungeons
{
    public static class Utils
    {
        public static DiceRoller StringToDiceRoller(string s)
        {
            if (s.Length != 3 || s[1] != 'd') {
                throw new ArgumentException("Bad roller format");
            }

            var numDice = s[0] - '0';
            var numSides = s[2] - '0';
            return new DiceRoller(numDice, numSides);
        }

        public static Dictionary<string, int> GetStatAndValueList(string s)
        {
            Dictionary<string, int> result = new();
            var stringList = s.Split(',');
            foreach(var str in stringList) {
                var statAndValueStr = str.Trim().Split(' ');
                if (statAndValueStr.Length != 2) {
                    throw new Exception($"Badly formatted stat: {str}");
                }

                // TODO: validate stat name and value.
                var name = statAndValueStr[0].Trim();
                var value = Convert.ToInt32(statAndValueStr[1]);
                result[name] = value;
            }
            return result;
        }
    }
}
