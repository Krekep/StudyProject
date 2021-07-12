using System;
using System.Collections.Generic;
using System.Text;

namespace Simulator.Events
{
    public class ImportEventArgs : EventArgs
    {
        
    }
    public static class ImportEvent
    {
        public static event EventHandler<ImportEventArgs> Notify;
        public static void KnockKnock(object sender)
        {
            ImportEventArgs args = new ImportEventArgs();
            //EventHandler<MoveEventArgs> handler = Notify;
            Notify(sender, args);
        }
    }
}
