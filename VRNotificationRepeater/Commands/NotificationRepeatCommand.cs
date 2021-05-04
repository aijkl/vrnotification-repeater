using System;
using System.Threading;
using System.Linq;
using Aijkl.VR.NotificationRepeater.Wrapppers;
using Spectre.Console;
using Spectre.Console.Cli;
using Valve.VR;
using Windows.UI.Notifications.Management;
using Windows.UI.Notifications;
using System.Collections.Generic;
using XSNotifications;
using System.Threading.Tasks;
using System.Windows.Forms;
using Aijkl.VR.NotificationRepeater.Settings;
using WinApi.User32;
using System.Diagnostics;

namespace Aijkl.VR.NotificationRepeater.Commands
{
    public class NotificationRepeatCommand : Command<NotificationRepeatCommandSettings>
    {                
        private AppSettings appSettings;
        private CancellationTokenSource cancellationTokenSource;
        private CVRSystemWrappper cvrSystemWapper;
        private XSNotifier xsNotifier = null;
        private ulong overlayWindowHandle = 0;
        private List<UserNotification> cachedUserNotifications;

        public override int Execute(CommandContext context, NotificationRepeatCommandSettings settings)
        {
            try
            {
                if (settings.HideWindow)
                {
                    User32Methods.ShowWindow(Process.GetCurrentProcess().MainWindowHandle, 0);
                }

                return ExcuteAsync(context).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                throw;
            }
        }
        private async Task<int> ExcuteAsync(CommandContext context)
        {
            LoadSettingFile();

            UserNotificationListener userNotificationListener = UserNotificationListener.Current;
            UserNotificationListenerAccessStatus userNotificationListenerAccessStatus = userNotificationListener.GetAccessStatus();
            if (userNotificationListenerAccessStatus != UserNotificationListenerAccessStatus.Allowed)
            {
                AnsiConsole.WriteLine(appSettings.LanguageDataSet.GetValue(nameof(LanguageDataSet.NotificationAccessDenied)));
                return 1;
            }

            InitCVRSystemWrappper();
            ConnectToXSOverlay();

            cancellationTokenSource = new CancellationTokenSource();
            cachedUserNotifications = new List<UserNotification>();
           
            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                IReadOnlyList<UserNotification> userNotifications = await userNotificationListener.GetNotificationsAsync(NotificationKinds.Toast);

                if (userNotifications.Count != 0)
                {
                    if (cachedUserNotifications.Count == 0)
                    {
                        foreach (var userNotification in userNotifications)
                        {
                            cachedUserNotifications.Add(userNotification);
                        }                        
                    }
                    else
                    {
                        foreach (var notification in userNotifications.Where(x => !cachedUserNotifications.Any(y => y.Id == x.Id)))
                        {
                            IReadOnlyList<AdaptiveNotificationText> notificationTexts = notification.Notification.Visual.GetBinding(KnownNotificationBindings.ToastGeneric).GetTextElements();
                            if (notificationTexts[0] != null && notificationTexts[1] != null && !string.IsNullOrEmpty(notificationTexts[1].Text))
                            {
                                xsNotifier.SendNotification(new XSNotification()
                                {
                                    MessageType = XSNotifications.Enum.XSMessageType.Notification,
                                    Title = notificationTexts[0].Text,
                                    Content = notificationTexts[1].Text
                                });                                
                            }
                            cachedUserNotifications.Add(notification);
                        }
                    }
                }

                Thread.Sleep(appSettings.NotificationCheackIntervalMiliSecond);
            }

            return 0;
        }
        private void InitCVRSystemWrappper()
        {
            try
            {
                AnsiConsole.Status().Start(appSettings.LanguageDataSet.GetValue(nameof(LanguageDataSet.OpenVRInitializing)), action =>
                {
                    cvrSystemWapper = new CVRSystemWrappper();

                    EVROverlayError vrOverlayError = OpenVR.Overlay.CreateOverlay(Guid.NewGuid().ToString(), appSettings.ApplicationID, ref overlayWindowHandle);
                    if (vrOverlayError != EVROverlayError.None)
                    {
                        throw new Exception($"{nameof(EVROverlayError)} {vrOverlayError}");
                    }
                });
            }
            catch (Exception)
            {
                AnsiConsole.WriteLine(appSettings.LanguageDataSet.GetValue(nameof(LanguageDataSet.OpenVRInitError)));
                throw;
            }
            cvrSystemWapper.CVREvent += CvrSystemWapper_CVREvent;
            cvrSystemWapper.BeginEventLoop();
        }
        private void LoadSettingFile()
        {
            try
            {
                appSettings = AppSettings.LoadFromFile();
                Table table = new Table();
                table.AddColumn("PropertyName");
                table.AddColumn("Value");
                foreach (var item in appSettings.GetType().GetProperties())
                {
                    if (item.PropertyType == typeof(string))
                    {
                        table.AddRow(item.Name, (string)item.GetValue(appSettings));
                    }
                }
                AnsiConsole.Render(table);
            }
            catch (Exception)
            {
                AnsiConsole.WriteLine(LanguageDataSet.CONFIGURE_FILE_ERROR);
                throw;
            }
        }
        private void ConnectToXSOverlay()
        {
            Exception cachedException = null;
            for (int i = 0; i < appSettings.XSOverlayConnectRetryCount; i++)
            {

                try
                {
                    xsNotifier = new XSNotifier();
                    return;
                }
                catch (Exception ex)
                {
                    AnsiConsole.WriteLine(appSettings.LanguageDataSet.GetValue(nameof(LanguageDataSet.XSOverlayConnectError)));
                    cachedException = ex; 
                }
                Thread.Sleep(appSettings.XSOverlayConnectRetryIntervalMiliSeocond);
            }
            throw cachedException;
        }
        private void CvrSystemWapper_CVREvent(object sender, CVREventArgs e)
        {
            foreach (var vrEvent in e.VREvents)
            {
                AnsiConsole.WriteLine(((EVREventType)vrEvent.eventType).ToString());
                switch ((EVREventType)vrEvent.eventType)
                {
                    case EVREventType.VREvent_Quit:
                        cancellationTokenSource.Cancel();
                        break;
                }
            }
        }        
    }
}
