using System;
using System.Collections.Generic;
using Valve.VR;

namespace Aijkl.VR.NotificationRepeater.Wrappers
{
    public class CVREventArgs : EventArgs
    {
        public CVREventArgs(List<VREvent_t> vrEvents)
        {
            VREvents = vrEvents;
        }

        public List<VREvent_t> VREvents { get; }
    }
}
