using System.Collections.Concurrent;

namespace IceCoffee.Common
{
    public class EventLoop
    {
        #region 构造方法

        public EventLoop()
        {
        }

        #endregion 构造方法

        #region 嵌套类

        public struct MetaEvent
        {
            public EventHandler<EventArgs> EventHandler;
            public object Sender;
            public EventArgs Args;

            public MetaEvent(EventHandler<EventArgs> eventHandler, object sender, EventArgs args)
            {
                this.EventHandler = eventHandler;
                this.Sender = sender;
                this.Args = args;
            }
        }

        #endregion 嵌套类

        #region 字段&属性

        private MetaEvent _currentEvent;

        /// <summary>
        /// EventQueue
        /// </summary>
        private ConcurrentQueue<MetaEvent> _eventsQueue = new ConcurrentQueue<MetaEvent>();

        private volatile bool _isRunning = false;

        /// <summary>
        /// Returns true if the event loop is running; otherwise returns false.
        /// </summary>
        public bool IsRunning
        {
            get { return _isRunning; }
        }

        #endregion 字段&属性

        #region 方法

        public void PostEvent(MetaEvent _event)
        {
            _eventsQueue.Enqueue(_event);
        }

        /// <summary>
        /// Enters the main event loop and waits until Exit() is called.
        /// </summary>
        public void Exec()
        {
            _isRunning = true;
            while (_isRunning)
            {
                if (_eventsQueue.IsEmpty)
                    // Thread.Yield();
                    Thread.Sleep(20);
                else
                    ProcessEvents();
            }
        }

        /// <summary>
        /// Tells the event loop to exit.
        /// </summary>
        public void Exit()
        {
            _isRunning = false;
        }

        /// <summary>
        /// Processes pending events until there are no more events to process.
        /// </summary>
        private void ProcessEvents()
        {
            do
            {
                _eventsQueue.TryDequeue(out _currentEvent);
                _currentEvent.EventHandler.Invoke(_currentEvent.Sender, _currentEvent.Args);
            }
            while (_eventsQueue.IsEmpty == false);
        }

        #endregion 方法
    }
}