using Microsoft.AspNetCore.Mvc;

namespace StatifyWeb.Controllers;

public class HomeController : Controller
{
	private readonly ILogger<HomeController> _logger;

	public HomeController(ILogger<HomeController> logger)
	{
		_logger = logger;
	}

	public IActionResult Error()
	{
		return View();
	}

	public IActionResult Success()
	{
		return View();
	}
	
}