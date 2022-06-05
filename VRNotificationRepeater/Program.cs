using Spectre.Console.Cli;
using Aijkl.VR.NotificationRepeater.Commands;

namespace Aijkl.VR.NotificationRepeater
{
    class Program
    {
        static int Main(string[] args)
        {                                    
            CommandApp commandApp = new CommandApp();
            commandApp.Configure(x =>
            {
                x.AddCommand<NotificationRepeatCommand>("run");
                x.AddCommand<RegisterCommand>("register");
                x.AddCommand<UnRegisterCommand>("deregister");
            });
            return commandApp.Run(args);
        }
    }
}
