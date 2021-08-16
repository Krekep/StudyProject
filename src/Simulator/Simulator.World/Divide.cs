using SFML.Graphics;

using Simulator.Events;

using System;
using System.Collections.Generic;
using System.Text;

namespace Simulator.World
{
    public class Divide : IAction
    {
        public Color ActionColor()
        {
            return Color.Yellow;
        }

        public void Process(Unit unit)
        {
            if (unit.Energy >= Swamp.EnergyLimit * unit.Size / 50)
            {
                DivideEvent.KnockKnock(unit, unit.Coords);
            }
        }

        public ActionType Type()
        {
            return ActionType.Divide;
        }

        public int Value()
        {
            return Swamp.WaitValue * 8;
        }
    }
}
