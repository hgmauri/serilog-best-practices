using Microsoft.AspNetCore.Mvc;

namespace Serilog.BestPractices.Controllers;

[ApiController]
[Route("[controller]")]
public class SerilogController : ControllerBase
{
	[HttpGet]
	public IActionResult GetAsync()
	{
		Log.Information("ok");

		return Ok();
	}
}
