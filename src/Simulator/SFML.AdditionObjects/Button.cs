using SFML.Graphics;
using SFML.System;

using System;
using System.Collections.Generic;
using System.Text;

namespace Simulator
{
    public class Button : Transformable, Drawable
    {
        private RectangleShape shape;
        private Texture unpressed;
        private Texture pressed;
        private bool isPressed;
        private Text textBlock;

        public string Text
        {
            get { return textBlock.DisplayedString; }
            private set { textBlock.DisplayedString = value; }
        }


        public Vector2f Size { get { return shape.Size; } set { shape.Size = value; } }
        public Vector2f Coords { get { return shape.Position; } set { shape.Position = value; textBlock.Position = new Vector2f(value.X + 2, value.Y + 2); } }
        public Color FillColor { get { return shape.FillColor; } set { shape.FillColor = value; } }
        public Color OutlineColor { get { return shape.OutlineColor; } set { shape.OutlineColor = value; } }
        public float OutlineThickness { get { return shape.OutlineThickness; } set { shape.OutlineThickness = value; } }

        public Button()
        {
            textBlock = new Text();
            shape = new RectangleShape();
            isPressed = false;
        }

        public Button(int left, int top, int width, int height)
        {
            textBlock = new Text();
            shape = new RectangleShape(new Vector2f(width, height));
            Coords = new Vector2f(left, top);
            isPressed = false;
        }

        public Button(int left, int top, int width, int height, string text, uint charSize)
        {
            textBlock = new Text(text, Content.Font, charSize);
            shape = new RectangleShape(new Vector2f(width, height));
            Coords = new Vector2f(left, top);
            isPressed = false;
        }

        public void SetTexture(Texture unpressed, Texture pressed)
        {
            this.unpressed = unpressed;
            this.pressed = pressed;
        }

        public void SetTexture(Texture texture)
        {
            this.unpressed = texture;
            this.pressed = texture;
            shape.Texture = texture;
        }

        public void Press()
        {
            isPressed = true;
            shape.Texture = pressed;
        }
        public void Unpress()
        {
            isPressed = false;
            shape.Texture = unpressed;
        }
        public void Click()
        {
            if (isPressed)
                shape.Texture = unpressed;
            else
                shape.Texture = pressed;
            isPressed ^= true;
        }
        public bool IsHit(int x, int y)
        {
            return shape.GetGlobalBounds().Contains(x, y);
        }
        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(shape);
            target.Draw(textBlock);
        }
    }
}
