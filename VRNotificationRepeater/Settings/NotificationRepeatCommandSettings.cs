using Spectre.Console.Cli;

namespace Aijkl.VR.NotificationRepeater.Settings
{
    public class NotificationRepeatCommandSettings : CommandSettings
    {
        [CommandOption("-h || --hide-window")]
        public bool HideWindow { set; get; }
    }
}
