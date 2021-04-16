using System;
using System.Collections.Generic;
using System.Text;

namespace Simulator
{
    class Photosyntesis : IAction
    {
        public void Process(Unit unit)
        {
            int energy = (int)(Simulator.SunPower * Math.Pow(Simulator.EnvDensity, unit.position[0]) * unit.Chlorophyl);
            unit.TakeEnergy(energy);
        }

        public int Value()
        {
            return Simulator.WaitValue * 4;
        }
    }
}
