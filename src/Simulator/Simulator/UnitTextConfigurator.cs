using SFML.Graphics;
using SFML.System;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simulator
{
    public static class UnitTextConfigurator
    {
        public const int UnitTextLeftBound = Program.LeftMapOffset + Simulator.WorldWidth * Simulator.Scale + 20;
        public const int UnitTextTopBound = Program.TopMapOffset + (Program.TextSize + 5) * 3 + 10;
        public const int UnitTextHeight = (Program.TextSize + 20) * AmountUnitInfo;
        public const int UnitTextWidth = 100;

        private static int choosenID;

        /// zero - unit energy
        /// first - unit genes as integer sequence
        static Dictionary<int, TextBox> unitDescription;
        public const int AmountUnitInfo = 2;

        public static Unit ChoosenUnit { get; private set; }

        static UnitTextConfigurator()
        {
            choosenID = -1;
            ChoosenUnit = null;
            unitDescription = new Dictionary<int, TextBox>(AmountUnitInfo);
            ConfigureUnitDescription();
        }

        private static void ConfigureUnitDescription()
        {
            unitDescription[0] = new TextBox(UnitTextLeftBound, UnitTextTopBound + (Program.TextSize + 7) * 0, "Energy - ");
            unitDescription[1] = new TextBox(UnitTextLeftBound, UnitTextTopBound + (Program.TextSize + 7) * 1, "Genes - ");
        }

        public static void ChooseUnit(Unit unit)
        {
            ChoosenUnit = unit;
            unitDescription[0].SetText(unit.Energy.ToString());
            unitDescription[1].SetText(GetStringGenes(ChoosenUnit));
        }

        public static string GetStringGenes(Unit unit)
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
            return StringToGenesArray(unitDescription[1].GetText().Substring("Genes - ".Length), '|');
        }

        public static IAction[][] StringToGenesArray(string input, char separator)
        {
            string[] temp = input.Split(separator);
            IAction[][] result = new IAction[temp.Length][];
            for (int i = 0; i < temp.Length; i++)
            {
                result[i] = new IAction[temp[i].Length];
                int seq = int.Parse(Reverse(temp[i]));
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
                        default:
                            result[i][j] = new Wait();
                            break;
                    }
                    seq /= 10;
                }
            }
            return result;
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
            unitDescription[choosenID].UpdateText(text);
        }

        public static void BackspaceHandle()
        {
            unitDescription[choosenID].BackspaceHandle();
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
                    unitDescription[0].SetText($"{ChoosenUnit.Energy}");
                    unitDescription[1].SetText(GetStringGenes(ChoosenUnit));
                }
                else
                {
                    ClearUnitDescription();
                }
            }
        }

        public static void ChooseUnitTextField(int x, int y)
        {
            int result = (y - UnitTextTopBound) / (Program.TextSize + 7);
            for (int id = 0; id < AmountUnitInfo; id++)
            {
                unitDescription[id].Unchoose();
            }
            unitDescription[result].Choose();
            choosenID = result;
        }

        public static void ClearUnitDescription()
        {
            for (int id = 0; id < AmountUnitInfo; id++)
            {
                unitDescription[id].SetText("");
            }
        }

        public static bool MouseHandle(int x, int y)
        {
            for (int id = 0; id < AmountUnitInfo; id++)
            {
                if (unitDescription[id].IsHit(x, y))
                {
                    for (int k = 0; k < AmountUnitInfo; k++)
                    {
                        unitDescription[k].Unchoose();
                    }
                    unitDescription[id].Choose();
                    choosenID = id;
                    return true;
                }
            }
            return false;
        }

        public static void Draw()
        {
            for (int id = 0; id < AmountUnitInfo; id++)
            {
                Program.Window.Draw(unitDescription[id]);
            }
        }


        public static int GetEnergyInfo()
        {
            return GetInt(unitDescription[0].GetText());
        }

        private static int GetInt(string input)
        {
            int result = 0;
            for (int i = 0; i < input.Length; i++)
            {
                if ('0' <= input[i] && input[i] <= '9')
                    result = result * 10 + int.Parse(input[i].ToString());
                else
                    return ChoosenUnit.Energy;
            }
            return result;
        }

        public static void EscapeHandle()
        {
            for (int id = 0; id < AmountUnitInfo; id++)
            {
                unitDescription[id].Unchoose();
            }
        }
    }
}
