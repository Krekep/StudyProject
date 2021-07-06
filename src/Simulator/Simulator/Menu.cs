using SFML.Graphics;
using SFML.System;

using System;
using System.Collections.Generic;
using System.Text;

using TGUI;

namespace Simulator
{
    static class Menu
    {
        private static ChildWindow menuForm;
        private static int menuLeftBound;
        private static int menuTopBound;
        private static int menuHeight;
        private static int menuWidth;

        private static Button export;
        private static int exportSize;
        private static int exportLeftSide;
        private static int exportTopSide;

        private const int AmountOfButtons = 4;
        private static Button[] importButtons;
        private static int importSize;
        private static int importLeftSide;
        private static int[] importTopSide;

        private static Vertex[] splitLine;

        private static TextBox exportName;

        private static Label[] importName;
        private static string[] files;
        private static int firstShowing = 0;

        private const int ScrollPerString = 1;

        static Menu()
        {
            menuForm = new ChildWindow();
            menuForm.Name = "Menu";
            menuForm.Title = "Menu title";
            menuForm.TitleAlignment = HorizontalAlignment.Center;
            menuForm.Renderer.BackgroundColor = Color.Black;
            menuForm.Renderer.BorderColor = Color.White;
            menuForm.Visible = false;
            menuForm.Closed += MenuForm_Closed;
            Program.MainGui.Add(menuForm);

            export = new Button();
            export.Renderer.Texture = Content.ExportButton;
            export.Clicked += ExportButton_Click;
            menuForm.Add(export);

            importButtons = new Button[AmountOfButtons];
            importTopSide = new int[AmountOfButtons];
            for (int i = 0; i < AmountOfButtons; i++)
            {
                importButtons[i] = new Button();
                importButtons[i].Renderer.Texture = Content.ImportButton;
                importButtons[i].Clicked += ImportButton_Click;
                menuForm.Add(importButtons[i]);
            }

            splitLine = new Vertex[2];

            exportName = new TextBox();
            exportName.Renderer.BackgroundColor = new Color(100, 75, 0);
            exportName.Renderer.BorderColor = Color.White;
            exportName.TextSize = Program.CharacterSize;
            menuForm.Add(exportName);

            importName = new Label[AmountOfButtons];
            for (int i = 0; i < AmountOfButtons; i++)
            {
                importName[i] = new Label();
                importName[i].Renderer.BackgroundColor = Color.Red;
                importName[i].Renderer.BorderColor = Color.White;
                importName[i].TextSize = Program.CharacterSize;
                menuForm.Add(importName[i]);
                //importName[i].OutlineThickness = 1;
            }
        }

        private static void MenuForm_Closed(object sender, EventArgs e)
        {
            //menuForm.CloseWindow();
            Program.CloseMenu();
            menuForm.Visible = false;
        }

        private static void ExportButton_Click(object sender, SignalArgsVector2f e)
        {
            string fileName = exportName.Text;
            WorldExporter.Export(fileName, Program.World);
        }

        private static void ImportButton_Click(object sender, SignalArgsVector2f e)
        {
            for (int i = 0; i < AmountOfButtons; i++)
            {
                if (e.Value.Y > importTopSide[i])
                {
                    string fileName = importName[i].Text;
                    WorldImporter.Import(fileName, Program.World);
                }
            }
        }

        public static void Open()
        {
            files = new string[4];
            UpdateFiles();
            exportName.Text = "";
            menuForm.Visible = true;

            menuLeftBound = (int)(Program.Window.Size.X / 2 - 100.0 * Program.Window.Size.X / Program.Window.Size.Y);
            menuTopBound = (int)(Program.Window.Size.Y / 2 - 190.0 * Program.Window.Size.Y / Program.Window.Size.X);
            menuHeight = Math.Max((int)(Program.Window.Size.Y / 2 + 190.0 * Program.Window.Size.Y / Program.Window.Size.X) - menuTopBound + 10, (Program.TopMapOffset - 10) * 7);
            menuWidth = (int)(Program.Window.Size.X / 2 + 100.0 * Program.Window.Size.X / Program.Window.Size.Y) - menuLeftBound;
            menuForm.Position = new Vector2f(menuLeftBound, menuTopBound);
            menuForm.Size = new Vector2f(menuWidth, menuHeight);

            exportSize = (Program.TopMapOffset - 10);
            exportLeftSide = menuWidth - 5 - exportSize;
            exportTopSide = 5;
            export.Position = new Vector2f(exportLeftSide, exportTopSide);
            export.Size = new Vector2f(exportSize, exportSize);

            importSize = exportSize;
            importLeftSide = exportLeftSide;
            for (int i = 0; i < AmountOfButtons; i++)
            {
                importTopSide[i] = exportTopSide + exportSize + 5 + 1 + 10 + (15 + importSize) * i;
                importButtons[i].Size = new Vector2f(importSize, importSize);
                importButtons[i].Position = new Vector2f(importLeftSide, importTopSide[i]);
            }

            splitLine = new Vertex[2] { new Vertex(new Vector2f(0, exportTopSide + exportSize + 5)),
                                        new Vertex(new Vector2f(menuWidth, exportTopSide + exportSize + 5))};



            exportName.Position = new Vector2f(10, exportTopSide);
            exportName.Size = new Vector2f(menuWidth * 3 / 4, exportSize);

            for (int i = 0; i < AmountOfButtons; i++)
            {
                importName[i].Position = new Vector2f(10, exportTopSide + exportSize + 5 + 1 + 10 + (15 + importSize) * i);
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
                //importName[i].Size = new Vector2f(menuWidth * 3 / 4, exportSize);
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

        public static void EscapeHandler()
        {
            Program.CloseMenu();
        }
    }
}
