using SFML.Graphics;
using SFML.System;

using Simulator.World;

using System;
using System.Collections.Generic;
using System.Text;

using TGUI;

namespace Simulator
{
    public static class WorldTextConfigurator
    {
        public const int WorldTextLeftBound = Program.LeftMapOffset + 20;
        public const int WorldTextTopBound = Program.TopMapOffset + Simulator.WorldHeight * Program.ViewScale + + 10;
        public const int WorldTextHeight = (Program.CharacterSize + 5) * AmountWorldInfo;
        public const int WorldTextWidth = 100;
        private static int choosenID;

        /// <summary>
        /// Unit text blocks: 0 - ground heat, 1 - sun heat, cell fall chance, environment density
        /// </summary>
        static Dictionary<int, (Label, TextBox)> worldDescription;
        public const int AmountWorldInfo = 4;

        static WorldTextConfigurator()
        {
            choosenID = -1;
            worldDescription = new Dictionary<int, (Label, TextBox)>(AmountWorldInfo);
            ConfigureWorldDescription();
        }

        private static void ConfigureWorldDescription()
        {
            Label tempLabel = new Label();
            TextBox textBoxTemp = new TextBox();
            tempLabel.Text = "Ground heat: ";
            tempLabel.Position = new Vector2f(WorldTextLeftBound, WorldTextTopBound);
            tempLabel.TextSize = Program.CharacterSize;
            textBoxTemp.Position = new Vector2f(WorldTextLeftBound + tempLabel.Size.X, WorldTextTopBound);
            textBoxTemp.Text = $"{Program.World.GroundPower}";
            textBoxTemp.TextSize = Program.CharacterSize;
            textBoxTemp.Size = new Vector2f(Program.CharacterSize * 4, Program.CharacterSize + 10);
            textBoxTemp.Renderer.BackgroundColor = Color.Black;
            Program.MainGui.Add(tempLabel);
            Program.MainGui.Add(textBoxTemp);
            worldDescription[0] = (tempLabel, textBoxTemp);

            tempLabel = new Label();
            textBoxTemp = new TextBox();
            tempLabel.Text = "Sun heat: ";
            tempLabel.TextSize = Program.CharacterSize;
            tempLabel.Position = new Vector2f(WorldTextLeftBound, WorldTextTopBound + Program.CharacterSize + 5);
            textBoxTemp.Position = new Vector2f(WorldTextLeftBound + tempLabel.Size.X, WorldTextTopBound + Program.CharacterSize + 5);
            textBoxTemp.Text = $"{Program.World.SunPower}";
            textBoxTemp.TextSize = Program.CharacterSize;
            textBoxTemp.Size = new Vector2f(Program.CharacterSize * 4, Program.CharacterSize + 10);
            textBoxTemp.Renderer.BackgroundColor = Color.Black;
            Program.MainGui.Add(tempLabel);
            Program.MainGui.Add(textBoxTemp);
            worldDescription[1] = (tempLabel, textBoxTemp);

            tempLabel = new Label();
            textBoxTemp = new TextBox();
            tempLabel.Text = "Cell fall chance: ";
            tempLabel.TextSize = Program.CharacterSize;
            tempLabel.Position = new Vector2f(WorldTextLeftBound, WorldTextTopBound + (Program.CharacterSize + 5) * 2);
            textBoxTemp.Position = new Vector2f(WorldTextLeftBound + tempLabel.Size.X, WorldTextTopBound + (Program.CharacterSize + 5) * 2);
            textBoxTemp.Text = $"{(int)Program.World.DropChance * 100}";
            textBoxTemp.TextSize = Program.CharacterSize;
            textBoxTemp.Size = new Vector2f(Program.CharacterSize * 4, Program.CharacterSize + 10);
            textBoxTemp.Renderer.BackgroundColor = Color.Black;
            Program.MainGui.Add(tempLabel);
            Program.MainGui.Add(textBoxTemp);
            worldDescription[2] = (tempLabel, textBoxTemp);

            tempLabel = new Label();
            textBoxTemp = new TextBox();
            tempLabel.Text = "Environment density: ";
            tempLabel.TextSize = Program.CharacterSize;
            tempLabel.Position = new Vector2f(WorldTextLeftBound, WorldTextTopBound + (Program.CharacterSize + 5) * 3);
            textBoxTemp.Position = new Vector2f(WorldTextLeftBound + tempLabel.Size.X, WorldTextTopBound + (Program.CharacterSize + 5) * 3);
            textBoxTemp.Text = $"{(int)Program.World.EnvDensity * 100}";
            textBoxTemp.TextSize = Program.CharacterSize;
            textBoxTemp.Size = new Vector2f(Program.CharacterSize * 4, Program.CharacterSize + 10);
            textBoxTemp.Renderer.BackgroundColor = Color.Black;
            Program.MainGui.Add(tempLabel);
            Program.MainGui.Add(textBoxTemp);
            worldDescription[3] = (tempLabel, textBoxTemp);
        }

        /// <summary>
        /// This method providing the ability to add characters to the selected field 
        /// </summary>
        /// <param name="text"></param>
        //public static void UpdateWorldInfo(string text)
        //{
        //    worldDescription[choosenID].UpdateText(text);
        //}

        /// <summary>
        /// This method providing the ability to delete lasta characters from the selected field 
        /// </summary>
        //public static void BackspaceHandle()
        //{
        //    worldDescription[choosenID].BackspaceHandle();
        //}

        public static void WorldResetText()
        {
            worldDescription[0].Item2.Text = $"{Program.World.GroundPower}";
            worldDescription[1].Item2.Text = ($"{Program.World.SunPower}");
            worldDescription[2].Item2.Text = ($"{(int)(Program.World.DropChance * 100)}");
            worldDescription[3].Item2.Text = ($"{(int)(Program.World.EnvDensity * 100)}");
        }


        public static (bool, int, int, double, double) GetParameters()
        {
            var result = (false, 0, 0, 0.0, 0.0);
            bool success = true;
            int value = 0;
            string temp;

            temp = worldDescription[0].Item2.Text;
            if (GetInt(temp, out value))
            {
                success &= true;
                result.Item2 = value;
            }
            else
                success = false;

            temp = worldDescription[1].Item2.Text;
            if (GetInt(temp, out value))
            {
                success &= true;
                result.Item3 = value;
            }
            else
                success = false;

            temp = worldDescription[2].Item2.Text;
            if (GetInt(temp, out value))
            {
                success &= true;
                result.Item4 = value / 100.0;
            }
            else
                success = false;

            temp = worldDescription[3].Item2.Text;
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
