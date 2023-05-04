namespace XWingTO.Core
{
	public class ScenarioSelector
	{
		public int SelectScenario(List<int> previousScenarios)
		{
			List<int> availableScenarios = new List<int>();
			foreach (int scenario in Enum.GetValues(typeof(Scenario)))
			{
				availableScenarios.Add(scenario);
			}

			if (!previousScenarios.Any())
			{
			}
			else
			{
				availableScenarios = availableScenarios.Except(previousScenarios).ToList();

				if (availableScenarios.None())
				{
					availableScenarios = new List<int>{1,2,3,4};
				}
			}

			return availableScenarios.Random();
		}
	}
}
