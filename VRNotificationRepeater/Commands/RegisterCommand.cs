using Aijkl.VR.NotificationRepeater.Wrappers;
using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.IO;
using Valve.VR;

namespace Aijkl.VR.NotificationRepeater.Commands
{
    public class RegisterCommand : Command
    {
        public override int Execute(CommandContext context)
        {
            var appSettings = AppSettings.LoadFromFile();

            try
            {
                var cvrSystemHelper = new CvrSystemWrapper(EVRApplicationType.VRApplication_Utility);
                var vrApplicationError = cvrSystemHelper.CvrApplications.AddApplicationManifest(Path.GetFullPath(appSettings.ApplicationManifestPath), false);
                AnsiConsoleHelper.MarkupLine($"{(appSettings.LanguageDataSet.GetValue(vrApplicationError == EVRApplicationError.None ? nameof(LanguageDataSet.StreamVRAddManifestSuccess) : nameof(LanguageDataSet.StreamVRAddManifestFailure)), vrApplicationError == EVRApplicationError.None ? AnsiConsoleWrappper.State.Success : AnsiConsoleWrappper.State.Failure)}", vrApplicationError == EVRApplicationError.None ? AnsiConsoleWrappper.State.Success : AnsiConsoleWrappper.State.Failure);
                if (vrApplicationError != (int)EVREventType.VREvent_None)
                {
                    AnsiConsoleHelper.MarkupLine(vrApplicationError.ToString(), AnsiConsoleHelper.State.Failure);
                }
            }
            catch (Exception ex)
            {
                AnsiConsoleHelper.MarkupLine(appSettings.LanguageDataSet.GetValue(nameof(LanguageDataSet.StreamVRAddManifestFailure)), AnsiConsoleHelper.State.Failure);
                AnsiConsole.WriteException(ex);
                return 1;
            }
            return 0;
        }
    }
}
