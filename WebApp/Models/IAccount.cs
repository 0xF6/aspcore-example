namespace WebApp.Models
{
    public interface IAccount
    {
        string Login { get; }
        string PassHash { get; }
        bool IsAvailable { get; }
    }
}