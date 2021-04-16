using System;
using System.IO;

using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Simulator
{
    class Program
    {
        static RenderWindow win;
        public static int RandomSeed { get; private set; }
        static bool isRunning = false;
        static bool isUnitDescription = false;
        public const int LeftMapOffset = 20;
        public const int TopMapOffset = 50;

        static RectangleShape swordShape;
        private const int swordSize = TopMapOffset - 10;
        private const int swordLeftSide = LeftMapOffset;
        private const int swordTopSide = 5;

        static RectangleShape lightningShape;
        private const int lightningSize = TopMapOffset - 10;
        private const int lightningLeftSide = LeftMapOffset  + TopMapOffset;
        private const int lightningTopSide = 5;

        static Text[] unitDescription;
        private const int amountUnitInfo = 1;

        public static RenderWindow Window { get { return win; } }
        static void Main(string[] args)
        {
            RandomSeed = DateTime.Now.Millisecond;
            unitDescription = new Text[amountUnitInfo];
            LoadContent();
            ConfigureIcons();
            ConfigureUnitDescription();

            win = new RenderWindow(new VideoMode(1024, 728), "Evolution Simulator");

            win.Closed += Win_Closed;
            win.Resized += Win_Resized;
            win.KeyPressed += Win_KeyPressed;
            win.MouseButtonPressed += Win_MouseButton;

            while (win.IsOpen)
            {
                win.DispatchEvents();
                if (isRunning)
                    Simulator.Update();
                

                win.Clear(Color.Black);

                DrawIcons();
                if (isUnitDescription)
                {
                    foreach (Text d in unitDescription)
                    {
                        win.Draw(d);
                    }
                }
                Simulator.Draw();

                win.Display();
            }
        }

        private static void ConfigureUnitDescription()
        {
            unitDescription[0] = new Text("Test", Content.Font);
            unitDescription[0].Position = new Vector2f(LeftMapOffset + Simulator.WorldWidth * Simulator.Scale + 10, TopMapOffset + 100);
        }

        private static void ConfigureIcons()
        {
            swordShape = new RectangleShape(new Vector2f(swordSize, swordSize));
            swordShape.Texture = Content.Sword;
            swordShape.Position = new Vector2f(swordLeftSide, swordTopSide);

            lightningShape = new RectangleShape(new Vector2f(lightningSize, lightningSize));
            lightningShape.Texture = Content.Lightning;
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
                int y = (e.X - LeftMapOffset) / Simulator.Scale;
                int x = (e.Y - TopMapOffset) / Simulator.Scale;
                if (!Simulator.IsFree(x, y))
                {
                    isUnitDescription = true;
                    isRunning = false;
                }
                else
                {
                    isUnitDescription = false;
                }
            }
        }

        private static void SetEnergyMap()
        {
            lightningShape.Texture = Content.PressedLightning;
            swordShape.Texture = Content.Sword;
        }

        private static void SetActionMap()
        {
            lightningShape.Texture = Content.Lightning;
            swordShape.Texture = Content.PressedSword;
        }

        private static void Win_KeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.P)
                isRunning ^= true;
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
