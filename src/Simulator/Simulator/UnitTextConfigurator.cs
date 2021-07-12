using SFML.Graphics;
using SFML.System;

using Simulator.World;

using System;
using System.Collections.Generic;
using System.Text;
using TGUI;

namespace Simulator
{
    public static class UnitTextConfigurator
    {
        public const int UnitTextLeftBound = Program.LeftMapOffset + Swamp.WorldWidth * Program.ViewScale + 20;
        public const int UnitTextTopBound = Program.TopMapOffset + (Program.CharacterSize + 5) * 3 + 10;
        public const int UnitTextHeight = (Program.CharacterSize + 20) * AmountUnitInfo;
        public const int UnitTextWidth = 100;

        /// <summary>
        /// Unit text blocks: 0 - unit energy, 1 - unit genes as integer sequence, 2 - chlorophyl,
        /// 3 - last direction, 4 - favorite direction, 5 - capacity, 6 - coords,
        /// 7 - attack power, 8 - parent, 9 - current action
        /// </summary>
        static Dictionary<int, (Label, TextBox)> unitDescription;
        public const int AmountUnitInfo = 10;

        public static Unit ChoosenUnit { get; private set; }

        static UnitTextConfigurator()
        {
            ChoosenUnit = null;
            unitDescription = new Dictionary<int, (Label, TextBox)>(AmountUnitInfo);
            ConfigureUnitDescription();
        }

