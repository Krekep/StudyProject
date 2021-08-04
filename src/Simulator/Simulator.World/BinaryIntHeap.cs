using System;
using System.Collections.Generic;
using System.Text;

namespace Simulator.World
{
    public class BinaryIntHeap
    {
        int[] data;
        int sizeOfTree;
        int maxSize;

        public BinaryIntHeap(int size)
        {
            data = new int[size + 1];
            for (int i = 0; i < size + 1; i++)
                data[i] = Int32.MaxValue;
            sizeOfTree = 0;
            maxSize = size + 1;
        }

        public int Min()
        {
            if (sizeOfTree == 0)
                return Int32.MaxValue;
            else
                return data[1];
        }

        public void Insert(int value)
        {
            if (sizeOfTree + 1 >= maxSize)
                return;
            data[sizeOfTree + 1] = value;
            sizeOfTree++;
            BalanceHeap();
        }

        private void BalanceHeap()
        {
            for (int i = sizeOfTree; i > 1; i /= 2)
            {
                int x = i / 2;
                if (x * 2 + 1 < maxSize && data[x] > data[x * 2 + 1])
                {
                    int t = data[x];
                    data[x] = data[x * 2 + 1];
                    data[x * 2 + 1] = t;
                }
                if (data[x] > data[x * 2])
                {
                    int t = data[x];
                    data[x] = data[x * 2];
                    data[x * 2] = t;
                }
            }
        }

        public int PopMin()
        {
            int result = data[1];
            data[1] = data[sizeOfTree];
            data[sizeOfTree] = Int32.MaxValue;
            sizeOfTree--;
            BalanceHeap();
            return result;
        }

        internal void Clear()
        {
            for (int i = 0; i < maxSize; i++)
                data[i] = Int32.MaxValue;
            sizeOfTree = 0;
        }

        internal void RemoveValue(int unit)
        {
            throw new NotImplementedException();
        }
    }
}
