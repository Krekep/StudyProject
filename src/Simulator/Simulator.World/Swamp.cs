using Simulator.Events;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Simulator.World
{
    public class Swamp
    {
        public static readonly int ThreadCounter = Environment.ProcessorCount - 1;
        //public static readonly int ThreadCounter = 1;
        private Task[] tasks;

        public const int WorldHeight = 200;
        public const int WorldWidth = 300;
        public const int EnergyLimit = 1500;
        public const int DeathLimit = -200;
        public const int EnergyDegradation = -1;
        /// <summary>
        /// Based point for other actions value
        /// </summary>
        public const int WaitValue = 100;
        public const int AttackLimit = 20;
        public const int ChlorophylLimit = 20;
        public const int AttackValue = 100;
        public const int ChlorophylValue = 75;
        public const int MaxUnitSize = 3;

        // Changeable parameters
        public int GroundPower { get; private set; }
        public int SunPower { get; private set; }
        public double EnvDensity { get; private set; }
        public double DropChance { get; private set; }

        // Swamp addition info
        public int Seed { get; private set; }
        public int Timer { get; private set; }

        /// <summary>
        /// List of available units
        /// </summary>
        public List<Unit> Units { get; private set; }
        private List<Unit> queueAdd;
        private List<List<Unit>> delList = new List<List<Unit>>();

        private Unit[,] map;

        public int[] SunEnergyMap { get; private set; } = new int[WorldHeight];
        public int[] GroundEnergyMap { get; private set; } = new int[WorldHeight];

        public void Initialize(int seed)
        {
            Storage.CurrentWorld = this;
            MoveEvent.Notify += MoveUnit;
            DivideEvent.Notify += AddUnit;
            DeathEvent.Notify += DeleteUnit;
            GroundPower = 7;
            SunPower = 5;
            EnvDensity = 0.85;
            DropChance = 0.1;

            Seed = seed;
            Timer = 0;

            tasks = new Task[ThreadCounter];
            queueAdd = new List<Unit>();
            for (int i = 0; i < ThreadCounter; i++)
            {
                int top = i * (WorldHeight - 1) / ThreadCounter + 2;
                int bottom = (i + 1) * (WorldHeight - 1) / ThreadCounter - 1;
                delList.Add(new List<Unit>());
            }

            for (int i = 0; i < WorldHeight; i++)
            {
                SunEnergyMap[i] = (int)(SunPower * Math.Pow(EnvDensity, i));
                GroundEnergyMap[i] = (int)(GroundPower * Math.Pow(EnvDensity, WorldHeight - i - 1));
            }

            map = new Unit[WorldWidth, WorldHeight];
            Units = new List<Unit>();

            for (int l = 1; l <= MaxUnitSize; l++)
                for (int i = 0; i < 500 / MaxUnitSize; i++)
                {
                    Unit unit = Creator.CreateUnit(5000, i, l);
                    if (unit == null)
                        continue;
                    map[unit.Coords[0], unit.Coords[1]] = unit;
                    Units.Add(unit);
                }
        }

        public void Import(int seed, int timer, int groundPower, int sunPower, double envDensity, double dropChance, List<Unit> units)
        {
            Seed = seed;
            Timer = timer;

            GroundPower = groundPower;
            SunPower = sunPower;
            EnvDensity = envDensity;
            DropChance = dropChance;
            Units.Clear();
            Units = units;
            map = new Unit[WorldWidth, WorldHeight];
            foreach (Unit unit in units)
            {
                for (int i = 0; i < unit.Size; i++)
                    for (int j = 0; j < unit.Size; j++)
                        map[unit.Coords[0] + i, unit.Coords[1] + j] = unit;
            }

            for (int i = 0; i < WorldHeight; i++)
            {
                SunEnergyMap[i] = (int)(SunPower * Math.Pow(EnvDensity, i));
                GroundEnergyMap[i] = (int)(GroundPower * Math.Pow(EnvDensity, WorldHeight - i - 1));
            }

            //Storage.CurrentWorld = this;
        }

        internal bool IsFree(int x, int y, int area, Unit unit)
        {
            if (x < 0 || y < 0 || (x + area - 1) >= WorldWidth || (y + area - 1) >= WorldHeight)
                return false;
            bool isFree = true;
            for (int i = 0; i < area; i++)
                for (int j = 0; j < area; j++)
                    isFree &= (map[x + i, y + j] == unit || map[x + i, y + j] == null);
            return isFree;
        }

        public Unit GetUnit(int x, int y)
        {
            if (x < 0 || y < 0 || x >= WorldWidth || y >= WorldHeight)
                return null;
            return map[x, y];
        }

        public void Update()
        {
            Timer += 1;
            foreach (Unit unit in Units)
                unit.Process();
            RecountEnergyParallel();
            AddNewCellsParallel();
            DropCells();
            for (int i = 0; i < ThreadCounter; i++)
            {
                int t = i;
                tasks[i] = Task.Run(() => ClearAddQueue(t));
            }
            Task.WaitAll(tasks);
            queueAdd.Clear();
            DeleteDeadCells();
        }

        private void DropCells()
        {
            foreach (Unit unit in Units)
                if (PseudoRandom.NextDouble() < DropChance)
                    unit.Move(0, 1);
        }

        private void AddNewCellsParallel()
        {
            for (int i = 0; i < ThreadCounter; i++)
            {
                int t = i;
                tasks[i] = Task.Run(() => AddNewCells(t));
            }
        }
        private void AddNewCells(int number)
        {
            int bottom = number * Units.Count / ThreadCounter;
            int top = (number + 1) * Units.Count / ThreadCounter;
            for (int i = bottom; i < top; i++)
            {
                var unit = Units[i];
                if (unit.Status == UnitStatus.Divide)
                    unit.Divide();
            }
        }


        private void DeleteDeadCells()
        {
            for (int i = 0; i < ThreadCounter; i++)
            {
                int t = i;
                tasks[i] = Task.Run(() => ChooseDeadParallel(t));
            }
            Task.WaitAll(tasks);
            for (int i = 0; i < ThreadCounter; i++)
            {
                int t = i;
                tasks[i] = Task.Run(() => DeleteParallel(t));
            }
            Task.WaitAll(tasks);
        }

        private void DeleteParallel(int number)
        {
            Monitor.Enter(Units);
            foreach (Unit unit in delList[number])
                DeathEvent.KnockKnock(unit, unit.Coords);
            Monitor.Exit(Units);
            delList[number].Clear();
        }

        private void ChooseDeadParallel(int number)
        {
            int bottom = number * Units.Count / ThreadCounter;
            int top = (number + 1) * Units.Count / ThreadCounter;
            for (int i = bottom; i < top; i++)
            {
                var unit = Units[i];
                if (unit.Status == UnitStatus.Dead)
                    delList[number].Add(unit);
            }
        }

        private void DeleteUnit(object sender, DeathEventArgs e)
        {
            var unit = (Unit)sender;
            Units.Remove(unit);
            for (int i = 0; i < unit.Size; i++)
                for (int j = 0; j < unit.Size; j++)
                    map[e.Position[0] + i, e.Position[1] + j] = null;
        }

        private void RecountEnergyParallel()
        {
            for (int i = 0; i < ThreadCounter; i++)
            {
                int t = i;
                tasks[i] = Task.Run(() => RecountEnergy(t));
            }
            Task.WaitAll(tasks);
        }

        private void RecountEnergy(int number)
        {
            int bottom = number * Units.Count / ThreadCounter;
            int top = (number + 1) * Units.Count / ThreadCounter;
            for (int i = bottom; i < top; i++)
            {
                var unit = Units[i];
                if (unit.Status != UnitStatus.Corpse)
                    unit.TakeEnergy((SunEnergyMap[unit.Coords[1]] + GroundEnergyMap[unit.Coords[1] + unit.Size - 1]) * unit.Size + EnergyDegradation);
                else
                    unit.TakeEnergy(-(SunEnergyMap[unit.Coords[1]] + GroundEnergyMap[unit.Coords[1] + unit.Size - 1]) * unit.Size + EnergyDegradation);
            }
        }

        private void RecountEnergy()
        {
            foreach (Unit unit in Units)
            {
                if (unit.Status != UnitStatus.Corpse)
                    unit.TakeEnergy(SunEnergyMap[unit.Coords[1]] + GroundEnergyMap[unit.Coords[1]] + EnergyDegradation * unit.Size);
                else
                    unit.TakeEnergy(-(SunEnergyMap[unit.Coords[1]] + GroundEnergyMap[unit.Coords[1]]) + EnergyDegradation * unit.Size);
            }
        }


        private void ClearAddQueue(int number)
        {
            List<Unit> accepted = new List<Unit>();
            for (int k = number * queueAdd.Count / ThreadCounter; k < (number + 1) * queueAdd.Count / ThreadCounter; k++)
            {
                var parent = queueAdd[k];
                if (parent.Energy > Swamp.EnergyLimit * parent.Size / 50)
                {
                    var child = Creator.CreateChild(parent);
                    if (child != null)
                    {
                        accepted.Add(child);
                        for (int i = 0; i < child.Size; i++)
                            for (int j = 0; j < child.Size; j++)
                                map[child.Coords[0] + i, child.Coords[1] + j] = child;

                        parent.TakeEnergy(-parent.Energy / 2 - EnergyLimit / 100);
                    }
                    else if (parent.Energy > EnergyLimit)
                        parent.TakeEnergy(EnergyLimit - parent.Energy);
                }
            }
            Monitor.Enter(Units);
            Units.AddRange(accepted);
            Monitor.Exit(Units);
        }

        private void AddUnit(object sender, DivideEventArgs e)
        {
            var parent = (Unit)sender;
            Monitor.Enter(queueAdd);
            queueAdd.Add(parent);
            Monitor.Exit(queueAdd);

        }

        public void UpdateParameters(int groundPower, int sunPower, double dropChance, double envDensity)
        {
            GroundPower = groundPower;
            SunPower = sunPower;
            DropChance = dropChance;
            EnvDensity = envDensity;

            for (int i = 0; i < WorldHeight; i++)
            {
                SunEnergyMap[i] = (int)(SunPower * Math.Pow(EnvDensity, i));
                GroundEnergyMap[i] = (int)(GroundPower * Math.Pow(EnvDensity, WorldHeight - i - 1));
            }
        }

        private void MoveUnit(object sender, MoveEventArgs e)
        {
            var unit = (Unit)sender;

            for (int i = 0; i < unit.Size; i++)
                for (int j = 0; j < unit.Size; j++)
                    map[e.OldPosition[0] + i, e.OldPosition[1] + j] = null;
            for (int i = 0; i < unit.Size; i++)
                for (int j = 0; j < unit.Size; j++)
                    map[e.NewPosition[0] + i, e.NewPosition[1] + j] = unit;
        }
    }
}
