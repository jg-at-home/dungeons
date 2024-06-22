namespace DragonsAndDungeons
{
    public class Class
    {
        public string Name { get; set; }
        public DiceRoller DiceRoller { get; set; }
        public string PrimaryStat { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
