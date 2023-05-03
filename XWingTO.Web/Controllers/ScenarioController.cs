using Humanizer;
using Microsoft.AspNetCore.Mvc;
using XWingTO.Core;

namespace XWingTO.Web.Controllers
{
	public class ScenarioController : Controller
	{
		public IActionResult RandomScenario()
		{
			List<int> scenarios = new List<int> {1, 2, 3, 4};

			int selectedScenario = scenarios.Random();

			Scenario scenario = (Scenario) selectedScenario;

			return View("RandomScenario",scenario.Humanize());
		}
	}
}
