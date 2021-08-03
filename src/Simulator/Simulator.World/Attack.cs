﻿using SFML.Graphics;

using System;

namespace Simulator.World
{
    public class Attack : IAction
    {
        public Color ActionColor()
        {
            return Color.Red;
        }

        public void Process(Unit unit)
        {
            /*
            int t = PseudoRandom.Next(101);
            if (t < 60)
            {
                if (unit.Attack(unit.LastDirection / 3 - 1, unit.LastDirection % 3 - 1))
                    return;
            }
            else if (t < 85)
            {
                if (unit.Attack(unit.Direction[0], unit.Direction[1]))
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
                if (unit.Attack(x, y))
                        return;
            }
            else
            {
                int direction = PseudoRandom.Next(9);
                if (unit.Attack(direction / 3 - 1, direction % 3 - 1))
                    return;
            }
            */
        }

        public void Process(int unitNumber)
        {
            int t = PseudoRandom.Next(101);
            var world = Storage.CurrentWorld;
            if (t < 60)
            {
                world.Units.Attack(unitNumber, world.Units.UnitsLastDirection[unitNumber] / 3 - 1, world.Units.UnitsLastDirection[unitNumber] % 3 - 1);
            }
            else if (t < 85)
            {
                int[] favDir = world.Units.UnitsDirection[unitNumber];
                if (world.IsFree(favDir[0], favDir[1]))
                {
                    world.Units.Attack(unitNumber, favDir[0], favDir[1]);
                    return;
                }
                int x = 0;
                int y = 0;
                if (favDir[0] == 0 && favDir[1] == 0)
                {
                    x = PseudoRandom.Next(-1, 2);
                    y = PseudoRandom.Next(-1, 2);
                }
                else if (favDir[0] != 0 && favDir[1] != 0)
                {
                    if (PseudoRandom.Next(0, 2) == 0)
                    {
                        x = (favDir[0] - favDir[0]);
                        y = favDir[1];
                    }
                    else
                    {
                        x = favDir[0];
                        y = (favDir[1] - favDir[1]);
                    }
                }
                else
                {
                    if (favDir[0] == 0)
                    {
                        x = (favDir[0] + (PseudoRandom.Next(0, 2) == 0 ? -1 : 1));
                        y = favDir[1];
                    }
                    else
                    {
                        x = favDir[0];
                        y = (favDir[1] + (PseudoRandom.Next(0, 2) == 0 ? -1 : 1));
                    }
                }
                world.Units.Attack(unitNumber, x, y);
            }
            else
            {
                int direction = PseudoRandom.Next(9);
                world.Units.Attack(unitNumber, direction / 3 - 1, direction % 3 - 1);
            }
        }

        public ActionType Type()
        {
            return ActionType.Attack;
        }

        public int Value()
        {
            return Swamp.WaitValue * 5;
        }
    }
}
