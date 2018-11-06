namespace WebApp.Etc.EF
{
    using System;

    public interface IIdentifiable
    {
        Guid UID { get; set; }
    }
}