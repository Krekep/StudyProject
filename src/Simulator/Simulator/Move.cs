using SFML.Graphics;
using SFML.System;
using SFML.Window;

using System;
using System.Collections.Generic;
using System.Text;

namespace Simulator
{
    class Move : IAction
    {
        public Color ActionColor()
        {
            
            return Color.Blue;
        }

        public void Process(Unit unit)
        {
            int j = 0;
            for (int i = 0; i < 5; i++)
            {
                int t = Simulator.Random.Next(101);
                if (t < 50)
                {
                    if (unit.Move(unit.LastDirection / 3 - 1, unit.LastDirection % 3 - 1))
                        return;
                }
                else if (t < 30)
                {
                    if (j < 9 && unit.Directions[j] != -1)
                    {
                        if (unit.Move(unit.Directions[j] / 3 - 1, unit.Directions[j] % 3 - 1))
                            return;
                        j += 1;
                    }
                    else
                    {
                        int direction = Simulator.Random.Next(9);
                        if (unit.Move(direction / 3 - 1, direction % 3 - 1))
                            return;
                    }
                }
                else
                {
                    int direction = Simulator.Random.Next(9);
                    if (unit.Move(direction / 3 - 1, direction % 3 - 1))
                        return;
                }
            }
        }

        public int Value()
        {
            return Simulator.WaitValue * 2;
        }
    }
}
