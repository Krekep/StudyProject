using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace Simulator
{
    class Unit : Transformable, Drawable
    {
        const int UNIT_SIZE = 1;
        RectangleShape shape;
        public int[] position { get; private set; }
        private IAction[][] genes;
        private int currentAction;
        public int LastDirection { get; private set; }
        public int Energy { get; private set; }
        public bool IsAlive { get; private set; }

        public Unit(int energy, int[] position, IAction[][] genes)
        {
            this.Energy = energy;
            this.position = position;
            this.genes = genes;
            LastDirection = 4;
            currentAction = 0;
            IsAlive = true;

            shape = new RectangleShape(new Vector2f(UNIT_SIZE * Simulator.Scale, UNIT_SIZE * Simulator.Scale));
        }

        private bool CheckCell(int x, int y)
        {
            return Simulator.IsFree(x, y);
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            shape.Position = new Vector2f(position[1] * UNIT_SIZE * Simulator.Scale, position[0] * UNIT_SIZE * Simulator.Scale);
            shape.FillColor = new Color(255, (byte)((Energy + .0) / Simulator.EnergyLimit * 255), 0);

            target.Draw(shape);
        }

        public bool Move(int x, int y)
        {
            int[] newPosition = new int[] { position[0] + x, position[1] + y };
            LastDirection = (x + 1) * 3 + (y + 1);
            if (CheckCell(newPosition[0], newPosition[1]))
            {
                Simulator.MoveUnit(position, newPosition);
                position[0] = newPosition[0];
                position[1] = newPosition[1];
                return true;
            }
            return false;
        }

        internal void TakeEnergy(int energy)
        {
            Energy += energy;
            if (Energy <= 0)
            {
                Energy = 0;
                IsAlive = false;
            }
            if (Energy > Simulator.EnergyLimit)
                Energy = Simulator.EnergyLimit;
        }

        public void Process()
        {
            var temp = genes[currentAction / genes.Length];
            temp[currentAction % temp.Length].Process(this);
        }
    }
}
