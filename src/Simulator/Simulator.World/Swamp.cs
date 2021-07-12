using Simulator.Events;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Simulator.World
{
    public class Swamp
    {
        private static int threadCounter = Environment.ProcessorCount - 1;
        private Task[] tasks;
        private int[,] chunkCoords;
        private List<HashSet<Unit>> chunks;
        private HashSet<Unit> outOfChunks;

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
        public HashSet<Unit> Units { get; private set; }

        private Unit[,] map;

        private int[] sunEnergyMap = new int[WorldHeight];
        private int[] groundEnergyMap = new int[WorldHeight];

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

            tasks = new Task[threadCounter];
            chunkCoords = new int[threadCounter, 2];
            chunks = new List<HashSet<Unit>>();
            outOfChunks = new HashSet<Unit>();
            for (int i = 0; i < threadCounter; i++)
            {
                int top = i * (WorldHeight - 1) / threadCounter + 2;
                int bottom = (i + 1) * (WorldHeight - 1) / threadCounter - 1;
                chunkCoords[i, 0] = top;
                chunkCoords[i, 1] = bottom;
                chunks.Add(new HashSet<Unit>());
            }

            for (int i = 0; i < WorldHeight; i++)
            {
                sunEnergyMap[i] = (int)(SunPower * Math.Pow(EnvDensity, i));
                groundEnergyMap[i] = (int)(GroundPower * Math.Pow(EnvDensity, WorldHeight - i - 1));
            }

            map = new Unit[WorldWidth, WorldHeight];
            Units = new HashSet<Unit>();

            for (int i = 0; i < 500; i++)
            {
                Unit unit = Creator.CreateUnit(5000, i);
                map[unit.Coords[0], unit.Coords[1]] = unit;

                bool inChunk = false;
                for (int j = 0; j < threadCounter; j++)
                {
                    if (chunkCoords[j, 0] <= unit.Coords[1] && unit.Coords[1] <= chunkCoords[j, 1])
                    {
                        chunks[j].Add(unit);
                        inChunk = true;
                        break;
                    }
                }
                if (!inChunk)
                    outOfChunks.Add(unit);
            }
            foreach (HashSet<Unit> chunk in chunks)
                Units.UnionWith(chunk);
            Units.UnionWith(outOfChunks);
        }

        public void Import(int seed, int timer, int groundPower, int sunPower, double envDensity, double dropChance, HashSet<Unit> units)
        {
            Seed = seed;
            Timer = timer;

            GroundPower = groundPower;
            SunPower = sunPower;
            EnvDensity = envDensity;
            DropChance = dropChance;
            Units.Clear();
            for (int i = 0; i < threadCounter; i++)
            {
                chunks[i].Clear();
            }
            outOfChunks.Clear();
            Units = units;
            map = new Unit[WorldWidth, WorldHeight];
            foreach (Unit unit in units)
            {
                map[unit.Coords[0], unit.Coords[1]] = unit;
                if (!InChunk(unit.Coords[1]))
                    outOfChunks.Add(unit);
                else
                {
                    int k = unit.Coords[1] * threadCounter / WorldHeight;
                    chunks[k].Add(unit);
                }
            }

            for (int i = 0; i < WorldHeight; i++)
            {
                sunEnergyMap[i] = (int)(SunPower * Math.Pow(EnvDensity, i));
                groundEnergyMap[i] = (int)(GroundPower * Math.Pow(EnvDensity, WorldHeight - i - 1));
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


        public void UpdateByThreads()
        {
            Timer += 1;
            List<Unit> temp = new List<Unit>(outOfChunks);
            for (int i = 0; i < threadCounter; i++)
            {
                int t = i;
                tasks[i] = Task.Run(() => UpdateRows(t));
            }
            Task.WaitAll(tasks);
            foreach (Unit unit in temp)
            {
                unit.Process();
            }

            //RecountEnergy();
            RecountEnergyParallel();
            DeleteDeadCells();
            AddNewCells();
            DropCells();
        }

        public void UpdateRows(int number)
        {
            int temp = (int)number;
            List<Unit> chunk = new List<Unit>(chunks[temp]);
            foreach (Unit unit in chunk)
            {
                unit.Process();
            }
        }

        private void DropCells()
        {
            foreach (Unit unit in Units)
                if (PseudoRandom.Next(101) / 100.0 < DropChance)
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
                DeathEvent.KnockKnock(unit, unit.Coords);
            }
        }

        private void DeleteUnit(object sender, DeathEventArgs e)
        {
            var unit = (Unit)sender;
            Units.Remove(unit);
            map[e.Position[0], e.Position[1]] = null;

            if (!InChunk(unit.Coords[1]))
                RemoveOutOfChunks(unit);
            else
            {
                int k = unit.Coords[1] * threadCounter / WorldHeight;
                chunks[k].Remove(unit);
            }
        }

        private void RecountEnergyParallel()
        {
            for (int i = 0; i < threadCounter; i++)
            {
                int t = i;
                tasks[i] = Task.Run(() => RecountEnergy(t));
            }
            RecountEnergy(-1);
            Task.WaitAll(tasks);
        }

        /// <summary>
        /// Adds energy to each unit, depending on its position. Accepts a number of chunk as a parameter. -1 is outOfChunks, threadCounter is all Units. 
        /// </summary>
        /// <param name="units"></param>
        private void RecountEnergy(int number)
        {
            HashSet<Unit> units = null;
            if (number == -1)
            {
                units = outOfChunks;
            }
            if (number == threadCounter)
            {
                units = Units;
            }
            if (0 <= number && number < threadCounter)
            {
                units = chunks[number];
            }
            foreach (Unit unit in units)
            {
                if (unit.Status != UnitStatus.Corpse)
                {
                    unit.TakeEnergy(sunEnergyMap[unit.Coords[1]]);
                    unit.TakeEnergy(groundEnergyMap[unit.Coords[1]]);
                }
                else
                {
                    unit.TakeEnergy(-sunEnergyMap[unit.Coords[1]]);
                    unit.TakeEnergy(-groundEnergyMap[unit.Coords[1]]);
                }

                unit.TakeEnergy(EnergyDegradation);
            }
            //foreach (Unit unit in units)
            //    if (!CheckOverpopulation(unit))
            //        unit.TakeEnergy(-EnergyLimit / 20);
        }

        private void RecountEnergy()
        {
            foreach (Unit unit in Units)
            {
                if (unit.Status != UnitStatus.Corpse)
                {
                    unit.TakeEnergy(sunEnergyMap[unit.Coords[1]]);
                    unit.TakeEnergy(groundEnergyMap[unit.Coords[1]]);
                }
                else
                {
                    unit.TakeEnergy(-sunEnergyMap[unit.Coords[1]]);
                    unit.TakeEnergy(-groundEnergyMap[unit.Coords[1]]);
                }
                unit.TakeEnergy(EnergyDegradation);
            }
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

        private void AddUnit(object sender, DivideEventArgs e)
        {
            var child = (Unit)sender;
            Units.Add(child);
            map[e.Position[0], e.Position[1]] = child;

            if (!InChunk(e.Position[1]))
                AddOutOfChunks(child);
            else
            {
                int k = e.Position[1] * threadCounter / WorldHeight;
                chunks[k].Add(child);
            }
        }

        public void UpdateParameters(int groundPower, int sunPower, double dropChance, double envDensity)
        {
            GroundPower = groundPower;
            SunPower = sunPower;
            DropChance = dropChance;
            EnvDensity = envDensity;

            for (int i = 0; i < WorldHeight; i++)
            {
                sunEnergyMap[i] = (int)(SunPower * Math.Pow(EnvDensity, i));
                groundEnergyMap[i] = (int)(GroundPower * Math.Pow(EnvDensity, WorldHeight - i - 1));
            }
        }

        private void MoveUnit(object sender, MoveEventArgs e)
        {
            var unit = (Unit)sender;
            map[e.OldPosition[0], e.OldPosition[1]] = null;
            map[e.NewPosition[0], e.NewPosition[1]] = unit;


            bool oldInChunk = InChunk(e.OldPosition[1]);
            bool newInChunk = InChunk(e.NewPosition[1]);
            if (oldInChunk)
            {
                if (!newInChunk)
                {
                    AddOutOfChunks(unit);
                    int k = ChunkIdx(e.OldPosition[1]);
                    chunks[k].Remove(unit);
                }
            }
            else
            {
                if (newInChunk)
                {
                    int k = ChunkIdx(e.NewPosition[1]);
                    chunks[k].Add(unit);
                    RemoveOutOfChunks(unit);
                }
            }
        }

        private static int ChunkIdx(int y)
        {
            return y * threadCounter / WorldHeight;
        }

        private static bool InChunk(int y)
        {
            return (y % ((WorldHeight - 1) / threadCounter) != 0) && (y % ((WorldHeight - 1) / threadCounter) != 1);
        }

        private void RemoveOutOfChunks(Unit unit)
        {
            Monitor.Enter(outOfChunks);
            outOfChunks.Remove(unit);
            Monitor.Exit(outOfChunks);
        }

        private void AddOutOfChunks(Unit unit)
        {
            Monitor.Enter(outOfChunks);
            outOfChunks.Add(unit);
            Monitor.Exit(outOfChunks);
        }
    }
}
