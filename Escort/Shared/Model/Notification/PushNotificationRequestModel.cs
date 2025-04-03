using Newtonsoft.Json;

namespace Shared.Model.Notification
{
    public class PushNotificationRequestModel
    {
        public string DeviceToken { get; set; } = "";
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public int MsgId { get; set; } = 0;
        public string NotificationType { get; set; } = "";
        public int NotificationId { get; set; } = 0;
        public string ClickAction { get; set; } = "FLUTTER_NOTIFICATION_CLICK";
        public PushNotificationDataModel Data { get; set; } = new PushNotificationDataModel();

        public Dictionary<string, string> ToDictionary()
        {
            var innerData  = new Dictionary<string, string>();
            if (Data != null)
            {
                innerData.Add(nameof(Data.UserName), Data.UserName??"");
                innerData.Add(nameof(Data.UserImage), Data.UserImage ?? "");
            }
            else
            {
                innerData.Add(nameof(PushNotificationDataModel.UserName), "");
                innerData.Add(nameof(PushNotificationDataModel.UserImage), "");
            }
            

            var dictionary = new Dictionary<string, string>
            {
                { nameof(DeviceToken), DeviceToken },
                { nameof(Title), Title },
                { nameof(Message), Message },
                { nameof(MsgId), MsgId.ToString() },
                { nameof(NotificationType), NotificationType },
                { nameof(NotificationId), NotificationId.ToString() },
                { nameof(ClickAction), ClickAction },
                { nameof(Data), JsonConvert.SerializeObject(innerData) }
            };

            return dictionary;
        }
    }




}
