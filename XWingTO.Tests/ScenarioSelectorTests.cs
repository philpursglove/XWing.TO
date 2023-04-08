using NUnit.Framework;
using XWingTO.Core;

namespace XWingTO.Tests;

[TestFixture]
public class ScenarioSelectorTests
{
	[Test]
	public void When_No_Scenarios_Have_Been_Played_Any_Scenario_Is_Available()
	{
		ScenarioSelector selector = new ScenarioSelector();

		List<int> previousScenarios = new List<int>();

		int result = selector.SelectScenario(previousScenarios);

		List<int> allResults = new List<int> {1, 2, 3, 4};

		Assert.That(allResults.Contains(result));
	}
}