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
        public const int WorldHeight = 50;
        public const int WorldWidth = 100;
        public const int EnergyLimit = 1000;
        public const int EnergyDegradation = -1;
        public const int GroundPower = 5;
        public const int SunPower = 7;
        public const double EnvDensity = 0.80;
        public const int WaitValue = 100;  // based point for other actions value
        public const double DropChance = 0.05;

        private static int timer;
        private static byte[,] map;
        private static List<Unit> units;
        public static Random Random;
        private static Text year;
        private static Text count;
        private static Text seed;
        private const int Left = 0 * Scale + Program.LeftMapOffset;
        private const int Top = 0 * Scale + Program.TopMapOffset;
        private const int Bottom = WorldHeight * Scale + 1 + Program.TopMapOffset;
        private const int Right = WorldWidth * Scale + 1 + Program.LeftMapOffset;

        static Simulator()
        {
            Random = new Random(Program.RandomSeed);
            timer = 0;

            map = new byte[WorldHeight, WorldWidth];
            units = new List<Unit>();

            for (int i = 0; i < 50; i++)
            {
                Unit unit = Creator.CreateUnit(2000);
                units.Add(unit);
                map[unit.position[0], unit.position[1]] = 1;
            }

            year = new Text($"Year: {timer}", Content.Font);
            year.Position = new Vector2f(Right + 20, Top + 10);
            count = new Text($"Units: {units.Count}", Content.Font);
            count.Position = new Vector2f(Right + 20, Top + 40);
            seed = new Text($"Seed: {Program.RandomSeed}", Content.Font);
            seed.Position = new Vector2f(Right + 20, Top + 70);
        }

        internal static bool IsFree(int x, int y)
        {
            if (x < 0 || y < 0 || x >= WorldHeight || y >= WorldWidth)
                return false;
            if (map[x, y] == 0)
                return true;
            return false;
        }

        public static void Update()
        {
            timer += 1;
            foreach (Unit unit in units)
                unit.Process();
            RecountEnergy();
            DeleteDeadCells();
            AddNewCells();
            DropCells();
        }

        private static void DropCells()
        {
            foreach (Unit unit in units)
                if (Random.Next(101) / 100.0 < DropChance)
                    unit.Move(1, 0);
        }

        private static void AddNewCells()
        {
            List<Unit> addList = new List<Unit>();
            foreach (Unit unit in units)
                if (unit.Status == UnitStatus.Divide)
                    addList.Add(unit);
            foreach (Unit unit in addList)
                unit.Divide();
        }

        private static void DeleteDeadCells()
        {
            List<Unit> delList = new List<Unit>();
            foreach (Unit unit in units)
                if (unit.Status == UnitStatus.Dead)
                    delList.Add(unit);
            foreach (Unit unit in delList)
            {
                units.Remove(unit);
                map[unit.position[0], unit.position[1]] = 0;
            }
        }

        private static void RecountEnergy()
        {
            foreach (Unit unit in units)
                unit.TakeEnergy((int)(GroundPower * Math.Pow(EnvDensity, WorldHeight - unit.position[0])));
            foreach (Unit unit in units)
                unit.TakeEnergy(EnergyDegradation);
            //foreach (Unit unit in units)
            //    if (!CheckOverpopulation(unit))
            //        unit.TakeEnergy(-EnergyLimit / 20);
        }

        private static bool CheckOverpopulation(Unit unit)
        {
            for (int i = 0; i < 9; i++)
            {
                if (IsFree(unit.position[0] + i / 3 - 1, unit.position[1] + i % 3 - 1))
                    return true;
            }
            return false;
        }

        internal static void AddUnit(Unit child)
        {
            map[child.position[0], child.position[1]] = 1;
            units.Add(child);
        }

        public static void Draw()
        {
            DrawBound();
            foreach (Drawable unit in units)
                Program.Window.Draw(unit);
            DrawText();
        }

        private static void DrawText()
        {
            year.DisplayedString = $"Year: {timer}";
            count.DisplayedString = $"Units: {units.Count}";
            Program.Window.Draw(year);
            Program.Window.Draw(count);
            Program.Window.Draw(seed);
        }

        private static void DrawBound()
        {

            Vertex[] bottomLine = new Vertex[2] { new Vertex(new Vector2f(Left, Bottom)),
                                                  new Vertex(new Vector2f(Right, Bottom))};
            Vertex[] topLine = new Vertex[2] { new Vertex(new Vector2f(Left, Top)),
                                               new Vertex(new Vector2f(Right, Top))};
            Vertex[] leftLine = new Vertex[2] { new Vertex(new Vector2f(Left, Top)),
                                                new Vertex(new Vector2f(Left, Bottom))};
            Vertex[] rightLine = new Vertex[2] { new Vertex(new Vector2f(Right, Top)),
                                                 new Vertex(new Vector2f(Right, Bottom))};

            Program.Window.Draw(topLine, PrimitiveType.Lines);
            Program.Window.Draw(bottomLine, PrimitiveType.Lines);
            Program.Window.Draw(leftLine, PrimitiveType.Lines);
            Program.Window.Draw(rightLine, PrimitiveType.Lines);
        }

        internal static void MoveUnit(int[] oldPosition, int[] newPosition)
        {
            map[oldPosition[0], oldPosition[1]] = 0;
            map[newPosition[0], newPosition[1]] = 1;
        }
    }
}
