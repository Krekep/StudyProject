using SFML.Graphics;

namespace Simulator.World
{
    public class Storage
    {
        public static IAction[][] GenesBlocks;
        public static int[] ValuesOfBlocks;
        public const int AmountBlocks = 10;
        public static Swamp CurrentWorld { get; set; }
        public static Color[] EnergyColors = new Color[256];
        static Storage()
        {
            for (byte i = 0; i < 255; i++)
                EnergyColors[i] = new Color(255, i, 0);
            EnergyColors[255] = new Color(255, 255, 0);

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
        public static int CalculateValue(IAction[] block)
        {
            int result = 0;
            foreach (IAction action in block)
                result += action.Value();
            return result;
        }
    }
}
