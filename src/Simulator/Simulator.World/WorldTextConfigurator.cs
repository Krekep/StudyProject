﻿using SFML.Graphics;

using Simulator.World;

using System;
using System.Collections.Generic;
using System.Text;

namespace Simulator
{
    public static class WorldTextConfigurator
    {
        public const int WorldTextLeftBound = Simulator.LeftMapOffset + 20;
        public const int WorldTextTopBound = Simulator.TopMapOffset + Simulator.WorldHeight * Simulator.ViewScale + + 10;
        public const int WorldTextHeight = (Content.CharacterSize + 5) * AmountWorldInfo;
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
            worldDescription[0] = new TextBox(WorldTextLeftBound, WorldTextTopBound + (Content.CharacterSize + 5) * 0, $"Ground heat - ");
            worldDescription[1] = new TextBox(WorldTextLeftBound, WorldTextTopBound + (Content.CharacterSize + 5) * 1, $"Sun heat - ");
            worldDescription[2] = new TextBox(WorldTextLeftBound, WorldTextTopBound + (Content.CharacterSize + 5) * 2, $"Cell fall chance - ");
            worldDescription[3] = new TextBox(WorldTextLeftBound, WorldTextTopBound + (Content.CharacterSize + 5) * 3, $"Environment density - ");

            worldDescription[0].SetText($"{Storage.CurrentWorld.GroundPower}");
            worldDescription[1].SetText($"{Storage.CurrentWorld.SunPower}");
            worldDescription[2].SetText($"{(int)(Storage.CurrentWorld.DropChance * 100)}");
            worldDescription[3].SetText($"{(int)(Storage.CurrentWorld.EnvDensity * 100)}");
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

        public static void WorldResetText()
        {
            worldDescription[0].SetText($"{Storage.CurrentWorld.GroundPower}");
            worldDescription[1].SetText($"{Storage.CurrentWorld.SunPower}");
            worldDescription[2].SetText($"{(int)(Storage.CurrentWorld.DropChance * 100)}");
            worldDescription[3].SetText($"{(int)(Storage.CurrentWorld.EnvDensity * 100)}");
        }

        public static void ChooseWorldTextField(int x, int y)
        {
            int result = (y - WorldTextTopBound) / (Content.CharacterSize + 5);
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

        public static void Draw(RenderWindow win)
        {
            for (int id = 0; id < AmountWorldInfo; id++)
            {
                win.Draw(worldDescription[id]);
            }
        }

        public static void EscapeHandle()
        {
            for (int id = 0; id < AmountWorldInfo; id++)
            {
                worldDescription[id].Unchoose();
            }
        }


        public static (bool, int, int, double, double) GetParameters()
        {
            var result = (false, 0, 0, 0.0, 0.0);
            bool success = true;
            int value = 0;
            string temp;

            temp = worldDescription[0].GetEnteredText();
            if (GetInt(temp, out value))
            {
                success &= true;
                result.Item2 = value;
            }
            else
                success = false;

            temp = worldDescription[1].GetEnteredText();
            if (GetInt(temp, out value))
            {
                success &= true;
                result.Item3 = value;
            }
            else
                success = false;

            temp = worldDescription[2].GetEnteredText();
            if (GetInt(temp, out value))
            {
                success &= true;
                result.Item4 = value / 100.0;
            }
            else
                success = false;

            temp = worldDescription[3].GetEnteredText();
            if (GetInt(temp, out value))
            {
                success &= true;
                result.Item5 = value / 100.0;
            }
            else
                success = false;

            if (success)
            {
                result.Item1 = true;
                Events.ErrorHandler.KnockKnock(null, "Successful applying of world parameters.", true);
            }
            else
            {
                result.Item1 = false;
                Events.ErrorHandler.KnockKnock(null, "Error in applying world parameters. Invalid number.", false);
            }
            return result;
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
