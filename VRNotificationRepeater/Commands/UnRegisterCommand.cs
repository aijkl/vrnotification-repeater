using Aijkl.VR.NotificationRepeater.Wrappers;
using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.IO;
using Valve.VR;

namespace Aijkl.VR.NotificationRepeater.Commands
{
    public class UnRegisterCommand : Command
    {
        public override int Execute(CommandContext context)
        {
            var appSettings = AppSettings.LoadFromFile();

            try
            {
                var cvrSystemHelper = new CvrSystemWrapper(EVRApplicationType.VRApplication_Utility);
                var vrApplicationError = cvrSystemHelper.CvrApplications.RemoveApplicationManifest(Path.GetFullPath(appSettings.ApplicationManifestPath));
                AnsiConsoleHelper.MarkupLine(appSettings.LanguageDataSet.GetValue(vrApplicationError == EVRApplicationError.None ? nameof(LanguageDataSet.StreamVRRemoveManifestSuccess) : nameof(LanguageDataSet.StreamVRRemoveManifestFailure)), vrApplicationError == EVRApplicationError.None ? AnsiConsoleHelper.State.Success : AnsiConsoleHelper.State.Failure);
            }
            catch (Exception ex)
            {
                AnsiConsoleHelper.MarkupLine(appSettings.LanguageDataSet.GetValue(nameof(LanguageDataSet.StreamVRRemoveManifestFailure)), AnsiConsoleHelper.State.Failure);
                AnsiConsole.WriteException(ex);
                return 1;
            }
            return 0;
        }
    }
}
