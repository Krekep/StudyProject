using SFML.Graphics;

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

        public void Process(int unitNumber)
        {
            
        }

        public ActionType Type()
        {
            return ActionType.Wait;
        }

        public int Value()
        {
            return Swamp.WaitValue;
        }
    }
}
