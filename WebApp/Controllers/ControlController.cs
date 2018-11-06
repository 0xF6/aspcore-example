namespace WebApp.Controllers
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using Etc;
    using Fluent.Task;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Models;

    public class ControlController : AbstractController
    {
        public IActionResult Index() => RedirectToActionPermanent("Panel");

        public IActionResult Auth(AuthModel model)
        {
            var pass = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(model.Password))
                .Aggregate("", (с, t) => $"{с}{t:x2}");
            var acc = Account.FirstOrDefault(x => x.Login.Equals(model.Login, StringComparison.InvariantCulture));
            if (acc == null) return View("Login", new LoginViewModel { IsError = true });
            if (!acc.PassHash.Equals(pass, StringComparison.InvariantCulture))
                return View("Login", new LoginViewModel { IsError = true });
            this.HttpContext.Session.SetString("key", acc.UID.ToString());
            return RedirectToAction("Panel");
        }

        public IActionResult Panel()
        {
            if (!CurrentAccount.IsAvailable)
                return View("Login");
            return RedirectToAction("ViewListSites");
        }

        public IActionResult ViewListSites() => 
            !CurrentAccount.IsAvailable ? 
                View("Login") : 
                View(WebSite.GetAll());

        public IActionResult EditSite(Guid uid) => 
            !CurrentAccount.IsAvailable ? 
                View("Login") : 
                View(WebSite.GetByID(uid));

        [HttpPost]
        public IActionResult EditSite(WebSite model, [FromServices] TaskScheduler scheduler)
        {
            if (!CurrentAccount.IsAvailable)
                return View("Login");
            if (model.UID == default)
            {
                model.Insert();
                WebSiteMonitor.From(model, scheduler);
                return RedirectToAction("ViewListSites");
            }
            WebSiteMonitor.RemoveAndStop(model);
            WebSiteMonitor.From(model, scheduler);
            if(!model.Update())
                Trace.Fail("failed update");
            return RedirectToAction("ViewListSites");
        }

        public IActionResult AddSite() => View("EditSite", new WebSite());

        public ControlController(IAccount account) : base(account) { }
    }
}