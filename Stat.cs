namespace DragonsAndDungeons
{
    public struct StatInfo
    {
        public string Name;
        public string Abbreviation;
    };

    public class Stat
    {
        public Stat(string name, string abbrev)
        {
            Name = name;
            Abbreviation = abbrev;
        }
        
        public string Name { get; private set; }
        public string Abbreviation { get; private set; }
        public int Value => BaseValue + modifiers_;
        public int Modifiers => modifiers_;
        public int BaseValue { get; set; }

        public void ClampToMin(int minValue)
        {
            var value = BaseValue + modifiers_;
            if (Value < minValue) {
                var delta = minValue - value;
                BaseValue += delta;
            }
        }

        public void AddModifier(int mod)
        {
            modifiers_ += mod;
        }

        private int modifiers_;
    }
}
