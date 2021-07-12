using Simulator.Events;

using System;
using System.Threading;

namespace Simulator.World
{
    public enum UnitStatus : int
    { 
        Dead = -2,
        Corpse = -1,
        Alive = 0,
        Divide = 1
    }

    public class Unit
    {
        public int Capacity { get; private set; }
        public int[] Coords { get; private set; }
        public IAction[][] Genes { get; private set; }
        private int[] currentAction;
        public int LastDirection { get; private set; }
        /// <summary>
        /// Unit's favorite direction (x, y) - (+/-1, +/-1)
        /// </summary>
        public int[] Direction { get; private set; }
        public int Energy { get; private set; }
        public UnitStatus Status { get; private set; }
        public int Chlorophyl { get; private set; }
        public int AttackPower { get; private set; }
        public int Parent { get; private set; }


        private volatile int isUsing;
        public Unit(int energy, int lastDirection, int capacity, int chlorophyl, int attackPower, int status, int[] position, int[] directions, IAction[][] genes, int parent)
        {
            this.Energy = energy;
            this.Coords = position;
            this.Direction = directions;
            this.Genes = genes;
            this.Capacity = capacity;
            this.Chlorophyl = chlorophyl;
            this.AttackPower = attackPower;
            this.Parent = parent;
            isUsing = 0;

            LastDirection = lastDirection;
            currentAction = new int[2] { 0, 0 };
            Status = (UnitStatus)status;
        }
        public Unit(int energy, int[] position, int[] directions, IAction[][] genes, int capacity, int chlorophyl, int attackPower, int parent)
        {
            this.Energy = energy;
            this.Coords = position;
            this.Direction = directions;
            this.Genes = genes;
            this.Capacity = capacity;
            this.Chlorophyl = chlorophyl;
            this.AttackPower = attackPower;
            this.Parent = parent;
            isUsing = 0;

            LastDirection = 4;
            currentAction = new int[2] { 0, 0 };
            Status = UnitStatus.Alive;
        }

        private bool CheckCell(int x, int y)
        {
            return Storage.CurrentWorld.IsFree(x, y);
        }

        public bool Move(int x, int y)
        {
            int[] newPosition = new int[] { Coords[0] + x, Coords[1] + y };
            LastDirection = (x + 1) * 3 + (y + 1);
            if (CheckCell(newPosition[0], newPosition[1]))
            {
                MoveEvent.KnockKnock(this, Coords, newPosition);
                Coords[0] = newPosition[0];
                Coords[1] = newPosition[1];
                return true;
            }
            return false;
        }
        public bool Attack(int x, int y)
        {
            int[] targetPosition = new int[] { Coords[0] + x, Coords[1] + y };
            LastDirection = (x + 1) * 3 + (y + 1);
            var unit = Storage.CurrentWorld.GetUnit(targetPosition[0], targetPosition[1]);
            if (unit != null && LastDirection != 4)
            {
                int temp = -Math.Min(this.Energy / (Swamp.AttackLimit * 2 - AttackPower), unit.Energy);
                if (unit.Status == UnitStatus.Corpse)
                {
                    temp = unit.Energy / (Swamp.AttackLimit + 1 - AttackPower);
                }
                unit.TakeEnergy(temp);
                this.TakeEnergy(-temp * 7 / 10);
                return true;
            }
            return false;
        }

        public void TakeEnergy(int energy)
        {
            Energy += energy;
            if (Energy <= 0)
            {
                if (Energy <= Swamp.DeathLimit)
                    Status = UnitStatus.Dead;
                else
                    Status = UnitStatus.Corpse;
            }
            if (Energy > Swamp.EnergyLimit)
            {
                Status = UnitStatus.Divide;
            }
        }

        public void Divide()
        {
            Unit child = Creator.CreateChild(this);
            if (child != null)
            {
                DivideEvent.KnockKnock(child, child.Coords);
                Energy /= 2;
                Energy -= Swamp.EnergyLimit / 100;
            }
            else
                Energy = Swamp.EnergyLimit;
            Status = UnitStatus.Alive;
        }

        public void Process()
        {
            if (Status == UnitStatus.Corpse)
                return;
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

        internal void Lock()
        {
            while (Interlocked.CompareExchange(ref isUsing, 1, 0) == 1)
            { Thread.Sleep(0); }
        }

        internal void Unlock()
        {
            isUsing = 0;
        }
    }
}