        private static void ConfigureUnitDescription()
        {
            Label tempLabel = new Label();
            TextBox textBoxTemp = new TextBox();
            tempLabel.Text = "Energy: ";
            tempLabel.TextSize = Program.CharacterSize;
            tempLabel.Position = new Vector2f(UnitTextLeftBound, UnitTextTopBound);
            textBoxTemp.Position = new Vector2f(UnitTextLeftBound + tempLabel.Size.X, UnitTextTopBound);
            textBoxTemp.TextSize = Program.CharacterSize;
            textBoxTemp.Size = new Vector2f(Program.CharacterSize * 4, Program.CharacterSize + 10);
            textBoxTemp.Renderer.BackgroundColor = Color.Black;
            Program.MainGui.Add(tempLabel);
            Program.MainGui.Add(textBoxTemp);
            unitDescription[0] = (tempLabel, textBoxTemp);

            tempLabel = new Label();
            textBoxTemp = new TextBox();
            tempLabel.Text = "Genes: ";
            tempLabel.TextSize = Program.CharacterSize;
            tempLabel.Position = new Vector2f(UnitTextLeftBound, UnitTextTopBound + Program.CharacterSize + 10);
            textBoxTemp.Position = new Vector2f(UnitTextLeftBound + tempLabel.Size.X, UnitTextTopBound + Program.CharacterSize + 10);
            textBoxTemp.TextSize = Program.CharacterSize;
            textBoxTemp.Size = new Vector2f(Program.CharacterSize * 4, Program.CharacterSize + 10);
            //textBoxTemp.Renderer.BackgroundColor = Color.Black;
            Program.MainGui.Add(tempLabel);
            Program.MainGui.Add(textBoxTemp);
            unitDescription[1] = (tempLabel, textBoxTemp);

            tempLabel = new Label();
            textBoxTemp = new TextBox();
            tempLabel.Text = "Chlorophyl: ";
            tempLabel.TextSize = Program.CharacterSize;
            tempLabel.Position = new Vector2f(UnitTextLeftBound, UnitTextTopBound + (Program.CharacterSize + 10) * 2);
            textBoxTemp.Position = new Vector2f(UnitTextLeftBound + tempLabel.Size.X, UnitTextTopBound + (Program.CharacterSize + 10) * 2);
            textBoxTemp.TextSize = Program.CharacterSize;
            textBoxTemp.Size = new Vector2f(Program.CharacterSize * 4, Program.CharacterSize + 10);
            textBoxTemp.Renderer.BackgroundColor = Color.Black;
            Program.MainGui.Add(tempLabel);
            Program.MainGui.Add(textBoxTemp);
            unitDescription[2] = (tempLabel, textBoxTemp);

            tempLabel = new Label();
            textBoxTemp = new TextBox();
            tempLabel.Text = "Last Direction: ";
            tempLabel.TextSize = Program.CharacterSize;
            tempLabel.Position = new Vector2f(UnitTextLeftBound, UnitTextTopBound + (Program.CharacterSize + 10) * 3);
            textBoxTemp.Position = new Vector2f(UnitTextLeftBound + tempLabel.Size.X, UnitTextTopBound + (Program.CharacterSize + 10) * 3);
            textBoxTemp.TextSize = Program.CharacterSize;
            textBoxTemp.Size = new Vector2f(Program.CharacterSize * 4, Program.CharacterSize + 10);
            textBoxTemp.Renderer.BackgroundColor = Color.Black;
            Program.MainGui.Add(tempLabel);
            Program.MainGui.Add(textBoxTemp);
            unitDescription[3] = (tempLabel, textBoxTemp);

            tempLabel = new Label();
            textBoxTemp = new TextBox();
            tempLabel.Text = "Favorite Direction: ";
            tempLabel.TextSize = Program.CharacterSize;
            tempLabel.Position = new Vector2f(UnitTextLeftBound, UnitTextTopBound + (Program.CharacterSize + 10) * 4);
            textBoxTemp.Position = new Vector2f(UnitTextLeftBound + tempLabel.Size.X, UnitTextTopBound + (Program.CharacterSize + 10) * 4);
            textBoxTemp.TextSize = Program.CharacterSize;
            textBoxTemp.Size = new Vector2f(Program.CharacterSize * 4, Program.CharacterSize + 10);
            textBoxTemp.Renderer.BackgroundColor = Color.Black;
            Program.MainGui.Add(tempLabel);
            Program.MainGui.Add(textBoxTemp);
            unitDescription[4] = (tempLabel, textBoxTemp);

            tempLabel = new Label();
            textBoxTemp = new TextBox();
            tempLabel.Text = "Capacity: ";
            tempLabel.TextSize = Program.CharacterSize;
            tempLabel.Position = new Vector2f(UnitTextLeftBound, UnitTextTopBound + (Program.CharacterSize + 10) * 5);
            textBoxTemp.Position = new Vector2f(UnitTextLeftBound + tempLabel.Size.X, UnitTextTopBound + (Program.CharacterSize + 10) * 5);
            textBoxTemp.TextSize = Program.CharacterSize;
            textBoxTemp.Size = new Vector2f(Program.CharacterSize * 4, Program.CharacterSize + 10);
            textBoxTemp.Renderer.BackgroundColor = Color.Black;
            Program.MainGui.Add(tempLabel);
            Program.MainGui.Add(textBoxTemp);
            unitDescription[5] = (tempLabel, textBoxTemp);

            tempLabel = new Label();
            textBoxTemp = new TextBox();
            tempLabel.Text = "Coords: ";
            tempLabel.TextSize = Program.CharacterSize;
            tempLabel.Position = new Vector2f(UnitTextLeftBound, UnitTextTopBound + (Program.CharacterSize + 10) * 6);
            textBoxTemp.Position = new Vector2f(UnitTextLeftBound + tempLabel.Size.X, UnitTextTopBound + (Program.CharacterSize + 10) * 6);
            textBoxTemp.TextSize = Program.CharacterSize;
            textBoxTemp.Size = new Vector2f(Program.CharacterSize * 8, Program.CharacterSize + 10);
            textBoxTemp.Renderer.BackgroundColor = Color.Black;
            Program.MainGui.Add(tempLabel);
            Program.MainGui.Add(textBoxTemp);
            unitDescription[6] = (tempLabel, textBoxTemp);

            tempLabel = new Label();
            textBoxTemp = new TextBox();
            tempLabel.Text = "AttackPower: ";
            tempLabel.TextSize = Program.CharacterSize;
            tempLabel.Position = new Vector2f(UnitTextLeftBound, UnitTextTopBound + (Program.CharacterSize + 10) * 7);
            textBoxTemp.Position = new Vector2f(UnitTextLeftBound + tempLabel.Size.X, UnitTextTopBound + (Program.CharacterSize + 10) * 7);
            textBoxTemp.TextSize = Program.CharacterSize;
            textBoxTemp.Size = new Vector2f(Program.CharacterSize * 8, Program.CharacterSize + 10);
            textBoxTemp.Renderer.BackgroundColor = Color.Black;
            Program.MainGui.Add(tempLabel);
            Program.MainGui.Add(textBoxTemp);
            unitDescription[7] = (tempLabel, textBoxTemp);

            tempLabel = new Label();
            textBoxTemp = new TextBox();
            tempLabel.Text = "Parent: ";
            tempLabel.TextSize = Program.CharacterSize;
            tempLabel.Position = new Vector2f(UnitTextLeftBound, UnitTextTopBound + (Program.CharacterSize + 10) * 8);
            textBoxTemp.Position = new Vector2f(UnitTextLeftBound + tempLabel.Size.X, UnitTextTopBound + (Program.CharacterSize + 10) * 8);
            textBoxTemp.TextSize = Program.CharacterSize;
            textBoxTemp.Size = new Vector2f(Program.CharacterSize * 8, Program.CharacterSize + 10);
            textBoxTemp.Renderer.BackgroundColor = Color.Black;
            Program.MainGui.Add(tempLabel);
            Program.MainGui.Add(textBoxTemp);
            unitDescription[8] = (tempLabel, textBoxTemp);

            tempLabel = new Label();
            textBoxTemp = new TextBox();
            tempLabel.Text = "Current action: ";
            tempLabel.TextSize = Program.CharacterSize;
            tempLabel.Position = new Vector2f(UnitTextLeftBound, UnitTextTopBound + (Program.CharacterSize + 10) * 9);
            textBoxTemp.Position = new Vector2f(UnitTextLeftBound + tempLabel.Size.X, UnitTextTopBound + (Program.CharacterSize + 10) * 9);
            textBoxTemp.TextSize = Program.CharacterSize;
            textBoxTemp.Size = new Vector2f(Program.CharacterSize * 8, Program.CharacterSize + 10);
            textBoxTemp.Renderer.BackgroundColor = Color.Black;
            Program.MainGui.Add(tempLabel);
            Program.MainGui.Add(textBoxTemp);
            unitDescription[9] = (tempLabel, textBoxTemp);
        }

