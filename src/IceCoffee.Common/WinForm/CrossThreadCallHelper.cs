namespace IceCoffee.Common.WinForm
{
    public static class CrossThreadCallHelper
    {
        private static SynchronizationContext? _context;

        public static  void Init(SynchronizationContext? mainThreadSyncContext)
        {
            _context = mainThreadSyncContext;
        }

        public static void Post(Action<string> appendTextCallBack, string message)
        {
            _context?.Post(SendOrPostCallback, new Args(appendTextCallBack, message));
        }

        public static void Send(Action<string> appendTextCallBack, string message)
        {
            _context?.Send(SendOrPostCallback, new Args(appendTextCallBack, message));
        }

        private static void SendOrPostCallback(object? state)
        {
            if (state is Args args)
            {
                args.AppendTextCallBack.Invoke(args.Message);
            }
        }

        internal class Args
        {
            public Args(Action<string> appendTextCallBack, string message)
            {
                AppendTextCallBack = appendTextCallBack;
                Message = message;
            }

            public Action<string> AppendTextCallBack { get; set; }
            public string Message { get; set; }
        }
    }
}