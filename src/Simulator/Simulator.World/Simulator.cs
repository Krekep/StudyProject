using SFML.Graphics;
using SFML.System;

using Simulator.World;

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Simulator
{
    public enum TypeOfMap
    {
        MapOfEnergy,
        MapOfActions
    }

    public class Simulator : Transformable, Drawable
    {
        public const int ViewScale = 4;
        public const int WorldHeight = 100;
        public const int WorldWidth = 200;
        public const int EnergyLimit = 1000;
        public const int EnergyDegradation = -1;
        public const int WaitValue = 100;  // based point for other actions value

        // Changeable parameters
        public int GroundPower;
        public int SunPower;
        public double EnvDensity;
        public double DropChance;

        // World addition info
        public int Seed { get; private set; }
        public int Timer { get; private set; }

        // List of available units
        public List<Unit> Units { get; private set; }
        public TypeOfMap ChoosenMap { get; set; }

        private Unit[,] map;
        public static Random Random;
        private Text yearText;
        private Text countText;
        private Text seedText;

        // For display
        public const int LeftMapOffset = 10;
        public const int TopMapOffset = 50;
        private const int Left = 0 * ViewScale + LeftMapOffset;
        private const int Top = 0 * ViewScale + TopMapOffset;
        private const int Bottom = WorldHeight * ViewScale + 1 + TopMapOffset;
        private const int Right = WorldWidth * ViewScale + 1 + LeftMapOffset;

        public void Initialize(int seed)
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
                Unit unit = Creator.CreateUnit(2000, this);
                Units.Add(unit);
                map[unit.Coords[0], unit.Coords[1]] = unit;
            }

            yearText = new Text($"Year: {Timer}", Content.Font, Content.TextSize);
            yearText.Position = new Vector2f(Right + 20, Top + 10);
            countText = new Text($"Units: {Units.Count}", Content.Font, Content.TextSize);
            countText.Position = new Vector2f(Right + 20, Top + Content.TextSize + 5 + 10);
            seedText = new Text($"Seed: {seed}", Content.Font, Content.TextSize);
            seedText.Position = new Vector2f(Right + 20, Top + 2 * (Content.TextSize + 5) + 10);

            Storage.CurrentWorld = this;
        }

        public void Import(int seed, int timer, int groundPower, int sunPower, double envDensity, double dropChance, List<Unit> units)
        {
            Seed = seed;
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

        internal Unit GetUnit(int x, int y)
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
                unit.TakeEnergy((int)(GroundPower * Math.Pow(EnvDensity, WorldHeight - unit.Coords[1])));
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

        public void Draw(RenderTarget target, RenderStates states)
        {
            states.Transform *= Transform; 
            DrawBound(target);
            foreach (Drawable unit in Units)
                target.Draw(unit, states);
            DrawText(target);
        }

        private void DrawText(RenderTarget target)
        {
            yearText.DisplayedString = $"Year: {Timer}";
            countText.DisplayedString = $"Units: {Units.Count}";
            target.Draw(yearText);
            target.Draw(countText);
            target.Draw(seedText);
        }



        private Vertex[] bottomLine = new Vertex[2] { new Vertex(new Vector2f(Left, Bottom)),
                                                      new Vertex(new Vector2f(Right, Bottom))};
        private Vertex[] topLine = new Vertex[2] { new Vertex(new Vector2f(Left, Top)),
                                                   new Vertex(new Vector2f(Right, Top))};
        private Vertex[] leftLine = new Vertex[2] { new Vertex(new Vector2f(Left, Top)),
                                                    new Vertex(new Vector2f(Left, Bottom))};
        private Vertex[] rightLine = new Vertex[2] { new Vertex(new Vector2f(Right, Top)),
                                                     new Vertex(new Vector2f(Right, Bottom))};
        private void DrawBound(RenderTarget target)
        {
            target.Draw(topLine, PrimitiveType.Lines);
            target.Draw(bottomLine, PrimitiveType.Lines);
            target.Draw(leftLine, PrimitiveType.Lines);
            target.Draw(rightLine, PrimitiveType.Lines);
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

        public Unit MouseHandle(int x, int y)
        {
            if (new IntRect(LeftMapOffset, TopMapOffset, WorldWidth * ViewScale, WorldHeight * ViewScale).Contains(x, y))
            {
                var unit = GetUnit((x - LeftMapOffset) / ViewScale, (y - TopMapOffset) / ViewScale);
                return unit;
            }
            return null;
        }
    }
}
