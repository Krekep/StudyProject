using System;
using System.Collections.Generic;
using System.Text;

namespace Simulator
{
    class Wait : IAction
    {
        public void Process(Unit unit)
        {
            
        }

        public int Value()
        {
            return Simulator.WaitValue;
        }
    }
}
