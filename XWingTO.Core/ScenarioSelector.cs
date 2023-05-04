namespace XWingTO.Core
{
	public class ScenarioSelector
	{
		public int SelectScenario(List<int> previousScenarios)
		{
            var availableScenarios = Enum.GetValues(typeof(Scenario)).Cast<int>();
			if (!previousScenarios.Any())
			{
			}
			else
			{
				availableScenarios = availableScenarios.Except(previousScenarios).ToList();

				if (availableScenarios.None())
				{
					availableScenarios = Enum.GetValues(typeof(Scenario)).Cast<int>();
				}
			}

			return availableScenarios.Random();
		}
	}
}
