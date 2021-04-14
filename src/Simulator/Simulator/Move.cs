using System;
using System.Collections.Generic;
using System.Text;

namespace Simulator
{
    class Move : IAction
    {
        public void Process(Unit unit)
        { 
            for (int i = 0; i < 9; i++)
            {
                int t = Simulator.Random.Next(11);
                if (t < 7)
                    unit.Move(unit.LastDirection / 3 - 1, unit.LastDirection % 3 - 1);
                else
                {
                    int direction = Simulator.Random.Next(9);
                    unit.Move(direction / 3 - 1, direction % 3 - 1);
                }
            }
        }
    }
}
