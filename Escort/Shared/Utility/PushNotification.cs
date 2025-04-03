using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Shared.Common;
using Shared.Model.Notification;

namespace Shared.Utility
{
    public static class PushNotifications
    {
        public async static Task<PushNotifiationResponseModel> SendPushNotification(PushNotificationRequestModel pushNotificationRequestModel)
        {
            try
            {
                // Set the relative path to your service account key JSON file within the "Firebase" folder
                var serviceAccountPath = FirebaseKeys.FCMServerKeyFilePath;

                if (FirebaseApp.DefaultInstance == null)
                {
                    // Load the service account key and initialize the credentials
                    var credentials = GoogleCredential.FromFile(serviceAccountPath).CreateScoped(FirebaseKeys.FirebaseMessagingUrl);

                    // Initialize the Firebase Admin SDK
                    FirebaseApp.Create(new AppOptions()
                    {
                        Credential = credentials
                    });
                }

                // Load the service account key and initialize the credentials
               
                var dataDictionary = pushNotificationRequestModel.ToDictionary();

                // Initialize the Firebase Admin SDK
                

                // Construct the FCM message
                var message = new Message()
                {
                    // Set the target device registration token
                    Token = pushNotificationRequestModel.DeviceToken,
                    // Configure the notification properties
                    Notification = new Notification()
                    {
                        Title = pushNotificationRequestModel.Title,
                        Body = pushNotificationRequestModel.Message
                    },
                    Data = dataDictionary
                };

                // Send the FCM message
                var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                return new PushNotifiationResponseModel() { Status = true, Message = response };
            }
            catch (Exception ex)
            {
                return new PushNotifiationResponseModel() { Status = true, Message = ex.Message };
            }
        }
    }

}
