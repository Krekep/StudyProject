using System;
using System.Collections.Generic;
using System.Text;

namespace Simulator
{
    interface IAction
    {
        void Process(Unit unit);
    }
}
