using SFML.Graphics;
using SFML.System;
using SFML.Window;

using System;
using System.Collections.Generic;
using System.Text;

namespace Simulator
{
    public class TextBox : Transformable, Drawable
    {
        private static Color pressedDefaultColor = new Color(64, 64, 64);
        private static Color unpressedDefaultColor = Color.Black;

        public bool IsChoosen { get; private set; }
        private RectangleShape backlight;
        private string fixedText;

        private Color pressedColor;
        private Color unpressedColor;

        public Vector2f Size { get { return backlight.Size; } set { backlight.Size = value; } }
        public Vector2f Coords { get { return backlight.Position; } set { backlight.Position = value; textBlock.Position = value; } }
        private Text textBlock;
        public bool IsFixedSize { get; set; }

        private int len;


        public TextBox(Color pressedColor, Color unpressedColor) : this(0, 0, "", pressedColor, unpressedColor)
        {
        }
        public TextBox(Color color) : this(0, 0, "", color, color)
        {
        }
        public TextBox(int x, int y) : this(x, y, "", pressedDefaultColor, unpressedDefaultColor)
        {
        }
        public TextBox(int x, int y, Color pressedColor, Color unpressedColor) : this(x, y, "", pressedColor, unpressedColor)
        {
        }
        public TextBox(int x, int y, string fixedText) : this(x, y, fixedText, pressedDefaultColor, unpressedDefaultColor)
        {
        }
        public TextBox(int x, int y, string fixedText, Color pressedColor, Color unpressedColor)
        {
            this.pressedColor = pressedColor;
            this.unpressedColor = unpressedColor;
            IsChoosen = false;
            this.fixedText = fixedText;
            textBlock = new Text(fixedText, Content.Font, Program.TextSize);
            backlight = new RectangleShape(new Vector2f(textBlock.GetLocalBounds().Width + 10, textBlock.GetLocalBounds().Height + 10));
            backlight.FillColor = this.unpressedColor;
            len = 0;

            Coords = new Vector2f(x, y);
        }

        public void SetText(string text)
        {
            textBlock.DisplayedString = fixedText + text;
            len += text.Length;
            if (!IsFixedSize)
                Size = new Vector2f(textBlock.GetLocalBounds().Width + 10, textBlock.GetLocalBounds().Height + 10);
        }
        public string GetText()
        {
            return textBlock.DisplayedString;
        }

        public void BackspaceHandle()
        {
            if (len == 0)
                return;
            string temp = textBlock.DisplayedString;
            temp = temp.Remove(temp.Length - 1);
            textBlock.DisplayedString = temp;
            len -= 1;
            if (!IsFixedSize)
                Size = new Vector2f(textBlock.GetLocalBounds().Width + 10, textBlock.GetLocalBounds().Height + 10);
        }

        public void UpdateText(string text)
        {
            textBlock.DisplayedString += text;
            len += text.Length;
            if (!IsFixedSize)
                Size = new Vector2f(textBlock.GetLocalBounds().Width + 10, textBlock.GetLocalBounds().Height + 10);
        }

        public void Choose()
        {
            backlight.FillColor = pressedColor;
        }

        public void Unchoose()
        {
            backlight.FillColor = unpressedColor;
        }

        public bool IsHit(int x, int y)
        {
            return backlight.GetGlobalBounds().Contains(x, y);
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(backlight);
            target.Draw(textBlock);
        }

        internal void Clear()
        {
            textBlock.DisplayedString = "";
        }
    }
}
