using Simulator.World;

using System;
using System.Collections.Generic;
using System.Text;

namespace Simulator
{
    static class Creator
    {

        public static Unit CreateUnit(int capacity, Simulator world)
        {
            int x = Simulator.Random.Next(Simulator.WorldWidth);
            int y = Simulator.Random.Next(Simulator.WorldHeight);
            int energy = Simulator.EnergyLimit / 2;
            int amountOfBlocks = Simulator.Random.Next(1, Storage.AmountBlocks);
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
                if (Storage.CurrentWorld.IsFree(x, y))
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
                int t = Simulator.Random.Next(Storage.AmountBlocks);
                if (capacity - Storage.valuesOfBlocks[t] < 0)
                {
                    result[i] = Storage.genesBlocks[0];
                    capacity -= Storage.valuesOfBlocks[0];
                }
                else
                {
                    result[i] = Storage.genesBlocks[t];
                    capacity -= Storage.valuesOfBlocks[t];
                }
                i += 1;
            }
            for (; i < amountOfBlocks; i++)
                result[i] = Storage.genesBlocks[0];
            return result;
        }        
    }
}
