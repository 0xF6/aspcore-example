namespace WebApp.Models
{
    using System;

    public class NullAccount : IAccount
    {
        public string Login => throw new Exception("Session unauthorized.");
        public string PassHash => throw new Exception("Session unauthorized.");
        public bool IsAvailable => false;
    }
}