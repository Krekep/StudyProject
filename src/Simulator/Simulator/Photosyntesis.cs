using SFML.Graphics;

using System;
using System.Collections.Generic;
using System.Text;

namespace Simulator
{
    class Photosyntesis : IAction
    {
        public Color ActionColor()
        {
            return Color.Green;
        }

        public void Process(Unit unit)
        {
            int energy = (int)(Simulator.SunPower * Math.Pow(Simulator.EnvDensity, unit.position[1]) * unit.Chlorophyl);
            unit.TakeEnergy(energy);
        }

        public int Value()
        {
            return Simulator.WaitValue * 4;
        }
    }
}
