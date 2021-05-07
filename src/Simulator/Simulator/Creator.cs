using System;
using System.Collections.Generic;
using System.Text;

namespace Simulator
{
    static class Creator
    {
        static IAction[][] genesBlocks;
        static int[] valuesOfBlocks;
        const int AmountBlocks = 6;

        static int[] directions;
        static Creator()
        {
            directions = new int[9] { 0, 1, 2,
                                      3, 4, 5,
                                      6, 7, 8 };
            genesBlocks = new IAction[AmountBlocks][];
            valuesOfBlocks = new int[AmountBlocks];

            genesBlocks[0] = new IAction[1] { new Wait() };
            genesBlocks[1] = new IAction[2] { new Move(), new Move() };
            genesBlocks[2] = new IAction[1] { new Move() };
            genesBlocks[3] = new IAction[2] { new Photosyntesis(), new Wait() };
            genesBlocks[4] = new IAction[2] { new Photosyntesis(), new Move() };
            genesBlocks[5] = new IAction[2] { new Photosyntesis(), new Photosyntesis() };

            for (int i = 0; i < AmountBlocks; i++)
                foreach (IAction action in genesBlocks[i])
                    valuesOfBlocks[i] += action.Value();
        }

        public static Unit CreateUnit(int capacity)
        {
            int x = Simulator.Random.Next(Simulator.WorldWidth);
            int y = Simulator.Random.Next(Simulator.WorldHeight);
            int energy = Simulator.EnergyLimit / 2;
            int amountOfBlocks = Simulator.Random.Next(1, AmountBlocks);
            int[] dir = FillDirection();
            IAction[][] genes = FillGenes(amountOfBlocks, ref capacity);
            int chlorophyl = FillChlorophyl(ref capacity);

            Unit unit = new Unit(energy, new int[] { x, y }, dir,  genes, capacity, chlorophyl);

            return unit;
        }

        internal static Unit CreateChild(Unit parent)
        {
            int x = 0, y = 0;
            bool fl = false;
            for (int i = 0; i < 9; i++)
            {
                int temp = (8 - parent.LastDirection + i) % 9;
                x = parent.Coords[0] + temp / 3 - 1;
                y = parent.Coords[1] + temp % 3 - 1;
                if (Simulator.IsFree(x, y))
                {
                    fl = true;
                    break;
                }
            }
            if (!fl)
                return null;
            int energy = parent.Energy / 2;
            int[] dir = FillDirection();
            IAction[][] genes = parent.Genes;
            int capacity = parent.Capacity;
            int chlorophyl = parent.Chlorophyl;
            Unit child = new Unit(energy, new int[] { x, y }, dir, genes, capacity, chlorophyl);
            return child;
        }
        private static int FillChlorophyl(ref int capacity)
        {
            int i = 1;
            while (Simulator.Random.Next(2) < 1 && capacity > 50 * i)
            {
                capacity -= (75 * i);
                i += 1;
            }
            return i;
        }

        private static int[] FillDirection()
        {
            int[] dir = new int[2];
            dir[0] = Simulator.Random.Next(-1, 2);
            dir[1] = Simulator.Random.Next(-1, 2);
            return dir;
        }

        private static IAction[][] FillGenes(int amountOfBlocks, ref int capacity)
        {
            IAction[][] result = new IAction[amountOfBlocks][];
            int i = 0;
            while (i < amountOfBlocks && capacity > (amountOfBlocks - i) * Simulator.WaitValue && capacity > 0)
            {
                int t = Simulator.Random.Next(AmountBlocks);
                if (capacity - valuesOfBlocks[t] < 0)
                {
                    result[i] = genesBlocks[0];
                    capacity -= valuesOfBlocks[0];
                }
                else
                {
                    result[i] = genesBlocks[t];
                    capacity -= valuesOfBlocks[t];
                }
                i += 1;
            }
            for (; i < amountOfBlocks; i++)
                result[i] = genesBlocks[0];
            return result;
        }        
    }
}
