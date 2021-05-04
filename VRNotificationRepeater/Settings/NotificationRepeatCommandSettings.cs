using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aijkl.VR.NotificationRepeater.Settings
{
    public class NotificationRepeatCommandSettings : CommandSettings
    {
        [CommandOption("-h || --hide-window")]
        public bool HideWindow { set; get; }
    }
}
