using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tolltech.MuserUI.Models;
using TolltechCore;

namespace Tolltech.MuserUI.Controllers
{
    [Route("")]
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IGuidFactory guidFactory;

        public HomeController(ILogger<HomeController> logger, IGuidFactory guidFactory)
        {
            _logger = logger;
            this.guidFactory = guidFactory;
        }

        [HttpGet("")]
        [AllowAnonymous]
        public IActionResult Index()
        {
            if (SafeUserId.HasValue)
            {
                return RedirectToAction("Index","SyncWizard");
            }
            else
            {
                return View();
            } 
        }

        [HttpGet("privacy")]
        public IActionResult Privacy()
        {
            return View((object)guidFactory.Create().ToString());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
