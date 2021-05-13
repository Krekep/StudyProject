using System;
using System.Collections.Generic;
using System.Text;

namespace Simulator.World
{
    internal class Storage
    {
        internal static IAction[][] GenesBlocks;
        internal static int[] ValuesOfBlocks;
        internal const int AmountBlocks = 10;
        internal static Simulator CurrentWorld { get; set; }

        static int[] directions;
        static Storage()
        {
            directions = new int[9] { 0, 1, 2,
                                      3, 4, 5,
                                      6, 7, 8 };
            GenesBlocks = new IAction[AmountBlocks][];
            ValuesOfBlocks = new int[AmountBlocks];

            GenesBlocks[0] = new IAction[1] { new Wait() };
            GenesBlocks[1] = new IAction[2] { new Move(), new Move() };
            GenesBlocks[2] = new IAction[1] { new Move() };
            GenesBlocks[3] = new IAction[1] { new Photosyntesis() };
            GenesBlocks[4] = new IAction[2] { new Photosyntesis(), new Move() };
            GenesBlocks[5] = new IAction[2] { new Photosyntesis(), new Photosyntesis() };
            GenesBlocks[6] = new IAction[2] { new Attack(), new Attack() };
            GenesBlocks[7] = new IAction[2] { new Attack(), new Move() };
            GenesBlocks[8] = new IAction[2] { new Attack(), new Wait() };
            GenesBlocks[9] = new IAction[1] { new Attack() };

            for (int i = 0; i < AmountBlocks; i++)
                ValuesOfBlocks[i] = CalculateValue(GenesBlocks[i]);
        }
        internal static int CalculateValue(IAction[] block)
        {
            int result = 0;
            foreach (IAction action in block)
                result += action.Value();
            return result;
        }
    }
}
