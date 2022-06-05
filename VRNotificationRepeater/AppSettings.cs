using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Aijkl.VR.NotificationRepeater
{
    public class AppSettings
    {
        [JsonIgnore]
        public static readonly string FileRelativePath = "./Resources/appsettings.json";

        [JsonProperty("applicationID")]
        public string ApplicationID { set; get; }

        [JsonProperty("applicationManifestPath")]
        public string ApplicationManifestPath { set; get; }

        [JsonProperty("notificationCheckIntervalMilliSecond")]
        public int NotificationCheckIntervalMilliSecond { set; get; }

        [JsonProperty("xsOverlayConnectRetryCount")]
        public int XSOverlayConnectRetryCount { set; get; }

        [JsonProperty("xsOverlayConnectRetryIntervalMilliSecond")]
        public int XSOverlayConnectRetryIntervalMilliSecond { set; get; }

        [JsonProperty("languageDataSet")]
        public LanguageDataSet LanguageDataSet { set; get; }

        public static AppSettings LoadFromFile()
        { 
            return JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText(Path.GetFullPath(FileRelativePath)));
        }
        public void SaveToFile()
        {
            File.WriteAllText(Path.GetFullPath(FileRelativePath), JsonConvert.SerializeObject(this));
        }
    }

    public class LanguageDataSet
    {
        public const string ConfigureFileError = "Configuration file error";
        public const string Error = "An error has occurred";
        public string GetValue(string memberName)
        {
            Dictionary<string, string> keyValuePairs = (Dictionary<string, string>)GetType().GetProperty(memberName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)?.GetValue(this);
            if (keyValuePairs.TryGetValue(CultureInfo.CurrentCulture.TwoLetterISOLanguageName, out string value))
            {
                return value;
            }

            value = keyValuePairs.ToList().FirstOrDefault().Value;
            value = string.IsNullOrEmpty(value) ? string.Empty : value;
            return value;
        }

        [JsonProperty("General.Configure")]
        public Dictionary<string, string> GeneralConfigure { set; get; }

        [JsonProperty("General.Exit")]
        public Dictionary<string, string> GeneralExit { set; get; }        

        [JsonProperty("General.MutexError")]
        public Dictionary<string, string> GeneralMutexError { set; get; }

        [JsonProperty("Config.Error")]
        public Dictionary<string, string> ConfigError { set; get; }     

        [JsonProperty("XSOverlay.ConnectError")]
        public Dictionary<string, string> XSOverlayConnectError { set; get; }        

        [JsonProperty("OpenVR.Initializing")]
        public Dictionary<string, string> OpenVRInitializing { set; get; }

        [JsonProperty("OpenVR.InitError")]
        public Dictionary<string, string> OpenVRInitError { set; get; }        

        [JsonProperty("Notification.AccessDenied")]
        public Dictionary<string, string> NotificationAccessDenied { set; get; }        

        [JsonProperty("StreamVR.AddManifest.Success")]
        public Dictionary<string, string> StreamVRAddManifestSuccess { set; get; }

        [JsonProperty("StreamVR.AddManifest.Failure")]
        public Dictionary<string, string> StreamVRAddManifestFailure { set; get; }

        [JsonProperty("StreamVR.RemoveManifest.Success")]
        public Dictionary<string, string> StreamVRRemoveManifestSuccess { set; get; }

        [JsonProperty("StreamVR.RemoveManifest.Failure")]
        public Dictionary<string, string> StreamVRRemoveManifestFailure { set; get; }
    }
}
