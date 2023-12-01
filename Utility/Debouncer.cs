using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace D2MTranslator.Utility
{
    public class Debouncer
    {
        private Timer _timer;

        public void Debounce(int interval, Action action)
        {
            _timer?.Dispose();
            _timer = new Timer(_ => action(), null, interval, Timeout.Infinite);
        }
    }
}
