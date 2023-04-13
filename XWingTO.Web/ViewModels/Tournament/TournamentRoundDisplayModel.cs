using XWingTO.Core;

namespace XWingTO.Web.ViewModels.Tournament;

public class TournamentRoundDisplayModel
{
	public int Round { get; set; }

	public List<TournamentGameDisplayModel> Games { get; set; }

	public Scenario Scenario { get; set; }

	public TournamentRoundDisplayModel()
	{
		Games = new List<TournamentGameDisplayModel>();
	}
}