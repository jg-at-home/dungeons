using System;

namespace DragonsAndDungeons
{
    public class DiceRoller
    {
        public DiceRoller(int numDice, int numSides)
        {
            if (numDice <= 0 || numDice > 9 || numSides <= 0 || numSides > 9) {
                throw new ArgumentException("invalid dice roller values");
            }

            numDice_ = numDice;
            numSides_ = numSides;
        }

        public int Roll()
        {
            int total = 0;
            for(var i = 1; i <= numDice_; ++i) {
                var roll = rng_.Next(1, numSides_ + 1);
                total += roll;
            }
            return total;
        }

        private Random rng_ = new Random(Environment.TickCount);
        private int numDice_;
        private int numSides_;
    }
}
