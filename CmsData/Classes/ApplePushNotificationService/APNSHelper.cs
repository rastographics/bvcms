using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsData.Classes.ApplePushNotificationService
{
    class APNSHelper
    {
        public static void sendNotification(List<MobileAppPushRegistration> registrations, string title, string content)
        {
            APNSConnection connection = new APNSConnection();
            connection.open();

            APNSAlert alert = new APNSAlert("New Notification", "You have a notification!");

            APNSMessage message = new APNSMessage();
            message.addAlert(alert);
            message.addSound("default");
            message.addCommand(3);

            APNSNotification notification = new APNSNotification(message, true);

            foreach (MobileAppPushRegistration registration in registrations)
            {
                notification.setDeviceToken(registration.RegistrationId);
                notification.sendViaConnection(connection);
            }

            connection.close();
        }
    }
}
