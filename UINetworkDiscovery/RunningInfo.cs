using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UINetworkDiscovery
{
    public class RunningInfo
    {
        private int _running;

        public RunningInfo()
        {
            TotalAlgorithmsRan = 0;
            _running = 0;
            CancelationPending = false;
        }

        public void RemoveRunningAlgorithm(bool wasCanceled)
        {
            if (wasCanceled)
                CancelationPending = true;
            _running--;
        }

        public void AddRunningAlgorithm()
        {
            _running++;
            TotalAlgorithmsRan = Math.Max(TotalAlgorithmsRan, _running);
        }

        public bool IsRunning => _running > 0;


        public int RunningAlgorithms => _running;

        public int TotalAlgorithmsRan { get; private set; }

        public bool CancelationPending { get; set; }

    }
}
