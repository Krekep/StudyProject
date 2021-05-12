using System;
using System.Collections.Generic;
using System.IO;

using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Simulator
{

    enum TextField: int
    {
        None = -1,
        UnitText = 1,
        WorldText = 2
    }

    class Program
    {
        static RenderWindow win;
        public static int RandomSeed { get; set; }
        static bool isRunning = false;
        private static bool isOpenMenu;
        static bool isTextEntered = false;

        public static TextField ChoosenField { get; private set; }

        public static RenderWindow Window { get { return win; } }

        public static Simulator World { get; set; }

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
                    World.Update();

                UnitTextConfigurator.ChoosenUnitUpdateInfo(isRunning);

                win.Clear(Color.Black);

                if (isOpenMenu)
                {
                    Menu.Draw();
                }
                else
                {
                    Icons.Draw();
                    UnitTextConfigurator.Draw(win);
                    WorldTextConfigurator.Draw(win);
                    World.Position = new Vector2f(Simulator.LeftMapOffset, Simulator.TopMapOffset);
                    win.Draw(World);
                }
                win.Display();
            }
        }

        private static void Initialize()
        {

            RandomSeed = DateTime.Now.Millisecond;
            ChoosenField = TextField.None;

            World = new Simulator();
            World.Initialize(RandomSeed);
        }

        public static void SetEnergyMap()
        {
            World.ChoosenMap = TypeOfMap.MapOfEnergy;
            Icons.SetMap(TypeOfMap.MapOfEnergy);
        }

        public static void SetActionMap()
        {
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
            else if (UnitTextConfigurator.MouseHandle(e.X, e.Y))
            {
                ChoosenField = TextField.UnitText;
            }
            else if (WorldTextConfigurator.MouseHandle(e.X, e.Y))
            {
                ChoosenField = TextField.WorldText;
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

        internal static void OpenMenu()
        {
            isRunning = false;
            isOpenMenu = true;
            Menu.Open();
        }

        internal static void Restart()
        {
            isRunning = false;
            World.Initialize(RandomSeed);
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
                ChoosenField = TextField.None;
            }
            else if (e.Code == Keyboard.Key.Backspace)
            {
                if (ChoosenField == TextField.UnitText)
                    UnitTextConfigurator.BackspaceHandle();
                if (ChoosenField == TextField.WorldText)
                    WorldTextConfigurator.BackspaceHandle();
                if (isOpenMenu)
                    Menu.BackspaceHandle();
            }
            else if (e.Code == Keyboard.Key.P && !isOpenMenu)
                isRunning ^= true;
            else if (e.Code == Keyboard.Key.Enter && ChoosenField != TextField.None)
            {
                if (UnitTextConfigurator.ChoosenUnit != null)
                {
                    int energy = UnitTextConfigurator.GetEnergyInfo();
                    IAction[][] genes = UnitTextConfigurator.GetGenesArray();
                    UnitTextConfigurator.ChoosenUnit.TakeEnergy(energy - UnitTextConfigurator.ChoosenUnit.Energy);
                    UnitTextConfigurator.ChoosenUnit.SetGenes(genes);
                }

                int groundPower = WorldTextConfigurator.GetGroundPower();
                int sunPower = WorldTextConfigurator.GetSunPower();
                double dropChance = WorldTextConfigurator.GetDropChance();
                double envDensity = WorldTextConfigurator.GetEnvDensity();

                World.UpdateParameters(groundPower, sunPower, dropChance, envDensity);
                WorldTextConfigurator.WorldUpdateInfo();
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