        public static void ChooseUnit(Unit unit)
        {
            ChoosenUnit = unit;
            unitDescription[0].Item2.Text = unit.Energy.ToString();
            unitDescription[1].Item2.Text = GetStringGenes(ChoosenUnit);
            unitDescription[1].Item2.Size = new Vector2f(unitDescription[1].Item2.Text.Length * Program.CharacterSize, Program.CharacterSize + 10);
            unitDescription[2].Item2.Text = unit.Chlorophyl.ToString();
            unitDescription[3].Item2.Text = unit.LastDirection.ToString();
            unitDescription[4].Item2.Text = $"[{unit.Direction[0]},{unit.Direction[1]}]";
            unitDescription[5].Item2.Text = unit.Capacity.ToString();
            unitDescription[6].Item2.Text = $"[{unit.Coords[0]},{unit.Coords[1]}]";
            unitDescription[7].Item2.Text = unit.AttackPower.ToString();
            unitDescription[8].Item2.Text = unit.Parent.ToString();
            unitDescription[9].Item2.Text = unit.GetCurrentAction().Type().ToString();
        }

        private static string GetStringGenes(Unit unit)
        {
            StringBuilder genes = new StringBuilder(10);
            IAction[][] temp = unit.Genes;
            for (int i = 0; i < temp.Length - 1; i++)
            {
                for (int j = 0; j < temp[i].Length; j++)
                    genes.Append((int)temp[i][j].Type());
                genes.Append("|");
            }
            for (int j = 0; j < temp[temp.Length - 1].Length; j++)
                genes.Append((int)temp[temp.Length - 1][j].Type());
            return genes.ToString();
        }
        public static IAction[][] GetGenesArray()
        {
            return StringToGenesArray(unitDescription[1].Item2.Text, '|');
        }

        private static IAction[][] StringToGenesArray(string input, char separator)
        {
            string[] temp = input.Split(separator);
            IAction[][] result = new IAction[temp.Length][];
            for (int i = 0; i < temp.Length; i++)
            {
                result[i] = new IAction[temp[i].Length];
                int seq = 0;
                bool isParsed = int.TryParse(Reverse(temp[i]), out seq);
                if (!isParsed)
                {
                    Events.ErrorHandler.KnockKnock(null, "Error in applying unit genes. Invalid number.", false);
                    return null;
                }
                for (int j = 0; j < temp[i].Length; j++)
                {
                    switch (seq % 10)
                    {
                        case (int)ActionType.Wait:
                            result[i][j] = new Wait();
                            break;
                        case (int)ActionType.Move:
                            result[i][j] = new Move();
                            break;
                        case (int)ActionType.Photosyntesis:
                            result[i][j] = new Photosyntesis();
                            break;
                        case (int)ActionType.Attack:
                            result[i][j] = new Attack();
                            break;
                        default:
                            result[i][j] = new Wait();
                            break;
                    }
                    seq /= 10;
                }
            }
            Events.ErrorHandler.KnockKnock(null, "Successful applying of unit parameters.", true);
            return result;
        }

