using SFML.Graphics;

using Simulator.Events;
using Simulator.ResourseLoaders;

using System;
using System.Threading;

namespace Simulator.World
{
    public enum UnitStatus : sbyte
    { 
        Dead = -2,
        Corpse = -1,
        Alive = 0,
        Divide = 1
    }

    public class Unit
    {
        public int Size { get; private set; }
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

        public Color ActionColor { get; private set; }
        public Color EnergyColor { get; private set; }

        private volatile int isUsing;
        public Unit(int size, int energy, int lastDirection, int capacity, int chlorophyl, int attackPower, int status, int[] position, int[] directions, IAction[][] genes, int parent)
        {
            this.Size = size;
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

            if (Status != UnitStatus.Corpse)
            {
                EnergyColor = Storage.EnergyColors[Energy / Size * 255 / Swamp.EnergyLimit];
                ActionColor = GetCurrentAction().ActionColor();
            }
            else
            {
                EnergyColor = MyColors.DarkGray;
                ActionColor = MyColors.DarkGray;
            }
        }
        public Unit(int size,  int energy, int[] position, int[] directions, IAction[][] genes, int capacity, int chlorophyl, int attackPower, int parent)
        {
            this.Size = size;
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
            EnergyColor = Storage.EnergyColors[Energy / Size * 255 / Swamp.EnergyLimit];
            ActionColor = GetCurrentAction().ActionColor();
        }

        private static bool CheckCell(int x, int y, int area, Unit unit)
        {
            return Storage.CurrentWorld.IsFree(x, y, area, unit);
        }
        public bool Move(int dirX, int dirY)
        {
            int x = dirX, y = dirY;
            int[] newPosition = new int[] { Coords[0] + x, Coords[1] + y };
            LastDirection = (x + 1) * 3 + (y + 1);
            Unit neighbour;
            bool isSuccess = false;
            if (LastDirection == 4)
                return false;
            if (x != 0 && y == 0)
            {
                if (x == 1)
                    x *= Size;
                for (int i = 0; i < Size; i++)
                {
                    neighbour = Storage.CurrentWorld.GetUnit(Coords[0] + x, Coords[1] + y - i * y);
                    if (neighbour != null)
                    {
                        if (neighbour.Size >= Size || !neighbour.Move(dirX, dirY))
                        {
                            isSuccess = false;
                            break;
                        }
                    }
                    isSuccess = true;
                }
            }
            else if (x == 0)
            {
                if (y == 1)
                    y *= Size;
                for (int i = 0; i < Size; i++)
                {
                    neighbour = Storage.CurrentWorld.GetUnit(Coords[0] + x - i * x, Coords[1] + y);
                    if (neighbour != null)
                    {
                        if (neighbour.Size >= Size || !neighbour.Move(dirX, dirY))
                        {
                            isSuccess = false;
                            break;
                        }
                    }
                    isSuccess = true;
                }
            }

            if (CheckCell(newPosition[0], newPosition[1], Size, this))
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
            LastDirection = (x + 1) * 3 + (y + 1);
            Unit attackedUnits;
            int temp = 0;
            int recievedEnergy = 0;
            bool isSuccess = false;
            if (LastDirection == 4)
                return false;
            if (x != 0)
            {
                if (x == 1)
                    x *= Size;
                for (int i = 0; i < Size; i++)
                {
                    attackedUnits = Storage.CurrentWorld.GetUnit(Coords[0] + x, Coords[1] + y - i * Math.Sign(y));
                    if (attackedUnits != null)
                    {
                        temp = -Math.Min(this.Energy / (Swamp.AttackLimit * 2 - AttackPower), attackedUnits.Energy - Swamp.DeathLimit * attackedUnits.Size);
                        //if (attackedUnits.Status == UnitStatus.Corpse)
                        //    temp <<= 2;
                        attackedUnits.TakeEnergy(temp);
                        recievedEnergy -= temp;
                        isSuccess = true;
                    }
                }
            }
            else 
            {
                if (y == 1)
                    y *= Size;
                for (int i = 0; i < Size; i++)
                {
                    attackedUnits = Storage.CurrentWorld.GetUnit(Coords[0] + x - i * Math.Sign(x), Coords[1] + y);
                    if (attackedUnits != null)
                    {
                        temp = -Math.Min(this.Energy / (Swamp.AttackLimit * 2 - AttackPower), attackedUnits.Energy - Swamp.DeathLimit * attackedUnits.Size);
                        //if (attackedUnits.Status == UnitStatus.Corpse)
                        //    temp <<= 2;
                        attackedUnits.TakeEnergy(temp);
                        recievedEnergy -= temp;
                        isSuccess = true;
                    }
                }
            }

            this.TakeEnergy(recievedEnergy * 8 / 10);
            return isSuccess;
        }

        public void TakeEnergy(int energy)
        {
            Energy += energy;
            if (Energy <= 0)
            {
                if (Energy <= Swamp.DeathLimit * Size)
                    Status = UnitStatus.Dead;
                else
                    Status = UnitStatus.Corpse;
                EnergyColor = MyColors.Gray;
                ActionColor = MyColors.Gray;
            }
            else
                EnergyColor = Storage.EnergyColors[Math.Min(Energy, Swamp.EnergyLimit * Size) * 255 / (Swamp.EnergyLimit * Size)];
            if (Energy > Swamp.EnergyLimit * Size)
            {
                Status = UnitStatus.Divide;
            }
        }

        public void Divide()
        {
            DivideEvent.KnockKnock(this, this.Coords);
            Status = UnitStatus.Alive;
        }

        public void Process()
        {
            if (Status == UnitStatus.Corpse)
                return;
            var temp = Genes[currentAction[0]];
            temp[currentAction[1]].Process(this);
            ActionColor = temp[currentAction[1]].ActionColor();
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
