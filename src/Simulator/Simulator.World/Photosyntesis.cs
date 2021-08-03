using SFML.Graphics;
using System;

namespace Simulator.World
{
    public class Photosyntesis : IAction
    {
        public Color ActionColor()
        {
            return Color.Green;
        }

        public ActionType Type()
        {
            return ActionType.Photosyntesis;
        }
        public void Process(Unit unit)
        {
            int energy = (int)(Storage.CurrentWorld.SunPower * Math.Pow(Storage.CurrentWorld.EnvDensity, unit.Coords[1]) * unit.Chlorophyl);
            unit.TakeEnergy(energy);
        }

        public int Value()
        {
            return Swamp.WaitValue * 4;
        }

        public void Process(int unitNumber)
        {
            int energy = (int)(Storage.CurrentWorld.SunPower * Math.Pow(Storage.CurrentWorld.EnvDensity, Storage.CurrentWorld.Units.UnitsCoords[unitNumber][1]) * Storage.CurrentWorld.Units.UnitsChlorophyl[unitNumber]);
            Storage.CurrentWorld.Units.TakeEnergy(unitNumber, energy);
        }
    }
}