        /// <summary>
        /// Take char by width in text. Doesn't work properly.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        private static char GetChar(string text, int width)
        {
            char res = '\0';

            int s_width = 0;
            for (int i = 0; i <= text.Length; i++)
            {
                if (s_width < width && i != text.Length)
                    s_width += (int)Content.Font.GetGlyph(text[i], Program.CharacterSize + 2, true, 3).Advance;
                else
                {
                    res = text[i - 1];
                    break;
                }
            }

            return res;
        }
        public static string ShowDescription(int x, int y)
        {
            if (unitDescription[1].Item2.Text.Length == 0)
                return null;
            if (unitDescription[1].Item2.Position.X < x &&
                unitDescription[1].Item2.Position.Y < y &&
                unitDescription[1].Item2.Position.X + unitDescription[1].Item2.Size.X > x &&
                unitDescription[1].Item2.Position.Y + unitDescription[1].Item2.Size.Y > y)
            {
                int left = (int)unitDescription[1].Item2.Position.X;
                int top = (int)unitDescription[1].Item2.Position.Y;
                char temp = GetChar(unitDescription[1].Item2.Text, x - left);
                if (temp == '\0')
                    return null;
                if (temp == '|')
                    return "Gene block separator";
                if (temp < '0' || temp > '9')
                    return null;
                int gene = temp - '0';
                string result;
                switch (gene)
                {
                    case (int)ActionType.Wait:
                        result = "Wait action. Unit does nothing. Gray color.";
                        break;
                    case (int)ActionType.Move:
                        result = "Move action. Unit is moving somewhere. Blue color.";
                        break;
                    case (int)ActionType.Photosyntesis:
                        result = "Photosyntesis action. Unit photosynthesizes energy. Green color.";
                        break;
                    case (int)ActionType.Attack:
                        result = "Attack action. Unit try to attack another unit. Red color.";
                        break;
                    default:
                        result = "Wait action. Unit does nothing. Gray color.";
                        break;
                }
                return result;
            }
            return null;
        }

        private static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public static void ResetUnit()
        {
            ChoosenUnit = null;
        }

        public static void UpdateUnitInfo(string text)
        {
            //unitDescription[choosenID].UpdateText(text);
        }

        public static void ChoosenUnitUpdateInfo(bool isRunning)
        {
            if (isRunning)
            {
                if (ChoosenUnit != null)
                {
                    if (ChoosenUnit.Status == UnitStatus.Dead)
                    {
                        ChoosenUnit = null;
                        return;
                    }
                    unitDescription[0].Item2.Text = $"{ChoosenUnit.Energy}";
                    unitDescription[1].Item2.Text = GetStringGenes(ChoosenUnit);
                    unitDescription[2].Item2.Text = ChoosenUnit.Chlorophyl.ToString();
                    unitDescription[3].Item2.Text = ChoosenUnit.LastDirection.ToString();
                    unitDescription[4].Item2.Text = $"[{ChoosenUnit.Direction[0]},{ChoosenUnit.Direction[1]}]";
                    unitDescription[5].Item2.Text = ChoosenUnit.Capacity.ToString();
                    unitDescription[6].Item2.Text = $"[{ChoosenUnit.Coords[0]},{ChoosenUnit.Coords[1]}]";
                    unitDescription[7].Item2.Text = ChoosenUnit.AttackPower.ToString();
                    unitDescription[8].Item2.Text = ChoosenUnit.Parent.ToString();
                    unitDescription[9].Item2.Text = ChoosenUnit.GetCurrentAction().Type().ToString();
                }
                else
                {
                    ClearUnitDescription();
                }
            }
        }

        public static void ClearUnitDescription()
        {
            for (int id = 0; id < AmountUnitInfo; id++)
            {
                unitDescription[id].Item2.Text = "";
            }
        }

        public static int GetEnergyInfo()
        {
            return GetInt(unitDescription[0].Item2.Text);
        }

        private static int GetInt(string input)
        {
            int result = 0;
            for (int i = 0; i < input.Length; i++)
            {
                if ('0' <= input[i] && input[i] <= '9')
                    result = result * 10 + int.Parse(input[i].ToString());
                else
                {
                    Events.ErrorHandler.KnockKnock(null, "Error in applying unit parameters. Invalid number.", false);
                    return ChoosenUnit.Energy;
                }
            }
            Events.ErrorHandler.KnockKnock(null, "Successful applying of unit parameters.", true);
            return result;
        }
    }
}
