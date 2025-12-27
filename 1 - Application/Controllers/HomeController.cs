using Cocus.Domain.Interfaces.Services;
using Cocus.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Cocus.Mvc.Controllers
{
	[Authorize]
	public class HomeController : Controller
	{
		private readonly IFlightService _flightService;

		public HomeController(IFlightService flightService)
		{
			_flightService = flightService;
		}

		public async Task<IActionResult> Index()
		{
			var report = await _flightService.GetFlightReportAsync();
			return View(report);
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
