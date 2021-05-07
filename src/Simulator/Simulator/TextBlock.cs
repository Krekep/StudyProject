﻿using SFML.Graphics;
using SFML.System;

using System;
using System.Collections.Generic;
using System.Text;

namespace Simulator
{
    public class TextBlock : Transformable, Drawable
    {
        private static Color DefaultColor = new Color(64, 64, 64);

        public bool IsChoosen { get; private set; }
        private RectangleShape backlight;
        public string Text { get { return textBlock.DisplayedString; } set { textBlock.DisplayedString = value; } }
        public Vector2f Size { get { return backlight.Size; } set { backlight.Size = value; } }
        public Vector2f Coords { get { return backlight.Position; } set { backlight.Position = value; textBlock.Position = value; } }
        private Text textBlock;

        public Color FillColor { get { return backlight.FillColor; } set { backlight.FillColor = value; } }
        public Color OutlineColor { get { return backlight.OutlineColor; } set { backlight.OutlineColor = value; } }

        public TextBlock(int x, int y) : this(x, y, "", DefaultColor)
        {
        }
        public TextBlock(int x, int y, Color unpressedColor) : this(x, y, "", unpressedColor)
        {
        }
        public TextBlock(Color color) : this(0, 0, "", color)
        {
        }
        public TextBlock(int x, int y, string text) : this(x, y, text, DefaultColor)
        {
        }
        public TextBlock(int x, int y, string text, Color color)
        {
            IsChoosen = false;
            textBlock = new Text(text, Content.Font, Program.TextSize);
            backlight = new RectangleShape(new Vector2f(textBlock.GetLocalBounds().Width + 10, textBlock.GetLocalBounds().Height + 10));

            this.Text = text;
            this.FillColor = color;
            Coords = new Vector2f(x, y);
        }
        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(backlight);
            target.Draw(textBlock);
        }
    }
}
