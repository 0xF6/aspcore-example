namespace WebApp.Etc.EF
{
    using System;
    using Microsoft.EntityFrameworkCore;

    public interface IRepository<TEntity, out TDbType> : IDisposable 
        where TDbType : DbContext, new() 
        where TEntity : class, IIdentifiable
    {
        TDbType getContext();
        
        bool Insert();
        bool Update();
        bool Remove();
    }
}