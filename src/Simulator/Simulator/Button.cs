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

        public Vector2f Size { get { return shape.Size; } set { shape.Size = value; } }
        public Vector2f Coords { get { return shape.Position; } set { shape.Position = value; } }

        public Button()
        {
            shape = new RectangleShape();
            isPressed = false;
        }

        public Button(int left, int top, int width, int height)
        {
            shape = new RectangleShape(new Vector2f(width, height));
            shape.Position = new Vector2f(left, top);
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
            Program.Window.Draw(shape);
        }
    }
}
