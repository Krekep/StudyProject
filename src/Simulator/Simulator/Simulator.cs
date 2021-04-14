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
        public const int GroundPower = 20;
        public const double EnvDensity = 0.80;

        private static int timer;
        private static byte[,] map;
        private static List<Unit> units;
        public static Random Random;
        private static Text year;
        private static Text count;

        static Simulator()
        {
            Random = new Random(Program.RandomSeed);
            timer = 0;

            map = new byte[WorldHeight, WorldWidth];
            units = new List<Unit>();

            for (int i = 0; i < 500; i++)
            {
                Unit unit = Creator.CreateUnit();
                units.Add(unit);
                map[unit.position[0], unit.position[1]] = 1;
            }

            year = new Text($"Year: {timer}", Program.Font);
            year.Position = new Vector2f((WorldWidth + 5) * Scale, 10);
            count = new Text($"Units: {units.Count}", Program.Font);
            count.Position = new Vector2f((WorldWidth + 5) * Scale, 40);
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
        }

        private static void DeleteDeadCells()
        {
            List<Unit> delList = new List<Unit>();
            foreach (Unit unit in units)
                if (!unit.IsAlive)
                    delList.Add(unit);
            foreach (Unit unit in delList)
                units.Remove(unit);
        }

        private static void RecountEnergy()
        {
            foreach (Unit unit in units)
                unit.TakeEnergy((int)(GroundPower * Math.Pow(EnvDensity, WorldHeight - unit.position[0])));
            foreach (Unit unit in units)
                unit.TakeEnergy(EnergyDegradation);
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
        }

        private static void DrawBound()
        {

            Vertex[] bottomLine = new Vertex[2] { new Vertex(new Vector2f(0 * Scale, WorldHeight * Scale + 1)),
                                                  new Vertex(new Vector2f(WorldWidth * Scale + 1, WorldHeight * Scale + 1))};
            Vertex[] topLine = new Vertex[2] { new Vertex(new Vector2f(0 * Scale, 0 * Scale)),
                                               new Vertex(new Vector2f(WorldWidth * Scale, 0 * Scale))};
            Vertex[] leftLine = new Vertex[2] { new Vertex(new Vector2f(0 * Scale, 0 * Scale)),
                                                new Vertex(new Vector2f(0 * Scale, WorldHeight * Scale))};
            Vertex[] rightLine = new Vertex[2] { new Vertex(new Vector2f(WorldWidth * Scale + 1, 0 * Scale + 1)),
                                                 new Vertex(new Vector2f(WorldWidth * Scale + 1, WorldHeight * Scale + 1))};

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
