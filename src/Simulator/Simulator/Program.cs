using System;
using System.Diagnostics;

using SFML.Graphics;
using SFML.System;
using SFML.Window;
using TGUI;

using Simulator.Events;
using Simulator.World;
using System.Threading;

namespace Simulator
{

    enum TextField : int
    {
        None = -1,
        UnitText = 1,
        WorldText = 2,
        Seed = 3
    }

    public enum TypeOfMap
    {
        MapOfEnergy,
        MapOfActions,
    }

    class Program
    {
        static RenderWindow win;
        static Gui gui;
        static bool isRunning = false;
        private static bool isOpenMenu;

        /// <summary>
        /// Buttons: 0 - energy map, 1 - action map, 2 - create, 3 - restart, 4 - menu, 5 - apply
        /// </summary>
        static Button[] buttons = new Button[6];
        /// <summary>
        /// Text blocks: 0 - seed, 1 - dropdownText, 2 - map name, 3 - error field, 4 - fps counter, 5 - year, 6 - unit count
        /// </summary>
        static Label[] labels = new Label[7];

        public static Color Orange = new Color(255, 128, 0);
        public static Color DarkGreen = new Color(0, 47, 31);
        public static Color DarkRed = new Color(104, 28, 35);
        public static Color Gray = new Color(64, 64, 64);
        public static Color DarkGray = new Color(255, 255, 255);

        public const int ViewScale = 6;
        public const int LeftMapOffset = 10;
        public const int TopMapOffset = 70;
        public const int CharacterSize = 25;
        private const int width = 1600;
        private const int height = 900;

        static TextBox seedText;

        public static TypeOfMap ChoosenMap { get; private set; }
        public static TextField ChoosenField { get; private set; }

        public static RenderWindow Window { get { return win; } }
        public static Gui MainGui { get { return gui; } }

        public static Swamp World { get; set; }

        static void Main(string[] args)
        {
            win = new RenderWindow(new VideoMode(width, height), "Evolution Swamp");
            gui = new Gui(win);

            Initialize();

            win.Closed += Win_Closed;
            win.Resized += Win_Resized;
            win.KeyPressed += Win_KeyPressed;
            win.MouseButtonPressed += Win_MouseButton;
            win.MouseMoved += Win_MouseMoved;
            win.MouseWheelScrolled += Win_MouseWheelScrolled;
            ErrorHandler.Notify += DisplayMessage;
            Stopwatch stopwatch = new Stopwatch();
            double fps;

            while (win.IsOpen)
            {
                stopwatch.Restart();
                win.DispatchEvents();
                if (isRunning)
                {
                    World.UpdateByThreads();
                    //World.Update();
                    labels[5].Text = $"Year: {World.Timer}";
                    labels[6].Text = $"Units: {World.Units.UnitsNumbers.Count}";
                }

                UnitTextConfigurator.ChoosenUnitUpdateInfo(isRunning);

                win.Clear(Color.Black);
                fps = 1000 / stopwatch.Elapsed.TotalMilliseconds;
                labels[4].Text = $"FPS: {(int)fps}";

                WorldRenderer.Draw();
                gui.Draw();
                win.Display();
            }
        }

        private static void DisplayMessage(object sender, Events.ErrorEventArgs e)
        {
            labels[3].Text = e.Message;
            if (e.IsSuccess)
                labels[3].Renderer.BackgroundColor = DarkGreen;
            else
                labels[3].Renderer.BackgroundColor = DarkRed;
        }

        private static void Initialize()
        {
            ChoosenField = TextField.None;
            ChoosenMap = TypeOfMap.MapOfEnergy;
            World = new Swamp();
            World.Initialize(DateTime.Now.Millisecond);
            WorldTextConfigurator.WorldResetText();
            ButtonInitalizer();
            TextInitializer();
        }

