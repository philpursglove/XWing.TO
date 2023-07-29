namespace XWingTO.Web.Controllers
{
	public class ScenarioController : Controller
	{
        private readonly ILogger _logger;

        public ScenarioController(ILogger logger)
        {
            _logger = logger;
        }

		public IActionResult RandomScenario()
		{
			_logger.LogInformation("RandomScenario called");

			var scenarios = Enum.GetValues<Scenario>().Select(s => s).Cast<int>();

			int selectedScenario = scenarios.Random();

			Scenario scenario = (Scenario) selectedScenario;

			return View("RandomScenario",scenario.Humanize());
		}
	}
}
