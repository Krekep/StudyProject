using System;
using System.Collections.Generic;
using System.IO;

using SFML.Graphics;
using SFML.System;
using SFML.Window;

using Simulator.Events;

namespace Simulator
{

    enum TextField : int
    {
        None = -1,
        UnitText = 1,
        WorldText = 2,
        Seed = 3
    }

    class Program
    {
        static RenderWindow win;
        public static int RandomSeed { get; set; }
        static bool isRunning = false;
        private static bool isOpenMenu;
        static bool isTextEntered = false;

        static Color orange = new Color(255, 128, 0);
        static Color darkGreen = new Color(0, 47, 31);
        static Color darkRed = new Color(104, 28, 35);

        static TextBlock dropDownWindow;

        static TextBlock mapName;
        static TextBlock errorField;
        static TextBox seedText;
        static Button createButton;

        public static TextField ChoosenField { get; private set; }

        public static RenderWindow Window { get { return win; } }

        public static Simulator World { get; set; }


        static void Main(string[] args)
        {
            Initialize();

            win = new RenderWindow(new VideoMode(1280, 720), "Evolution Simulator");

            win.Closed += Win_Closed;
            win.Resized += Win_Resized;
            win.KeyPressed += Win_KeyPressed;
            win.MouseButtonPressed += Win_MouseButton;
            win.TextEntered += Win_TextEntered;
            win.MouseMoved += Win_MouseMoved;
            win.MouseWheelScrolled += Win_MouseWheelScrolled;
            ErrorHandler.Notify += DisplayMessage;

            while (win.IsOpen)
            {
                win.DispatchEvents();
                if (isRunning)
                    World.Update();

                UnitTextConfigurator.ChoosenUnitUpdateInfo(isRunning);

                win.Clear(Color.Black);

                if (isOpenMenu)
                {
                    Menu.Draw();
                }
                else
                {
                    win.Draw(mapName);
                    win.Draw(errorField);
                    win.Draw(createButton);

                    Icons.Draw();
                    win.Draw(seedText);
                    UnitTextConfigurator.Draw(win);
                    WorldTextConfigurator.Draw(win);
                    win.Draw(dropDownWindow);

                    win.Draw(World);
                }
                win.Display();
            }
        }

        private static void DisplayMessage(object sender, Events.ErrorEventArgs e)
        {
            errorField.Text = e.Message;
            if (e.IsSuccess)
                errorField.FillColor = darkGreen;
            else
                errorField.FillColor = darkRed;
        }

        private static void Initialize()
        {
            dropDownWindow = new TextBlock(Color.Black);
            dropDownWindow.CharacterSize = Content.CharacterSize * 3 / 5;

            mapName = new TextBlock(Simulator.LeftMapOffset + Simulator.WorldWidth * 2 / 5 * Simulator.ViewScale, Simulator.TopMapOffset / 2, Color.Black);
            mapName.CharacterSize = Content.CharacterSize * 3 / 4;
            mapName.Text = "Map of energy.";

            errorField = new TextBlock(Simulator.LeftMapOffset + Simulator.WorldWidth * 2 / 5 * Simulator.ViewScale, Simulator.TopMapOffset + Simulator.WorldHeight * Simulator.ViewScale + 10, darkGreen);
            errorField.CharacterSize = Content.CharacterSize * 3 / 4;
            errorField.Text = "Everything OK.";

            RandomSeed = DateTime.Now.Millisecond;
            ChoosenField = TextField.None;

            seedText = new TextBox(Simulator.WorldWidth * Simulator.ViewScale + 1 + Simulator.LeftMapOffset + 20, Simulator.TopMapOffset + 2 * (Content.CharacterSize + 5) + 10, "Seed: ");
            seedText.SetText($"{RandomSeed}");

            int widthCreateButton = (Simulator.TopMapOffset - 10) * 2;
            int heightCreateButton = Simulator.TopMapOffset - 10;
            int leftCreateButton = Simulator.LeftMapOffset + Simulator.WorldWidth * Simulator.ViewScale - widthCreateButton - 10;
            int topCreateButton = 5;
            createButton = new Button(leftCreateButton, topCreateButton, widthCreateButton, heightCreateButton, "Create", Content.CharacterSize);
            createButton.FillColor = new Color(64, 64, 64);

            World = new Simulator();
            World.Initialize(RandomSeed);
            World.Position = new Vector2f(Simulator.LeftMapOffset, Simulator.TopMapOffset);
        }

        public static void SetEnergyMap()
        {
            mapName.Text = "Map of energy.";
            World.ChoosenMap = TypeOfMap.MapOfEnergy;
            Icons.SetMap(TypeOfMap.MapOfEnergy);
        }

        public static void SetActionMap()
        {
            mapName.Text = "Map of actions.";
            World.ChoosenMap = TypeOfMap.MapOfActions;
            Icons.SetMap(TypeOfMap.MapOfActions);
        }

