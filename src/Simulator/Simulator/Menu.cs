using SFML.Graphics;
using SFML.System;

using System;
using System.Collections.Generic;
using System.Text;

namespace Simulator
{
    static class Menu
    {
        private static RectangleShape menuForm;
        private static int menuLeftBound;
        private static int menuTopBound;
        private static int menuHeight;
        private static int menuWidth;

        private static Button close;
        private static int closeSize;
        private static int closeLeftSide;
        private static int closeTopSide;

        private static Button export;
        private static int exportSize;
        private static int exportLeftSide;
        private static int exportTopSide;

        private const int AmountOfButtons = 4;
        private static Button[] import;
        private static int importSize;
        private static int importLeftSide;
        private static int[] importTopSide;

        private static Vertex[] splitLine;

        private static TextBox exportName;

        private static TextBlock[] importName;
        private static string[] files;
        private static int firstShowing = 0;

        private const int ScrollPerString = 1;

        static Menu()
        {
            menuForm = new RectangleShape();
            menuForm.FillColor = Color.Black;
            menuForm.OutlineColor = Color.White;
            menuForm.OutlineThickness = 2;

            close = new Button();
            close.SetTexture(Content.CloseButton);

            export = new Button();
            export.SetTexture(Content.ExportButton, Content.PressedExportButton);
            export.Unpress();

            import = new Button[AmountOfButtons];
            importTopSide = new int[AmountOfButtons];
            for (int i = 0; i < AmountOfButtons; i++)
            {
                import[i] = new Button();
                import[i].SetTexture(Content.ImportButton, Content.PressedImportButton);
                import[i].Unpress();
            }

            splitLine = new Vertex[2];

            exportName = new TextBox(new Color(100, 75, 0), new Color(100, 75, 0));
            exportName.IsFixedSize = true;
            exportName.OutlineColor = Color.White;
            exportName.OutlineThickness = 1;

            importName = new TextBlock[AmountOfButtons];
            for (int i = 0; i < AmountOfButtons; i++)
            {
                importName[i] = new TextBlock(Color.Red);
                importName[i].OutlineColor = Color.White;
                importName[i].OutlineThickness = 1;
            }
        }

        public static void Open()
        {
            files = new string[4];
            UpdateFiles();
            exportName.Clear();


            menuLeftBound = (int)(Program.Window.Size.X / 2 - 100.0 * Program.Window.Size.X / Program.Window.Size.Y);
            menuTopBound = (int)(Program.Window.Size.Y / 2 - 190.0 * Program.Window.Size.Y / Program.Window.Size.X);
            menuHeight = Math.Max((int)(Program.Window.Size.Y / 2 + 190.0 * Program.Window.Size.Y / Program.Window.Size.X) - menuTopBound + 10, (Simulator.TopMapOffset - 10) * 7);
            menuWidth = (int)(Program.Window.Size.X / 2 + 100.0 * Program.Window.Size.X / Program.Window.Size.Y) - menuLeftBound;
            menuForm.Position = new Vector2f(menuLeftBound, menuTopBound);
            menuForm.Size = new Vector2f(menuWidth, menuHeight);

            closeSize = 7;
            closeLeftSide = menuLeftBound + menuWidth - closeSize;
            closeTopSide = menuTopBound;
            close.Coords = new Vector2f(closeLeftSide, closeTopSide);
            close.Size = new Vector2f(closeSize, closeSize);

            exportSize = (int)(Simulator.TopMapOffset - 10);
            exportLeftSide = menuLeftBound + menuWidth - closeSize - exportSize;
            exportTopSide = menuTopBound + closeSize;
            export.Coords = new Vector2f(exportLeftSide, exportTopSide);
            export.Size = new Vector2f(exportSize, exportSize);
            export.Unpress();

            importSize = exportSize;
            importLeftSide = exportLeftSide;
            for (int i = 0; i < AmountOfButtons; i++)
            {
                importTopSide[i] = exportTopSide + exportSize + 5 + 1 + 10 + (15 + importSize) * i;
                import[i].Size = new Vector2f(importSize, importSize);
                import[i].Coords = new Vector2f(importLeftSide, importTopSide[i]);
            }

            splitLine = new Vertex[2] { new Vertex(new Vector2f(menuLeftBound, exportTopSide + exportSize + 5)),
                                        new Vertex(new Vector2f(menuLeftBound + menuWidth, exportTopSide + exportSize + 5))};



            exportName.Coords = new Vector2f(menuLeftBound + 10, exportTopSide);
            exportName.Size = new Vector2f(menuWidth * 3 / 4, exportSize);

            for (int i = 0; i < AmountOfButtons; i++)
            {
                importName[i].Coords = new Vector2f(menuLeftBound + 10, exportTopSide + exportSize + 5 + 1 + 10 + (15 + importSize) * i);
                importName[i].Text = files[firstShowing + i];
                importName[i].Size = new Vector2f(menuWidth * 3 / 4, exportSize);
            }
        }

        internal static void Scroll(float delta)
        {
            int temp = -(int)delta / ScrollPerString;
            if (firstShowing + temp + AmountOfButtons >= files.Length)
                firstShowing = files.Length - AmountOfButtons;
            else if (firstShowing + temp < 0)
                firstShowing = 0;
            else
                firstShowing += temp;
            UpdateDisplayed();
        }

        private static void UpdateDisplayed()
        {
            for (int i = 0; i < AmountOfButtons; i++)
            {
                importName[i].Text = files[firstShowing + i];
                importName[i].Size = new Vector2f(menuWidth * 3 / 4, exportSize);
            }
        }

        private static void UpdateFiles()
        {
            string[] temp = WorldImporter.GetFilesArray();
            if (temp.Length > files.Length)
            {
                files = new string[temp.Length];
            }
            for (int i = 0; i < temp.Length; i++)
            {
                files[i] = temp[i];
            }
            UpdateDisplayed();
        }

        public static void Draw()
        {
            Program.Window.Draw(menuForm);
            Program.Window.Draw(exportName);
            Program.Window.Draw(export);
            Program.Window.Draw(close);
            Program.Window.Draw(splitLine, PrimitiveType.Lines);
            for (int i = 0; i < AmountOfButtons; i++)
            {
                Program.Window.Draw(import[i]);
                Program.Window.Draw(importName[i]);
            }
        }

        public static void MouseHandle(int x, int y)
        {
            if (close.IsHit(x, y))
            {
                Program.CloseMenu();
                return;
            }
            if (export.IsHit(x, y))
            {
                ExportButtonClick();
                return;
            }
            for (int id = 0; id < AmountOfButtons; id++)
            {
                if (import[id].IsHit(x, y))
                {
                    ImportButtonClick(id);
                    break;
                }
            }
        }

        public static void EscapeHandler()
        {
            Program.CloseMenu();
        }

        private static void ExportButtonClick()
        {
            string fileName = exportName.GetText();
            WorldExporter.Export(fileName, Program.World);
        }

        private static void ImportButtonClick(int id)
        {
            string fileName = importName[id].Text;
            WorldImporter.Import(fileName, Program.World);
        }

        public static void BackspaceHandle()
        {
           exportName.BackspaceHandle();
        }

        public static void UpdateExportName(string text)
        {
            exportName.UpdateText(text);
        }
    }
}
