using Aijkl.VR.NotificationRepeater.Wrapppers;
using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.IO;
using Valve.VR;

namespace Aijkl.VR.NotificationRepeater.Commands
{
    public class DeRegisterCommand : Command
    {
        public override int Execute(CommandContext context)
        {
            AppSettings appSettings = AppSettings.LoadFromFile();

            try
            {
                CVRSystemWrappper cvrSystemhelper = new CVRSystemWrappper(EVRApplicationType.VRApplication_Utility);
                EVRApplicationError vrApplicationError = cvrSystemhelper.CVRApplications.RemoveApplicationManifest(Path.GetFullPath(appSettings.ApplicationManifestPath));
                AnsiConsoleWrappper.WrapMarkupLine(appSettings.LanguageDataSet.GetValue(vrApplicationError == EVRApplicationError.None ? nameof(LanguageDataSet.StreamVRRemoveManifestSuccess) : nameof(LanguageDataSet.StreamVRRemoveManifestFailure)), vrApplicationError == EVRApplicationError.None ? AnsiConsoleWrappper.State.Success : AnsiConsoleWrappper.State.Failure);
            }
            catch (Exception ex)
            {
                AnsiConsoleWrappper.WrapMarkupLine(appSettings.LanguageDataSet.GetValue(nameof(LanguageDataSet.StreamVRRemoveManifestFailure)), AnsiConsoleWrappper.State.Failure);
                AnsiConsole.WriteException(ex);
                return 1;
            }
            return 0;
        }
    }
}
