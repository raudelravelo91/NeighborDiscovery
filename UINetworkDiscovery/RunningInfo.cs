using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UINetworkDiscovery
{
    public class RunningInfo
    {
        private int running;

        public RunningInfo()
        {
            TotalAlgorithmsRan = 0;
            running = 0;
            CancelationPending = false;
        }

        public void RemoveRunningAlgorithm(bool wasCanceled)
        {
            if (wasCanceled)
                CancelationPending = true;
            running--;
        }

        public void AddRunningAlgorithm()
        {
            running++;
            TotalAlgorithmsRan = Math.Max(TotalAlgorithmsRan, running);
        }

        public bool IsRunning => running > 0;


        public int RunningAlgorithms => running;

        public int TotalAlgorithmsRan { get; private set; }

        public bool CancelationPending { get; set; }

    }
}
