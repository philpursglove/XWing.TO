using Humanizer;
using Microsoft.AspNetCore.Mvc;
using XWingTO.Core;

namespace XWingTO.Web.Controllers
{
	public class ScenarioController : Controller
	{
		public IActionResult RandomScenario()
		{
			var scenarios = Enum.GetValues<Scenario>().Select(s => s).Cast<int>();

			int selectedScenario = scenarios.Random();

			Scenario scenario = (Scenario) selectedScenario;

			return View("RandomScenario",scenario.Humanize());
		}
	}
}
