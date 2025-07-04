﻿using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Model;

namespace ThriftStoreWebApp.Services
{
    public class EmailSender
    {
        public static void SendEmail(string senderName, string senderEmail, string toName, string toEmail,
            string textContent, string subject)
        {
            var apiInstance = new TransactionalEmailsApi();
            SendSmtpEmailSender Email = new SendSmtpEmailSender(senderName, senderEmail);
            SendSmtpEmailTo smtpEmailTo = new SendSmtpEmailTo(toEmail, toName);
            List<SendSmtpEmailTo> To = new List<SendSmtpEmailTo>();
            To.Add(smtpEmailTo);

            try
            {
                var sendSmtpEmail = new SendSmtpEmail(Email, To, null, null, null, textContent, subject);
                CreateSmtpEmail result = apiInstance.SendTransacEmail(sendSmtpEmail);
                Console.WriteLine("Email Sender OK: \n" + result.ToJson());
            }
            catch (Exception e)
            {
                Console.WriteLine("Email Sender Failure: \n" + e.Message);
            }
        }
    }
}
