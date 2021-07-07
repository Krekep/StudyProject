using System;
using System.Collections.Generic;

namespace Simulator.World
{
    public class Swamp
    {
        public const int WorldHeight = 100;
        public const int WorldWidth = 200;
        public const int EnergyLimit = 1500;
        public const int DeathLimit = -300;
        public const int EnergyDegradation = -1;
        /// <summary>
        /// Based point for other actions value
        /// </summary>
        public const int WaitValue = 100;
        public const int AttackLimit = 20;
        public const int ChlorophylLimit = 20;
        public const int AttackValue = 100;
        public const int ChlorophylValue = 75;

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

        private Unit[,] map;
        public static Random Random;

        public void Initialize(int seed)
        {
            GroundPower = 7;
            SunPower = 5;
            EnvDensity = 0.85;
            DropChance = 0.1;

            Seed = seed;
            Random = new Random(Seed);
            Timer = 0;

            map = new Unit[WorldWidth, WorldHeight];
            Units = new List<Unit>();

            for (int i = 0; i < 500; i++)
            {
                Unit unit = Creator.CreateUnit(5000, i);
                Units.Add(unit);
                map[unit.Coords[0], unit.Coords[1]] = unit;
            }

            Storage.CurrentWorld = this;
        }

        public void Import(int seed, int timer, int groundPower, int sunPower, double envDensity, double dropChance, List<Unit> units)
        {
            Seed = seed;
            Timer = timer;
            Random = new Random(seed);

            GroundPower = groundPower;
            SunPower = sunPower;
            EnvDensity = envDensity;
            DropChance = dropChance;
            Units.Clear();
            Units = units;
            map = new Unit[WorldWidth, WorldHeight];
            foreach (Unit unit in units)
            {
                map[unit.Coords[0], unit.Coords[1]] = unit;
            }

            //Storage.CurrentWorld = this;
        }

        internal bool IsFree(int x, int y)
        {
            if (x < 0 || y < 0 || x >= WorldWidth || y >= WorldHeight)
                return false;
            if (map[x, y] != null)
                return false;
            return true;
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
            RecountEnergy();
            DeleteDeadCells();
            AddNewCells();
            DropCells();
        }

        private void DropCells()
        {
            foreach (Unit unit in Units)
                if (Random.Next(101) / 100.0 < DropChance)
                    unit.Move(0, 1);
        }

        private void AddNewCells()
        {
            List<Unit> addList = new List<Unit>();
            foreach (Unit unit in Units)
                if (unit.Status == UnitStatus.Divide)
                    addList.Add(unit);
            foreach (Unit unit in addList)
                unit.Divide();
        }

        private void DeleteDeadCells()
        {
            List<Unit> delList = new List<Unit>();
            foreach (Unit unit in Units)
                if (unit.Status == UnitStatus.Dead)
                    delList.Add(unit);
            foreach (Unit unit in delList)
            {
                Units.Remove(unit);
                map[unit.Coords[0], unit.Coords[1]] = null;
            }
        }

        private void RecountEnergy()
        {
            foreach (Unit unit in Units)
            {
                int ground = (int)(GroundPower * Math.Pow(EnvDensity, WorldHeight - unit.Coords[1]));
                int sun = (int)(SunPower * Math.Pow(EnvDensity, unit.Coords[1]));
                if (unit.Status != UnitStatus.Corpse)
                {
                    unit.TakeEnergy(sun);
                    unit.TakeEnergy(ground);
                }
                else
                {
                    unit.TakeEnergy(-sun);
                    unit.TakeEnergy(-ground);
                }
            }
            foreach (Unit unit in Units)
                unit.TakeEnergy(EnergyDegradation);
            //foreach (Unit unit in units)
            //    if (!CheckOverpopulation(unit))
            //        unit.TakeEnergy(-EnergyLimit / 20);
        }

        private bool CheckOverpopulation(Unit unit)
        {
            for (int i = 0; i < 9; i++)
            {
                if (IsFree(unit.Coords[0] + i / 3 - 1, unit.Coords[1] + i % 3 - 1))
                    return true;
            }
            return false;
        }

        internal void AddUnit(Unit child)
        {
            map[child.Coords[0], child.Coords[1]] = child;
            Units.Add(child);
        }


        public void UpdateParameters(int groundPower, int sunPower, double dropChance, double envDensity)
        {
            GroundPower = groundPower;
            SunPower = sunPower;
            DropChance = dropChance;
            EnvDensity = envDensity;
        }

        internal void MoveUnit(Unit unit, int[] oldPosition, int[] newPosition)
        {
            map[oldPosition[0], oldPosition[1]] = null;
            map[newPosition[0], newPosition[1]] = unit;
        }
    }
}
