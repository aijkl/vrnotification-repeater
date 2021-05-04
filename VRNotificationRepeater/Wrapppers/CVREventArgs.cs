using System;
using System.Collections.Generic;
using Valve.VR;

namespace Aijkl.VR.NotificationRepeater.Wrapppers
{
    public class CVREventArgs : EventArgs
    {
        public CVREventArgs(List<VREvent_t> vrEvents) : base()
        {
            VREvents = vrEvents;
        }

        public List<VREvent_t> VREvents { private set; get; }
    }
}
