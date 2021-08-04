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
        private List<HashSet<int>> chunks;
        private HashSet<int> outOfChunks;

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

        private int[,] mapNumbers;
        public static Random Random;
        public UnitDataBase Units { get; private set; }

        private int[] sunEnergyMap = new int[WorldHeight];
        private int[] groundEnergyMap = new int[WorldHeight];
        public void Initialize(int seed)
        {
            Storage.CurrentWorld = this;
            MoveEvent.Notify += MoveUnit;
            DivideEvent.Notify += AddUnit;
            DeathEvent.Notify += DeleteUnit;
            GroundPower = 20;
            SunPower = 15;
            EnvDensity = 0.8;
            DropChance = 0.12;

            Seed = seed;
            Random = new Random(Seed);
            Timer = 0;

            tasks = new Task[threadCounter];
            chunkCoords = new int[threadCounter, 2];
            chunks = new List<HashSet<int>>();
            outOfChunks = new HashSet<int>();
            for (int i = 0; i < threadCounter; i++)
            {
                int top = i * (WorldHeight - 1) / threadCounter + 2;
                int bottom = (i + 1) * (WorldHeight - 1) / threadCounter - 1;
                chunkCoords[i, 0] = top;
                chunkCoords[i, 1] = bottom;
                chunks.Add(new HashSet<int>());
            }

            for (int i = 0; i < WorldHeight; i++)
            {
                sunEnergyMap[i] = (int)(SunPower * Math.Pow(EnvDensity, i));
                groundEnergyMap[i] = (int)(GroundPower * Math.Pow(EnvDensity, WorldHeight - i - 1));
            }

            mapNumbers = new int[WorldWidth, WorldHeight];
            for (int i = 0; i < WorldWidth; i++)
                for (int j = 0; j < WorldHeight; j++)
                    mapNumbers[i, j] = -1;
            Units = new UnitDataBase(500);
            foreach (int unit in Units.UnitsNumbers)
            {
                mapNumbers[Units.UnitsCoords[unit][0], Units.UnitsCoords[unit][1]] = unit;

                bool inChunk = false;
                for (int j = 0; j < threadCounter; j++)
                {
                    if (chunkCoords[j, 0] <= Units.UnitsCoords[unit][1] && Units.UnitsCoords[unit][1] <= chunkCoords[j, 1])
                    {
                        chunks[j].Add(unit);
                        inChunk = true;
                        break;
                    }
                }
                if (!inChunk)
                    outOfChunks.Add(unit);
            }

        }

        public void Import(int seed, int timer, int groundPower, int sunPower, double envDensity, double dropChance, List<(int, int, int, int, int[], int, int, int[], int[], IAction[][], UnitStatus)> units)
        {
            Seed = seed;
            Timer = timer;
            Random = new Random(seed);

            GroundPower = groundPower;
            SunPower = sunPower;
            EnvDensity = envDensity;
            DropChance = dropChance;

            Units.Import(units);
            mapNumbers = new int[WorldWidth, WorldHeight];
            for (int i = 0; i < WorldWidth; i++)
                for (int j = 0; j < WorldHeight; j++)
                    mapNumbers[i, j] = -1;

            foreach (int unit in Units.UnitsNumbers)
                mapNumbers[Units.UnitsCoords[unit][0], Units.UnitsCoords[unit][1]] = unit;

            for (int i = 0; i < WorldHeight; i++)
            {
                sunEnergyMap[i] = (int)(SunPower * Math.Pow(EnvDensity, i));
                groundEnergyMap[i] = (int)(GroundPower * Math.Pow(EnvDensity, WorldHeight - i - 1));
            }

            for (int i = 0; i < threadCounter; i++)
            {
                chunks[i].Clear();
            }
            outOfChunks.Clear();
            foreach (int unit in Units.UnitsNumbers)
            {
                if (!InChunk(Units.UnitsCoords[unit][1]))
                    outOfChunks.Add(unit);
                else
                {
                    int k = Units.UnitsCoords[unit][1] * threadCounter / WorldHeight;
                    chunks[k].Add(unit);
                }
            }
            //Storage.CurrentWorld = this;
        }

        internal bool IsFree(int x, int y)
        {
            if (x < 0 || y < 0 || x >= WorldWidth || y >= WorldHeight)
                return false;
            if (mapNumbers[x, y] != -1)
                return false;
            return true;
        }

        public int GetUnit(int x, int y)
        {
            if (x < 0 || y < 0 || x >= WorldWidth || y >= WorldHeight)
                return -1;
            return mapNumbers[x, y];
        }

        public void Update()
        {
            Timer += 1;
            foreach (int unit in Units.UnitsNumbers)
            {
                if (Units.UnitsStatus[unit] != UnitStatus.Corpse)
                    Units.Process(unit);
            }
            RecountEnergy();
            DeleteDeadCells();
            AddNewCells();
            DropCells();
        }


        public void UpdateByThreads()
        {
            Timer += 1;
            List<int> temp = new List<int>(outOfChunks);
            for (int i = 0; i < threadCounter; i++)
            {
                int t = i;
                tasks[i] = Task.Run(() => UpdateRows(t));
            }
            Task.WaitAll(tasks);
            foreach (int unit in temp)
            {
                if (Units.UnitsStatus[unit] != UnitStatus.Corpse)
                    Units.Process(unit);
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
            List<int> chunk = new List<int>(chunks[temp]);
            foreach (int unit in chunk)
            {
                if (Units.UnitsStatus[unit] != UnitStatus.Corpse)
                    Units.Process(unit);
            }
        }

        private void DropCells()
        {
            foreach (int unit in Units.UnitsNumbers)
                if (PseudoRandom.Next(101) / 100.0 < DropChance)
                    Units.MoveUnit(unit, 0, 1);
        }

        private void AddNewCells()
        {
            List<int> addList = new List<int>();
            foreach (int unit in Units.UnitsNumbers)
                if (Units.UnitsStatus[unit] == UnitStatus.Divide)
                    addList.Add(unit);
            foreach (int unit in addList)
                Units.Divide(unit);
        }

        private void DeleteDeadCells()
        {
            List<int> delList = new List<int>();
            foreach (int unit in Units.UnitsNumbers)
                if (Units.UnitsStatus[unit] == UnitStatus.Dead)
                    delList.Add(unit);
            foreach (int unit in delList)
            {
                DeathEvent.KnockKnock(unit, Units.UnitsCoords[unit]);
            }
        }

        private void RecountEnergy()
        {
            foreach (int unit in Units.UnitsNumbers)
            {
                int ground = groundEnergyMap[Units.UnitsCoords[unit][1]];
                int sun = sunEnergyMap[Units.UnitsCoords[unit][1]];
                if (Units.UnitsStatus[unit] != UnitStatus.Corpse)
                {
                    Units.TakeEnergy(unit, sun);
                    Units.TakeEnergy(unit, ground);
                }
                else
                {
                    Units.TakeEnergy(unit, -sun);
                    Units.TakeEnergy(unit, -ground);
                }
            }
            foreach (int unit in Units.UnitsNumbers)
                Units.TakeEnergy(unit, EnergyDegradation);
            //foreach (Unit unit in units)
            //    if (!CheckOverpopulation(unit))
            //        unit.TakeEnergy(-EnergyLimit / 20);
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
            HashSet<int> units = null;
            if (number == -1)
            {
                units = outOfChunks;
            }
            if (number == threadCounter)
            {
                units = Units.UnitsNumbers;
            }
            if (0 <= number && number < threadCounter)
            {
                units = chunks[number];
            }
            foreach (int unit in units)
            {
                if (Units.UnitsStatus[unit] != UnitStatus.Corpse)
                {
                    Units.TakeEnergy(unit, sunEnergyMap[Units.UnitsCoords[unit][1]]);
                    Units.TakeEnergy(unit, groundEnergyMap[Units.UnitsCoords[unit][1]]);
                }
                else
                {
                    Units.TakeEnergy(unit, -sunEnergyMap[Units.UnitsCoords[unit][1]]);
                    Units.TakeEnergy(unit, -groundEnergyMap[Units.UnitsCoords[unit][1]]);
                }

                Units.TakeEnergy(unit, EnergyDegradation);
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
            var childNumber = (int)sender;
            mapNumbers[e.Position[0], e.Position[1]] = childNumber;

            // WTF???
            if (!InChunk(e.Position[1]))
                AddOutOfChunks(childNumber);
            else
            {
                int k = e.Position[1] * threadCounter / WorldHeight;
                chunks[k].Add(childNumber);
            }
        }

        private void DeleteUnit(object sender, DeathEventArgs e)
        {
            var unit = (int)sender; 
            mapNumbers[e.Position[0], e.Position[1]] = -1;
            if (!InChunk(Units.UnitsCoords[unit][1]))
                RemoveOutOfChunks(unit);
            else
            {
                int k = Units.UnitsCoords[unit][1] * threadCounter / WorldHeight;
                chunks[k].Remove(unit);
            }
            Units.Delete(unit);
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
            var unit = (int)sender;
            mapNumbers[e.OldPosition[0], e.OldPosition[1]] = -1;
            mapNumbers[e.NewPosition[0], e.NewPosition[1]] = unit;


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

        private void RemoveOutOfChunks(int unit)
        {
            Monitor.Enter(outOfChunks);
            outOfChunks.Remove(unit);
            Monitor.Exit(outOfChunks);
        }

        private void AddOutOfChunks(int unit)
        {
            Monitor.Enter(outOfChunks);
            outOfChunks.Add(unit);
            Monitor.Exit(outOfChunks);
        }
    }
}
