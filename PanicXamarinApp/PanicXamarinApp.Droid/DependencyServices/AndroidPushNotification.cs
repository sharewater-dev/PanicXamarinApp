using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using PanicXamarinApp.Droid.DependencyServices;
using PanicXamarinApp.DependencyServices;
using PushSharp.Google;
using PushSharp.Core;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidPushNotification))]
namespace PanicXamarinApp.Droid.DependencyServices
{
    public class AndroidPushNotification : IPushNotification
    {
        public void sendNotification()
        {
            // API Key AIzaSyCwrNBZJjJAYtFfIHlJZhVBAxvIONg8xMY 
            // Configuration
         //   project = panic - 149713
            var config = new GcmConfiguration("panic-149713", "AIzaSyCwrNBZJjJAYtFfIHlJZhVBAxvIONg8xMY", "PanicXamarinApp.Droid4");
           
            // Create a new broker
            var gcmBroker = new GcmServiceBroker(config);

            // Wire up events         
           
            gcmBroker.OnNotificationFailed += (notification, aggregateEx) => 
            {

                aggregateEx.Handle(ex => {

                    // See what kind of exception it was to further diagnose
                    if (ex is GcmNotificationException)
                    {
                        var notificationException = (GcmNotificationException)ex;

                        // Deal with the failed notification
                        var gcmNotification = notificationException.Notification;
                        var description = notificationException.Description;

                        Console.WriteLine($"GCM Notification Failed: ID={gcmNotification.MessageId}, Desc={description}");
                    }
                    else if (ex is GcmMulticastResultException)
                    {
                        var multicastException = (GcmMulticastResultException)ex;

                        foreach (var succeededNotification in multicastException.Succeeded)
                        {
                            Console.WriteLine($"GCM Notification Failed: ID={succeededNotification.MessageId}");
                        }

                        foreach (var failedKvp in multicastException.Failed)
                        {
                            var n = failedKvp.Key;
                            var e = failedKvp.Value;

                            Console.WriteLine($"GCM Notification Failed: ID={n.MessageId}, Desc={e.Message}");
                        }

                    }
                    else if (ex is DeviceSubscriptionExpiredException)
                    {
                        var expiredException = (DeviceSubscriptionExpiredException)ex;

                        var oldId = expiredException.OldSubscriptionId;
                        var newId = expiredException.NewSubscriptionId;

                        Console.WriteLine($"Device RegistrationId Expired: {oldId}");

                        if (!string.IsNullOrWhiteSpace(newId))
                        {
                            // If this value isn't null, our subscription changed and we should update our database
                            Console.WriteLine($"Device RegistrationId Changed To: {newId}");
                        }
                    }
                    else if (ex is RetryAfterException)
                    {
                        var retryException = (RetryAfterException)ex;
                        // If you get rate limited, you should stop sending messages until after the RetryAfterUtc date
                        Console.WriteLine($"GCM Rate Limited, don't send more until after {retryException.RetryAfterUtc}");
                    }
                    else
                    {
                        Console.WriteLine("GCM Notification Failed for some unknown reason");
                    }

                    // Mark it as handled
                    return true;
                });
            };

            gcmBroker.OnNotificationSucceeded += (notification) => 
            {
                Console.WriteLine("GCM Notification Sent!");
            };

            // Start the broker
            gcmBroker.Start();

            string[] MY_REGISTRATION_IDS = new string[1];

            MY_REGISTRATION_IDS[0] = "ramarane89@gmail.com";
            foreach (var regId in MY_REGISTRATION_IDS)
            {
                // Queue a notification to send
                gcmBroker.QueueNotification(new GcmNotification
                {
                    NotificationData= Newtonsoft.Json.Linq.JObject.Parse("{ \"ramwwindia@gmail.com\" : \"ramarane89\" }"),
                    RegistrationIds = new List<string> {  regId} 
                   
                });            
            }

        }
    }
}