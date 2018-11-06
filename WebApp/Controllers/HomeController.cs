namespace WebApp.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Models;

    public class HomeController : Controller
    {
        public IActionResult Index() => View(WebSite.GetAll());
    }
}
