using System.Collections.Generic;
using System.Text;

namespace DragonsAndDungeons
{
    public class Character
    {
        public Character(string name, Race race, Class @class)
        {
            Name = name;
            Race = race;
            Class = @class;

            foreach(var item in statInfo) {
                statTable_[item.Abbreviation] = new Stat(item.Name, item.Abbreviation);
            }
        }

        public string Name { get; set; }
        public override string ToString() => Name;
        public Race Race { get; set; }
        public Class Class { get; set; }
        public int Gold { get; set; }
        public Stat GetStat(string abbrev) => statTable_[abbrev];
        public ICollection<Stat> Stats => statTable_.Values;

        public string StatString
        {
            get
            {
                var sb = new StringBuilder();
                foreach(var info in statInfo) {
                    var stat = statTable_[info.Abbreviation];
                    var indicator = (stat.Abbreviation == Class.PrimaryStat) ? "*" : string.Empty;
                    sb.Append($"{stat.Abbreviation}{indicator}:{stat.Value}");
                    if (stat.Modifiers != 0) {
                        sb.Append($"({stat.BaseValue}) ");
                    }
                    else {
                        sb.Append(' ');
                    }
                }
                return sb.ToString();
            }
        }

        public static List<StatInfo> statInfo = new();

        private Dictionary<string, Stat> statTable_ = new();
    }
}
