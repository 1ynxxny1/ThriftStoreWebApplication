using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Model;

namespace ThriftStoreWebApp.Services
{
    public static class EmailSender
    {
        public static void SendEmail(
            string senderName,
            string senderEmail,
            string recipientName,
            string recipientEmail,
            string messageBody,
            string subject)
        {
            var apiInstance = new TransactionalEmailsApi();

            var sender = new SendSmtpEmailSender(senderName, senderEmail);
            var recipient = new SendSmtpEmailTo(recipientEmail, recipientName);
            var recipients = new List<SendSmtpEmailTo> { recipient };

            try
            {
                var email = new SendSmtpEmail(
                    sender,
                    recipients,
                    htmlContent: null,
                    textContent: messageBody,
                    subject: subject
                );

                var result = apiInstance.SendTransacEmail(email);
                Console.WriteLine("[EmailSender] Email sent successfully:\n" + result.ToJson());
            }
            catch (Exception ex)
            {
                Console.WriteLine("[EmailSender] Failed to send email:\n" + ex.Message);
            }
        }
    }
}
