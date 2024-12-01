using Microsoft.AspNetCore.Mvc;
using SLICGL_IBT_Management.Models;
using System.Diagnostics;

namespace SLICGL_IBT_Management.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        
    }
}
