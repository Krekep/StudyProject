using SFML.Graphics;
using SFML.System;

using System;
using System.Collections.Generic;
using System.Text;

namespace Simulator
{
    static class Icons
    {
        private static Button sword;
        private const int swordSize = Program.TopMapOffset - 10;
        private const int swordLeftSide = Program.LeftMapOffset + Program.TopMapOffset;
        private const int swordTopSide = 5;

        private static Button lightning;
        private const int lightningSize = Program.TopMapOffset - 10;
        private const int lightningLeftSide = Program.LeftMapOffset;
        private const int lightningTopSide = 5;

        private static Button restart;
        private const int restartSize = Program.TopMapOffset - 10;
        private const int restartLeftSide = Program.LeftMapOffset + Simulator.WorldWidth * Simulator.Scale + 10;
        private const int restartTopSide = 5;

        private static Button menu;
        private const int menuSize = Program.TopMapOffset - 10;
        private const int menuLeftSide = Program.LeftMapOffset + Simulator.WorldWidth * Simulator.Scale + Program.TopMapOffset + 10;
        private const int menuTopSide = 5;

        static Icons()
        {
            sword = new Button(swordLeftSide, swordTopSide, swordSize, swordSize);
            sword.SetTexture(Content.Sword, Content.PressedSword);
            sword.Unpress();

            //swordShape = new RectangleShape(new Vector2f(swordSize, swordSize));
            //swordShape.Texture = Content.Sword;
            //swordShape.Position = new Vector2f(swordLeftSide, swordTopSide);

            lightning = new Button(lightningLeftSide, lightningTopSide, lightningSize, lightningSize);
            lightning.SetTexture(Content.Lightning, Content.PressedLightning);
            lightning.Press();

            restart = new Button(restartLeftSide, restartTopSide, restartSize, restartSize);
            restart.SetTexture(Content.RestartButton, Content.PressedRestartButton);
            restart.Unpress();

            menu = new Button(menuLeftSide, menuTopSide, menuSize, menuSize);
            menu.SetTexture(Content.MenuButton, Content.PressedMenuButton);
            menu.Unpress();
        }

        internal static void Draw()
        {
            Program.Window.Draw(sword);
            Program.Window.Draw(lightning);
            Program.Window.Draw(restart);
            Program.Window.Draw(menu);
        }

        internal static void SetMap(TypeOfMap map)
        {
            if (map == TypeOfMap.MapOfEnergy)
            {
                lightning.Press();
                sword.Unpress();
            }
            if (map == TypeOfMap.MapOfActions)
            {
                lightning.Unpress();
                sword.Press();
            }    
        }

        internal static void ButtonHandler(int x, int y)
        {
            if (sword.IsHit(x, y))
            {
                Program.SetActionMap();
            }
            else if (lightning.IsHit(x, y))
            {
                Program.SetEnergyMap();
            }
            else if (restart.IsHit(x, y))
            {
                Program.Restart();
            }
            else if (menu.IsHit(x, y))
            {
                Program.OpenMenu();
            }
        }
    }
}
