using System.Linq;


namespace Simulator.World
{
    static class Creator
    {

        public static Unit CreateUnit(int capacity, int number)
        {
            int x = PseudoRandom.Next(Swamp.WorldWidth);
            int y = PseudoRandom.Next(Swamp.WorldHeight);
            int energy = Swamp.EnergyLimit / 2;
            int amountOfBlocks = PseudoRandom.Next(1, Storage.AmountBlocks);
            int[] dir = FillDirection();
            IAction[][] genes = FillGenes(amountOfBlocks, ref capacity);
            int chlorophyl = FillChlorophyl(ref capacity);
            int attackPower = FillAttackPower(ref capacity, chlorophyl);

            Unit unit = new Unit(energy, new int[] { x, y }, dir,  genes, capacity, chlorophyl, attackPower, number);

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
            int[] dir = parent.Direction.Clone() as int[];
            if (PseudoRandom.Next(10) < 2)
                dir = MutateDirection(dir);
            int capacity = parent.Capacity;
            IAction[][] genes = parent.Genes.Select(t => t.ToArray()).ToArray();
            if (PseudoRandom.Next(10) < 3)
                genes = MutateGenes(genes, ref capacity);
            int chlorophyl = parent.Chlorophyl;
            int attackPower = parent.AttackPower;
            if (PseudoRandom.Next(10) < 2)
                MutateChlorophyl(ref chlorophyl, ref capacity, attackPower);
            if (PseudoRandom.Next(10) < 2)
                MutateAttackPower(ref attackPower, ref capacity, chlorophyl);
            Unit child = new Unit(energy, new int[] { x, y }, dir, genes, capacity, chlorophyl, attackPower, parent.Parent);
            return child;
        }

        private static IAction[][] MutateGenes(IAction[][] genes, ref int capacity)
        {
            if (PseudoRandom.Next(2) == 0)
            {
                int oldBlock = PseudoRandom.Next(genes.Length);
                int newBlock = PseudoRandom.Next(Storage.AmountBlocks);
                if (capacity + Storage.CalculateValue(genes[oldBlock]) - Storage.ValuesOfBlocks[newBlock] >= 0)
                {
                    capacity += Storage.CalculateValue(genes[oldBlock]);
                    capacity -= Storage.ValuesOfBlocks[newBlock];
                    genes[oldBlock] = Storage.GenesBlocks[newBlock];
                }
            }
            else
            {
                int newBlock = PseudoRandom.Next(Storage.AmountBlocks);
                if (capacity - Storage.ValuesOfBlocks[newBlock] >= 0)
                {
                    capacity -= Storage.ValuesOfBlocks[newBlock];
                    IAction[][] temp = new IAction[genes.Length + 1][];
                    for (int i = 0; i < genes.Length; i++)
                        temp[i] = genes[i];
                    temp[genes.Length] = Storage.GenesBlocks[newBlock];
                    genes = temp;
                }
            }
            return genes;
        }

        private static void MutateChlorophyl(ref int chlorophyl, ref int capacity, int attackPower)
        {
            if (PseudoRandom.Next(2) == 0 && capacity >= Swamp.ChlorophylValue && attackPower < Swamp.ChlorophylLimit / chlorophyl)
            {
                capacity -= Swamp.ChlorophylValue;
                chlorophyl += 1;
            }
            else if (chlorophyl > 1)
            {
                capacity += Swamp.ChlorophylValue;
                chlorophyl -= 1;
            }
        }

        private static void MutateAttackPower(ref int attackPower, ref int capacity, int chlorophyl)
        {
            if (PseudoRandom.Next(2) == 0 && capacity >= Swamp.AttackValue && attackPower < Swamp.AttackLimit / chlorophyl)
            {
                capacity -= Swamp.AttackValue;
                attackPower += 1;
            }
            else if (attackPower > 1)
            {
                capacity += Swamp.AttackValue;
                attackPower -= 1;
            }
        }

        private static int[] MutateDirection(int[] direction)
        {
            int[] delta = new int[2];
            delta[0] = PseudoRandom.Next(0, 2);
            delta[1] = PseudoRandom.Next(0, 2);
            if (direction[0] > 0)
                direction[0] -= delta[0];
            else if (direction[0] < 0)
                direction[0] += delta[0];
            else
                direction[0] = PseudoRandom.Next(0, 2) == 0 ? -1 : 1;
            if (direction[1] > 0)
                direction[1] -= delta[1];
            else if (direction[1] < 0)
                direction[1] += delta[1];
            else
                direction[1] = PseudoRandom.Next(0, 2) == 0 ? -1 : 1;
            return direction;
        }

        private static int FillChlorophyl(ref int capacity)
        {
            int i = 1;
            while (PseudoRandom.Next(2) < 1 && capacity >= Swamp.ChlorophylValue && i < Swamp.ChlorophylValue)
            {
                capacity -= Swamp.ChlorophylValue;
                i += 1;
            }
            return i;
        }

        private static int FillAttackPower(ref int capacity, int chlorophyl)
        {
            int i = 1;
            while (PseudoRandom.Next(2) < 1 && capacity >= Swamp.AttackValue && i < Swamp.AttackLimit / chlorophyl)
            {
                capacity -= Swamp.AttackValue;
                i += 1;
            }
            return i;
        }

        private static int[] FillDirection()
        {
            int[] dir = new int[2];
            dir[0] = PseudoRandom.Next(-1, 2);
            dir[1] = PseudoRandom.Next(-1, 2);
            return dir;
        }

        private static IAction[][] FillGenes(int amountOfBlocks, ref int capacity)
        {
            IAction[][] result = new IAction[amountOfBlocks][];
            int i = 0;
            while (i < amountOfBlocks && capacity > (amountOfBlocks - i) * Swamp.WaitValue && capacity > 0)
            {
                int t = PseudoRandom.Next(Storage.AmountBlocks);
                if (capacity - Storage.ValuesOfBlocks[t] < 0)
                {
                    result[i] = Storage.GenesBlocks[0];
                    capacity -= Storage.ValuesOfBlocks[0];
                }
                else
                {
                    result[i] = Storage.GenesBlocks[t];
                    capacity -= Storage.ValuesOfBlocks[t];
                }
                i += 1;
            }
            for (; i < amountOfBlocks; i++)
                result[i] = Storage.GenesBlocks[0];
            return result;
        }        
    }
}
