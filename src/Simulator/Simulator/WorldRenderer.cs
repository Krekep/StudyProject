using SFML.Graphics;
using SFML.System;
using Simulator.World;

using System.Collections.Generic;

namespace Simulator
{
    static class WorldRenderer
    {
        private const int Left = 0 * Program.ViewScale + Program.LeftMapOffset;
        private const int Top = 0 * Program.ViewScale + Program.TopMapOffset;
        private const int Bottom = Swamp.WorldHeight * Program.ViewScale + 1 + Program.TopMapOffset;
        private const int Right = Swamp.WorldWidth * Program.ViewScale + 1 + Program.LeftMapOffset;

        private static Vertex[] bottomLine = new Vertex[2] { new Vertex(new Vector2f(Left, Bottom)),
                                                      new Vertex(new Vector2f(Right, Bottom))};
        private static Vertex[] topLine = new Vertex[2] { new Vertex(new Vector2f(Left, Top)),
                                                   new Vertex(new Vector2f(Right, Top))};
        private static Vertex[] leftLine = new Vertex[2] { new Vertex(new Vector2f(Left, Top)),
                                                    new Vertex(new Vector2f(Left, Bottom))};
        private static Vertex[] rightLine = new Vertex[2] { new Vertex(new Vector2f(Right, Top)),
                                                     new Vertex(new Vector2f(Right, Bottom))};

        private static RectangleShape unitShape = new RectangleShape(new Vector2f(Program.ViewScale, Program.ViewScale));
        private static List<Unit> relatives = new List<Unit>(500);
        public static void Draw()
        {
            List<Unit> temp = Program.World.Units;
            List<Unit> relatives = new List<Unit>(500);
            Program.Window.Draw(rightLine, PrimitiveType.Lines);
            Program.Window.Draw(topLine, PrimitiveType.Lines);
            Program.Window.Draw(leftLine, PrimitiveType.Lines);
            Program.Window.Draw(bottomLine, PrimitiveType.Lines);
            unitShape.OutlineThickness = 0;
            foreach (Unit unit in temp)
            {
                if (UnitTextConfigurator.ChoosenUnit != null && unit.Parent == UnitTextConfigurator.ChoosenUnit.Parent)
                {
                    relatives.Add(unit);
                    continue;
                }
                unitShape.Position = new Vector2f(Program.LeftMapOffset + unit.Coords[0] * Program.ViewScale, Program.TopMapOffset + unit.Coords[1] * Program.ViewScale);
                unitShape.FillColor = ChooseColor(unit);

                Program.Window.Draw(unitShape);
            }

            foreach (Unit unit in relatives)
            {
                unitShape.OutlineColor = Color.White;
                unitShape.OutlineThickness = 1;
                if (UnitTextConfigurator.ChoosenUnit == unit)
                    unitShape.OutlineColor = Color.Green;
                unitShape.Position = new Vector2f(Program.LeftMapOffset + unit.Coords[0] * Program.ViewScale, Program.TopMapOffset + unit.Coords[1] * Program.ViewScale);
                unitShape.FillColor = ChooseColor(unit);

                Program.Window.Draw(unitShape);
            }
        }

        private static Color ChooseColor(Unit unit)
        {
            switch (Program.ChoosenMap)
            {
                case TypeOfMap.MapOfEnergy:
                    {
                        if (unit.Status == UnitStatus.Corpse)
                            return Program.Gray;
                        return Storage.EnergyColors[unit.Energy * 255 / Swamp.EnergyLimit];
                    }
                case TypeOfMap.MapOfActions:
                    {
                        if (unit.Status == UnitStatus.Corpse)
                            return Program.DarkGray;
                        return unit.GetCurrentAction().ActionColor();
                    }
                default:
                    return Color.Black;
            }
        }
    }
}
