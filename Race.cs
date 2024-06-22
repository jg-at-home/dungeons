using System.Collections.Generic;

namespace DragonsAndDungeons
{
    public enum Ability
    {
        None,
        Infravision,
        SecretDoorDetection,
        DefensiveBonus,
        InitiativeBonus
    }

    public class Race
    {
        public string Name { get; set; }
        public Dictionary<string, int> MinStats { get; private set; } = new();
        public Dictionary<string, int> StatMods { get; private set; } = new();
        public Ability Ability { get; set; } = Ability.None;
        public List<Class> AllowedClasses { get; private set; } = new();

        public int GetModForStat(string statAbbrev)
        {
            if (MinStats.TryGetValue(statAbbrev, out var mod)) {
                return mod;
            }

            return 0;
        }

        public int GetMinForStat(string statAbbrev)
        {
            if (MinStats.TryGetValue(statAbbrev, out var min))
            {
                return min;
            }

            return 0;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
