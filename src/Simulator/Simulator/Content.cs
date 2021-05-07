using SFML.Graphics;
using SFML.Window;

using System;

namespace Simulator
{
    static class Content
    {
        public const string ResourcesPath = "../" + "../" + "../" + "../" + "../" + "../" + "resources";
        public const string PathToSaves = ResourcesPath + "/saves/";
        public const string PathToPictures = ResourcesPath + "/pictures/";
        public const string PathToFonts = ResourcesPath + "/fonts/";
        public const string FileExtension = ".evo";

        static Content()
        {
            Content.LoadFont("arial.ttf");
            Content.LoadSword();
            Content.LoadRestartButton();
            Content.LoadLightning();
            Content.LoadMenu();
            Content.LoadExport();
            Content.LoadImport();
            Content.LoadClose();
        }

        public static Texture Sword { get; private set; }
        public static Texture PressedSword { get; private set; }

        private static void LoadSword()
        {
            Sword = new Texture(PathToPictures + "sword.png");
            PressedSword = new Texture(PathToPictures + "sword_pressed.png");
        }

        public static Texture Lightning { get; private set; }
        public static Texture PressedLightning { get; private set; }
        private static void LoadLightning()
        {
            Lightning = new Texture(PathToPictures + "lightning.png");
            PressedLightning = new Texture(PathToPictures + "lightning_pressed.png");
        }

        public static Texture RestartButton { get; private set; }
        public static Texture PressedRestartButton { get; private set; }
        private static void LoadRestartButton()
        {
            RestartButton = new Texture(PathToPictures + "restart.png");
            PressedRestartButton = new Texture(PathToPictures + "restart_pressed.png");
        }

        public static Font Font { get; private set; }
        private static void LoadFont(string fontName)
        {
            Font = new Font(PathToFonts + fontName);
        }

        public static Texture MenuButton { get; private set; }
        public static Texture PressedMenuButton { get; private set; }
        private static void LoadMenu()
        {
            MenuButton = new Texture(PathToPictures + "menu.png");
            PressedMenuButton = new Texture(PathToPictures + "menu_pressed.png");
        }

        public static Texture ExportButton { get; private set; }
        public static Texture PressedExportButton { get; private set; }
        private static void LoadExport()
        {
            ExportButton = new Texture(PathToPictures + "export.png");
            PressedExportButton = new Texture(PathToPictures + "export_pressed.png");
        }

        public static Texture ImportButton { get; private set; }
        public static Texture PressedImportButton { get; private set; }
        private static void LoadImport()
        {
            ImportButton = new Texture(PathToPictures + "import.png");
            PressedImportButton = new Texture(PathToPictures + "import_pressed.png");
        }

        public static Texture CloseButton { get; private set; }
        private static void LoadClose()
        {
            CloseButton = new Texture(PathToPictures + "close.png");
        }
    }
}
