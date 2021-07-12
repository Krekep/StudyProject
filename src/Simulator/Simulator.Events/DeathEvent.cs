using System;
using System.Collections.Generic;
using System.Text;

namespace Simulator.Events
{
    public class DeathEventArgs : EventArgs
    {
        public int[] Position { get; set; }
    }
    public static class DeathEvent
    {
        public static event EventHandler<DeathEventArgs> Notify;
        public static void KnockKnock(object sender, int[] position)
        {
            DeathEventArgs args = new DeathEventArgs();
            args.Position = position;
            //EventHandler<DeathEventArgs> handler = Notify;
            Notify(sender, args);
        }
    }
}
