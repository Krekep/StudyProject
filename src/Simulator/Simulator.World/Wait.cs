using SFML.Graphics;

using System;
using System.Collections.Generic;
using System.Text;

namespace Simulator.World
{
    public class Wait : IAction
    {
        public Color ActionColor()
        {
            return new Color(128, 128, 128);
        }

        public void Process(Unit unit)
        {
            
        }

        public ActionType Type()
        {
            return ActionType.Wait;
        }

        public int Value()
        {
            return Simulator.WaitValue;
        }
    }
}
