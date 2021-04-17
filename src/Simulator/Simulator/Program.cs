using System;
using System.Collections.Generic;
using System.IO;

using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Simulator
{
    enum TypeOfMap
    {
        MapOfEnergy,
        MapOfActions
    }

    class Program
    {
        static RenderWindow win;
        public static int RandomSeed { get; private set; }
        static bool isRunning = false;
        static bool isTextEntered = false;
        public const int LeftMapOffset = 20;
        public const int TopMapOffset = 50;
        public const int TextSize = 25;

        static RectangleShape swordShape;
        private const int swordSize = TopMapOffset - 10;
        private const int swordLeftSide = LeftMapOffset + TopMapOffset;
        private const int swordTopSide = 5;

        static RectangleShape lightningShape;
        private const int lightningSize = TopMapOffset - 10;
        private const int lightningLeftSide = LeftMapOffset;
        private const int lightningTopSide = 5;

        private const int unitTextLeftBound = LeftMapOffset + Simulator.WorldWidth * Simulator.Scale + 20;
        private const int unitTextTopBound = TopMapOffset + (TextSize + 5) * 3 + 10;
        private const int unitTextHeight = (TextSize + 20) * amountUnitInfo;
        private const int unitTextWidth = 100;
        static Dictionary<int, Text> unitDescription;
        private const int amountUnitInfo = 1;
        private static int choosenID;

        private static RectangleShape fieldIllumination;

        private static Unit choosenUnit;

        public static TypeOfMap ChoosenMap { get; private set; }

        public static RenderWindow Window { get { return win; } }
        static void Main(string[] args)
        {
            Initialize();

            win = new RenderWindow(new VideoMode(1024, 728), "Evolution Simulator");

            win.Closed += Win_Closed;
            win.Resized += Win_Resized;
            win.KeyPressed += Win_KeyPressed;
            win.MouseButtonPressed += Win_MouseButton;
            win.TextEntered += Win_TextEntered;

            while (win.IsOpen)
            {
                win.DispatchEvents();
                if (isRunning)
                    Simulator.Update();
                
                choosenUnitUpdateInfo();

                win.Clear(Color.Black);

                win.Draw(fieldIllumination);
                DrawIcons();
                foreach (var id in unitDescription.Keys)
                {
                    win.Draw(unitDescription[id]);
                }
                Simulator.Draw();

                win.Display();
            }
        }

        private static void Initialize()
        {
            RandomSeed = DateTime.Now.Millisecond;
            choosenID = -1;
            choosenUnit = null;
            unitDescription = new Dictionary<int, Text>();
            LoadContent();
            ConfigureIcons();
            ConfigureUnitDescription();

            fieldIllumination = new RectangleShape(new Vector2f(200, TextSize + 10));
            fieldIllumination.FillColor = Color.Black;

            ChoosenMap = TypeOfMap.MapOfEnergy;
        }

        private static void choosenUnitUpdateInfo()
        {
            if (isRunning)
            {
                if (choosenUnit != null)
                {
                    if (choosenUnit.Status == UnitStatus.Dead)
                    {
                        choosenUnit = null;
                        return;
                    }
                    unitDescription[0].DisplayedString = $"Energy - {choosenUnit.Energy}";
                }
                else
                {
                    ClearUnitDescription();
                }
            }
        }

        private static void ConfigureUnitDescription()
        {
            unitDescription[0] = new Text("Energy - ", Content.Font, TextSize);
            unitDescription[0].Position = new Vector2f(unitTextLeftBound, unitTextTopBound);
        }

        private static void ConfigureIcons()
        {
            swordShape = new RectangleShape(new Vector2f(swordSize, swordSize));
            swordShape.Texture = Content.Sword;
            swordShape.Position = new Vector2f(swordLeftSide, swordTopSide);

            lightningShape = new RectangleShape(new Vector2f(lightningSize, lightningSize));
            lightningShape.Texture = Content.PressedLightning;
            lightningShape.Position = new Vector2f(lightningLeftSide, lightningTopSide);
        }

        private static void DrawIcons()
        {
            win.Draw(swordShape);
            win.Draw(lightningShape);
        }

        private static void LoadContent()
        {
            Content.LoadFont("arial.ttf");
            Content.LoadSword();
            Content.LoadLightning();
        }

        private static int ChooseUnitTextField(int x, int y)
        {
            return (y - unitTextTopBound) / (TextSize + 20);
        }

        private static void ClearUnitDescription()
        {
            unitDescription[0].DisplayedString = "Energy - ";
        }

        private static void SetEnergyMap()
        {
            ChoosenMap = TypeOfMap.MapOfEnergy;
            lightningShape.Texture = Content.PressedLightning;
            swordShape.Texture = Content.Sword;
        }

        private static void SetActionMap()
        {
            ChoosenMap = TypeOfMap.MapOfActions;
            lightningShape.Texture = Content.Lightning;
            swordShape.Texture = Content.PressedSword;
        }

        private static void Win_MouseButton(object sender, MouseButtonEventArgs e)
        {
            if (new IntRect(swordLeftSide, swordTopSide, swordSize, swordSize).Contains(e.X, e.Y))
            {
                SetActionMap();
            }
            else if (new IntRect(lightningLeftSide, lightningTopSide, lightningSize, lightningSize).Contains(e.X, e.Y))
            {
                SetEnergyMap();
            }
            else if (new IntRect(LeftMapOffset, TopMapOffset, Simulator.WorldWidth * Simulator.Scale, Simulator.WorldHeight * Simulator.Scale).Contains(e.X, e.Y))
            {
                int x = (e.X - LeftMapOffset) / Simulator.Scale;
                int y = (e.Y - TopMapOffset) / Simulator.Scale;
                var unit = Simulator.GetUnit(x, y);
                ClearUnitDescription();
                if (unit != null)
                {
                    choosenUnit = unit;
                    unitDescription[0].DisplayedString += unit.Energy.ToString();
                    isRunning = false;
                }
                else
                {
                    choosenUnit = null;
                }
            }
            else if (new IntRect(unitTextLeftBound, unitTextTopBound, unitTextWidth, unitTextHeight).Contains(e.X, e.Y))
            {
                choosenID = ChooseUnitTextField(e.X, e.Y);
                
                fieldIllumination.Position = new Vector2f(unitTextLeftBound, unitTextTopBound + choosenID * (TextSize + 5));
                fieldIllumination.FillColor = Color.Cyan;
            }
        }

        private static void Win_TextEntered(object sender, TextEventArgs e)
        {
            if (!isTextEntered)
                return;
            if (choosenID == -1)
                return;
            if (choosenID < amountUnitInfo)
            {
                unitDescription[choosenID].DisplayedString += e.Unicode;
            }
        }

        private static void Win_KeyPressed(object sender, KeyEventArgs e)
        {
            isTextEntered = false;
            if (e.Code == Keyboard.Key.Escape)
            {
                fieldIllumination.FillColor = Color.Black;
                choosenID = -1;
            }
            else if (e.Code == Keyboard.Key.Backspace && choosenID != -1)
            {
                if (choosenID < amountUnitInfo)
                {
                    string temp = unitDescription[choosenID].DisplayedString;
                    temp = temp.Remove(temp.Length - 1);
                    unitDescription[choosenID].DisplayedString = temp;
                }
            }
            else if (e.Code == Keyboard.Key.P)
                isRunning ^= true;
            else if (e.Code == Keyboard.Key.Enter)
            {
                if (choosenID < amountUnitInfo && choosenUnit != null)
                {
                    int energy = GetInt(unitDescription[0].DisplayedString);
                    choosenUnit.TakeEnergy(energy - choosenUnit.Energy);
                }
            }
            else
                isTextEntered = true;
        }

        private static int GetInt(string input)
        {
            int result = 0;
            for (int i = 0; i < input.Length; i++)
            {
                if ('0' <= input[i] && input[i] <= '9')
                    result = result * 10 + int.Parse(input[i].ToString());
            }
            return result;
        }

        private static void Win_Resized(object sender, SizeEventArgs e)
        {
            win.SetView(new View(new FloatRect(0, 0, e.Width, e.Height)));
        }

        private static void Win_Closed(object sender, EventArgs e)
        {
            win.Close();
        }
    }
}
