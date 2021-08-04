using Simulator.Events;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simulator.World
{
    public class UnitDataBase
    {
        /// <summary>
        /// List of available units
        /// </summary>
        public HashSet<int> UnitsNumbers { get; private set; }
        public List<int> UnitsEnergy { get; private set; }
        public List<int> UnitsCapacity { get; private set; }
        public List<int> UnitsChlorophyl { get; private set; }
        public List<int> UnitsAttackPower { get; private set; }
        public List<int> UnitsParent { get; private set; }
        public List<IAction[][]> UnitsGenes { get; private set; }
        public List<int[]> UnitsCoords { get; private set; }
        private List<int[]> unitsCurrentAction { get; set; }
        public List<int> UnitsLastDirection { get; private set; }
        public List<int[]> UnitsDirection { get; private set; }
        public List<UnitStatus> UnitsStatus { get; private set; }
        private HashSet<int> availableNumbers;
        private BinaryIntHeap availableNumbersSupportingHeap;


        public UnitDataBase(int count)
        {
            availableNumbers = new HashSet<int>();
            availableNumbersSupportingHeap = new BinaryIntHeap(World.Swamp.WorldHeight * World.Swamp.WorldWidth);
            UnitsNumbers = new HashSet<int>();
            UnitsEnergy = new List<int>(World.Swamp.WorldHeight * World.Swamp.WorldWidth);
            UnitsCapacity = new List<int>(World.Swamp.WorldHeight * World.Swamp.WorldWidth);
            UnitsChlorophyl = new List<int>(World.Swamp.WorldHeight * World.Swamp.WorldWidth);
            UnitsAttackPower = new List<int>(World.Swamp.WorldHeight * World.Swamp.WorldWidth);
            unitsCurrentAction = new List<int[]>(World.Swamp.WorldHeight * World.Swamp.WorldWidth);
            UnitsLastDirection = new List<int>(World.Swamp.WorldHeight * World.Swamp.WorldWidth);
            UnitsParent = new List<int>(World.Swamp.WorldHeight * World.Swamp.WorldWidth);
            UnitsCoords = new List<int[]>(World.Swamp.WorldHeight * World.Swamp.WorldWidth);
            UnitsDirection = new List<int[]>(World.Swamp.WorldHeight * World.Swamp.WorldWidth);
            UnitsGenes = new List<IAction[][]>(World.Swamp.WorldHeight * World.Swamp.WorldWidth);
            UnitsStatus = new List<UnitStatus>(World.Swamp.WorldHeight * World.Swamp.WorldWidth);

            for (int i = 0; i < World.Swamp.WorldHeight * World.Swamp.WorldWidth; i++)
            {
                availableNumbers.Add(i);
                availableNumbersSupportingHeap.Insert(i);
                UnitsEnergy.Add(default);
                UnitsCapacity.Add(default);
                UnitsChlorophyl.Add(default);
                UnitsAttackPower.Add(default);
                unitsCurrentAction.Add(default);
                UnitsLastDirection.Add(default);
                UnitsParent.Add(default);
                UnitsCoords.Add(default);
                UnitsDirection.Add(default);
                UnitsGenes.Add(default);
                UnitsStatus.Add(default);
            }

            for (int k = 0; k < count; k++)
            {
                int i = availableNumbersSupportingHeap.PopMin();
                var unit = CreatorForDataBase.CreateUnit(5000, i);
                UnitsNumbers.Add(i);
                availableNumbers.Remove(i);
                UnitsEnergy[i] = unit.Item1;
                UnitsCapacity[i] = unit.Item2;
                UnitsChlorophyl[i] = unit.Item3;
                UnitsAttackPower[i] = unit.Item4;
                unitsCurrentAction[i] = unit.Item5;
                UnitsLastDirection[i] = unit.Item6;
                UnitsParent[i] = unit.Item7;
                UnitsCoords[i] = unit.Item8;
                UnitsDirection[i] = unit.Item9;
                UnitsGenes[i] = unit.Item10;
                UnitsStatus[i] = unit.Item11;
            }
        }

        public void Import(List<(int, int, int, int, int[], int, int, int[], int[], IAction[][], UnitStatus)> units)
        {
            availableNumbersSupportingHeap.Clear();
            availableNumbers.Clear();
            UnitsNumbers.Clear();
            UnitsEnergy.Clear();
            UnitsCapacity.Clear();
            UnitsChlorophyl.Clear();
            UnitsAttackPower.Clear();
            unitsCurrentAction.Clear();
            UnitsLastDirection.Clear();
            UnitsParent.Clear();
            UnitsCoords.Clear();
            UnitsDirection.Clear();
            UnitsGenes.Clear();
            UnitsStatus.Clear();

            for (int i = 0; i < World.Swamp.WorldHeight * World.Swamp.WorldWidth; i++)
            {
                availableNumbersSupportingHeap.Insert(i);
                availableNumbers.Add(i);
                UnitsEnergy.Add(default);
                UnitsCapacity.Add(default);
                UnitsChlorophyl.Add(default);
                UnitsAttackPower.Add(default);
                unitsCurrentAction.Add(default);
                UnitsLastDirection.Add(default);
                UnitsParent.Add(default);
                UnitsCoords.Add(default);
                UnitsDirection.Add(default);
                UnitsGenes.Add(default);
                UnitsStatus.Add(default);
            }

            for (int k = 0; k < units.Count; k++)
            {
                int i = availableNumbersSupportingHeap.PopMin();
                var unit = units[k];
                UnitsNumbers.Add(i);
                availableNumbers.Remove(i);
                UnitsEnergy[i] = unit.Item1;
                UnitsCapacity[i] = unit.Item2;
                UnitsChlorophyl[i] = unit.Item3;
                UnitsAttackPower[i] = unit.Item4;
                unitsCurrentAction[i] = unit.Item5;
                UnitsLastDirection[i] = unit.Item6;
                UnitsParent[i] = unit.Item7;
                UnitsCoords[i] = unit.Item8;
                UnitsDirection[i] = unit.Item9;
                UnitsGenes[i] = unit.Item10;
                UnitsStatus[i] = unit.Item11;
            }
        }

        internal void Attack(int unitNumber, int x, int y)
        {
            int[] targetPosition = new int[] { UnitsCoords[unitNumber][0] + x, UnitsCoords[unitNumber][1] + y };
            UnitsLastDirection[unitNumber] = (x + 1) * 3 + (y + 1);
            var unit = Storage.CurrentWorld.GetUnit(targetPosition[0], targetPosition[1]);
            if (unit != -1 && UnitsLastDirection[unitNumber] != 4)
            {
                int temp = -Math.Min(UnitsEnergy[unitNumber] / (Swamp.AttackLimit * 2 - UnitsAttackPower[unitNumber]), UnitsEnergy[unit]);
                if (UnitsStatus[unit] == UnitStatus.Corpse)
                {
                    temp = UnitsEnergy[unit] / (Swamp.AttackLimit + 1 - UnitsAttackPower[unitNumber]);
                }
                TakeEnergy(unit, temp);
                TakeEnergy(unitNumber, -temp * 7 / 10);
            }
        }

        internal void Process(int unit)
        {
            var genesSeq = UnitsGenes[unit];
            var currGenesBlock = genesSeq[unitsCurrentAction[unit][0]];
            var currAction = currGenesBlock[unitsCurrentAction[unit][1]];
            currAction.Process(unit);
            unitsCurrentAction[unit][1]++;
            if (unitsCurrentAction[unit][1] >= currGenesBlock.Length)
            {
                unitsCurrentAction[unit][0]++;
                unitsCurrentAction[unit][1] = 0;
                if (unitsCurrentAction[unit][0] >= UnitsGenes[unit].Length)
                    unitsCurrentAction[unit][0] = 0;
            }
        }

        internal void MoveUnit(int unit, int x, int y)
        {
            int[] newPosition = new int[] { UnitsCoords[unit][0] + x, UnitsCoords[unit][1] + y };
            UnitsLastDirection[unit] = (x + 1) * 3 + (y + 1);
            if (CheckCell(newPosition[0], newPosition[1]))
            {
                MoveEvent.KnockKnock(unit, UnitsCoords[unit], newPosition);
                UnitsCoords[unit][0] = newPosition[0];
                UnitsCoords[unit][1] = newPosition[1];
            }
        }

        private bool CheckCell(int x, int y)
        {
            return Storage.CurrentWorld.IsFree(x, y);
        }

        internal void Divide(int unit)
        {
            int number = availableNumbersSupportingHeap.PopMin();
            var child = CreatorForDataBase.CreateChild(unit, number);
            if (child.Item11 != UnitStatus.Dead)
            {
                DivideEvent.KnockKnock(child.Item7, child.Item8);

                UnitsNumbers.Add(number);
                availableNumbers.Remove(number);
                UnitsEnergy[number] = child.Item1;
                UnitsCapacity[number] = child.Item2;
                UnitsChlorophyl[number] = child.Item3;
                UnitsAttackPower[number] = child.Item4;
                unitsCurrentAction[number] = child.Item5;
                UnitsLastDirection[number] = child.Item6;
                UnitsParent[number] = UnitsParent[unit];
                UnitsCoords[number] = child.Item8;
                UnitsDirection[number] = child.Item9;
                UnitsGenes[number] = child.Item10;
                UnitsStatus[number] = child.Item11;

                UnitsEnergy[unit] /= 2;
                UnitsEnergy[unit] -= Swamp.EnergyLimit / 100;
            }
            else
            {
                UnitsEnergy[unit] = Swamp.EnergyLimit;
                availableNumbersSupportingHeap.Insert(number);
            }
            UnitsStatus[unit] = UnitStatus.Alive;
        }

        public void TakeEnergy(int unit, int energy)
        {
            UnitsEnergy[unit] += energy;
            if (UnitsEnergy[unit] <= 0)
            {
                if (UnitsEnergy[unit] <= Swamp.DeathLimit)
                    UnitsStatus[unit] = UnitStatus.Dead;
                else
                    UnitsStatus[unit] = UnitStatus.Corpse;
            }
            if (UnitsEnergy[unit] > Swamp.EnergyLimit)
            {
                UnitsStatus[unit] = UnitStatus.Divide;
            }
        }

        internal void Delete(int unit)
        {
            UnitsNumbers.Remove(unit);
            availableNumbers.Add(unit);
            availableNumbersSupportingHeap.Insert(unit);
            UnitsEnergy[unit] = default;
            UnitsCapacity[unit] = default;
            UnitsChlorophyl[unit] = default;
            UnitsAttackPower[unit] = default;
            unitsCurrentAction[unit] = default;
            UnitsLastDirection[unit] = default;
            UnitsParent[unit] = default;
            UnitsCoords[unit] = default;
            UnitsDirection[unit] = default;
            UnitsGenes[unit] = default;
            UnitsStatus[unit] = default;
        }

        public IAction GetCurrentAction(int unit)
        {
            var temp = UnitsGenes[unit][unitsCurrentAction[unit][0]];
            return temp[unitsCurrentAction[unit][1]];
        }
    }
}
