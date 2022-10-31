using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProyectoX.Models;
using Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoX.Controllers
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
      SearchService searchService = new SearchService();

  //  var res =  searchService.Find(@"X:\X_XP_Viewer\Wigos System\WGC\GUI", "Me.ScreenMode = ENUM_SCREEN_SELECT_MODE.SSM_SELECTION");

      return View();
    }

    public IActionResult Privacy()
    {
      return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  }
}
