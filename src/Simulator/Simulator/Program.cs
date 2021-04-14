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
        private static string pathToFont = Directory.GetCurrentDirectory() + "/../" + "../" + "../" + "../" + "../" + "resources/fonts/arial.ttf";

        public static RenderWindow Window { get { return win; } }
        static void Main(string[] args)
        {
            win = new RenderWindow(new VideoMode(800, 600), "Evolution Simulator");

            RandomSeed = DateTime.Now.Second;
            Font = new Font(pathToFont);

            win.Closed += WinClosed;
            win.Resized += WinResized;

            while (win.IsOpen)
            {
                win.DispatchEvents();

                Simulator.Update();

                win.Clear(Color.Black);

                Simulator.Draw();

                win.Display();
            }
        }

        private static void WinResized(object sender, SizeEventArgs e)
        {
            win.SetView(new View(new FloatRect(0, 0, e.Width, e.Height)));
        }

        private static void WinClosed(object sender, EventArgs e)
        {
            win.Close();
        }
    }
}
