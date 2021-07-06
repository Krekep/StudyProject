using SFML.Graphics;
using SFML.System;
using TGUI;

using System;
using System.Collections.Generic;
using System.Text;

namespace Simulator
{
    static class WorldRenderer
    {
        private const int Left = 0 * Program.ViewScale + Program.LeftMapOffset;
        private const int Top = 0 * Program.ViewScale + Program.TopMapOffset;
        private const int Bottom = Simulator.WorldHeight * Program.ViewScale + 1 + Program.TopMapOffset;
        private const int Right = Simulator.WorldWidth * Program.ViewScale + 1 + Program.LeftMapOffset;

        private static Vertex[] bottomLine = new Vertex[2] { new Vertex(new Vector2f(Left, Bottom)),
                                                      new Vertex(new Vector2f(Right, Bottom))};
        private static Vertex[] topLine = new Vertex[2] { new Vertex(new Vector2f(Left, Top)),
                                                   new Vertex(new Vector2f(Right, Top))};
        private static Vertex[] leftLine = new Vertex[2] { new Vertex(new Vector2f(Left, Top)),
                                                    new Vertex(new Vector2f(Left, Bottom))};
        private static Vertex[] rightLine = new Vertex[2] { new Vertex(new Vector2f(Right, Top)),
                                                     new Vertex(new Vector2f(Right, Bottom))};

        private static RectangleShape unitShape = new RectangleShape(new Vector2f(Program.ViewScale, Program.ViewScale));

        public static void Draw()
        {
            List<Unit> temp = Program.World.Units;
            Program.Window.Draw(rightLine, PrimitiveType.Lines);
            Program.Window.Draw(topLine, PrimitiveType.Lines);
            Program.Window.Draw(leftLine, PrimitiveType.Lines);
            Program.Window.Draw(bottomLine, PrimitiveType.Lines);
            foreach (Unit unit in temp)
            {
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
                        if (unit.Status == UnitStatus.Dead)
                            return Program.Gray;
                        return new Color(255, (byte)((unit.Energy + .0) / Simulator.EnergyLimit * 255), 0);
                    }
                case TypeOfMap.MapOfActions:
                    {
                        if (unit.Status == UnitStatus.Dead)
                            return Color.Black;
                        return unit.GetCurrentAction().ActionColor();
                    }
                default:
                    return Color.Black;
            }
        }
    }
}
