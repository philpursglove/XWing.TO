namespace XWingTO.Tests;

[TestFixture]
public class ScenarioSelectorTests
{
	private List<int> allScenarios = new List<int>{1,2,3,4};

	[Test]
	public void When_No_Scenarios_Have_Been_Played_Any_Scenario_Is_Available()
	{
		ScenarioSelector selector = new ScenarioSelector();

		List<int> previousScenarios = new List<int>();

		int result = selector.SelectScenario(previousScenarios);

		Assert.That(allScenarios.Contains(result));
	}

	[Test]
	public void When_All_Scenarios_Have_Been_Played_Any_Scenario_Is_Available()
	{
		ScenarioSelector selector = new ScenarioSelector();

		List<int> previousScenarios = new List<int> {1, 2, 3, 4};

		int result = selector.SelectScenario(previousScenarios);

		Assert.That(allScenarios.Contains(result));
	}

	[TestCase(new[] {1}, new[]{2,3,4})]
	[TestCase(new[] { 1,2 }, new[] { 3, 4 })]
	[TestCase(new[] { 1,2,3 }, new[] { 4 })]
	[TestCase(new[] { 1, 3 }, new[] { 2, 4 })]
	public void When_Some_Scenarios_Have_Been_Played_Only_The_Remaining_Ones_Are_Available(int[] previousRoundScenarios, int[] expectedAvailableScenarios)
	{
		List<int> previousScenarios = previousRoundScenarios.ToList();
		List<int> availableScenarios = expectedAvailableScenarios.ToList();

		ScenarioSelector selector = new ScenarioSelector();

		int result = selector.SelectScenario(previousScenarios);

		Assert.That(availableScenarios.Contains(result));
	}
}