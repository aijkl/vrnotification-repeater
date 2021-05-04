using Aijkl.VR.NotificationRepeater.Wrapppers;
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
            AppSettings appSettings = AppSettings.LoadFromFile();

            try
            {
                CVRSystemWrappper cvrSystemhelper = new CVRSystemWrappper(EVRApplicationType.VRApplication_Utility);
                EVRApplicationError vrApplicationError = cvrSystemhelper.CVRApplications.AddApplicationManifest(Path.GetFullPath(appSettings.ApplicationManifestPath), false);
                AnsiConsoleWrappper.WrapMarkupLine($"{(appSettings.LanguageDataSet.GetValue(vrApplicationError == EVRApplicationError.None ? nameof(LanguageDataSet.StreamVRAddManifestSuccess) : nameof(LanguageDataSet.StreamVRAddManifestFailure)), vrApplicationError == EVRApplicationError.None ? AnsiConsoleWrappper.State.Success : AnsiConsoleWrappper.State.Failure)}", vrApplicationError == EVRApplicationError.None ? AnsiConsoleWrappper.State.Success : AnsiConsoleWrappper.State.Failure);
                if (vrApplicationError != (int)EVREventType.VREvent_None)
                {
                    AnsiConsoleWrappper.WrapMarkupLine(vrApplicationError.ToString(), AnsiConsoleWrappper.State.Failure);
                }
            }
            catch (Exception ex)
            {
                AnsiConsoleWrappper.WrapMarkupLine(appSettings.LanguageDataSet.GetValue(nameof(LanguageDataSet.StreamVRAddManifestFailure)), AnsiConsoleWrappper.State.Failure);
                AnsiConsole.WriteException(ex);
                return 1;
            }
            return 0;
        }
    }
}
