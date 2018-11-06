namespace WebApp.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Models;

    public abstract class AbstractController : Controller
    {
        public IAccount CurrentAccount { get; }

        protected AbstractController([FromServices] IAccount account) => CurrentAccount = account;
    }
}