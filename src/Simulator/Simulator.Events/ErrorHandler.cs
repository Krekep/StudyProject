using System;

namespace Simulator.Events
{
    public class ErrorEventArgs : EventArgs
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
    }
    public static class ErrorHandler
    {
        public static event EventHandler<ErrorEventArgs> Notify;
        private static long lastKnock = 0;
        private static bool lastSuccess = true;
        public static void KnockKnock(object sender, string message, bool success)
        {
            if (Math.Abs(lastKnock - DateTime.Now.Second) < 1 && !lastSuccess)
                return;
            lastKnock = DateTime.Now.Second;
            lastSuccess = success;
            ErrorEventArgs args = new ErrorEventArgs();
            args.Message = message;
            args.IsSuccess = success;
            //EventHandler<ErrorEventArgs> handler = Notify;
            Notify(sender, args);
        }
    }
}
