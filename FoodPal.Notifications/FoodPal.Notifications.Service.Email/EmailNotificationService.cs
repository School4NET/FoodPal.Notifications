using FoodPal.Notification.Service.Email;
using FoodPal.Notifications.Dto.Intern;
using System;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace FoodPal.Notifications.Service.Email
{
    public class EmailNotificationService : IEmailNotificationService
    {
        public string Key = "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";
        public string From = "foodpal.services@gmail.com";

        public async Task<bool> Send(NotificationServiceDto notificationServiceDto)
        {
            var client = new SendGridClient(this.Key);
            var sendGridMessage = MailHelper.CreateSingleEmail(
                        new EmailAddress(this.From),
                        new EmailAddress(notificationServiceDto.Email),
                        notificationServiceDto.Subject,
                        "",
                        notificationServiceDto.Body
                    );
            var response = await client.SendEmailAsync(sendGridMessage);

            return response.IsSuccessStatusCode;
        }
    }
}