        private static void ButtonInitalizer()
        {
            buttons[0] = new Button();
            Console.WriteLine(buttons[0].Renderer);
            buttons[0].Position = new Vector2f(LeftMapOffset, 10);
            buttons[0].Size = new Vector2f(TopMapOffset - 15, TopMapOffset - 15);
            buttons[0].Renderer.Texture = Content.PressedLightning;
            buttons[0].Clicked += EnergyMap_Click;
            gui.Add(buttons[0]);

            buttons[1] = new Button();
            buttons[1].Position = new Vector2f(LeftMapOffset + TopMapOffset, 10);
            buttons[1].Size = new Vector2f(TopMapOffset - 15, TopMapOffset - 15);
            buttons[1].Renderer.Texture = Content.Sword;
            buttons[1].Clicked += ActionMap_Click;
            gui.Add(buttons[1]);

            int widthCreateButton = (TopMapOffset - 10) * 2;
            int heightCreateButton = TopMapOffset - 15;
            int leftCreateButton = LeftMapOffset + Swamp.WorldWidth * ViewScale - widthCreateButton - 10;
            int topCreateButton = 10;
            buttons[2] = new Button();
            buttons[2].Position = new Vector2f(leftCreateButton, topCreateButton);
            buttons[2].Size = new Vector2f(widthCreateButton, heightCreateButton);
            buttons[2].Text = "Create";
            buttons[2].Renderer.BackgroundColor = Gray;
            buttons[2].Renderer.TextColor = Color.White;
            buttons[2].Clicked += Create_Click;
            gui.Add(buttons[2]);

            buttons[3] = new Button();
            buttons[3].Position = new Vector2f(LeftMapOffset + Swamp.WorldWidth * ViewScale + 10, 10);
            buttons[3].Size = new Vector2f(TopMapOffset - 15, TopMapOffset - 15);
            buttons[3].Renderer.Texture = Content.RestartButton;
            buttons[3].Clicked += Restart_Click;
            gui.Add(buttons[3]);

            buttons[4] = new Button();
            buttons[4].Position = new Vector2f(LeftMapOffset + Swamp.WorldWidth * ViewScale + TopMapOffset + 10, 10);
            buttons[4].Size = new Vector2f(TopMapOffset - 15, TopMapOffset - 15);
            buttons[4].Renderer.Texture = Content.MenuButton;
            buttons[4].Clicked += Menu_Click;
            gui.Add(buttons[4]);

            int widthApplyButton = (TopMapOffset - 10) * 2;
            int heightApplyButton = TopMapOffset - 15;
            int leftApplyButton = LeftMapOffset + Swamp.WorldWidth * ViewScale - widthCreateButton - 10;
            int topApplyButton = 10 + TopMapOffset + Swamp.WorldHeight * ViewScale;
            buttons[5] = new Button();
            buttons[5].Position = new Vector2f(leftApplyButton, topApplyButton);
            buttons[5].Size = new Vector2f(widthApplyButton, heightApplyButton);
            buttons[5].Text = "Apply";
            buttons[5].TextSize = (uint)Math.Min(heightApplyButton - 8, widthApplyButton / 4);
            buttons[5].Renderer.BackgroundColor = Gray;
            buttons[5].Renderer.TextColor = Color.White;
            buttons[5].Clicked += Apply_Clicked;
            gui.Add(buttons[5]);
        }

        private static void TextInitializer()
        {
            seedText = new TextBox();
            seedText.Position = new Vector2f(Swamp.WorldWidth * ViewScale + 1 + LeftMapOffset + CharacterSize * 4, TopMapOffset + 2 * (CharacterSize + 5) + 10);
            seedText.TextSize = CharacterSize;
            seedText.Size = new Vector2f(Program.CharacterSize * 4, Program.CharacterSize + 10);
            seedText.Text = $"{World.Seed}";
            gui.Add(seedText);

            labels[0] = new Label();
            labels[0].Position = new Vector2f(Swamp.WorldWidth * ViewScale + 1 + LeftMapOffset + 20, TopMapOffset + 2 * (CharacterSize + 5) + 10);
            labels[0].Text = "Seed: ";
            labels[0].TextSize = CharacterSize;
            gui.Add(labels[0]);

            labels[1] = new Label();
            labels[1].TextSize = CharacterSize * 3 / 5;
            labels[1].Visible = false;
            labels[1].MoveToFront();
            gui.Add(labels[1]);

            labels[2] = new Label();
            labels[2].Position = new Vector2f(LeftMapOffset + Swamp.WorldWidth * 2 / 5 * ViewScale, TopMapOffset / 2);
            labels[2].Text = "Map of energy.";
            labels[2].TextSize = CharacterSize * 3 / 5;
            gui.Add(labels[2]);

            labels[3] = new Label();
            labels[3].Position = new Vector2f(LeftMapOffset + Swamp.WorldWidth * 2 / 5 * ViewScale, TopMapOffset + Swamp.WorldHeight * ViewScale + 10);
            labels[3].Text = "Everything OK.";
            labels[3].TextSize = CharacterSize * 3 / 5;
            labels[3].Renderer.BackgroundColor = DarkGreen;
            gui.Add(labels[3]);

            labels[4] = new Label();
            labels[4].Position = new Vector2f(width - 100, 5);
            labels[4].TextSize = CharacterSize * 3 / 5;
            labels[4].Renderer.TextColor = Color.White;
            gui.Add(labels[4]);

            labels[5] = new Label();
            labels[5].Position = new Vector2f(Swamp.WorldWidth * ViewScale + 21 + LeftMapOffset, TopMapOffset);
            labels[5].Text = "Year:";
            labels[5].TextSize = CharacterSize;
            gui.Add(labels[5]);

            labels[6] = new Label();
            labels[6].Position = new Vector2f(Swamp.WorldWidth * ViewScale + 21 + LeftMapOffset, TopMapOffset + CharacterSize + 5 + 10);
            labels[6].Text = "Units:";
            labels[6].TextSize = CharacterSize;
            gui.Add(labels[6]);
        }
        private static void Apply_Clicked(object sender, SignalArgsVector2f e)
        {
            if (UnitTextConfigurator.ChoosenUnit != -1)
            {
                int energy = UnitTextConfigurator.GetEnergyInfo();
                IAction[][] genes = UnitTextConfigurator.GetGenesArray();
                World.Units.TakeEnergy(UnitTextConfigurator.ChoosenUnit, energy - World.Units.UnitsEnergy[UnitTextConfigurator.ChoosenUnit]);
                //UnitTextConfigurator.ChoosenUnit.SetGenes(genes);
            }
            var parameters = WorldTextConfigurator.GetParameters();

            if (parameters.Item1)
                World.UpdateParameters(parameters.Item2, parameters.Item3, parameters.Item4, parameters.Item5);
            WorldTextConfigurator.WorldResetText();
        }

