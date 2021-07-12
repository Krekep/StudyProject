using System;
using System.Collections.Generic;
using System.Text;

namespace Simulator.Events
{
    public class MoveEventArgs : EventArgs
    {
        public int[] OldPosition { get; set; }
        public int[] NewPosition { get; set; }
    }
    public static class MoveEvent
    {
        public static event EventHandler<MoveEventArgs> Notify;
        public static void KnockKnock(object sender, int[] oldPosition, int[] newPosition)
        {
            MoveEventArgs args = new MoveEventArgs();
            args.OldPosition = oldPosition;
            args.NewPosition = newPosition;
            //EventHandler<MoveEventArgs> handler = Notify;
            Notify(sender, args);
        }
    }
}
