﻿namespace IdentityMVC.Models.Entities
{
    public class SmtpSettings
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public string SenderName { get; set; }//ahmet

        public string SenderEmail { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

    }
}
