﻿using SFML.Graphics;
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
        private static VertexArray vertexArray;

        private static Vertex leftTop;
        private static Vertex leftBottom;
        private static Vertex rightTop;
        private static Vertex rightBottom;
        public static void Draw()
        {
            var unitsData = Program.World.Units;
            vertexArray = new VertexArray(PrimitiveType.Quads, (uint)unitsData.UnitsNumbers.Count * 4);
            unitShape.OutlineThickness = 0;
            uint i = 0;
            foreach (int unit in unitsData.UnitsNumbers)
            {
                leftTop = new Vertex(new Vector2f(Program.LeftMapOffset + unitsData.UnitsCoords[unit][0] * Program.ViewScale, Program.TopMapOffset + unitsData.UnitsCoords[unit][1] * Program.ViewScale));
                leftBottom = new Vertex(new Vector2f(Program.LeftMapOffset + unitsData.UnitsCoords[unit][0] * Program.ViewScale, Program.TopMapOffset + unitsData.UnitsCoords[unit][1] * Program.ViewScale + Program.ViewScale));
                rightTop = new Vertex(new Vector2f(Program.LeftMapOffset + unitsData.UnitsCoords[unit][0] * Program.ViewScale + Program.ViewScale, Program.TopMapOffset + unitsData.UnitsCoords[unit][1] * Program.ViewScale));
                rightBottom = new Vertex(new Vector2f(Program.LeftMapOffset + unitsData.UnitsCoords[unit][0] * Program.ViewScale + Program.ViewScale, Program.TopMapOffset + unitsData.UnitsCoords[unit][1] * Program.ViewScale + Program.ViewScale));
                if (UnitTextConfigurator.ChoosenUnit != -1 && unitsData.UnitsParent[unit] == unitsData.UnitsParent[UnitTextConfigurator.ChoosenUnit])
                {
                    if (UnitTextConfigurator.ChoosenUnit == unit)
                    {
                        leftTop.Color = Color.Green;
                        leftBottom.Color = Color.Green;
                        rightTop.Color = Color.Green;
                        rightBottom.Color = Color.Green;
                    }
                    else 
                    {
                        leftTop.Color = Color.White;
                        leftBottom.Color = Color.White;
                        rightTop.Color = Color.White;
                        rightBottom.Color = Color.White;
                    }
                }
                else
                {
                    leftTop.Color = ChooseColor(unit);
                    leftBottom.Color = ChooseColor(unit);
                    rightTop.Color = ChooseColor(unit);
                    rightBottom.Color = ChooseColor(unit);
                }
                uint t = i << 2;
                vertexArray[t] = leftTop;
                vertexArray[t + 1] = leftBottom;
                vertexArray[t + 2] = rightBottom;
                vertexArray[t + 3] = rightTop;
                i++;
            }
            Program.Window.Draw(vertexArray);
            Program.Window.Draw(bottomLine, PrimitiveType.Lines);
            Program.Window.Draw(topLine, PrimitiveType.Lines);
            Program.Window.Draw(leftLine, PrimitiveType.Lines);
            Program.Window.Draw(rightLine, PrimitiveType.Lines);
            vertexArray.Dispose();
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

        private static Color ChooseColor(int unit)
        {
            var unitsData = Program.World.Units;
            switch (Program.ChoosenMap)
            {
                case TypeOfMap.MapOfEnergy:
                    {
                        if (unitsData.UnitsStatus[unit] == UnitStatus.Corpse)
                            return Program.Gray;
                        return Storage.EnergyColors[unitsData.UnitsEnergy[unit] * 255 / Swamp.EnergyLimit];
                    }
                case TypeOfMap.MapOfActions:
                    {
                        if (unitsData.UnitsStatus[unit] == UnitStatus.Corpse)
                            return Program.DarkGray;
                        return unitsData.GetCurrentAction(unit).ActionColor();
                    }
                default:
                    return Color.Black;
            }
        }
    }
}
