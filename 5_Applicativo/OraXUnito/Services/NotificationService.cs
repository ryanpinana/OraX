using OraX.Models;
using Plugin.LocalNotification;

namespace OraX.Services
{
    public class NotificationService
    {
        public async Task ScheduleNotification(
            AttivitaDb attivita)
        {
            if (!attivita.NotificheAttive)
                return;

            var request = new NotificationRequest
            {
                NotificationId = attivita.Id,

                Title = "Promemoria attività",

                Description = attivita.Titolo,

                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now.AddSeconds(15)
                }
            };

            await LocalNotificationCenter.Current.Show(request);
        }
    }
}