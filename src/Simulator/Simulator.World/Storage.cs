using System;
using System.Collections.Generic;
using System.Text;

namespace Simulator.World
{
    internal class Storage
    {
        internal static IAction[][] genesBlocks;
        internal static int[] valuesOfBlocks;
        internal const int AmountBlocks = 6;
        internal static Simulator CurrentWorld { get; set; }

        static int[] directions;
        static Storage()
        {
            directions = new int[9] { 0, 1, 2,
                                      3, 4, 5,
                                      6, 7, 8 };
            genesBlocks = new IAction[AmountBlocks][];
            valuesOfBlocks = new int[AmountBlocks];

            genesBlocks[0] = new IAction[1] { new Wait() };
            genesBlocks[1] = new IAction[2] { new Move(), new Move() };
            genesBlocks[2] = new IAction[1] { new Move() };
            genesBlocks[3] = new IAction[2] { new Photosyntesis(), new Wait() };
            genesBlocks[4] = new IAction[2] { new Photosyntesis(), new Move() };
            genesBlocks[5] = new IAction[2] { new Photosyntesis(), new Photosyntesis() };

            for (int i = 0; i < AmountBlocks; i++)
                foreach (IAction action in genesBlocks[i])
                    valuesOfBlocks[i] += action.Value();
        }
    }
}
