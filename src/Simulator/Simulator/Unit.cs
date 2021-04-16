using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace Simulator
{
    public enum UnitStatus : int
    { 
        Dead = -1,
        Alive = 0,
        Divide = 1
    }
    class Unit : Transformable, Drawable
    {
        const int Unit_Size = 1;
        public int Capacity { get; private set; }
        RectangleShape shape;
        public int[] position { get; private set; }
        public IAction[][] Genes { get; private set; }
        private int[] currentAction;
        public int LastDirection { get; private set; }
        public int[] Directions { get; private set; }
        public int Energy { get; private set; }
        public UnitStatus Status { get; private set; }
        public int Chlorophyl { get; private set; }

        public Unit(int energy, int[] position, int[] directions, IAction[][] genes, int capacity, int chlorophyl)
        {
            this.Energy = energy;
            this.position = position;
            this.Directions = directions;
            this.Genes = genes;
            this.Capacity = capacity;
            this.Chlorophyl = chlorophyl;

            LastDirection = 4;
            currentAction = new int[2] { 0, 0 };
            Status = UnitStatus.Alive;


            shape = new RectangleShape(new Vector2f(Unit_Size * Simulator.Scale, Unit_Size * Simulator.Scale));
        }

        private bool CheckCell(int x, int y)
        {
            return Simulator.IsFree(x, y);
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            shape.Position = new Vector2f(position[1] * Unit_Size * Simulator.Scale + Program.LeftMapOffset, position[0] * Unit_Size * Simulator.Scale + Program.TopMapOffset);
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
                Status = UnitStatus.Dead;
            }
            if (Energy > Simulator.EnergyLimit)
            {
                Status = UnitStatus.Divide;
            }
        }

        public void Divide()
        {
            Unit child = Creator.CreateChild(this);
            if (child != null)
            {
                Simulator.AddUnit(child);
                Energy /= 2;
            }
            else
                Energy = Simulator.EnergyLimit;
            Status = UnitStatus.Alive;
        }

        public void Process()
        {
            var temp = Genes[currentAction[0]];
            temp[currentAction[1]].Process(this);
            currentAction[1] += 1;
            if (currentAction[1] >= temp.Length)
            {
                currentAction[0] += 1;
                currentAction[1] = 0;
                if (currentAction[0] >= Genes.Length)
                    currentAction[0] = 0;
            }
        }
    }
}
