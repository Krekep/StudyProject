using SFML.Graphics;
using SFML.Window;

namespace Simulator
{
    static class Content
    {
        public const string ResourcesPath = "../" + "../" + "../" + "../" + "../" + "../" + "resources";
        public static Font Font { get; private set; }
        public static Texture Sword { get; private set; }
        public static Texture PressedSword { get; private set; }
        public static Texture Lightning { get; private set; }
        public static Texture PressedLightning { get; private set; }
        public static void LoadSword()
        {
            Sword = new Texture(ResourcesPath + "/pictures/" + "sword.png");
            PressedSword = new Texture(ResourcesPath + "/pictures/" + "pressed_sword.png");
        }
        public static void LoadLightning()
        {
            Lightning = new Texture(ResourcesPath + "/pictures/" + "lightning.png");
            PressedLightning = new Texture(ResourcesPath + "/pictures/" + "pressed_lightning.png");
        }
        public static void LoadFont(string fontName)
        {
            Font = new Font(ResourcesPath + "/fonts/" + fontName);
        }
    }
}
