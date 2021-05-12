using SFML.Graphics;
using SFML.System;

using Simulator.World;

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

    public class Unit : Transformable, Drawable
    {
        const int UnitSize = 1;

        public int Capacity { get; private set; }
        RectangleShape shape;
        public int[] Coords { get; private set; }
        public IAction[][] Genes { get; private set; }
        private int[] currentAction;
        public int LastDirection { get; private set; }
        public int[] Direction { get; private set; }  // unit's favorite direction (x, y) - (1, 0), (0, -1), (0, 1), (-1, 0)
        public int Energy { get; private set; }
        public UnitStatus Status { get; private set; }
        public int Chlorophyl { get; private set; }
        public Unit(int energy, int lastDirection, int capacity, int chlorophyl, int status, int[] position, int[] directions, IAction[][] genes)
        {
            this.Energy = energy;
            this.Coords = position;
            this.Direction = directions;
            this.Genes = genes;
            this.Capacity = capacity;
            this.Chlorophyl = chlorophyl;

            LastDirection = lastDirection;
            currentAction = new int[2] { 0, 0 };
            Status = (UnitStatus)status;


            shape = new RectangleShape(new Vector2f(UnitSize * Simulator.ViewScale, UnitSize * Simulator.ViewScale));
        }
        public Unit(int energy, int[] position, int[] directions, IAction[][] genes, int capacity, int chlorophyl)
        {
            this.Energy = energy;
            this.Coords = position;
            this.Direction = directions;
            this.Genes = genes;
            this.Capacity = capacity;
            this.Chlorophyl = chlorophyl;

            LastDirection = 4;
            currentAction = new int[2] { 0, 0 };
            Status = UnitStatus.Alive;


            shape = new RectangleShape(new Vector2f(UnitSize * Simulator.ViewScale, UnitSize * Simulator.ViewScale));
        }

        private bool CheckCell(int x, int y)
        {
            return Storage.CurrentWorld.IsFree(x, y);
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            states.Transform *= Transform;

            shape.Position = new Vector2f(Coords[0] * UnitSize * Simulator.ViewScale, Coords[1] * UnitSize * Simulator.ViewScale);
            shape.FillColor = ChooseColor();

            target.Draw(shape, states);
        }

        private Color ChooseColor()
        {
            switch (Storage.CurrentWorld.ChoosenMap)
            {
                case TypeOfMap.MapOfEnergy:
                    return new Color(255, (byte)((Energy + .0) / Simulator.EnergyLimit * 255), 0);
                case TypeOfMap.MapOfActions:
                    return GetCurrentAction().ActionColor();
                default:
                    return Color.Black;
            }
        }

        public bool Move(int x, int y)
        {
            int[] newPosition = new int[] { Coords[0] + x, Coords[1] + y };
            LastDirection = (x + 1) * 3 + (y + 1);
            if (CheckCell(newPosition[0], newPosition[1]))
            {
                Storage.CurrentWorld.MoveUnit(this, Coords, newPosition);
                Coords[0] = newPosition[0];
                Coords[1] = newPosition[1];
                return true;
            }
            return false;
        }

        public void TakeEnergy(int energy)
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
                Storage.CurrentWorld.AddUnit(child);
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

        public IAction GetCurrentAction()
        {
            var temp = Genes[currentAction[0]];
            return temp[currentAction[1]];
        }

        public void SetGenes(IAction[][] genes)
        {
            Genes = genes;
        }
    }
}
