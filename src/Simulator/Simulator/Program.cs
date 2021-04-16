using System;
using System.IO;

using SFML.Graphics;
using SFML.Window;

namespace Simulator
{
    class Program
    {
        static RenderWindow win;
        public static int RandomSeed { get; private set; }
        public static Font Font { get; private set; }
        private static string pathToFont = Directory.GetCurrentDirectory() + "/../" + "../" + "../" + "../" + "../" + "../" + "resources/fonts/arial.ttf";
        static bool isRunning = false;

        public static RenderWindow Window { get { return win; } }
        static void Main(string[] args)
        {
            win = new RenderWindow(new VideoMode(1024, 728), "Evolution Simulator");

            RandomSeed = DateTime.Now.Millisecond;
            Font = new Font(pathToFont);

            win.Closed += Win_Closed;
            win.Resized += Win_Resized;
            win.KeyPressed += Win_KeyPressed;
            while (win.IsOpen)
            {
                win.DispatchEvents();
                if (isRunning)
                    Simulator.Update();
                

                win.Clear(Color.Black);

                Simulator.Draw();

                win.Display();
            }
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
