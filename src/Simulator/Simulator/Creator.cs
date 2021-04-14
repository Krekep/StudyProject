using System;
using System.Collections.Generic;
using System.Text;

namespace Simulator
{
    static class Creator
    {
        static IAction[][] genesBlocks;
        const int AMOUNT_BLOCKS = 1;

        static Creator()
        {
            genesBlocks = new IAction[AMOUNT_BLOCKS][];

            genesBlocks[0] = new IAction[2] { new Move(), new Wait() };
        }

        public static Unit CreateUnit()
        {
            int x = Simulator.Random.Next(Simulator.WorldHeight);
            int y = Simulator.Random.Next(Simulator.WorldWidth);
            int energy = Simulator.EnergyLimit / 2;

            Unit unit = new Unit(energy, new int[] { x, y },  genesBlocks);

            return unit;
        }

        private static int GenerateAnotherNum(int from, int to) => Simulator.Random.Next(from, to);
        public static int[] Shuffle(int[] Sequence)
        {
            if (null == Sequence)
                throw new ArgumentNullException(nameof(Sequence));
            for (int s = 0; s < Sequence.Length - 1; s++)
            {
                int GenObj = GenerateAnotherNum(s, Sequence.Length);

                var h = Sequence[s];
                Sequence[s] = Sequence[GenObj];
                Sequence[GenObj] = h;
            }
            return Sequence;
        }
    }
}
