using System.Linq;


namespace Simulator.World
{
    static class CreatorForDataBase
    {

        /// <summary>
        /// Method for creating units. Return Tuple of (item1 = energy, item2 = capacity, item3 = chlorophyl, item4 = attackPower, 
        /// item5 = currentAction, item6 = lastDirection, item7 = number of unit, item8 = position, item9 = favorite direction,
        /// item10 = genes sequence, item11 = unit status).
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public static (int, int, int, int, int[], int, int, int[], int[], IAction[][], UnitStatus) CreateUnit(int capacity, int number)
        {
            int x = PseudoRandom.Next(Swamp.WorldWidth);
            int y = PseudoRandom.Next(Swamp.WorldHeight);
            int energy = Swamp.EnergyLimit / 2;
            int amountOfBlocks = PseudoRandom.Next(1, Storage.AmountBlocks);
            int[] dir = FillDirection();
            IAction[][] genes = FillGenes(amountOfBlocks, ref capacity);
            int chlorophyl = FillChlorophyl(ref capacity);
            int attackPower = FillAttackPower(ref capacity, chlorophyl);

            
            var unit = (energy, capacity, chlorophyl, attackPower, new int[] { 0, 0 }, 4, number, new int[] { x, y }, dir, genes, UnitStatus.Alive);

            return unit;
        }

        /// <summary>
        /// Method for creating child from parent number. Return Tuple of (item1 = energy, item2 = capacity, item3 = chlorophyl, item4 = attackPower, 
        /// item5 = currentAction, item6 = lastDirection, item7 = number of unit, item8 = position, item9 = favorite direction,
        /// item10 = genes sequence, item11 = unit status).
        /// </summary>
        /// <param name="parentNumber"></param>
        /// <returns></returns>
        internal static (int, int, int, int, int[], int, int, int[], int[], IAction[][], UnitStatus) CreateChild(int parentNumber, int number)
        {
            int x = 0, y = 0;
            bool fl = false;
            var world = Storage.CurrentWorld;
            for (int i = 0; i < 9; i++)
            {
                int temp = (8 - world.Units.UnitsLastDirection[parentNumber] + i) % 9;
                x = world.Units.UnitsCoords[parentNumber][0] + temp / 3 - 1;
                y = world.Units.UnitsCoords[parentNumber][1] + temp % 3 - 1;
                if (Storage.CurrentWorld.IsFree(x, y))
                {
                    fl = true;
                    break;
                }
            }
            if (!fl)
                return (-1, -1, -1, -1, null, -1, -1, null, null, null, UnitStatus.Dead);
            int energy = world.Units.UnitsEnergy[parentNumber] / 2;
            int[] dir = world.Units.UnitsDirection[parentNumber].Clone() as int[];
            if (PseudoRandom.Next(10) < 2)
                dir = MutateDirection(dir);
            int capacity = world.Units.UnitsCapacity[parentNumber];
            IAction[][] genes = world.Units.UnitsGenes[parentNumber].Select(t => t.ToArray()).ToArray();
            if (PseudoRandom.Next(10) < 3)
                genes = MutateGenes(genes, ref capacity);
            int chlorophyl = world.Units.UnitsChlorophyl[parentNumber];
            int attackPower = world.Units.UnitsAttackPower[parentNumber];
            if (PseudoRandom.Next(10) < 2)
                MutateChlorophyl(ref chlorophyl, ref capacity, attackPower);
            if (PseudoRandom.Next(10) < 2)
                MutateAttackPower(ref attackPower, ref capacity, chlorophyl);
            return (energy, capacity, chlorophyl, attackPower, new int[] { 0, 0 }, 4, number, new int[] { x, y }, dir, genes, UnitStatus.Alive);
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
