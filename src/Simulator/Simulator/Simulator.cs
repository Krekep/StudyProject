using SFML.Graphics;
using SFML.System;

using System;
using System.Collections.Generic;
using System.Text;

namespace Simulator
{
    public static class Simulator
    {
        public const int Scale = 4;
        public const int WorldHeight = 100;
        public const int WorldWidth = 200;
        public const int EnergyLimit = 1000;
        public const int EnergyDegradation = -1;
        public const int WaitValue = 100;  // based point for other actions value

        // Changeable parameters
        public static int GroundPower;
        public static int SunPower;
        public static double EnvDensity;
        public static double DropChance;

        // World addition info
        public static int Seed { get; private set; }
        public static int Timer { get; private set; }

        // List of available units
        public static List<Unit> Units { get; private set; }

        private static Unit[,] map;
        public static Random Random;
        private static Text yearText;
        private static Text countText;
        private static Text seedText;
        private const int Left = 0 * Scale + Program.LeftMapOffset;
        private const int Top = 0 * Scale + Program.TopMapOffset;
        private const int Bottom = WorldHeight * Scale + 1 + Program.TopMapOffset;
        private const int Right = WorldWidth * Scale + 1 + Program.LeftMapOffset;

        public static void Initialize(int seed)
        {
            GroundPower = 5;
            SunPower = 5;
            EnvDensity = 0.80;
            DropChance = 0.05;

            Seed = seed;
            Random = new Random(Seed);
            Timer = 0;

            map = new Unit[WorldWidth, WorldHeight];
            Units = new List<Unit>();

            for (int i = 0; i < 500; i++)
            {
                Unit unit = Creator.CreateUnit(2000);
                Units.Add(unit);
                map[unit.Coords[0], unit.Coords[1]] = unit;
            }

            yearText = new Text($"Year: {Timer}", Content.Font, Program.TextSize);
            yearText.Position = new Vector2f(Right + 20, Top + 10);
            countText = new Text($"Units: {Units.Count}", Content.Font, Program.TextSize);
            countText.Position = new Vector2f(Right + 20, Top + Program.TextSize + 5 + 10);
            seedText = new Text($"Seed: {seed}", Content.Font, Program.TextSize);
            seedText.Position = new Vector2f(Right + 20, Top + 2 * (Program.TextSize + 5) + 10);
        }

        public static void Import(int seed, int timer, int groundPower, int sunPower, double envDensity, double dropChance, List<Unit> units)
        {
            Seed = seed;
            Program.RandomSeed = seed;
            seedText.DisplayedString = $"Seed: {seed}";
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
            Draw();
        }

        internal static bool IsFree(int x, int y)
        {
            if (x < 0 || y < 0 || x >= WorldWidth || y >= WorldHeight)
                return false;
            if (map[x, y] != null)
                return false;
            return true;
        }

        internal static Unit GetUnit(int x, int y)
        {
            if (x < 0 || y < 0 || x >= WorldWidth || y >= WorldHeight)
                return null;
            return map[x, y];
        }

        public static void Update()
        {
            Timer += 1;
            foreach (Unit unit in Units)
                unit.Process();
            RecountEnergy();
            DeleteDeadCells();
            AddNewCells();
            DropCells();
        }

        private static void DropCells()
        {
            foreach (Unit unit in Units)
                if (Random.Next(101) / 100.0 < DropChance)
                    unit.Move(0, 1);
        }

        private static void AddNewCells()
        {
            List<Unit> addList = new List<Unit>();
            foreach (Unit unit in Units)
                if (unit.Status == UnitStatus.Divide)
                    addList.Add(unit);
            foreach (Unit unit in addList)
                unit.Divide();
        }

        private static void DeleteDeadCells()
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

        private static void RecountEnergy()
        {
            foreach (Unit unit in Units)
                unit.TakeEnergy((int)(GroundPower * Math.Pow(EnvDensity, WorldHeight - unit.Coords[1])));
            foreach (Unit unit in Units)
                unit.TakeEnergy(EnergyDegradation);
            //foreach (Unit unit in units)
            //    if (!CheckOverpopulation(unit))
            //        unit.TakeEnergy(-EnergyLimit / 20);
        }

        private static bool CheckOverpopulation(Unit unit)
        {
            for (int i = 0; i < 9; i++)
            {
                if (IsFree(unit.Coords[0] + i / 3 - 1, unit.Coords[1] + i % 3 - 1))
                    return true;
            }
            return false;
        }

        internal static void AddUnit(Unit child)
        {
            map[child.Coords[0], child.Coords[1]] = child;
            Units.Add(child);
        }

        public static void Draw()
        {
            DrawBound();
            foreach (Drawable unit in Units)
                Program.Window.Draw(unit);
            DrawText();
        }

        private static void DrawText()
        {
            yearText.DisplayedString = $"Year: {Timer}";
            countText.DisplayedString = $"Units: {Units.Count}";
            Program.Window.Draw(yearText);
            Program.Window.Draw(countText);
            Program.Window.Draw(seedText);
        }



        private static Vertex[] bottomLine = new Vertex[2] { new Vertex(new Vector2f(Left, Bottom)),
                                                             new Vertex(new Vector2f(Right, Bottom))};
        private static Vertex[] topLine = new Vertex[2] { new Vertex(new Vector2f(Left, Top)),
                                                          new Vertex(new Vector2f(Right, Top))};
        private static Vertex[] leftLine = new Vertex[2] { new Vertex(new Vector2f(Left, Top)),
                                                           new Vertex(new Vector2f(Left, Bottom))};
        private static Vertex[] rightLine = new Vertex[2] { new Vertex(new Vector2f(Right, Top)),
                                                            new Vertex(new Vector2f(Right, Bottom))};
        private static void DrawBound()
        {
            Program.Window.Draw(topLine, PrimitiveType.Lines);
            Program.Window.Draw(bottomLine, PrimitiveType.Lines);
            Program.Window.Draw(leftLine, PrimitiveType.Lines);
            Program.Window.Draw(rightLine, PrimitiveType.Lines);
        }

        internal static void UpdateParameters(int groundPower, int sunPower, double dropChance, double envDensity)
        {
            GroundPower = groundPower;
            SunPower = sunPower;
            DropChance = dropChance;
            EnvDensity = envDensity;
        }

        internal static void MoveUnit(Unit unit, int[] oldPosition, int[] newPosition)
        {
            map[oldPosition[0], oldPosition[1]] = null;
            map[newPosition[0], newPosition[1]] = unit;
        }
    }
}
