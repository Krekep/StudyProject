using SFML.Graphics;

using System;
using System.Collections.Generic;
using System.Text;

namespace Simulator
{
    public static class WorldTextConfigurator
    {
        public const int WorldTextLeftBound = Program.LeftMapOffset + 20;
        public const int WorldTextTopBound = Program.TopMapOffset + Simulator.WorldHeight * Simulator.Scale + + 10;
        public const int WorldTextHeight = (Program.TextSize + 5) * AmountWorldInfo;
        public const int WorldTextWidth = 100;
        private static int choosenID;

        /// zero - ground heat
        /// first - sun heat
        /// second - cell fall chance
        /// third - environment density
        static Dictionary<int, TextBox> worldDescription;
        public const int AmountWorldInfo = 4;

        static WorldTextConfigurator()
        {
            choosenID = -1;
            worldDescription = new Dictionary<int, TextBox>(AmountWorldInfo);
            ConfigureWorldDescription();
        }

        private static void ConfigureWorldDescription()
        {
            worldDescription[0] = new TextBox(WorldTextLeftBound, WorldTextTopBound + (Program.TextSize + 5) * 0, $"Ground heat - ");
            worldDescription[1] = new TextBox(WorldTextLeftBound, WorldTextTopBound + (Program.TextSize + 5) * 1, $"Sun heat - ");
            worldDescription[2] = new TextBox(WorldTextLeftBound, WorldTextTopBound + (Program.TextSize + 5) * 2, $"Cell fall chance - ");
            worldDescription[3] = new TextBox(WorldTextLeftBound, WorldTextTopBound + (Program.TextSize + 5) * 3, $"Environment density - ");

            worldDescription[0].SetText($"{Simulator.GroundPower}");
            worldDescription[1].SetText($"{Simulator.SunPower}");
            worldDescription[2].SetText($"{(int)(Simulator.DropChance * 100)}");
            worldDescription[3].SetText($"{(int)(Simulator.EnvDensity * 100)}");
        }

        /// <summary>
        /// This method providing the ability to add characters to the selected field 
        /// </summary>
        /// <param name="text"></param>
        public static void UpdateWorldInfo(string text)
        {
            worldDescription[choosenID].UpdateText(text);
        }

        /// <summary>
        /// This method providing the ability to delete lasta characters from the selected field 
        /// </summary>
        public static void BackspaceHandle()
        {
            worldDescription[choosenID].BackspaceHandle();
        }

        public static void WorldUpdateInfo()
        {
            worldDescription[0].SetText($"{Simulator.GroundPower}");
            worldDescription[1].SetText($"{Simulator.SunPower}");
            worldDescription[2].SetText($"{(int)(Simulator.DropChance * 100)}");
            worldDescription[3].SetText($"{(int)(Simulator.EnvDensity * 100)}");
        }

        public static void ChooseWorldTextField(int x, int y)
        {
            int result = (y - WorldTextTopBound) / (Program.TextSize + 5);
            for (int id = 0; id < AmountWorldInfo; id++)
            {
                worldDescription[id].Unchoose();
            }
            worldDescription[result].Choose();
            choosenID = result;
        }

        public static bool MouseHandle(int x, int y)
        {
            for (int id = 0; id < AmountWorldInfo; id++)
            {
                if (worldDescription[id].IsHit(x, y))
                {
                    for (int k = 0; k < AmountWorldInfo; k++)
                    {
                        worldDescription[k].Unchoose();
                    }
                    choosenID = id;
                    worldDescription[id].Choose();
                    return true;
                }
            }
            return false;
        }

        public static void Draw()
        {
            for (int id = 0; id < AmountWorldInfo; id++)
            {
                Program.Window.Draw(worldDescription[id]);
            }
        }

        public static void EscapeHandle()
        {
            for (int id = 0; id < AmountWorldInfo; id++)
            {
                worldDescription[id].Unchoose();
            }
        }

        public static int GetGroundPower()
        {
            int result = 0;
            string temp = worldDescription[0].GetText().Substring("Ground heat - ".Length);
            if (GetInt(temp, out result))
                return result;
            else
                return Simulator.GroundPower;
        }

        public static int GetSunPower()
        {
            int result = 0;
            string temp = worldDescription[1].GetText().Substring("Sun heat - ".Length);
            if (GetInt(temp, out result))
                return result;
            else
                return Simulator.SunPower;
        }

        public static double GetDropChance()
        {
            int result = 0;
            string temp = worldDescription[2].GetText().Substring("Cell fall chance - ".Length);
            if (GetInt(temp, out result))
                return result / 100.0;
            else
                return Simulator.DropChance;
        }

        public static double GetEnvDensity()
        {
            int result = 0;
            string temp = worldDescription[3].GetText().Substring("Environment density - ".Length);
            if (GetInt(temp, out result))
                return result / 100.0;
            else
                return Simulator.EnvDensity;
        }

        private static bool GetInt(string input, out int result)
        {
            result = 0;
            for (int i = 0; i < input.Length; i++)
            {
                if ('0' <= input[i] && input[i] <= '9')
                    result = result * 10 + int.Parse(input[i].ToString());
                else
                    return false;
            }
            return true;
        }
    }
}
