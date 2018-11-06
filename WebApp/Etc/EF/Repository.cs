namespace WebApp.Etc.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    public abstract class Repository<TEntity, TDbType> : IRepository<TEntity, TDbType>, IIdentifiable where TEntity : class, IIdentifiable, new() where TDbType : DbContext, new()
    {
        [Key]
        public Guid UID { get; set; }
        private TDbType Context
        {
            get
            {
                lock (Guarder)
                {
                    return _clrDbType ?? (_clrDbType = new TDbType());
                }
            }
        }


        #region Repository Implementation
        
        public bool Insert()
        {
            Context.Attach(this);
            Context.Entry(this).State = EntityState.Added;
            var result = Context.SaveChanges();
            Trace.WriteLine($"{this} ef:delete result: {result}");
            return result > 0;
        }
        public bool Update()
        {
            try
            {
                Context.Attach(this);
                Context.Entry(this).State = EntityState.Modified;
                var result = Context.SaveChangesAsync().Result;

                Trace.WriteLine($"{this} ef:update result: {result}");

                return result > 0;
            }
            catch (Exception e)
            {
                Trace.WriteLine($"{this} ef:update fault {e}");
                return default;
            }
        }
        public bool Remove()
        {
            try
            {
                Context.Attach(this);
                Context.Entry(this).State = EntityState.Deleted;
                var result = Context.SaveChanges();
                Trace.WriteLine($"{this} ef:delete result: {result}");
                return result > 0;
            }
            catch (Exception e)
            {
                Trace.WriteLine($"{this} ef:remove fault {e}");
                return default;
            }
        }

        #endregion

        #region Static

        public static List<TEntity> GetAll() => getQuery().ToList();
        public static TEntity GetByID(Guid id) => getQuery().FirstOrDefault(x => x.UID == id);
        public static TEntity FirstOrDefault(Func<TEntity, bool> predicate) => getQuery().FirstOrDefault(predicate);
        public static IEnumerable<TEntity> Where(Func<TEntity, bool> predicate) => getQuery().AsEnumerable().Where(predicate);
        public static int Count(Func<TEntity, bool> predicate = null) => predicate != null ?
            getQuery().Count(predicate) :
            getQuery().Count();
        public static bool Any(Func<TEntity, bool> predicate = null) => predicate != null ?
            getQuery().Any(predicate) :
            getQuery().Any();
        public static IQueryable<TResult> Select<TResult>(Expression<Func<TEntity, TResult>> selector) =>
            getQuery().Select(selector);

        private static IQueryable<TEntity> getQuery() =>
            (new TEntity() as IRepository<TEntity, TDbType>)?.getContext().Set<TEntity>().AsNoTracking();

        public TDbType getContext() => Context;

        public void Dispose() => Context.Dispose();

        #endregion

        [NonSerialized]
        private static readonly object Guarder = new object();
        [NonSerialized]
        private TDbType _clrDbType;
    }
}