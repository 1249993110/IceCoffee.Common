using System;
using System.Collections.Concurrent;
using System.Threading;

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

        public class MetaEvent
        {
            public EventHandler eventHandler1;
            public EventHandler<EventArgs> eventHandler = null;
            public object sender = null;
            public EventArgs args = null;
        }

        #endregion 嵌套类

        #region 字段&属性

        private MetaEvent _currentEvent = null;

        /// <summary>
        /// EventQueue
        /// </summary>
        private ConcurrentQueue<MetaEvent> _eventsQueue = new ConcurrentQueue<MetaEvent>();

        private bool _isRunning = false;

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
                    Thread.Yield();
                else
                    processEvents();
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
        private void processEvents()
        {
            do
            {
                _eventsQueue.TryDequeue(out _currentEvent);
                _currentEvent.eventHandler.Invoke(_currentEvent.sender, _currentEvent.args);
            }
            while (_eventsQueue.IsEmpty == false);
        }

        #endregion 方法
    }
}