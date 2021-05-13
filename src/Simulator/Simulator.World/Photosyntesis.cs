﻿using SFML.Graphics;

using Simulator.World;

using System;
using System.Collections.Generic;
using System.Text;

namespace Simulator
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
            return Simulator.WaitValue * 4;
        }
    }
}