        private static void EnergyMap_Click(object sender, SignalArgsVector2f e)
        {
            labels[2].Text = "Map of energy.";
            ChoosenMap = TypeOfMap.MapOfEnergy;
            buttons[0].Renderer.Texture = Content.PressedLightning;
            buttons[1].Renderer.Texture = Content.Sword;
        }

        private static void ActionMap_Click(object sender, SignalArgsVector2f e)
        {
            labels[2].Text = "Map of action.";
            ChoosenMap = TypeOfMap.MapOfActions;
            buttons[0].Renderer.Texture = Content.Lightning;
            buttons[1].Renderer.Texture = Content.PressedSword;
        }

        private static void Create_Click(object sender, SignalArgsVector2f e)
        {
            int temp;
            bool fl = GetInt(seedText.Text, out temp);
            var parameters = WorldTextConfigurator.GetParameters();
            if (parameters.Item1 && fl)
            {
                World.UpdateParameters(parameters.Item2, parameters.Item3, parameters.Item4, parameters.Item5);
                World.Initialize(temp);
            }
            WorldTextConfigurator.WorldResetText();
        }

        private static void Restart_Click(object sender, SignalArgsVector2f e)
        {
            isRunning = false;
            World.Initialize(World.Seed);
        }

        private static void Menu_Click(object sender, SignalArgsVector2f e)
        {
            isRunning = false;
            ChoosenField = TextField.None;
            isOpenMenu = true;
            Menu.Open();
        }
        private static void Win_MouseButton(object sender, MouseButtonEventArgs e)
        {
            int x = (e.X - LeftMapOffset) / ViewScale;
            int y = (e.Y - TopMapOffset) / ViewScale;
            if (x >= 0 && y >= 0 && x < Swamp.WorldWidth && y < Swamp.WorldHeight)
            {
                var unit = World.GetUnit(x, y);
                if (unit != -1)
                {
                    UnitTextConfigurator.ChooseUnit(unit);
                    isRunning = false;
                }
                else if (unit == -1)
                {
                    UnitTextConfigurator.ClearUnitDescription();
                    UnitTextConfigurator.ResetUnit();
                }
            }
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
                    ErrorHandler.KnockKnock(null, "Error in receiving int from string.", false);
                    seed = World.Seed;
                    return false;
                }
            }
            ErrorHandler.KnockKnock(null, "Successful receiving int from string.", true);
            return true;
        }

        private static void Win_MouseMoved(object sender, MouseMoveEventArgs e)
        {
            string text = UnitTextConfigurator.ShowDescription(e.X, e.Y);
            if (text != null)
            {
                labels[1].Renderer.BackgroundColor = Orange;
                labels[1].Text = text;
                labels[1].Position = new Vector2f(e.X, e.Y);
                labels[1].Visible = true;
                labels[1].MoveToFront();
            }
            else
            { 
                labels[1].Visible = false;
            }
        }

        private static void Win_MouseWheelScrolled(object sender, MouseWheelScrollEventArgs e)
        {
            if (isOpenMenu)
                Menu.Scroll(e.Delta);
        }


        private static void Win_KeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.P && !isOpenMenu)
                isRunning ^= true;
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
