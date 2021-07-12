using SFML.Graphics;

using System;

namespace Simulator.World
{
    public class Move : IAction
    {
        public Color ActionColor()
        {
            
            return Color.Blue;
        }
        public ActionType Type()
        {
            return ActionType.Move;
        }

        public void Process(Unit unit)
        {
            int t = PseudoRandom.Next(101);
            if (t < 60)
            {
                if (unit.Move(unit.LastDirection / 3 - 1, unit.LastDirection % 3 - 1))
                    return;
            }
            else if (t < 85)
            {
                if (unit.Move(unit.Direction[0], unit.Direction[1]))
                    return;
                int x = 0;
                int y = 0;
                if (unit.Direction[0] == 0 && unit.Direction[1] == 0)
                {
                    x = PseudoRandom.Next(-1, 2);
                    y = PseudoRandom.Next(-1, 2);
                }
                else if (unit.Direction[0] != 0 && unit.Direction[1] != 0)
                {
                    if (PseudoRandom.Next(0, 2) == 0)
                    {
                        x = (unit.Direction[0] - unit.Direction[0]);
                        y = unit.Direction[1];
                    }
                    else
                    {
                        x = unit.Direction[0];
                        y = (unit.Direction[1] - unit.Direction[1]);
                    }
                }
                else
                {
                    if (unit.Direction[0] == 0)
                    {
                        x = (unit.Direction[0] + (PseudoRandom.Next(0, 2) == 0 ? -1 : 1));
                        y = unit.Direction[1];
                    }
                    else
                    {
                        x = unit.Direction[0];
                        y = (unit.Direction[1] + (PseudoRandom.Next(0, 2) == 0 ? -1 : 1));
                    }
                }
                if (unit.Move(x, y))
                    return;
            }
            else
            {
                int direction = PseudoRandom.Next(9);
                if (unit.Move(direction / 3 - 1, direction % 3 - 1))
                    return;
            }
        }

        public int Value()
        {
            return Swamp.WaitValue * 2;
        }
    }
}
