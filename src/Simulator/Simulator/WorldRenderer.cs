using SFML.Graphics;
using SFML.System;

using Simulator.ResourseLoaders;
using Simulator.World;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Simulator
{
    static class WorldRenderer
    {
        private static Task[] renderTasks;
        private static VertexArray[] vertexArrays;

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

        private static VertexArray vertexArray;

        private static Vertex leftTop;
        private static Vertex leftBottom;
        private static Vertex rightTop;
        private static Vertex rightBottom;

        static WorldRenderer()
        {
            renderTasks = new Task[Swamp.ThreadCounter];
            vertexArrays = new VertexArray[Swamp.ThreadCounter];
        }

        public static void DrawByThreads()
        {
            for (int i = 0; i < Swamp.ThreadCounter; i++)
            {
                int t = i;
                renderTasks[i] = Task.Run(() => ChooseColorParallel(t));
            }
            Task.WaitAll(renderTasks);

            for (int i = 0; i < Swamp.ThreadCounter; i++)
            {
                Program.Window.Draw(vertexArrays[i]);
                vertexArrays[i].Dispose();
            }
            Program.Window.Draw(bottomLine, PrimitiveType.Lines);
            Program.Window.Draw(topLine, PrimitiveType.Lines);
            Program.Window.Draw(leftLine, PrimitiveType.Lines);
            Program.Window.Draw(rightLine, PrimitiveType.Lines);
        }

        private static void ChooseColorParallel(int number)
        {
            var units = Program.World.Units;
            vertexArrays[number] = new VertexArray(PrimitiveType.Quads, (uint)units.Count * 4 / (uint)Swamp.ThreadCounter);
            Vertex leftTop;
            Vertex leftBottom;
            Vertex rightTop;
            Vertex rightBottom;
            int bottom = number * units.Count / Swamp.ThreadCounter;
            int top = (number + 1) * units.Count / Swamp.ThreadCounter;
            for (int i = bottom; i < top; i++)
            {
                var unit = units[i];
                int leftBound = Program.LeftMapOffset + unit.Coords[0] * Program.ViewScale;
                int topBound = Program.TopMapOffset + unit.Coords[1] * Program.ViewScale;
                int size = Program.ViewScale * unit.Size;
                leftTop = new Vertex(new Vector2f(leftBound, topBound));
                leftBottom = new Vertex(new Vector2f(leftBound, topBound + size));
                rightTop = new Vertex(new Vector2f(leftBound + size, topBound));
                rightBottom = new Vertex(new Vector2f(leftBound + size, topBound + size));
                if (UnitTextConfigurator.ChoosenUnit != null && unit.Parent == UnitTextConfigurator.ChoosenUnit.Parent)
                {
                    if (UnitTextConfigurator.ChoosenUnit == unit)
                        leftTop.Color = Color.Green;
                    else
                        leftTop.Color = Color.White;
                    leftBottom.Color = ChooseColor(unit);
                    rightTop.Color = leftBottom.Color;
                    rightBottom.Color = leftTop.Color;
                }
                else
                {
                    leftTop.Color = ChooseColor(unit);
                    leftBottom.Color = leftTop.Color;
                    rightTop.Color = leftTop.Color;
                    rightBottom.Color = leftTop.Color;
                }
                uint t = (uint)i << 2;
                vertexArrays[number][t] = leftTop;
                vertexArrays[number][t + 1] = leftBottom;
                vertexArrays[number][t + 2] = rightBottom;
                vertexArrays[number][t + 3] = rightTop;
            }
        }

        public static void Draw()
        {
            List<Unit> temp = Program.World.Units;
            vertexArray = new VertexArray(PrimitiveType.Quads, (uint)temp.Count * 4);
            uint i = 0;
            foreach (Unit unit in temp)
            {
                int leftBound = Program.LeftMapOffset + unit.Coords[0] * Program.ViewScale;
                int topBound = Program.TopMapOffset + unit.Coords[1] * Program.ViewScale;
                int size = Program.ViewScale * unit.Size;
                leftTop = new Vertex(new Vector2f(leftBound, topBound));
                leftBottom = new Vertex(new Vector2f(leftBound, topBound + size));
                rightTop = new Vertex(new Vector2f(leftBound + size, topBound));
                rightBottom = new Vertex(new Vector2f(leftBound + size, topBound + size));
                if (UnitTextConfigurator.ChoosenUnit != null && unit.Parent == UnitTextConfigurator.ChoosenUnit.Parent)
                {
                    if (UnitTextConfigurator.ChoosenUnit == unit)
                        leftTop.Color = Color.Green;
                    else 
                        leftTop.Color = Color.White;
                    leftBottom.Color = ChooseColor(unit);
                    rightTop.Color = leftBottom.Color;
                    rightBottom.Color = leftTop.Color;
                }
                else
                {
                    leftTop.Color = ChooseColor(unit);
                    leftBottom.Color = leftTop.Color;
                    rightTop.Color = leftTop.Color;
                    rightBottom.Color = leftTop.Color;
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
                        return unit.EnergyColor;
                    }
                case TypeOfMap.MapOfActions:
                    {
                        return unit.ActionColor;
                    }
                default:
                    return Color.Black;
            }
        }
    }
}