        private static void Win_MouseButton(object sender, MouseButtonEventArgs e)
        {
            Icons.ButtonHandler(e.X, e.Y);
            var unit = World.MouseHandle(e.X, e.Y);
            if (isOpenMenu)
            {
                Menu.MouseHandle(e.X, e.Y);
            }
            else if (UnitTextConfigurator.MouseButtonHandle(e.X, e.Y))
            {
                ChoosenField = TextField.UnitText;
                WorldTextConfigurator.EscapeHandle();
                seedText.Unchoose();
            }
            else if (WorldTextConfigurator.MouseHandle(e.X, e.Y))
            {
                ChoosenField = TextField.WorldText;
                UnitTextConfigurator.EscapeHandle();
                seedText.Unchoose();
            }
            else if (seedText.IsHit(e.X, e.Y))
            {
                seedText.Choose();
                ChoosenField = TextField.Seed;
                UnitTextConfigurator.EscapeHandle();
                WorldTextConfigurator.EscapeHandle();
            }
            else if (createButton.IsHit(e.X, e.Y))
            {
                CreateWorld();
            }
            else if (unit != null)
            {
                UnitTextConfigurator.ChooseUnit(unit);
                isRunning = false;
            }
            else if (unit == null)
            {
                UnitTextConfigurator.ClearUnitDescription();
                UnitTextConfigurator.ResetUnit();
            }
        }

        private static void CreateWorld()
        {
            int temp;
            bool fl = GetInt(seedText.GetEnteredText(), out temp);
            RandomSeed = temp;
            var parameters = WorldTextConfigurator.GetParameters();
            if (parameters.Item1 && fl)
            {
                World.UpdateParameters(parameters.Item2, parameters.Item3, parameters.Item4, parameters.Item5);
                World.Initialize(RandomSeed);
            }
            WorldTextConfigurator.WorldResetText();
        }
        private static bool GetInt(string input, out int seed)
        {
            seed = 0;
            for (int i = 0; i < input.Length; i++)
            {
                if ('0' <= input[i] && input[i] <= '9')
                    seed = seed * 10 + int.Parse(input[i].ToString());
                else
                {
                    ErrorHandler.KnockKnock(null, "Error in receiving seed of world. Invalid number.", false);
                    seed = RandomSeed;
                    return false;
                }
            }
            ErrorHandler.KnockKnock(null, "Successful receiving seed of world.", true);
            return true;
        }

        private static void Win_MouseMoved(object sender, MouseMoveEventArgs e)
        {
            string text = UnitTextConfigurator.ShowDescription(e.X, e.Y);
            if (text != null)
            {
                dropDownWindow.FillColor = orange;
                dropDownWindow.Text = text;
                dropDownWindow.Coords = new Vector2f(e.X, e.Y);
            }
            else
            {
                dropDownWindow.Text = "";
                dropDownWindow.FillColor = Color.Black;
                dropDownWindow.Coords = new Vector2f(0, 0);
            }
        }

        private static void Win_MouseWheelScrolled(object sender, MouseWheelScrollEventArgs e)
        {
            if (isOpenMenu)
                Menu.Scroll(e.Delta);
        }

        internal static void OpenMenu()
        {
            isRunning = false;
            isOpenMenu = true;
            ChoosenField = TextField.None;
            Menu.Open();
        }

        internal static void Restart()
        {
            isRunning = false;
            World.Initialize(World.Seed);
        }

        private static void Win_TextEntered(object sender, TextEventArgs e)
        {
            if (!isTextEntered)
                return;
            if (ChoosenField == TextField.UnitText)
            {
                UnitTextConfigurator.UpdateUnitInfo(e.Unicode);
            }
            if (ChoosenField == TextField.WorldText)
            {
                WorldTextConfigurator.UpdateWorldInfo(e.Unicode);
            }
            if (ChoosenField == TextField.Seed)
            {
                seedText.UpdateText(e.Unicode);
            }
            if (isOpenMenu)
            {
                Menu.UpdateExportName(e.Unicode);
            }
        }

        private static void Win_KeyPressed(object sender, KeyEventArgs e)
        {
            isTextEntered = false;
            if (e.Code == Keyboard.Key.Escape)
            {
                UnitTextConfigurator.EscapeHandle();
                WorldTextConfigurator.EscapeHandle();
                Menu.EscapeHandler();
                seedText.Unchoose();
                ChoosenField = TextField.None;
            }
            else if (e.Code == Keyboard.Key.Backspace)
            {
                if (ChoosenField == TextField.UnitText)
                    UnitTextConfigurator.BackspaceHandle();
                if (ChoosenField == TextField.WorldText)
                    WorldTextConfigurator.BackspaceHandle();
                if (ChoosenField == TextField.Seed)
                    seedText.BackspaceHandle();
                if (isOpenMenu)
                    Menu.BackspaceHandle();
            }
            else if (e.Code == Keyboard.Key.P && !isOpenMenu)
                isRunning ^= true;
            else if (e.Code == Keyboard.Key.Enter && ChoosenField != TextField.None)
            {
                if (ChoosenField == TextField.UnitText)
                {
                    if (UnitTextConfigurator.ChoosenUnit != null)
                    {
                        int energy = UnitTextConfigurator.GetEnergyInfo();
                        IAction[][] genes = UnitTextConfigurator.GetGenesArray();
                        UnitTextConfigurator.ChoosenUnit.TakeEnergy(energy - UnitTextConfigurator.ChoosenUnit.Energy);
                        UnitTextConfigurator.ChoosenUnit.SetGenes(genes);
                    }
                }
                else if (ChoosenField == TextField.WorldText)
                {
                    var parameters = WorldTextConfigurator.GetParameters();

                    if (parameters.Item1)
                        World.UpdateParameters(parameters.Item2, parameters.Item3, parameters.Item4, parameters.Item5);
                    WorldTextConfigurator.WorldResetText();
                }
            }
            else
                isTextEntered = true;
        }

        internal static void CloseMenu()
        {
            isOpenMenu = false;
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
