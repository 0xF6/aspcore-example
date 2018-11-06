namespace WebApp
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Security.Cryptography;
    using System.Text;
    using Etc;
    using Fluent.Task;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Models;


    public class Startup
    {
        public Startup()
        {
            TaskScheduler.Instance().Start();
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddHttpContextAccessor();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
            
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(1);
                options.Cookie.HttpOnly = true;
            });
            
            services.AddSingleton(serviceProvider => TaskScheduler.Instance());

            services.AddScoped(serviceProvider =>
            {

                var httpContext = serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;

                if (httpContext == null) return null;
                
                var session = httpContext.Session;
                var key = session.GetString("key");
                return key == null
                    ? Account.FromSessionKey(null)
                    : Account.FromSessionKey(key);
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, TaskScheduler scheduler)
        {
            Storage.Auto();
            if (!Account.Any())
            {
                var newPass = $"{Guid.NewGuid()}".Split('-').First();
                var defaultAccount = new Account
                {
                    Login = "admin",
                    PassHash = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(newPass))
                        .Aggregate("", (с, t) => $"{с}{t:x2}")
                };
                ;
                File.WriteAllText("./login_and_pass.txt", $"Login: {defaultAccount.Login}\nPass: {newPass}");
                defaultAccount.Insert();
            }
            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            foreach (var site in WebSite.GetAll())
                WebSiteMonitor.From(site, scheduler);

            scheduler.Start();
        }
    }
}
