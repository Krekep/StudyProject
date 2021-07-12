using System;
using System.Collections.Generic;
using System.Text;

namespace Simulator.Events
{
    public class DivideEventArgs : EventArgs
    {
        public int[] Position { get; set; }
    }
    public static class DivideEvent
    {
        public static event EventHandler<DivideEventArgs> Notify;
        public static void KnockKnock(object sender, int[] position)
        {
            DivideEventArgs args = new DivideEventArgs();
            args.Position = position;
            //EventHandler<DivideEventArgs> handler = Notify;
            Notify(sender, args);
        }
    }
}
