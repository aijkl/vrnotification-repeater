using System;
using System.Threading;
using System.Linq;
using Aijkl.VR.NotificationRepeater.Wrappers;
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
        private AppSettings _appSettings;
        private CancellationTokenSource _cancellationTokenSource;
        private CvrSystemWrapper _cvrSystemWrapper;
        private XSNotifier _xsNotifier;
        private ulong _overlayWindowHandle;
        private List<UserNotification> _cachedUserNotifications;

        public override int Execute(CommandContext context, NotificationRepeatCommandSettings settings)
        {
            try
            {
                if (settings.HideWindow)
                {
                    User32Methods.ShowWindow(Process.GetCurrentProcess().MainWindowHandle, 0);
                }

                return ExecuteAsync(context).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                throw;
            }
        }
        private async Task<int> ExecuteAsync(CommandContext context)
        {
            LoadSettingFile();

            var userNotificationListener = UserNotificationListener.Current;
            var userNotificationListenerAccessStatus = userNotificationListener.GetAccessStatus();
            if (userNotificationListenerAccessStatus != UserNotificationListenerAccessStatus.Allowed)
            {
                AnsiConsole.WriteLine(_appSettings.LanguageDataSet.GetValue(nameof(LanguageDataSet.NotificationAccessDenied)));
                return 1;
            }

            InitCvrSystemWrapper();
            ConnectToXsOverlay();

            _cancellationTokenSource = new CancellationTokenSource();
            _cachedUserNotifications = new List<UserNotification>();
           
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                var userNotifications = await userNotificationListener.GetNotificationsAsync(NotificationKinds.Toast);

                if (userNotifications.Count != 0)
                {
                    if (_cachedUserNotifications.Count == 0)
                    {
                        foreach (var userNotification in userNotifications)
                        {
                            _cachedUserNotifications.Add(userNotification);
                        }                        
                    }
                    else
                    {
                        foreach (var notification in userNotifications.Where(x => _cachedUserNotifications.All(y => y.Id != x.Id)))
                        {
                            var notificationTexts = notification.Notification.Visual.GetBinding(KnownNotificationBindings.ToastGeneric).GetTextElements();
                            if (notificationTexts[0] != null && notificationTexts[1] != null && !string.IsNullOrEmpty(notificationTexts[1].Text))
                            {
                                _xsNotifier.SendNotification(new XSNotification()
                                {
                                    MessageType = XSNotifications.Enum.XSMessageType.Notification,
                                    Title = notificationTexts[0].Text,
                                    Content = notificationTexts[1].Text
                                });                                
                            }
                            _cachedUserNotifications.Add(notification);
                        }
                    }
                }

                Thread.Sleep(_appSettings.NotificationCheckIntervalMilliSecond);
            }

            return 0;
        }
        private void InitCvrSystemWrapper()
        {
            try
            {
                AnsiConsole.Status().Start(_appSettings.LanguageDataSet.GetValue(nameof(LanguageDataSet.OpenVRInitializing)), action =>
                {
                    _cvrSystemWrapper = new CvrSystemWrapper();

                    var vrOverlayError = OpenVR.Overlay.CreateOverlay(Guid.NewGuid().ToString(), _appSettings.ApplicationID, ref _overlayWindowHandle);
                    if (vrOverlayError != EVROverlayError.None)
                    {
                        throw new Exception($"{nameof(EVROverlayError)} {vrOverlayError}");
                    }
                });
            }
            catch (Exception)
            {
                AnsiConsole.WriteLine(_appSettings.LanguageDataSet.GetValue(nameof(LanguageDataSet.OpenVRInitError)));
                throw;
            }
            _cvrSystemWrapper.CvrEvent += CvrSystemWrapper_CVREvent;
            _cvrSystemWrapper.BeginEventLoop();
        }
        private void LoadSettingFile()
        {
            try
            {
                _appSettings = AppSettings.LoadFromFile();
                var table = new Table();
                table.AddColumn("PropertyName");
                table.AddColumn("Value");
                foreach (var item in _appSettings.GetType().GetProperties())
                {
                    if (item.PropertyType == typeof(string))
                    {
                        table.AddRow(item.Name, (string)item.GetValue(_appSettings));
                    }
                }
                AnsiConsole.Write(table);
            }
            catch (Exception)
            {
                AnsiConsole.WriteLine(LanguageDataSet.ConfigureFileError);
                throw;
            }
        }
        private void ConnectToXsOverlay()
        {
            Exception cachedException = null;
            for (var i = 0; i < _appSettings.XSOverlayConnectRetryCount; i++)
            {

                try
                {
                    _xsNotifier = new XSNotifier();
                    return;
                }
                catch (Exception ex)
                {
                    AnsiConsole.WriteLine(_appSettings.LanguageDataSet.GetValue(nameof(LanguageDataSet.XSOverlayConnectError)));
                    cachedException = ex; 
                }
                Thread.Sleep(_appSettings.XSOverlayConnectRetryIntervalMilliSecond);
            }
            throw cachedException;
        }
        private void CvrSystemWrapper_CVREvent(object sender, CVREventArgs e)
        {
            foreach (var vrEvent in e.VREvents)
            {
                AnsiConsole.WriteLine(((EVREventType)vrEvent.eventType).ToString());
                switch ((EVREventType)vrEvent.eventType)
                {
                    case EVREventType.VREvent_Quit:
                        _cancellationTokenSource.Cancel();
                        break;
                }
            }
        }        
    }
}
