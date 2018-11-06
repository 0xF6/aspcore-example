namespace WebApp.Models
{
    using System;
    using Etc;
    using Etc.EF;
    public class Account : Repository<Account, Storage>, IAccount
    {
        public string Login { get; set; }
        public string PassHash { get; set; }
        public bool IsAvailable => true;

        public static IAccount FromSessionKey(string key)
        {
            if (Guid.TryParse(key, out var uid))
                return (IAccount) GetByID(uid) ?? new NullAccount();
            return new NullAccount();
        }
    }
}