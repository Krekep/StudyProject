using SFML.Graphics;

namespace Simulator.World
{
    public class Move : IAction
    {
        public Color ActionColor()
        {
            
            return Color.Blue;
        }
        public ActionType Type()
        {
            return ActionType.Move;
        }

        public void Process(Unit unit)
        {
            for (int i = 0; i < 5; i++)
            {
                int t = Swamp.Random.Next(101);
                if (t < 50)
                {
                    if (unit.Move(unit.LastDirection / 3 - 1, unit.LastDirection % 3 - 1))
                        return;
                }
                else if (t < 80)
                {
                    for (int deep = 0; deep <= 1; deep++)
                    {
                        int x = 0;
                        int y = 0;
                        if (unit.Direction[0] != 0 && unit.Direction[1] != 0)
                        {
                            if (Swamp.Random.Next(0, 2) == 0)
                            {
                                x = (unit.Direction[0] - deep * unit.Direction[0]);
                                y = unit.Direction[1];
                            }
                            else
                            {
                                x = unit.Direction[0];
                                y = (unit.Direction[1] - deep * unit.Direction[1]);
                            }
                        }
                        else if (unit.Direction[0] == 0 && unit.Direction[1] == 0)
                        {
                            x = Swamp.Random.Next(-1, 2);
                            y = Swamp.Random.Next(-1, 2);
                        }
                        else
                        {
                            x = unit.Direction[0] + unit.Direction[0] * deep * (Swamp.Random.Next(0, 2) == 0 ? -1 : 1);
                            y = unit.Direction[1] + unit.Direction[1] * deep * (Swamp.Random.Next(0, 2) == 0 ? -1 : 1);
                        }
                        if (unit.Move(x, y))
                            return;
                    }
                }
                else
                {
                    int direction = Swamp.Random.Next(9);
                    if (unit.Move(direction / 3 - 1, direction % 3 - 1))
                        return;
                }
            }
        }

        public int Value()
        {
            return Swamp.WaitValue * 2;
        }
    }
}
