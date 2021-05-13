using SFML.Graphics;

using System;
using System.Collections.Generic;
using System.Text;

namespace Simulator
{
    public enum ActionType: int
    {
        Wait = 0,
        Move = 1,
        Photosyntesis = 2,
        Attack = 3
    }
    public interface IAction
    {
        void Process(Unit unit);
        int Value();
        Color ActionColor();
        ActionType Type();
    }
}
